using UnityEngine;

/*
 * Processes given arrays of hull and hole points into single array, enforcing correct -wiseness.
 * Also provides convenience methods for accessing different hull/hole points
 */

namespace Sebastian.Geometry
{
    public class Polygon
    {

        public readonly Vector2[] points;
        public readonly int numPoints;

        public readonly int numHullPoints;

        public readonly int[] numPointsPerHole;
        public readonly int numHoles;

        readonly int[] holeStartIndices;

        public Polygon(Vector2[] hull)
        {
            numHullPoints = hull.Length;

            numPointsPerHole = new int[numHoles];
            holeStartIndices = new int[numHoles];
            int numHolePointsSum = 0;

            numPoints = numHullPoints + numHolePointsSum;
            points = new Vector2[numPoints];

            // add hull points, ensuring they wind in counterclockwise order
            bool reverseHullPointsOrder = !PointsAreCounterClockwise(hull);
            for (int i = 0; i < numHullPoints; i++)
            {
                points[i] = hull[(reverseHullPointsOrder) ? numHullPoints - 1 - i : i];
            }
        }

        bool PointsAreCounterClockwise(Vector2[] testPoints)
        {
            float signedArea = 0;
            for (int i = 0; i < testPoints.Length; i++)
            {
                int nextIndex = (i + 1) % testPoints.Length;
                signedArea += (testPoints[nextIndex].x - testPoints[i].x) * (testPoints[nextIndex].y + testPoints[i].y);
            }

            return signedArea < 0;
        }

        public int IndexOfFirstPointInHole(int holeIndex)
        {
            return holeStartIndices[holeIndex];
        }

        public int IndexOfPointInHole(int index, int holeIndex)
        {
            return holeStartIndices[holeIndex] + index;
        }

        public Vector2 GetHolePoint(int index, int holeIndex)
        {
            return points[holeStartIndices[holeIndex] + index];
        }

    }

}