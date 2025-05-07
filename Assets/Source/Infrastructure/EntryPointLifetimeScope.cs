using Source.Services.Factories;
using Source.Services.Factories.Contracts;
using VContainer;
using VContainer.Unity;

namespace Source.Infrastructure
{
    public class EntryPointLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder) =>
            builder.Register<IPathBuilderFactory, PathBuildersFactory>(Lifetime.Singleton);
    }
}
