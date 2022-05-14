using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Steelax.EasyRoslyn.Common
{
    public class ReferenceSettings
    {
        private readonly HashSet<Assembly> _assemblies;

        internal IEnumerable<Assembly> AssemblyCollection => _assemblies.ToList();

        public ReferenceSettings()
        {
            _assemblies = new HashSet<Assembly>();
        }
        private ReferenceSettings(ReferenceSettings other)
        {
            _assemblies = new HashSet<Assembly>(other._assemblies);
        }
        private void AddAssembly(Assembly assembly)
        {
            if (assembly is null)
                throw new NullReferenceException("Assembly reference is null");

            if (!_assemblies.Contains(assembly))
                _assemblies.Add(assembly);
        }

        public ReferenceSettings UseSystemRuntime()
        {
            var assembly = Assembly.Load("System.Runtime");
            return UseReference(assembly);
        }
        public ReferenceSettings UseNetStandard()
        {
            var assembly = Assembly.Load("netstandard");
            return UseReference(assembly);
        }
        public ReferenceSettings UseReference(Assembly assembly)
        {
            AddAssembly(assembly);
            return new ReferenceSettings(this);
        }
        public ReferenceSettings UseReference(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
                AddAssembly(assembly);
            return new ReferenceSettings(this);
        }
        public ReferenceSettings UseReference(Type type)
        {
            AddAssembly(type.Assembly);
            return new ReferenceSettings(this);
        }
        public ReferenceSettings UseReference(IEnumerable<Type> types)
        {
            foreach (var type in types)
                AddAssembly(type.Assembly);
            return new ReferenceSettings(this);
        }
        public ReferenceSettings UseReference(string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);
            return UseReference(assembly);
        }
    }
}
