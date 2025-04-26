using System.Collections.Generic;
using System.Linq;
using Source.Components.Contracts;
using UnityEngine;

namespace Source.Components
{
    public class LaserGun : MonoBehaviour
    {
        [SerializeField] private LaserRenderer _laserRenderer;

        private List<IInteractable> _previousInteractables;

        private void Awake() =>
            _previousInteractables = new List<IInteractable>();

        private void OnDisable() =>
            _previousInteractables?.ForEach(interactable => interactable.StopInteract());

        private void LateUpdate() =>
            _laserRenderer.RenderLine(OnRenderedLine);

        private void OnRenderedLine(Transform[] connectedObjects)
        {
            var interactables = new List<IInteractable>(connectedObjects.Length);

            foreach (var connectedObject in connectedObjects)
            {
                if (connectedObject.TryGetComponent(out IInteractable interactable) == false)
                    continue;

                interactables.Add(interactable);
                interactable.StartInteract();
            }

            var exceptInteractables = _previousInteractables.Except(interactables);

            _previousInteractables = interactables;

            foreach (var interactable in exceptInteractables)
                interactable.StopInteract();
        }
    }
}
