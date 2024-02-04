using Sebastian.Geometry;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GM.LevelMaker
{
    [RequireComponent(typeof(SplineLoop)), RequireComponent(typeof(Spline))]
    [ExecuteInEditMode]
    public class MeshFromSpline : MonoBehaviour
    {
        public MeshFilter meshFilter;
        public SplineLoop loopSpline;
        public Spline heightSpline;

        [Range(1, 50)]
        public int loopQuality = 20;

        [Range(2, 10)]
        public int heightQuality = 2;

        bool meshEdited = false;

        [Space]
        public bool capBottom = true;

        private void OnValidate()
        {
            if (loopSpline == null)
                loopSpline = GetComponent<SplineLoop>();

            if (heightSpline == null)
                heightSpline = GetComponent<Spline>();

            if (loopSpline != null)
            {
                loopSpline.OnEdit += () =>
                {
                    meshEdited = true;
                    heightSpline.RealignSpline(loopSpline, true);
                };
            }

            if (heightSpline != null)
            {
                heightSpline.OnEdit += () => { meshEdited = true; };
            }


#if UNITY_EDITOR

            Undo.undoRedoPerformed += () => { meshEdited = true; };
#endif
            meshEdited = true;
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (meshEdited)
            {
                RegenerateMesh();
                meshEdited = false;
            }
        }
#endif

        public void RegenerateMesh()
        {
            if (loopSpline == null || loopQuality <= 0 || loopSpline.GetCurveCount() < 3)
            {
                return;
            }

            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();

#if UNITY_EDITOR
                gameObject.AddComponent<MeshRenderer>().materials = new Material[] {
                AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat"),
                AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat"),
                AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat"),
                AssetDatabase.GetBuiltinExtraResource<Material>("Default-Diffuse.mat")
            };
#endif
            }

            List<Vector3> loopPoints = new List<Vector3>();

            int num_curves = loopSpline.GetCurveCount();
            float t = 0;

            for (int i = 0; i < num_curves; i++)
            {
                for (int x = 1; x <= loopQuality; x++)
                {
                    t = i + (float)x / loopQuality;

                    Vector3 next = loopSpline.GetPoint(t, true);
                    loopPoints.Add(next);
                }
            }

            CompositeShape compShape = new CompositeShape(loopPoints);
            Mesh mesh = compShape.GetMesh();

            if (mesh == null)
            {
                return;
            }

            mesh.vertices = loopPoints.ToArray();

            List<Vector3> heightPoints = new List<Vector3>();

            num_curves = heightSpline.GetCurveCount();
            t = 0;

            for (int i = 0; i < num_curves; i++)
            {
                for (int x = 1; x <= heightQuality; x++)
                {
                    t = i + (float)x / heightQuality;

                    Vector3 next = heightSpline.GetPoint(t, true);
                    heightPoints.Add(next - mesh.vertices[mesh.vertices.Length - 1]);
                }
            }

            mesh = MeshUtilities.ExtrudeMesh(mesh, heightPoints, capBottom);
            meshFilter.mesh = mesh;
        }

        public void Recenter()
        {
            if (loopSpline == null)
                return;

            loopSpline.RecenterSpline();
        }

        public void ResetLoop()
        {
            if (loopSpline == null)
                return;

            loopSpline.Reset();
        }

        public void ResetHeight()
        {
            if (heightSpline == null)
                return;

            heightSpline.Reset();
        }
    }
}