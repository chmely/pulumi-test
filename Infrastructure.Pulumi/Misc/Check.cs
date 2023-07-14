using System;

namespace LetsGetChecked.Infrastructure.Misc
{
    internal static class Check
    {
        public static string NotNullOrWhiteSpace(this string value, string paramName)
            => (string.IsNullOrWhiteSpace(value) ? null : value) ?? throw new ArgumentException($"Parameter {paramName} cannot be null or whitespace", paramName);
    }
}
