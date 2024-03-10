using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LiteDB;
using LiteDB.Async;

namespace abremir.AllMyBricks.Data.Extensions
{
    public static class LiteQueryableExtension
    {
        public static ILiteQueryableAsync<T> IncludeAll<T>(this ILiteQueryableAsync<T> liteQueryable) where T : class
        {
            return liteQueryable.Include(GetRecursivePaths(typeof(T)));
        }

        private static List<BsonExpression> GetRecursivePaths(Type pathType, string basePath = null)
        {
            var fields = pathType.GetProperties()
                .Where(property =>
                    !property.PropertyType.IsValueType
                    && (property.GetCustomAttribute<BsonRefAttribute>() != null
                        || (typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))));

            List<BsonExpression> paths = [];

            basePath = string.IsNullOrEmpty(basePath) ? "$" : basePath;

            foreach (var field in fields)
            {
                var path = typeof(IEnumerable).IsAssignableFrom(field.PropertyType)
                    ? $"{basePath}.{field.Name}[*]"
                    : $"{basePath}.{field.Name}";

                if (field.GetCustomAttribute<BsonRefAttribute>() != null)
                {
                    paths.Add(path);
                }

                paths.AddRange(GetRecursivePaths(field.PropertyType.GetInnerGenericType(), path));
            }

            return paths;
        }

        public static Type GetInnerGenericType(this Type type)
        {
            // Attempt to get the inner generic type
            Type innerType = type.GetGenericArguments().FirstOrDefault();

            // Recursively call this function until no inner type is found
            return innerType is null ? type : innerType.GetInnerGenericType();
        }
    }
}
