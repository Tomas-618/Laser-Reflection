using UnityEngine;

namespace Source.Configs
{
    [CreateAssetMenu(fileName = "LaserConfig", menuName = "Configs/LaserConfig")]
    public class LaserConfig : ScriptableObject
    {
        public float Length;
        public int MaxRedirectionsCount;
        public int VerticesCapacity;
        public LayerMask LayerMask;
    }
}
