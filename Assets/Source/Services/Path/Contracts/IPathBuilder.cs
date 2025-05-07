using Source.Data;
using UnityEngine;

namespace Source.Services.Path.Contracts
{
    public interface IPathBuilder
    {
        void BuildNonAlloc(Vector3[] vertices, LaserData data,
            out int verticesLength);
    }
}
