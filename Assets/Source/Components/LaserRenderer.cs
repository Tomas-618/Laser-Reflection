using Source.Configs;
using Source.Data;
using Source.Extensions;
using Source.Services;
using Source.Services.Contracts;
using UnityEngine;

namespace Source.Components
{
    public class LaserRenderer : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float _lineLength;
        [SerializeField, Min(0)] private int _maxRedirectionsCount;

        [SerializeField] private LaserConfig _laserConfig;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LayerMask _layerMask;

        private Vector3[] _vertices;
        private Transform _transform;
        private IPathBuilder _pathBuilder;

        private void Awake()
        {
            _pathBuilder = new PathBuilder(_laserConfig);
            _vertices = new Vector3[_laserConfig.VerticesCapacity];

            _transform = transform;
        }

        private void OnDisable()
        {
            if (_lineRenderer)
                _lineRenderer.positionCount = 0;
        }

        public void Render()
        {
            var laserData = new LaserData
            {
                Ray = new Ray(_transform.position, _transform.forward),
                Origin = _transform.ToData(),
                Length = _lineLength,
                MaxRedirectionsCount = _maxRedirectionsCount,
                LayerMask = _layerMask.value
            };

            _pathBuilder.BuildNonAlloc(_vertices, laserData, out int verticesLength);

            RenderPath(verticesLength);
        }

        private void RenderPath(int verticesLength)
        {
            _lineRenderer.positionCount = verticesLength;
            _lineRenderer.SetPositions(_vertices);
        }
    }
}
