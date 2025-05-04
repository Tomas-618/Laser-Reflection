using Source.Data;
using UnityEngine;

namespace Source.Services.Contracts
{
    public interface IPathBuilder
    {
        void BuildNonAlloc(Vector3[] vertices, Ray ray, TransformData transform,
            float lineLength, out int verticesLength, int maxRedirectionsCount, int layerMask);
    }
}
