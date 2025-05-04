using Source.Configs;
using Source.Services;
using Source.Services.Contracts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Source.Infrastructure
{
    public class EntryPointLifetimeScope : LifetimeScope
    {
        [SerializeField] private LaserConfig _laserConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IPathBuilder, PathBuilder>(Lifetime.Singleton);
            builder.RegisterInstance(_laserConfig);
        }
    }
}
