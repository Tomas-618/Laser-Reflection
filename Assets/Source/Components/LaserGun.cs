using UnityEngine;

namespace Source.Components
{
    public class LaserGun : MonoBehaviour
    {
        [SerializeField] private LaserRenderer _laserRenderer;

        private void LateUpdate() =>
            _laserRenderer.RenderLine();
    }
}
