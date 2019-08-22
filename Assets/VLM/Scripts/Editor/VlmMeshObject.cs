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

namespace Vlm.Editor
{
    /// <summary>
    /// Holds data about a mesh that will be baked.
    /// </summary>
    public struct VlmMeshObject
    {
        public VlmMeshObject(GameObject go, VlmBakeOptionsComponent bakeOptions = null)
        {
            MeshCollider = go.GetComponent<MeshCollider>();

            if (!MeshCollider)
            {
                MeshCollider = go.AddComponent<MeshCollider>();
                MeshCollider.hideFlags = HideFlags.HideAndDontSave;
                _attachedCollider = true;
            }
            else _attachedCollider = false;

            MeshBakeOptions = bakeOptions ? bakeOptions.BakeOptions : null;
            Transform = go.transform;

            MeshFilter mf = go.GetComponent<MeshFilter>();
            Mesh = mf ? mf.sharedMesh : null;
        }

        /// <summary>
        /// The mesh collider attached to this object for baking.
        /// </summary>
        public MeshCollider MeshCollider;

        /// <summary>
        /// The bake options component attached to this object for baking.
        /// </summary>
        public VlmBakeOptions MeshBakeOptions;

        /// <summary>
        /// The transform of this object.
        /// </summary>
        public Transform Transform;

        /// <summary>
        /// The mesh of this object.
        /// </summary>
        public Mesh Mesh;

        /// <summary>
        /// Whether a mesh collider was added to this object.
        /// </summary>
        private readonly bool _attachedCollider;
        
        /// <summary>
        /// Removes the mesh collider from this object is a new one was created.
        /// </summary>
        public void Cleanup()
        {
            if (MeshCollider && _attachedCollider) Object.DestroyImmediate(MeshCollider);
        }
    }
}
