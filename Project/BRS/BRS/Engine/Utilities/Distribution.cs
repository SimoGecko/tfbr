using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BRS.Engine.Utilities {
    class Distribution<T> {
        private Dictionary<T, float> _values;

        public Distribution(Dictionary<T, float> values) {
            _values = values;

            Normalize();
        }

        private void Normalize() {
            float sum = 0;

            foreach (var entry in _values) {
                sum += entry.Value;
            }

            for (int i = 0; i < _values.Count; ++i) {
                KeyValuePair<T, float> keyValue = _values.ElementAt(i);
                _values[keyValue.Key] = keyValue.Value / sum;
            }
        }

        public T Evaluate() {
            float val = MyRandom.Value;

            foreach (var entry in _values) {
                if (val <= entry.Value) return entry.Key;
                val -= entry.Value;
            }

            return default(T);
        }
    }
}
