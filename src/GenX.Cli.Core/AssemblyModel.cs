using System.Collections.Generic;

namespace GenX.Cli.Core
{
    public class Parameter
    {
        public string Name { get; set; }
        public string CLRType { get; set; }
        public string DefaultValue { get; set; }
        public int Position { get; set; }
        public bool IsRetVal { get; set; }
    }

    public class Method
    {
        public string Name { get; set; }
        public string CLRType { get; set; }
        public bool IsAbstract { get; set; }
        public bool IsFinal { get; set; }
        public bool IsHideBySig { get; set; }
        public bool IsStatic { get; set; }
        public bool IsVirtual { get; set; }
        public List<Parameter> Parameters { get; set; } = new List<Parameter>();
    }

    public class Property
    {
        public string Name { get; set; }
        public string CLRType { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
    }

    public class TypeEntity
    {
        public string Namespace { get; set; }
        public string Name { get; set; }
        public string Scope { get; set; }
        public List<Method> Constructors { get; set; } = new List<Method>();
        public List<Property> Properties { get; set; } = new List<Property>();
        public List<Method> Methods { get; set; } = new List<Method>();
    }

    public class AssemblyModel
    {
        public string Name { get; set; }
        public List<TypeEntity> Types { get; set; } = new List<TypeEntity>();
    }
}


