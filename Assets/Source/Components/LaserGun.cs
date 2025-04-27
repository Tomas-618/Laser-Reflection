using UnityEngine;

namespace Source.Components
{
    public class LaserGun : MonoBehaviour
    {
        [SerializeField] private LaserRenderer _laserRenderer;
        [SerializeField] private Interactor _interactor;

        private void LateUpdate() =>
            _laserRenderer.RenderLine(_interactor.Interact);
    }
}
