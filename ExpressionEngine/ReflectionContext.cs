using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpressionEngine
{
    public class ReflectionContext : IContext
    {
        private readonly object _targetObject;
        private readonly IDictionary<string, decimal> _dictionary;

        public ReflectionContext(object targetObject)
        {
            _targetObject = targetObject;
            _dictionary = targetObject as IDictionary<string, decimal>;
        }

        public decimal ResolveVariable(string name)
        {
            if (_dictionary != null)
            {
                if (_dictionary.TryGetValue(name, out var value))
                {
                    return value;
                }
                return 0;
            }

            var pi = _targetObject.GetType().GetProperty(name);
            if (pi == null)
                return 0;

            return (decimal)pi.GetValue(_targetObject);
        }

        public decimal CallFunction(string name, decimal[] arguments)
        {
            if (_dictionary != null)
            {
                throw new InvalidDataException($"Unknown function: '{name}'");
            }

            var mi = _targetObject.GetType().GetMethod(name);
            if (mi == null)
                throw new InvalidDataException($"Unknown function: '{name}'");

            var argObjs = arguments.Select(x => (object)x).ToArray();
            return (decimal)mi.Invoke(_targetObject, argObjs);
        }
    }
}
