using UnityEngine;

namespace Source.Components.Contracts
{
    public interface IRefractor
    {
        void Refract(ref Ray ray);
    }
}
