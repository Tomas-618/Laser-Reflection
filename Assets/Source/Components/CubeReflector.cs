using Source.Components.Contracts;
using UnityEngine;

namespace Source.Components
{
    public class CubeReflector : MonoBehaviour, IInteractable
    {
        [SerializeField] private LaserRenderer _laserRenderer;

        public void StartInteract() =>
            _laserRenderer.RenderLine();

        public void StopInteract() =>
            _laserRenderer.Clear();
    }
}
