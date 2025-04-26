using Source.Components.Contracts;
using UnityEngine;

namespace Source.Components
{
    public class Reflector : MonoBehaviour, IReflector
    {
        public Vector3 Reflect(Vector3 direction, RaycastHit hit) =>
            Vector3.Reflect(direction, hit.normal);
    }
}
