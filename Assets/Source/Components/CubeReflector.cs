using Source.Components.Contracts;
using UnityEngine;

namespace Source.Components
{
    public class CubeReflector : MonoBehaviour, IInteractable
    {
        [SerializeField] private LaserRenderer _laserRenderer;
        [SerializeField] private Interactor _interactor;

        private bool _isInteracted;

        private void LateUpdate()
        {
            if (_isInteracted)
                _laserRenderer.RenderLine(_interactor.Interact);
        }

        public void StartInteract()
        {
            if (_isInteracted)
                return;

            _isInteracted = true;
        }

        public void StopInteract()
        {
            if (_isInteracted == false)
                return;

            _isInteracted = false;
            _laserRenderer.Clear();
            _interactor.Disable();
        }
    }
}
