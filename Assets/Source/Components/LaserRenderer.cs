using System;
using Source.Configs;
using Source.Data;
using Source.Extensions;
using Source.Services.Factories.Contracts;
using Source.Services.Path.Contracts;
using Source.Utils;
using UnityEngine;
using VContainer;

namespace Source.Components
{
    public class LaserRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private LaserConfig _laserConfig;

        private Vector3[] _vertices;
        private Transform _transform;
        private IPathBuilder _pathBuilder;

        [Inject]
        private void Construct(IPathBuilderFactory pathBuilderFactory)
        {
            if (pathBuilderFactory == null)
                throw new ArgumentNullException(nameof(pathBuilderFactory));

            _pathBuilder = pathBuilderFactory.GetOrCreate(_laserConfig);
            _vertices = new Vector3[LaserUtils.CalculateVerticesCount
                (_laserConfig.MaxRedirectionsCount)];

            _transform = transform;
        }

        private void OnDisable()
        {
            if (_lineRenderer)
                _lineRenderer.positionCount = 0;
        }

        public void Render()
        {
            var laserData = new LaserData(_transform.ToData(),
                new Ray(_transform.position, _transform.forward));

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
