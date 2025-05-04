using UnityEngine;

namespace Source.Configs
{
    [CreateAssetMenu(fileName = "LaserConfig", menuName = "Configs/LaserConfig")]
    public class LaserConfig : ScriptableObject
    {
        public int VerticesCapacity;
    }
}
