using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GM.LevelMaker
{
    [CustomEditor(typeof(MeshFromSpline))]
    public class MeshFromSplineEditor : Editor
    {
        private MeshFromSpline meshFromSpline;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            meshFromSpline = target as MeshFromSpline;

            if (meshFromSpline == null)
            {
                return;
            }

            if (GUILayout.Button("Recenter"))
            {
                Undo.RecordObject(meshFromSpline, "RecenterSpline");
                meshFromSpline.Recenter();
                EditorUtility.SetDirty(meshFromSpline);
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Reset Loop Spline"))
            {
                Undo.RecordObject(meshFromSpline, "Reset Loop Spline");
                meshFromSpline.ResetLoop();
                EditorUtility.SetDirty(meshFromSpline);
            }

            if (GUILayout.Button("Reset Height Spline"))
            {
                Undo.RecordObject(meshFromSpline, "Reset Height Spline");
                meshFromSpline.ResetHeight();
                EditorUtility.SetDirty(meshFromSpline);
            }
        }
    }
}