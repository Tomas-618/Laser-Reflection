using UnityEngine;

namespace Source.Data
{
    public readonly ref struct LaserData
    {
        public readonly TransformData Origin;
        public readonly Ray Ray;

        public LaserData(TransformData origin, Ray ray)
        {
            Origin = origin;
            Ray = ray;
        }
    }
}
