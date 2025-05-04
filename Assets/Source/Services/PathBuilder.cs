using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Source.Components.Contracts;
using Source.Configs;
using Source.Data;
using Source.Services.Contracts;
using UnityEngine;

namespace Source.Services
{
    public class PathBuilder : IPathBuilder
    {
        private readonly HashSet<IRefractor> _refractorsHashes;

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
        public PathBuilder(LaserConfig laserConfig)
        {
            if (laserConfig == null)
                throw new ArgumentNullException(nameof(laserConfig));

            _refractorsHashes = new HashSet<IRefractor>(laserConfig.VerticesCapacity);
        }

        public void BuildNonAlloc(Vector3[] vertices, Ray ray, TransformData transform,
            float lineLength, out int verticesLength, int maxReflectionsCount, int layerMask)
        {
            _refractorsHashes.Clear();

            int currentVertexIndex = 0;
            int reflectionsCount = 0;

            bool isStopped = false;

            vertices[currentVertexIndex] = Vector3.zero;

            while (isStopped == false &&
                   Physics.Raycast(ray, out var hit, lineLength, layerMask))
            {
                SetRenderPoint(vertices, ++currentVertexIndex, transform, hit, ref ray);

                isStopped = ShouldStop(++reflectionsCount, maxReflectionsCount,
                    hit, out var reflector, out var refractor);

                if (isStopped)
                    continue;

                Reflect(reflector, ref ray, hit);
                Refract(vertices, ref currentVertexIndex, transform, refractor, ref ray, ref isStopped);
            }

            AddExtraVertex(vertices, ref currentVertexIndex, transform, lineLength, isStopped, ray);

            verticesLength = currentVertexIndex + 1;
        }

        private void SetRenderPoint(Vector3[] vertices, int currentIndex, TransformData transform,
            RaycastHit hit, ref Ray ray)
        {
            ray.origin = hit.point;
            AddVertex(vertices, currentIndex, transform, hit.point);
        }

        private void AddVertex(Vector3[] vertices, int currentIndex, TransformData transform, Vector3 point)
        {
            var localPoint = transform.InverseTransformPoint(point);

            vertices[currentIndex] = localPoint;
        }

        private bool ShouldStop(int reflectionsCount, int maxVerticesCount, RaycastHit hit,
            out IReflector reflector, out IRefractor refractor)
        {
            reflector = null;
            refractor = null;

            return reflectionsCount > maxVerticesCount ||
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

        private void Refract(Vector3[] vertices, ref int currentIndex, TransformData transform,
            IRefractor refractor, ref Ray ray, ref bool isStopped)
        {
            if (refractor == null)
                return;

            if (_refractorsHashes.Add(refractor) == false)
            {
                isStopped = true;

                return;
            }

            refractor.Refract(ref ray);
            AddVertex(vertices, ++currentIndex, transform, ray.origin);
        }

        private void AddExtraVertex(Vector3[] vertices, ref int currentIndex, TransformData transform,
            float lineLength, bool isStopped, Ray ray)
        {
            if (isStopped)
                return;

            vertices[++currentIndex] = transform.InverseTransformPoint
                (ray.GetPoint(lineLength));
        }
    }
}
