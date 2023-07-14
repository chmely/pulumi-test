using LetsGetChecked.Infrastructure.Misc;
using Pulumi;
using Pulumi.Aws;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LetsGetChecked.Infrastructure.Builders
{
    static class RegionProviders
    {
        private static readonly Dictionary<string, CustomResourceOptions> _regionProviders = new();

        public static Pulumi.CustomResourceOptions GetResourceOptions(Region region)
        {
            var key = region.ToString();
            if (_regionProviders.TryGetValue(key, out var cro))
                return cro;

            cro = new Pulumi.CustomResourceOptions
            {
                Provider = new Provider(
                    $"regionProvider-{region}",
                    new ProviderArgs
                    {
                        Region = region.ToString()
                    })
            };

            _regionProviders[key] = cro;

            //return cro;
            return new CustomResourceOptions();
        }
    }

    abstract class ResourceBuilder<TParentBuilder, TResource> : IResourceBuilder where TResource : Pulumi.CustomResource
    {
        protected readonly Region _region;
        protected readonly TParentBuilder _parentBuilder;

        public ResourceBuilder(TParentBuilder parentBuilder, Region region)
        {
            _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));
            _region = region;
        }

        protected Pulumi.CustomResourceOptions ResourceOptions()
            => RegionProviders.GetResourceOptions(_region);

        protected abstract Task<List<(string Name, Pulumi.CustomResource Resource)>> CreateResources();

        public async Task Create()
        {
            foreach (var (name, resource) in await CreateResources())
                ResourceLookup.AddResource(name, resource);
        }

        protected static List<(string Name, Pulumi.CustomResource Resource)> Result(params (string Name, Pulumi.CustomResource Resource)[] resources)
            => resources.ToList();

        protected static Task<List<(string Name, Pulumi.CustomResource Resource)>> ResultAsync(params (string Name, Pulumi.CustomResource Resource)[] resources)
            => Task.FromResult(Result(resources));
    }

    abstract class ResourceArgBuilder<TParentBuilder, TReturnArg>: IResourceArgBuilder<TReturnArg> where TReturnArg : Pulumi.ResourceArgs
    {
        protected readonly TParentBuilder _parentBuilder;
        public ResourceArgBuilder(TParentBuilder parentBuilder)
            => _parentBuilder = parentBuilder ?? throw new ArgumentNullException(nameof(parentBuilder));

        protected abstract Task<TReturnArg> Build();

        public Task<TReturnArg> Create() => Build();
    }
}
