using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Source.Data;
using UnityEngine;

namespace Source.Components
{
    public class LaserRenderer : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _lineLength;
        [SerializeField, Min(0)] private int _maxReflectionsCount;

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LayerMask _layerMask;

        private Transform _transform;
        private Coroutine _coroutine;

        private void Awake() =>
            _transform = transform;

        private void OnDisable() =>
            Clear();

        public void RenderLine(Action<Transform[]> callback = null) =>
            _coroutine ??= StartCoroutine(BuildLinePath(callback));

        public void Clear()
        {
            StopCoroutine();

            if (_lineRenderer)
                _lineRenderer.positionCount = 0;
        }

        private IEnumerator BuildLinePath(Action<Transform[]> callback)
        {
            int extraPointsCount = 2;

            var vertices = new List<LaserVertex>(_maxReflectionsCount + extraPointsCount)
            {
                new(Vector3.zero, _transform)
            };

            var ray = new Ray(_transform.position, _transform.forward);

            int reflectionsCount = 0;

            bool isStopped = false;

            while (isStopped == false &&
                   Physics.Raycast(ray, out var hit, _lineLength, _layerMask.value))
            {
                SetNextRenderPoint(vertices, ref ray, hit);

                if (ShouldStop(hit, ++reflectionsCount, out var reflector))
                {
                    isStopped = true;

                    continue;
                }

                ray.direction = reflector.Reflect(ray.direction, hit);

                yield return null;
            }

            AddExtraVertex(vertices, ray, isStopped);
            RenderPath(vertices);

            _coroutine = null;
            callback?.Invoke(vertices
                .Select(vertex => vertex.Connection)
                .Where(connection => connection)
                .ToArray());
        }

        private void AddExtraVertex(List<LaserVertex> vertices, Ray ray, bool isStopped)
        {
            if (isStopped)
                return;

            vertices.Add(new LaserVertex(_transform.InverseTransformPoint
                (ray.GetPoint(_lineLength))));
        }

        private void SetNextRenderPoint(List<LaserVertex> vertices, ref Ray ray, RaycastHit hit)
        {
            var localHitPoint = _transform.InverseTransformPoint(hit.point);

            ray.origin = hit.point;
            vertices.Add(new LaserVertex(localHitPoint, hit.transform));
        }

        private bool ShouldStop(RaycastHit hit, int reflectionsCount, out Reflector reflector)
        {
            reflector = null;

            return reflectionsCount > _maxReflectionsCount ||
                   hit.collider.TryGetComponent(out reflector) == false;
        }

        private void RenderPath(List<LaserVertex> vertices)
        {
            _lineRenderer.positionCount = vertices.Count;

            for (int i = 0; i < vertices.Count; i++)
                _lineRenderer.SetPosition(i, vertices[i].Position);
        }

        private void StopCoroutine()
        {
            if (_coroutine == null)
                return;

            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
}
