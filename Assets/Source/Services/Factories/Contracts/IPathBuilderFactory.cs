using Source.Configs;
using Source.Services.Path.Contracts;

namespace Source.Services.Factories.Contracts
{
    public interface IPathBuilderFactory
    {
        IPathBuilder GetOrCreate(LaserConfig laserConfig);
    }
}
