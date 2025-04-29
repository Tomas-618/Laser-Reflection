using System.Collections;
using System.Collections.Generic;
using Source.Components.Contracts;
using Source.Data;
using UnityEngine;

namespace Source.Components
{
    public class LaserRenderer : MonoBehaviour
    {
        private readonly List<LaserVertex> _vertices = new();
        private readonly HashSet<IRefractor> _refractors = new();

        [SerializeField, Min(0f)] private float _lineLength;
        [SerializeField, Min(0)] private int _maxReflectionsCount;
        [SerializeField, Min(0)] private int _verticesCapacity;

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LayerMask _layerMask;

        private Transform _transform;
        private Coroutine _coroutine;

        private void OnValidate()
        {
            _verticesCapacity = Mathf.Clamp(_verticesCapacity, _maxReflectionsCount, int.MaxValue);
            _refractors.EnsureCapacity(_verticesCapacity);
        }

        private void Awake()
        {
            _transform = transform;
            _vertices.Capacity = _verticesCapacity;
        }

        private void OnDisable() =>
            Clear();

        public void RenderLine() =>
            _coroutine ??= StartCoroutine(BuildLinePath());

        private void Clear()
        {
            _vertices.Clear();
            StopCoroutine();

            if (_lineRenderer)
                _lineRenderer.positionCount = 0;
        }

        private IEnumerator BuildLinePath()
        {
            _vertices.Clear();
            _vertices.Add(new LaserVertex(Vector3.zero, _transform));

            _refractors.Clear();

            var ray = new Ray(_transform.position, _transform.forward);

            int reflectionsCount = 0;

            bool isStopped = false;

            while (isStopped == false &&
                   Physics.Raycast(ray, out var hit, _lineLength, _layerMask.value))
            {
                SetRenderPoint(hit, ref ray);

                isStopped = ShouldStop(++reflectionsCount, hit, out var reflector, out var refractor);

                if (isStopped)
                    continue;

                Reflect(reflector, ref ray, hit);
                Refract(refractor, ref ray, ref isStopped, hit);

                yield return null;
            }

            AddExtraVertex(isStopped, ray);
            RenderPath();

            _coroutine = null;
        }

        private void SetRenderPoint(RaycastHit hit, ref Ray ray)
        {
            ray.origin = hit.point;
            AddVertex(hit.point, hit.transform);
        }

        private void AddVertex(Vector3 point, Transform connectedObject)
        {
            var localPoint = _transform.InverseTransformPoint(point);

            _vertices.Add(new LaserVertex(localPoint, connectedObject));
        }

        private bool ShouldStop(int reflectionsCount, RaycastHit hit, out IReflector reflector,
            out IRefractor refractor)
        {
            reflector = null;
            refractor = null;

            return reflectionsCount > _maxReflectionsCount ||
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

        private void Refract(IRefractor refractor, ref Ray ray, ref bool isStopped, RaycastHit hit)
        {
            if (refractor == null)
                return;

            if (_refractors.Add(refractor) == false)
            {
                isStopped = true;

                return;
            }

            refractor.Refract(ref ray);
            AddVertex(ray.origin, hit.transform);
        }

        private void AddExtraVertex(bool isStopped, Ray ray)
        {
            if (isStopped)
                return;

            _vertices.Add(new LaserVertex(_transform.InverseTransformPoint
                (ray.GetPoint(_lineLength))));
        }

        private void RenderPath()
        {
            _lineRenderer.positionCount = _vertices.Count;

            for (int i = 0; i < _vertices.Count; i++)
                _lineRenderer.SetPosition(i, _vertices[i].Position);
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
