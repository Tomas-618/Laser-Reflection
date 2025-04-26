using UnityEngine;

namespace Source.Components.Contracts
{
    public interface IReflector
    {
        Vector3 Reflect(Vector3 direction, RaycastHit hit);
    }
}
