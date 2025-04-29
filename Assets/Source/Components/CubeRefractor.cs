using Source.Components.Contracts;
using UnityEngine;

namespace Source.Components
{
    public class CubeRefractor : MonoBehaviour, IRefractor
    {
        private Transform _transform;

        private void Awake() =>
            _transform = transform;

        public void Refract(ref Ray ray)
        {
            ray.origin = _transform.position;
            ray.direction = _transform.forward;
        }
    }
}
