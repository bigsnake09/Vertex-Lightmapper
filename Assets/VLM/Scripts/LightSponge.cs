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
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

namespace Vlm
{
    /// <summary>
    /// A sponge that absorbs ambient and directional light.
    /// </summary>
    [AddComponentMenu("Rendering/VLM Light Sponge")]
    [ExecuteInEditMode]
    public class LightSponge : MonoBehaviour
    {
        /// <summary>
        /// The shape of this sponge.
        /// </summary>
        public SpongeShape Shape = SpongeShape.Sphere;

        /// <summary>
        /// How much light this sponge will absorb. (0 - 1).
        /// </summary>
        public float Intensity = 1.0f;

        /// <summary>
        /// The radius of the sponge when in sphere mode.
        /// </summary>
        public float SphereRadius = 10.0f;

        /// <summary>
        /// If true then baking this sponge will ignore normals facing away from the sponge.
        /// </summary>
        public bool IgnoreReverseNormals;

        /// <summary>
        /// The extends of the sponge when in box mode.
        /// </summary>
        public Vector3 BoxBounds = Vector3.one * 10.0f;

        public enum SpongeShape
        {
            Sphere,
            Box,
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/Light/Light Sponge")]
        public static void CreateNewLightSponge()
        {
            SceneView view = SceneView.currentDrawingSceneView;
            if (!view) view = SceneView.lastActiveSceneView;
            if (view)
            {
                Transform cameraT = view.camera.transform;

                GameObject newSponge = new GameObject("Light Sponge");
                newSponge.transform.position = cameraT.position + cameraT.forward * 10.0f;

                LightSponge ls = newSponge.AddComponent<LightSponge>();

                Selection.activeObject = newSponge;
            }
        }

        private void OnEnable()
        {
            SetIcon();
        }

        private void SetIcon()
        {
            Texture2D icon = Resources.Load<Texture2D>("Light Sponge Icon");
            if (!icon)
            {
                Debug.LogError("Couldn't find the light sponge icon texture in the resources folder! (Light Sponge Icon.png)");
                return;
            }

            MethodInfo iconMethod = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            iconMethod?.Invoke(null, new[] { (object)gameObject, icon });
        }

        #region Custom Editor Stuff

        [CustomEditor(typeof(LightSponge)), CanEditMultipleObjects]
        public class LightSpongeEditor : Editor
        {
            private readonly Color _handlesColor = new Color(0.9882353F, 0.9843137F, 0.5294118F);
            private BoxBoundsHandle _boundsHandle;

            public override void OnInspectorGUI()
            {
                LightSponge sponge = (LightSponge)target;

                EditorGUI.BeginChangeCheck();

                SpongeShape newShape = (SpongeShape)EditorGUILayout.EnumPopup(new GUIContent("Shape", "The shape of the sponge"), sponge.Shape);
                GUILayout.Space(10);

                bool newIgnore = EditorGUILayout.Toggle(new GUIContent("Ignore Opposite Normals", "Whether this sponge will ignore normals facing away from it."), sponge.IgnoreReverseNormals);
                float newIntensity = EditorGUILayout.Slider(new GUIContent("Intensity", "How much ambient and directional light this sponge will absorb."), sponge.Intensity, 0.0f, 1.0f);

                if (Mathf.Approximately(newIntensity, 0.0f)) EditorGUILayout.HelpBox("Light sponges with an intensity of 0 are excluded from lighting bakes.", MessageType.Warning);

                if (sponge.Shape == SpongeShape.Box)
                {
                    Vector3 newBounds = EditorGUILayout.Vector3Field(new GUIContent("Bounds", "The bounds of the sponge."), sponge.BoxBounds);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(sponge, "Updated Light Sponge");
                        sponge.Shape = newShape;
                        sponge.IgnoreReverseNormals = newIgnore;
                        sponge.Intensity = newIntensity;
                        sponge.BoxBounds = newBounds;
                    }
                }
                else
                {
                    float newRadius = EditorGUILayout.FloatField(new GUIContent("Radius", "The radius of the sponge."), sponge.SphereRadius);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(sponge, "Updated Light Sponge");
                        sponge.Shape = newShape;
                        sponge.IgnoreReverseNormals = newIgnore;
                        sponge.Intensity = newIntensity;
                        sponge.SphereRadius = newRadius;
                    }
                }
            }

            private void OnSceneGUI()
            {
                LightSponge sponge = (LightSponge)target;

                if (sponge.Shape == SpongeShape.Box) DoBoxHandles(sponge);
                else DoSphereHandles(sponge);
            }

            private void DoBoxHandles(LightSponge sponge)
            {
                /*---Box Handle---*/
                Handles.color = _handlesColor;
                if (_boundsHandle == null) _boundsHandle = new BoxBoundsHandle();
                _boundsHandle.size = sponge.BoxBounds;
                _boundsHandle.center = sponge.transform.position;
                _boundsHandle.DrawHandle();

                if (sponge.BoxBounds != _boundsHandle.size)
                {
                    Undo.RecordObject(sponge, "Updated Light Sponge Bounds");
                    sponge.BoxBounds = _boundsHandle.size;
                }

                Handles.color = Color.white;
            }

            private void DoSphereHandles(LightSponge sponge)
            {
                EditorGUI.BeginChangeCheck();

                /*---Radius Handle---*/
                Handles.color = _handlesColor;
                float newRadius = Handles.RadiusHandle(Quaternion.identity, sponge.transform.position, sponge.SphereRadius);

                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(sponge, "Updated Light Sponge Radius");
                    sponge.SphereRadius = newRadius;
                }

                Handles.color = Color.white;
            }
        }

        #endregion
#endif
    }
}
