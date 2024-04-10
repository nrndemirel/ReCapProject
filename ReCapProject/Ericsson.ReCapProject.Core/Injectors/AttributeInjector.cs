using Ericsson.ReCapProject.Core.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace Ericsson.ReCapProject.Core.Injectors
{
    public static class AttributeInjector
    {
        public static void AddInjectionByAttribute(this IServiceCollection services)
        {
            List<Type> allTypes = GetAllTypesInAssemblies();

            RegisterWithAttribute(ref services, typeof(InjectableSingletonAttribute), allTypes);
            RegisterWithAttribute(ref services, typeof(InjectableTransientAttribute), allTypes);
            RegisterWithAttribute(ref services, typeof(InjectableScopedAttribute), allTypes);
        }

        private static List<Type> GetAllTypesInAssemblies()
        {
            var allTypes = new List<Type>();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName.StartsWith("Ericsson.ReCapProject.Core") || a.FullName.StartsWith("Ericsson.ReCapProject.Persistence") || a.FullName.StartsWith("Ericsson.ReCapProject.Service"));

            foreach (var assembly in assemblies)
            {
                allTypes.AddRange(assembly.GetTypes());
            }

            return allTypes;
        }

        private static void RegisterWithAttribute(ref IServiceCollection services, Type injectableAttribute, IEnumerable<Type> allTypes)
        {
            var classesWithAttribute = allTypes.Where(t => t.IsClass && t.CustomAttributes.Any(a => a.AttributeType == injectableAttribute));
            var arrayOfClasses = classesWithAttribute.ToArray();

            foreach (var implementationType in classesWithAttribute)
            {
                var interfaceType = allTypes.FirstOrDefault(t => t.IsInterface && t.Name.Substring(1) == implementationType.Name);

                if (interfaceType == null) throw new Exception($"Failed to resolve interface for class {implementationType.Name}, unable to inject dependency!");

                if (injectableAttribute == typeof(InjectableTransientAttribute))
                    services.AddTransient(interfaceType, implementationType);
                if (injectableAttribute == typeof(InjectableScopedAttribute))
                    services.AddScoped(interfaceType, implementationType);
                if (injectableAttribute == typeof(InjectableSingletonAttribute))
                    services.AddSingleton(interfaceType, implementationType);
            }
        }
    }
}
