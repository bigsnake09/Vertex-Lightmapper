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
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Vlm.Editor
{
    /// <summary>
    /// Holds data for performing a bake.
    /// </summary>
    public class VlmBakeData
    {
        public VlmBakeData()
        {
            Light[] allLights = Object.FindObjectsOfType<Light>();

            DirectionalLights = allLights.Where(l => l.type == LightType.Directional).ToList();
            Lights = allLights.Where(l => l.type != LightType.Directional).ToList();

            LightSponges = Object.FindObjectsOfType<LightSponge>().ToList();

            Meshes = new List<VlmMeshObject>();
            MeshFilter[] meshes = Object.FindObjectsOfType<MeshFilter>();

            foreach (MeshFilter mesh in meshes)
            {
                if (!GameObjectUtility.AreStaticEditorFlagsSet(mesh.gameObject, StaticEditorFlags.LightmapStatic)) continue;

                VlmBakeOptionsComponent bakeOptions = mesh.GetComponent<VlmBakeOptionsComponent>();
                if (!bakeOptions || !bakeOptions.IgnoreLightmapper) Meshes.Add(new VlmMeshObject(mesh.gameObject));
            }
        }

        /// <summary>
        /// The meshes for this bake.
        /// </summary>
        public List<VlmMeshObject> Meshes;

        /// <summary>
        /// The directional lights for this bake.
        /// </summary>
        public List<Light> DirectionalLights;

        /// <summary>
        /// The non directional lights for this bake.
        /// </summary>
        public List<Light> Lights;

        /// <summary>
        /// The light sponges for this bake.
        /// </summary>
        public List<LightSponge> LightSponges;
    }
}
