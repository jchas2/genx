using GenX.Cli.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GenX.Cli.Infrastructure.Dotnet
{
    public class DotnetAssemblyReader : IAssemblyReader
    {
        private readonly IOutputWriter _outputWriter;

        public DotnetAssemblyReader(IOutputWriter outputWriter) => _outputWriter = outputWriter;

        public AssemblyModel Read(
            string filename, 
            string namespaceFilter)
        {
            var types = GetTypes(filename);

            if (types == null)
            {
                return null;
            }

            var filteredTypes = FilterTypes(types, namespaceFilter);

            if (filteredTypes == null || filteredTypes.Length == 0)
            {
                return null;
            }

            return MapModel(
                Path.GetFileNameWithoutExtension(filename),
                filteredTypes);
        }

        private AssemblyModel MapModel(string assemblyName, Type[] types)
        {
            var model = new AssemblyModel { Name = assemblyName };

            foreach (var type in types)
            {
                _outputWriter.Output.WriteLine(
                    string.Format(StringResources.ReadingAssemblyType, type.Name));

                var typeEntity = new TypeEntity
                {
                    Name = type.Name,
                    Namespace = type.Namespace
                };

                var ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

                foreach (var ctor in ctors)
                {
                    _outputWriter.Verbose.WriteLine(
                        string.Format(StringResources.WritingConstructor, ctor.Name));

                    var constructor = GetMethod(ctor);

                    ctor.GetParameters().ToList().ForEach(parm => constructor.Parameters.Add(
                        GetParameter(parm)));

                    typeEntity.Constructors.Add(constructor);
                }

                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                foreach (var pinfo in properties)
                {
                    _outputWriter.Verbose.WriteLine(
                        string.Format("    {0}", pinfo.Name));

                    typeEntity.Properties.Add(
                        GetProperty(pinfo));
                }

                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

                foreach (var minfo in methods)
                {
                    _outputWriter.Verbose.WriteLine(
                        string.Format(StringResources.WritingMethod, minfo.Name));

                    var method = GetMethod(minfo);
                    method.CLRType = minfo.ReturnType?.ToString();

                    minfo.GetParameters().ToList().ForEach(parm => method.Parameters.Add(
                        GetParameter(parm)));

                    typeEntity.Methods.Add(method);
                }

                model.Types.Add(typeEntity);
            }

            return model;
        }

        private Method GetMethod(MethodBase memberInfo) =>
            new Method
            {
                Name = memberInfo.Name,
                IsAbstract = memberInfo.IsAbstract,
                IsFinal = memberInfo.IsFinal,
                IsHideBySig = memberInfo.IsHideBySig,
                IsStatic = memberInfo.IsStatic,
                IsVirtual = memberInfo.IsVirtual
            };

        private Property GetProperty(PropertyInfo propertyInfo) =>
            new Property
            {
                CLRType = propertyInfo.PropertyType.ToString(),
                CanRead = propertyInfo.CanRead,
                CanWrite = propertyInfo.CanWrite,
                Name = propertyInfo.Name
            };

        private Parameter GetParameter(ParameterInfo parameterInfo) =>
            new Parameter
            {
                Name = parameterInfo.Name,
                CLRType = parameterInfo.ParameterType.ToString(),
                DefaultValue = parameterInfo.DefaultValue.ToString(),
                IsRetVal = parameterInfo.IsRetval,
                Position = parameterInfo.Position
            };

        private Type[] FilterTypes(Type[] types, string namespaceFilter)
        {
            if (string.IsNullOrEmpty(namespaceFilter) ||
                (namespaceFilter.Length == 1 && namespaceFilter == "*"))
            {
                _outputWriter.Verbose.WriteLine(StringResources.ReturningAllAssemblyTypes);
                return types;
            }

            var filteredTypes = new List<Type>(types.Length);

            string wildcardMatch = namespaceFilter.EndsWith("*")
                ? namespaceFilter.Substring(0, namespaceFilter.Length - 1)
                : null;

            if (!string.IsNullOrEmpty(wildcardMatch))
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format(StringResources.ApplyingWildcardMatch, namespaceFilter));
            }
            else
            {
                _outputWriter.Verbose.WriteLine(
                    string.Format(StringResources.FilteringOnNamespace, namespaceFilter));
            }

            filteredTypes.AddRange(types.Where(type =>
                (wildcardMatch != null && !string.IsNullOrEmpty(type.Namespace) && type.Namespace.StartsWith(wildcardMatch, StringComparison.CurrentCultureIgnoreCase)) ||
                (wildcardMatch == null && !string.IsNullOrEmpty(type.Namespace) && type.Namespace.Equals(namespaceFilter, StringComparison.CurrentCultureIgnoreCase))));

            return filteredTypes.ToArray();
        }

        private Type[] GetTypes(string filename)
        {
            Assembly assembly = null;

            _outputWriter.Output.WriteLine(
                string.Format(StringResources.LoadingAssembly, filename));

            try
            {
                assembly = Assembly.LoadFrom(filename);
            }
            catch (BadImageFormatException ex)
            {
                _outputWriter.Error.WriteLine(
                    string.Format(StringResources.BadImage, ex.Message));

                return null;
            }
            catch (Exception ex) when (ex is FileNotFoundException || ex is FileLoadException)
            {
                _outputWriter.Error.WriteLine(
                    string.Format(StringResources.FailedToLoadAssembly, filename, ex.Message));

                return null;
            }

            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types;
            }
        }

        private string ResolveScope(
            bool isPublic,
            bool isPrivate,
            bool isAssembly,
            bool isFamily,
            bool isFamilyOrAssembly)
        {
            if (isPublic)
            {
                return "public";
            }
            else if (isPrivate)
            {
                return "private";
            }
            else if (isAssembly)
            {
                return "internal";
            }
            else if (isFamily)
            {
                return "protected";
            }
            else if (isFamilyOrAssembly)
            {
                return "protected internal";
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
