using UnityEngine;

namespace Source.Data
{
    public readonly ref struct TransformData
    {
        private readonly Vector3 _position;
        private readonly Vector3 _scale;
        private readonly Quaternion _rotation;

        public TransformData(Vector3 position, Vector3 scale, Quaternion rotation)
        {
            _position = position;
            _scale = scale;
            _rotation = rotation;
        }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            var localPoint = point - _position;

            localPoint = Quaternion.Inverse(_rotation) * localPoint;

            localPoint = new Vector3(
                localPoint.x / _scale.x,
                localPoint.y / _scale.y,
                localPoint.z / _scale.z
            );

            return localPoint;
        }
    }
}
