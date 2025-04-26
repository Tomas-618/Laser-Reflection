using UnityEngine;

namespace Source.Data
{
    public struct LaserVertex
    {
        public LaserVertex(Vector3 position, Transform connection = null)
        {
            Position = position;
            Connection = connection;
        }

        public Vector3 Position { get; }

        public Transform Connection { get; }
    }
}
