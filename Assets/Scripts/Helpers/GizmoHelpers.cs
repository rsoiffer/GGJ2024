using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Helpers
{
    public sealed class ColorScope : IDisposable
    {
        private readonly Color _oldColor;

        public ColorScope(Color color)
        {
            _oldColor = Gizmos.color;
            Gizmos.color = color == default ? _oldColor : color;
        }

        public void Dispose()
        {
            Gizmos.color = _oldColor;
        }
    }

    // Adapted from https://gist.github.com/unitycoder/58f4b5d80f423d29e35c814a9556f9d9
    public static class GizmoHelpers
    {
        [PublicAPI]
        public static void DrawBounds(Bounds bounds, Color color = default)
        {
            Vector3 ruf = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, bounds.extents.z),
                rub = bounds.center + new Vector3(bounds.extents.x, bounds.extents.y, -bounds.extents.z),
                luf = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, bounds.extents.z),
                lub = bounds.center + new Vector3(-bounds.extents.x, bounds.extents.y, -bounds.extents.z),
                rdf = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, bounds.extents.z),
                rdb = bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, -bounds.extents.z),
                lfd = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, bounds.extents.z),
                lbd = bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, -bounds.extents.z);

            using (new ColorScope(color))
            {
                Gizmos.DrawLine(ruf, luf);
                Gizmos.DrawLine(ruf, rub);
                Gizmos.DrawLine(luf, lub);
                Gizmos.DrawLine(rub, lub);

                Gizmos.DrawLine(ruf, rdf);
                Gizmos.DrawLine(rub, rdb);
                Gizmos.DrawLine(luf, lfd);
                Gizmos.DrawLine(lub, lbd);

                Gizmos.DrawLine(rdf, lfd);
                Gizmos.DrawLine(rdf, rdb);
                Gizmos.DrawLine(lfd, lbd);
                Gizmos.DrawLine(lbd, rdb);
            }
        }

        [PublicAPI]
        public static void DrawCircle(
            Vector3 position,
            Vector3 up = default,
            Color color = default,
            float radius = 1.0f
        )
        {
            up = (up == default ? Vector3.up : up).normalized * radius;
            var forward = Vector3.Slerp(up, -up, 0.5f);
            var right = Vector3.Cross(up, forward).normalized * radius;

            var matrix = new Matrix4x4
            {
                m00 = right.x,
                m10 = right.y,
                m20 = right.z,
                m01 = up.x,
                m11 = up.y,
                m21 = up.z,
                m02 = forward.x,
                m12 = forward.y,
                m22 = forward.z
            };

            var lastPoint = position + matrix.MultiplyPoint3x4(new Vector3(Mathf.Cos(0), 0, Mathf.Sin(0)));

            using (new ColorScope(color))
            {
                for (var i = 0; i <= 90; i++)
                {
                    var nextPoint = position
                                    + matrix.MultiplyPoint3x4(
                                        new Vector3(Mathf.Cos(i * 4 * Mathf.Deg2Rad), 0f,
                                            Mathf.Sin(i * 4 * Mathf.Deg2Rad))
                                    );
                    Gizmos.DrawLine(lastPoint, nextPoint);
                    lastPoint = nextPoint;
                }
            }
        }

        [PublicAPI]
        public static void DrawCone(Vector3 position, Vector3 direction, Color color = default, float angle = 45)
        {
            var length = direction.magnitude;
            angle = Mathf.Clamp(angle, 0f, 90f);

            var forward = direction;
            var up = Vector3.Slerp(forward, -forward, 0.5f);
            var right = Vector3.Cross(forward, up).normalized * length;
            var slerpedVector = Vector3.Slerp(forward, up, angle / 90.0f);

            var farPlane = new Plane(-direction, position + forward);
            var distRay = new Ray(position, slerpedVector);

            farPlane.Raycast(distRay, out var dist);

            using (new ColorScope(color))
            {
                Gizmos.DrawRay(position, slerpedVector.normalized * dist);
                Gizmos.DrawRay(position, Vector3.Slerp(forward, -up, angle / 90.0f).normalized * dist);
                Gizmos.DrawRay(position, Vector3.Slerp(forward, right, angle / 90.0f).normalized * dist);
                Gizmos.DrawRay(position, Vector3.Slerp(forward, -right, angle / 90.0f).normalized * dist);
            }

            DrawCircle(position + forward, direction, color, (forward - slerpedVector.normalized * dist).magnitude);
            DrawCircle(
                position + forward * 0.5f,
                direction,
                color,
                (forward * 0.5f - slerpedVector.normalized * (dist * 0.5f)).magnitude
            );
        }

        [PublicAPI]
        public static void DrawArrow(
            Vector3 position,
            Vector3 direction,
            Color color = default,
            float angle = 15f,
            float headLength = 0.3f
        )
        {
            if (direction == Vector3.zero) return; // can't draw a thing
            if (angle < 0f) angle = Mathf.Abs(angle);
            if (angle > 0f)
            {
                var length = direction.magnitude;
                var arrowLength = length * Mathf.Clamp01(headLength);
                var headDir = direction.normalized * -arrowLength;
                DrawCone(position + direction, headDir, color, angle);
            }

            using (new ColorScope(color))
            {
                Gizmos.DrawRay(position, direction);
            }
        }
    }
}