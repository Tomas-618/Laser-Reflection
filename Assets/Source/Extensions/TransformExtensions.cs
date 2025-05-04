using Source.Data;
using UnityEngine;

namespace Source.Extensions
{
    public static class TransformExtensions
    {
        public static TransformData ToData(this Transform transform)
        {
            return new TransformData
            {
                Position = transform.position,
                Rotation = transform.rotation,
                Scale = transform.lossyScale
            };
        }
    }
}
