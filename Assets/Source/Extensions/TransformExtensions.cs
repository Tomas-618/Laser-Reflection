using Source.Data;
using UnityEngine;

namespace Source.Extensions
{
    public static class TransformExtensions
    {
        public static TransformData ToData(this Transform transform) =>
            new(transform.position, transform.lossyScale, transform.rotation);
    }
}
