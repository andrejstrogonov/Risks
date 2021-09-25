using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimplexPlan
{
    internal class Interval
    {
        internal double Min { get; set; }
        internal double Max { get; set; }
        internal Interval(double _min, double _max)
        {
            Min = _min;
            Max = _max;
        }
    }

    internal class VariableLimites:IEnumerable
    {
        private Dictionary<int, Interval> limites;
        internal Interval this[int index]
        {
            get {
                if (!limites.ContainsKey(index))
                    limites[index] = new Interval(0, double.PositiveInfinity);
                return limites[index];
            }
            set
            {
                limites[index] = value;
            }
        }
        internal VariableLimites Copy()
        {
            VariableLimites result = new VariableLimites();
            foreach(var l in limites)
            {
                result[l.Key] = l.Value;
            }
            return result;
        }

        public IEnumerator GetEnumerator()
        {
            return limites.GetEnumerator();
        }

        internal VariableLimites()
        {
            limites = new Dictionary<int, Interval>();
        }

        
    }
}
