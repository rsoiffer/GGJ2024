using System;
using UnityEngine;

namespace TowerDefense
{
    public class LaneDefinition : MonoBehaviour
    {
        public Transform[] laneMarkers;

        public float TotalLength()
        {
            var length = 0f;
            for (var i = 0; i < laneMarkers.Length - 1; i++)
                length += Vector2.Distance(laneMarkers[i].position, laneMarkers[i + 1].position);
            return length;
        }

        public Vector2 Position(float lengthAlongPath)
        {
            lengthAlongPath = Mathf.Clamp(lengthAlongPath, 0, TotalLength() - 1e-3f);

            var length = 0f;
            for (var i = 0; i < laneMarkers.Length - 1; i++)
            {
                var newLength = length + Vector2.Distance(laneMarkers[i].position, laneMarkers[i + 1].position);
                if (lengthAlongPath < newLength)
                {
                    var lerp = Mathf.InverseLerp(length, newLength, lengthAlongPath);
                    return Vector2.Lerp(laneMarkers[i].position, laneMarkers[i + 1].position, lerp);
                }

                length = newLength;
            }

            throw new ArgumentException(
                $"Invalid call to Position({lengthAlongPath}), total length is {TotalLength()}");
        }
    }
}