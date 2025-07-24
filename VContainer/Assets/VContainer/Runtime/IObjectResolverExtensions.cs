using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer
{
    public static class IObjectResolverExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Resolve<T>(this IObjectResolver resolver, object key = null, bool optional = false) => (T)resolver.Resolve(typeof(T), key, optional);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryResolve<T>(this IObjectResolver resolver, out T resolved, object key = null)
        {
            if (resolver.TryResolve(typeof(T), out var r, key))
            {
                resolved = (T)r;
                return true;
            }

            resolved = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ResolveOrDefault<T>(this IObjectResolver resolver, T defaultValue = default, object key = null)
        {
            if (resolver.TryResolve(typeof(T), out var value, key))
            {
                return (T)value;
            }

            return defaultValue;
        }

        // Using from CodeGen
        [Preserve]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ResolveNonGeneric(this IObjectResolver resolve, Type type, object key = null, bool optional = false) => resolve.Resolve(type, key, optional);

        public static object ResolveOrParameter(
            this IObjectResolver resolver,
            Type parameterType,
            string parameterName,
            IReadOnlyList<IInjectParameter> parameters,
            object key = null,
            bool optional = false)
        {
            if (parameters == null)
            {
                return resolver.Resolve(parameterType, key, optional);
            }
            
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];
                if (parameter.Match(parameterType, parameterName))
                {
                    return parameter.GetValue(resolver);
                }
            }

            return key != null ?
                resolver.Resolve(parameterType, key, optional) :
                resolver.Resolve(parameterType, optional);
        }
    }
}