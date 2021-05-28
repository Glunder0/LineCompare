using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LineCompare.Comparators;

namespace LineCompare
{
    public static class ComparatorLibrary
    {
        private static readonly Dictionary<string, IComparatorCreator> ComparatorCreators = new();
        
        static ComparatorLibrary()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!type.GetInterfaces().Contains(typeof(IComparatorCreator)))
                {
                    continue;
                }

                if (type.GetConstructor(Type.EmptyTypes) == null) // Check for the parameterless constructor
                {
                    continue;
                }

                var creator = Activator.CreateInstance(type) as IComparatorCreator;

                if (creator == null)
                {
                    continue;
                }
                
                ComparatorCreators[creator.Name] = creator;
            }
        }
        
        public static IEnumerable<string> GetComparatorNames()
        {
            return ComparatorCreators.Keys;
        }

        public static IComparator? GetComparator(string name, string firstFileName, string secondFileName)
        {
            return ComparatorCreators.TryGetValue(name, out var comparatorCreator) ?
                comparatorCreator.Create(firstFileName, secondFileName) : null;
        }
    }
}