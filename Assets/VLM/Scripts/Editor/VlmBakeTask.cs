// Copyright 2019 Adam Chivers
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Vlm.Editor
{
    public struct VlmBakeTask
    {
        /// <summary>
        /// Runs the bake task.
        /// </summary>
        public static void Bake()
        {
            EditorUtility.DisplayProgressBar("Vertex Light Mapper", "Gathering Scene Data", 0.0f);

            VlmBakeData data = new VlmBakeData();

            /*---Destroy Previous Lightmap Data---*/
            VlmBakedData[] prevData = Object.FindObjectsOfType<VlmBakedData>();
            for (int i = 0; i < prevData.Length; ++i) Object.DestroyImmediate(prevData[i]);

            /*---Bake Lighting---*/
            int count = data.Meshes.Count;
            for (int i = 0; i < count; ++i)
            {
                /*---Progress Report---*/
                VlmMeshObject meshObj = data.Meshes[i];
                if (!meshObj.Mesh) continue;

                EditorUtility.DisplayProgressBar("Vertex Lightmapper", $"Calculating Lighting ({meshObj.Transform.name} {i + 1} / {count})", (float)(i + 1) / count);

                Vector3[] verts = meshObj.Mesh.vertices;
                Vector3[] normals = meshObj.Mesh.normals;

                List<Color> colors = new List<Color>(meshObj.Mesh.colors);
                int colorInitLen = colors.Count;

                for (int j = 0; j < verts.Length; ++j)
                {
                    Vector3 vert = meshObj.Transform.TransformPoint(verts[j]);
                    Vector3 normal = meshObj.Transform.TransformDirection(normals[j]);

                    /*---Apply Ambient Light---*/
                    Color newColor = VlmMath.GetAmbientColor(normal);

                    /*---Apply Directional Lights---*/
                    for (int k = 0; k < data.DirectionalLights.Count; ++k) newColor += VlmMath.CalculateColorDirectional(vert, normal, data.DirectionalLights[k], meshObj.MeshBakeOptions);

                    /*---Apply Light Sponges---*/
                    for (int k = 0; k < data.LightSponges.Count; ++k) newColor *= VlmMath.CalculateColorSponge(vert, normal, data.LightSponges[k]);

                    /*---Apply Other Lights---*/
                    for (int k = 0; k < data.Lights.Count; ++k)
                    {
                        Light l = data.Lights[k];

                        if (l.type == LightType.Spot) newColor += VlmMath.CalculateColorSpot(vert, normal, l, meshObj.MeshBakeOptions);
                        else newColor += VlmMath.CalculateColorPoint(vert, normal, l, meshObj.MeshBakeOptions);
                    }

                    if (j < colorInitLen)
                    {
                        newColor.a = colors[j].a;
                        colors[j] = newColor;
                    }
                    else colors.Add(newColor);
                }

                /*---Apply Colors---*/
                meshObj.Mesh.colors = colors.ToArray();

                /*---Apply Data---*/
                VlmBakedData bakedData = meshObj.Transform.GetComponent<VlmBakedData>();
                if (!bakedData) bakedData = meshObj.Transform.gameObject.AddComponent<VlmBakedData>();

                bakedData.IgnoreNextApply = true;
                bakedData.Colors = colors.ToArray();
            }

            /*---Cleanup---*/
            foreach (VlmMeshObject mesh in data.Meshes) mesh.Cleanup();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            EditorUtility.ClearProgressBar();
        }
    }
}
