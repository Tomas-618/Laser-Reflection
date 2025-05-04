using System;
using Source.Configs;
using Source.Extensions;
using Source.Services.Contracts;
using UnityEngine;
using VContainer;

namespace Source.Components
{
    public class LaserRenderer : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _lineLength;
        [SerializeField, Min(0)] private int _maxReflectionsCount;

        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LayerMask _layerMask;

        private Vector3[] _vertices;
        private Transform _transform;
        private IPathBuilder _pathBuilder;

        [Inject]
        private void Construct(IPathBuilder pathBuilder, LaserConfig laserConfig)
        {
            if (laserConfig == null)
                throw new ArgumentNullException(nameof(laserConfig));

            _pathBuilder = pathBuilder ?? throw new ArgumentNullException(nameof(pathBuilder));
            _vertices = new Vector3[laserConfig.VerticesCapacity];
        }

        private void Awake() =>
            _transform = transform;

        private void OnDisable()
        {
            if (_lineRenderer)
                _lineRenderer.positionCount = 0;
        }

        public void Render()
        {
            var ray = new Ray(_transform.position, _transform.forward);

            _pathBuilder.BuildNonAlloc(_vertices, ray, _transform.ToData(), _lineLength,
                out int verticesLength, _maxReflectionsCount, _layerMask.value);

            RenderPath(verticesLength);
        }

        private void RenderPath(int verticesLength)
        {
            _lineRenderer.positionCount = verticesLength;
            _lineRenderer.SetPositions(_vertices);
        }
    }
}
