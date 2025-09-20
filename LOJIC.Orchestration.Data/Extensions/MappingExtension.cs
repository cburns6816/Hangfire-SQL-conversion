using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace LOJIC.Orchestration.Data.Extensions
{
    static class MappingExtension
    {
        public static void ApplyByNameSpace(this ModelBuilder modelBuilder, string configNamespace = "")
        {
            var applyGenericMethods = typeof(ModelBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            var applyGenericApplyConfigurationMethods = applyGenericMethods
                .Where(m =>
                    m.IsGenericMethod &&
                    m.Name.Equals("ApplyConfiguration", StringComparison.OrdinalIgnoreCase)
                );

            var applyGenericMethod = applyGenericApplyConfigurationMethods.FirstOrDefault(m =>
                m.GetParameters()
                    .FirstOrDefault<ParameterInfo>()?.ParameterType.Name == "IEntityTypeConfiguration`1"
                );

            var declaringType = new StackFrame(1).GetMethod()?.DeclaringType;

            if (declaringType != null)
            {
                var applicableTypes = declaringType?.Assembly
                    .GetTypes()
                    .Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters);

                if (!string.IsNullOrEmpty(configNamespace))
                {
                    applicableTypes = applicableTypes.Where(c => c.Namespace == configNamespace);
                }

                foreach (var type in applicableTypes)
                {
                    foreach (var iface in type.GetInterfaces())
                    {
                        // if type implements interface IEntityTypeConfiguration<SomeEntity>
                        if (iface.IsConstructedGenericType && iface.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>))
                        {
                            // make concrete ApplyConfiguration<SomeEntity> method
                            if (applyGenericMethod != null)
                            {
                                var applyConcreteMethod = applyGenericMethod.MakeGenericMethod(iface.GenericTypeArguments[0]);
                                // and invoke that with fresh instance of your configuration type
                                applyConcreteMethod.Invoke(modelBuilder, new object[] { Activator.CreateInstance(type) });
                            }

                            Console.WriteLine("applied model " + type.Name);
                            break;
                        }
                    }
                }
            }
        }
    }
}

