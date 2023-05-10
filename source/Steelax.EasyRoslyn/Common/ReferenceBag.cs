using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Steelax.EasyRoslyn.Common
{
    public class ReferenceBag: IEnumerable<Assembly>
    {
        readonly HashSet<Assembly> assemblies = new();

        internal ReferenceBag() { }

        public ReferenceBag UseReference(Assembly assembly)
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));

            assemblies.Add(assembly);

            return this;
        }
        public ReferenceBag UseReference(string assemblyName)
        {
            var assembly = Assembly.Load(assemblyName);

            return UseReference(assembly);
        }
        public ReferenceBag UseReference(Type type)
        {
            _ = type ?? throw new ArgumentNullException(nameof(type));

            return UseReference(type.Assembly);
        }

        public ReferenceBag UseSystemRuntime() => UseReference("System.Runtime");
        public ReferenceBag UseNetStandard() => UseReference("netstandard");

        public IEnumerator<Assembly> GetEnumerator() => assemblies.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
