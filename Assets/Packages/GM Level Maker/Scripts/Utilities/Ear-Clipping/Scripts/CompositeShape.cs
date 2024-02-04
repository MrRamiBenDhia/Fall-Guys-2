using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/*
 * Processes array of shapes into a single mesh
 * Automatically determines which shapes are solid, and which are holes
 * Ignores invalid shapes (contain self-intersections, too few points, overlapping holes)
 */

namespace Sebastian.Geometry
{
    public partial class CompositeShape
    {
        public Vector3[] vertices;
        public int[] triangles;

        List<Vector3> points;
        float height = 0;

        public CompositeShape(List<Vector3> points)
        {
            this.points = points;
        }

        public Mesh GetMesh()
        {
            Process();

            if (vertices == null || triangles == null)
                return null;

            return new Mesh()
            {
                vertices = vertices,
                triangles = triangles,
                normals = vertices.Select(x => Vector3.up).ToArray()
            };
        }

        public void Process()
        {
            CompositeShapeData shapeData = new CompositeShapeData(points.ToArray());

            if(!shapeData.IsValidShape)
            {
                return;
            }

            Polygon poly = new Polygon(shapeData.polygon.points);

            // Flatten the points arrays from all polygons into a single array, and convert the vector2s to vector3s.
            vertices = poly.points.Select(v2 => new Vector3(v2.x, height, v2.y)).ToArray();
            triangles = shapeData.triangles;
        }
    }
}