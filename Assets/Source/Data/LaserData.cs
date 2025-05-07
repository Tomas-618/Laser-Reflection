using UnityEngine;

namespace Source.Data
{
    public struct LaserData
    {
        public Ray Ray;
        public TransformData Origin;
        public float Length;
        public int MaxRedirectionsCount;
        public int LayerMask;
    }
}
