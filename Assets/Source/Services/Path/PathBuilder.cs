using System;
using System.Collections.Generic;
using Source.Components.Contracts;
using Source.Configs;
using Source.Data;
using Source.Services.Path.Contracts;
using Source.Utils;
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

            _refractorsHashes = new HashSet<IRefractor>(LaserUtils
                .CalculateVerticesCount(laserConfig.MaxRedirectionsCount));

            _length = laserConfig.Length;
            _maxRedirectionsCount = laserConfig.MaxRedirectionsCount;
            _layerMask = laserConfig.LayerMask.value;
        }

        public void BuildNonAlloc(Vector3[] vertices, LaserData data,
            out int verticesLength)
        {
            _refractorsHashes.Clear();

            var origin = data.Origin;

            var ray = data.Ray;

            int currentVertexIndex = 0;
            int redirectionsCount = 0;

            bool isStopped = false;

            vertices[currentVertexIndex] = Vector3.zero;

            while (isStopped == false &&
                   Physics.Raycast(ray, out var hit, _length, _layerMask))
            {
                SetRenderPoint(vertices, ++currentVertexIndex, origin, ref ray, hit);

                isStopped = ShouldStop(++redirectionsCount, hit, out var reflector,
                    out var refractor);

                if (isStopped)
                    continue;

                Reflect(reflector, ref ray, hit);
                Refract(vertices, ref currentVertexIndex, ref ray, origin, refractor,
                    ref isStopped);
            }

            AddExtraVertex(vertices, ref currentVertexIndex, ray, origin, isStopped);

            verticesLength = currentVertexIndex + 1;
        }

        private void SetRenderPoint(Vector3[] vertices, int currentIndex, TransformData origin,
            ref Ray ray, RaycastHit hit)
        {
            ray.origin = hit.point;
            AddVertex(vertices, currentIndex, origin, hit.point);
        }

        private void AddVertex(Vector3[] vertices, int currentIndex, TransformData origin, Vector3 point)
        {
            var localPoint = origin.InverseTransformPoint(point);

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

        private void Refract(Vector3[] vertices, ref int currentIndex, ref Ray ray,
            TransformData origin, IRefractor refractor, ref bool isStopped)
        {
            if (refractor == null)
                return;

            if (_refractorsHashes.Add(refractor) == false)
            {
                isStopped = true;

                return;
            }

            refractor.Refract(ref ray);
            AddVertex(vertices, ++currentIndex, origin, ray.origin);
        }

        private void AddExtraVertex(Vector3[] vertices, ref int currentIndex,
            Ray ray, TransformData origin, bool isStopped)
        {
            if (isStopped)
                return;

            vertices[++currentIndex] = origin.InverseTransformPoint
                (ray.GetPoint(_length));
        }
    }
}
