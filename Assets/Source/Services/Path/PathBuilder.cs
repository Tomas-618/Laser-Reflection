using System;
using System.Collections.Generic;
using Source.Components.Contracts;
using Source.Configs;
using Source.Data;
using Source.Services.Path.Contracts;
using UnityEngine;

namespace Source.Services.Path
{
    public class PathBuilder : IPathBuilder
    {
        private readonly HashSet<IRefractor> _refractorsHashes;
        private readonly float _length;
        private readonly int _maxRedirectionsCount;
        private readonly int _layerMask;

        public PathBuilder(LaserConfig laserConfig)
        {
            if (laserConfig == null)
                throw new ArgumentNullException(nameof(laserConfig));

            _refractorsHashes = new HashSet<IRefractor>(laserConfig.VerticesCapacity);
            _length = laserConfig.Length;
            _maxRedirectionsCount = laserConfig.MaxRedirectionsCount;
            _layerMask = laserConfig.LayerMask.value;
        }

        public void BuildNonAlloc(Vector3[] vertices, LaserData data,
            out int verticesLength)
        {
            _refractorsHashes.Clear();

            int currentVertexIndex = 0;
            int redirectionsCount = 0;

            bool isStopped = false;

            vertices[currentVertexIndex] = Vector3.zero;

            while (isStopped == false &&
                   Physics.Raycast(data.Ray, out var hit, _length, _layerMask))
            {
                SetRenderPoint(vertices, ++currentVertexIndex, ref data, hit);

                isStopped = ShouldStop(++redirectionsCount, hit, out var reflector,
                    out var refractor);

                if (isStopped)
                    continue;

                Reflect(reflector, ref data.Ray, hit);
                Refract(vertices, ref currentVertexIndex, ref data, refractor, ref isStopped);
            }

            AddExtraVertex(vertices, ref currentVertexIndex, data, isStopped);

            verticesLength = currentVertexIndex + 1;
        }

        private void SetRenderPoint(Vector3[] vertices, int currentIndex, ref LaserData data,
            RaycastHit hit)
        {
            data.Ray.origin = hit.point;
            AddVertex(vertices, currentIndex, data.Origin, hit.point);
        }

        private void AddVertex(Vector3[] vertices, int currentIndex, TransformData transform, Vector3 point)
        {
            var localPoint = transform.InverseTransformPoint(point);

            vertices[currentIndex] = localPoint;
        }

        private bool ShouldStop(int redirectionsCount, RaycastHit hit, out IReflector reflector,
            out IRefractor refractor)
        {
            reflector = null;
            refractor = null;

            return redirectionsCount > _maxRedirectionsCount ||
                   CheckAnyComponent(hit.collider, out reflector, ref refractor) == false;
        }

        private bool CheckAnyComponent(Collider other, out IReflector reflector, ref IRefractor refractor)
        {
            return other.TryGetComponent(out reflector) ||
                   other.TryGetComponent(out refractor);
        }

        private void Reflect(IReflector reflector, ref Ray ray, RaycastHit hit)
        {
            if (reflector != null)
                ray.direction = reflector.Reflect(ray.direction, hit);
        }

        private void Refract(Vector3[] vertices, ref int currentIndex, ref LaserData data,
            IRefractor refractor, ref bool isStopped)
        {
            if (refractor == null)
                return;

            if (_refractorsHashes.Add(refractor) == false)
            {
                isStopped = true;

                return;
            }

            refractor.Refract(ref data.Ray);
            AddVertex(vertices, ++currentIndex, data.Origin, data.Ray.origin);
        }

        private void AddExtraVertex(Vector3[] vertices, ref int currentIndex,
            LaserData data, bool isStopped)
        {
            if (isStopped)
                return;

            vertices[++currentIndex] = data.Origin.InverseTransformPoint
                (data.Ray.GetPoint(_length));
        }
    }
}
