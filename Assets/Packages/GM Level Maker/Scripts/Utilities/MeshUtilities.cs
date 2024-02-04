using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GM.LevelMaker
{
    public class MeshUtilities
    {
        public static Mesh OffsetMesh(Mesh _mesh, Vector3 _offset)
        {
            if (_offset == Vector3.zero)
                return _mesh;

            Vector3[] verts = _mesh.vertices;

            for (int vertI = 0; vertI < verts.Length; vertI++)
            {
                verts[vertI] += _offset;
            }

            _mesh.vertices = verts;

            return _mesh;
        }

        public static Mesh ReorientMesh(Mesh _mesh, Transform _orientAround)
        {
            if (_orientAround.rotation == Quaternion.identity)
                return _mesh;

            Vector3[] verts = _mesh.vertices;

            for (int vertI = 0; vertI < verts.Length; vertI++)
            {
                verts[vertI] = _orientAround.InverseTransformPoint(verts[vertI]);
            }

            _mesh.vertices = verts;

            return _mesh;
        }

        public static Mesh ExtrudeMesh(Mesh _mesh, List<Vector3> _extrusionPoints, bool _includeBottom = true)
        {
            if (_extrusionPoints[_extrusionPoints.Count - 1] == Vector3.zero)
                return _mesh;

            int trianlgeCount = _mesh.triangles.Length;

            List<Vector3> verts = _mesh.vertices.ToList();
            List<int> tris = _mesh.triangles.ToList();

            // Generate some temporary normals to use when reading the side spline
            Mesh tempNormalsMesh = GetNormalsMesh(_mesh);
            List<Vector3> trimmedTempNormals = tempNormalsMesh.normals.ToList();
            trimmedTempNormals.RemoveRange(0, trimmedTempNormals.Count - _mesh.vertices.Length);

            // First try adding one row of triangles
            List<Vector3> newVerts = new List<Vector3>();

            for (int extrusionID = 0; extrusionID < _extrusionPoints.Count; extrusionID++)
            {
                if (extrusionID == 0)
                {// this is only needed for the first extrusion point
                    verts.AddRange(verts); // add duplicates of the last verts so we have flat normals
                }

                newVerts.Clear();
                newVerts.AddRange(_mesh.vertices); // add new set of verts

                for (int i = 0; i < newVerts.Count; i++)
                {
                    Vector3 diff = _extrusionPoints[extrusionID];
                    float angle = Vector3.SignedAngle(trimmedTempNormals[0], trimmedTempNormals[i], Vector3.up);
                    diff = Quaternion.Euler(0, angle, 0) * diff;

                    newVerts[i] += diff; // Add the new extrusion ammount to each vert
                }

                verts.AddRange(newVerts);

                // Add the triangles to make up the side faces

                for (int vertI = 0; vertI < _mesh.vertices.Length; vertI++)
                {
                    if (vertI < _mesh.vertices.Length - 1)
                    {
                        tris.Add(verts.Count - _mesh.vertices.Length - _mesh.vertices.Length + vertI);
                        tris.Add(verts.Count - _mesh.vertices.Length - _mesh.vertices.Length + vertI + 1);
                        tris.Add(verts.Count - _mesh.vertices.Length + vertI + 1);

                        tris.Add(verts.Count - _mesh.vertices.Length - _mesh.vertices.Length + vertI);
                        tris.Add(verts.Count - _mesh.vertices.Length + vertI + 1);
                        tris.Add(verts.Count - _mesh.vertices.Length + vertI);
                    }
                    else
                    {
                        tris.Add(verts.Count - _mesh.vertices.Length - _mesh.vertices.Length + vertI);
                        tris.Add(verts.Count - _mesh.vertices.Length - _mesh.vertices.Length);
                        tris.Add(verts.Count - _mesh.vertices.Length);

                        tris.Add(verts.Count - _mesh.vertices.Length - _mesh.vertices.Length + vertI);
                        tris.Add(verts.Count - _mesh.vertices.Length);
                        tris.Add(verts.Count - _mesh.vertices.Length + vertI);
                    }
                }
            }

            // Add the bottom side
            if (_includeBottom)
            {
                List<int> newTris = new List<int>();
                newTris.AddRange(_mesh.triangles.Reverse().ToList());

                for (int triI = 0; triI < _mesh.triangles.Length; triI++)
                {
                    newTris[triI] += verts.Count;
                }

                verts.AddRange(newVerts);
                tris.AddRange(newTris);
            }

            // apply to mesh

            _mesh.vertices = verts.ToArray();
            _mesh.triangles = tris.ToArray();

            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();

            _mesh.subMeshCount = 4;

            UnityEngine.Rendering.SubMeshDescriptor subDescriptor = new UnityEngine.Rendering.SubMeshDescriptor(0, trianlgeCount);
            _mesh.SetSubMesh(0, subDescriptor);

            subDescriptor = new UnityEngine.Rendering.SubMeshDescriptor(trianlgeCount, (trianlgeCount * 2) + 12);
            _mesh.SetSubMesh(1, subDescriptor);

            subDescriptor = new UnityEngine.Rendering.SubMeshDescriptor((trianlgeCount * 3) + 12, (tris.Count - (trianlgeCount * 4)) - 12);
            _mesh.SetSubMesh(2, subDescriptor);

            subDescriptor = new UnityEngine.Rendering.SubMeshDescriptor(tris.Count - trianlgeCount, trianlgeCount);
            _mesh.SetSubMesh(3, subDescriptor);

            return _mesh;
        }

        static Mesh GetNormalsMesh(Mesh _original)
        {
            Mesh newMesh = new Mesh();

            newMesh.vertices = _original.vertices;
            newMesh.triangles = _original.triangles;

            List<Vector3> verts = newMesh.vertices.ToList();
            List<int> tris = newMesh.triangles.ToList();

            List<Vector3> newVerts = newMesh.vertices.ToList();

            // Make mirrored side

            for (int vertI = 0; vertI < newVerts.Count; vertI++)
            {
                newVerts[vertI] += Vector3.down;
            }

            // add new to existing

            verts.AddRange(newVerts);

            // Add side triangles

            for (int vertI = 0; vertI < newMesh.vertices.Length; vertI++)
            {
                if (vertI < newMesh.vertices.Length - 1)
                {
                    tris.Add(verts.Count + vertI);
                    tris.Add(verts.Count + vertI + 1);
                    tris.Add(verts.Count + vertI + 1 + newMesh.vertices.Length);

                    tris.Add(verts.Count + vertI);
                    tris.Add(verts.Count + vertI + 1 + newMesh.vertices.Length);
                    tris.Add(verts.Count + vertI + newMesh.vertices.Length);
                }
                else
                {
                    tris.Add(verts.Count + vertI);
                    tris.Add(verts.Count + 0);
                    tris.Add(verts.Count + newMesh.vertices.Length);

                    tris.Add(verts.Count + vertI);
                    tris.Add(verts.Count + newMesh.vertices.Length);
                    tris.Add(verts.Count + vertI + newMesh.vertices.Length);
                }
            }

            verts.AddRange(verts);

            // apply to mesh

            newMesh.vertices = verts.ToArray();
            newMesh.triangles = tris.ToArray();

            newMesh.RecalculateNormals();
            return newMesh;
        }
    }
}