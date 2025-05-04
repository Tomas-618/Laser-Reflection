using UnityEngine;

namespace Source.Data
{
    public struct TransformData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;

        public readonly Vector3 InverseTransformPoint(Vector3 point)
        {
            var localPoint = point - Position;

            localPoint = Quaternion.Inverse(Rotation) * localPoint;

            localPoint = new Vector3(
                localPoint.x / Scale.x,
                localPoint.y / Scale.y,
                localPoint.z / Scale.z
            );

            return localPoint;
        }
    }
}
