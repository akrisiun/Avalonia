using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace OmniXaml.Builder
{

    public class AddressPack : Collection<ConfiguredAssemblyWithNamespaces>
    {
        public AddressPack() { }
        public AddressPack(IEnumerable<ConfiguredAssemblyWithNamespaces> assemblyAndClrs) : base(assemblyAndClrs.ToList()) { }

        public Type Get(string name)
        {
            var numerate = (from configuredAssemblyWithNamespaces in Items
                            let g = configuredAssemblyWithNamespaces.Get(name)
                            where g != null
                            select g);

            Type result = numerate.FirstOrDefault();
            if (result == null)
            {
                if (name == "Style")
                    return typeof(Avalonia.Styling.Style);
                if (name == "Setter")
                    return typeof(Avalonia.Styling.Setter);
                if (name == "KeyboardNavigation")
                    return typeof(Avalonia.Input.KeyboardNavigation);

                //var CurrentDomain = global::System.AppDomain.CurrentDomain;
                //Assembly asm = System.Linq.Enumerable.Where(CurrentDomain.GetAssemblies(),
                //    (a) => a.FullName.Contains("Avalonia.Core")).First();
                Assembly asm = IntrospectionExtensions.GetTypeInfo(typeof(Avalonia.Styling.Style)).Assembly;

                var types = asm.DefinedTypes;
                
                var numerType = System.Linq.Enumerable.Where(types, (t) => t.FullName.EndsWith("." + name));
                TypeInfo foundType = numerType.First();
                if (foundType != null)
                    return foundType.DeclaringType;

                    // HashSet<string> names = new HashSet<string>();
                    List <string> toList = new List<string>();

                // Avalonia.Animation.Styles

                foreach (object item in Items) // in configuredAssemblyWithNamespaces
                {
                    //let g = configuredAssemblyWithNamespaces.Get(null)
                    //select g;
                    dynamic itemDyn = item;
                    IEnumerable<string> stringList = itemDyn.Strings;
                    if (stringList != null)
                        toList.AddRange(stringList);
                }


            }
            return result;
        }
    }
}