using System;
using System.Collections.Concurrent;
using System.Linq;

namespace LetsGetChecked.Infrastructure.Misc
{
    internal static class ResourceLookup
    {
        class Resource
        {
            public string Name { get; init; }
            public Type Type { get; init; }
            public Pulumi.CustomResource Value { get; init; }
        }

        static readonly ConcurrentBag<Resource> _resources = new ();

        internal static T GetResource<T>(string name) where T: Pulumi.CustomResource
        {
            var resource = _resources.FirstOrDefault(r => r.Type == typeof(T) && r.Name == name);
            return resource?.Value as T;
        }

        internal static void AddResource<T>(string name, T value) where T : Pulumi.CustomResource
            => _resources.Add(new Resource { Name = name, Type = value.GetType(), Value = value });
    }
}
