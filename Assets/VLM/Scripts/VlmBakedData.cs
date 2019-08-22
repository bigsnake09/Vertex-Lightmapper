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

using UnityEngine;

namespace Vlm
{
    /// <summary>
    /// Contains vertex colors that will be applied to the parent mesh when loaded.
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("")]
    public class VlmBakedData : MonoBehaviour
    {
        /// <summary>
        /// The colors stored in this mesh.
        /// </summary>
        public Color[] Colors = new Color[0];

        /// <summary>
        /// If true then the next apply will be ignored.
        /// </summary>
        public bool IgnoreNextApply;

        private void Start()
        {
            Apply();
        }

        public void Apply()
        {
            if (IgnoreNextApply)
            {
                IgnoreNextApply = false;
                return;
            }

            if (Colors.Length == 0)
            {
                Debug.LogError($"VML: Couldn't apply vertex colors to {name} - color array doesn't exist.");
                return;
            }

            MeshFilter mf = GetComponent<MeshFilter>();
            if (!mf)
            {
                Debug.LogError($"VML: Couldn't apply vertex colors to {name} - no mesh filter attached.");
                return;
            }

            Mesh m = mf.sharedMesh;
            if (!m)
            {
                Debug.LogError($"VLM: Couldn't apply vertex colors to {name} - mesh filter doesn't have a mesh.");
                return;
            }

            if (m.vertexCount != Colors.Length)
            {
                Debug.LogError($"VLM: Couldn't apply vertex colors to {name} - vertex count mismatch.");
                return;
            }

            Color[] colors = m.colors;

            bool buildingFromScratch = colors == null || colors.Length == 0;
            if (buildingFromScratch) colors = new Color[Colors.Length];

            for (int i = 0; i < colors.Length; ++i)
            {
                colors[i].r = Colors[i].r;
                colors[i].g = Colors[i].g;
                colors[i].b = Colors[i].b;
                if (buildingFromScratch) colors[i].a = 1.0f;
            }
            m.colors = colors;
        }
    }
}
