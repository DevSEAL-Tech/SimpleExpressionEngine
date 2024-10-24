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
        private readonly IDictionary<string, double> _dictionary;

        public ReflectionContext(object targetObject)
        {
            _targetObject = targetObject;
            _dictionary = targetObject as IDictionary<string, double>;
        }

        public double ResolveVariable(string name)
        {
            if (_dictionary != null)
            {
                if (_dictionary.TryGetValue(name, out var value))
                {
                    return value;
                }
                throw new InvalidDataException($"Unknown variable: '{name}'");
            }

            var pi = _targetObject.GetType().GetProperty(name);
            if (pi == null)
                throw new InvalidDataException($"Unknown variable: '{name}'");

            return (double)pi.GetValue(_targetObject);
        }

        public double CallFunction(string name, double[] arguments)
        {
            if (_dictionary != null)
            {
                throw new InvalidDataException($"Unknown function: '{name}'");
            }

            var mi = _targetObject.GetType().GetMethod(name);
            if (mi == null)
                throw new InvalidDataException($"Unknown function: '{name}'");

            var argObjs = arguments.Select(x => (object)x).ToArray();
            return (double)mi.Invoke(_targetObject, argObjs);
        }
    }
}
