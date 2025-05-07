using System.Collections.Generic;
using JetBrains.Annotations;
using Source.Configs;
using Source.Services.Factories.Contracts;
using Source.Services.Path;
using Source.Services.Path.Contracts;

namespace Source.Services.Factories
{
    public class PathBuildersFactory : IPathBuilderFactory
    {
        private readonly Dictionary<LaserConfig, IPathBuilder> _pathBuilders;

        [UsedImplicitly(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
        public PathBuildersFactory() =>
            _pathBuilders = new Dictionary<LaserConfig, IPathBuilder>();

        public IPathBuilder GetOrCreate(LaserConfig laserConfig)
        {
            return _pathBuilders.TryGetValue(laserConfig, out var pathBuilder)
                ? pathBuilder
                : Create(laserConfig);
        }

        private IPathBuilder Create(LaserConfig laserConfig)
        {
            var pathBuilder = new PathBuilder(laserConfig);

            _pathBuilders.Add(laserConfig, pathBuilder);

            return pathBuilder;
        }
    }
}
