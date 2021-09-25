using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SimplexPlan
{    
    [Serializable]
    public class Decidion
    {
        public Decidion() { }//для сериализации

        public void Deserialize()
        {
            Coefficients = new Dictionary<int, double>();
            foreach(var p in coefficients_serialize)
            {
                Coefficients[p.Key] = p.Value;
            }
        }

        public double MaxResult { get; set; }

        public class HelpCoef
        {
            public int Key { get; set; }
            public double Value { get; set; }
            public HelpCoef() { }
        }

        public List<HelpCoef> coefficients_serialize { get; set; }

        [XmlIgnore]
        public Dictionary<int,double> Coefficients { get; set; }

        public static List<int> Variables_id { get; set; }
        internal Decidion(double res, Dictionary<int,double> coefs)
        {
            MaxResult = res;
            Coefficients = coefs;
            coefficients_serialize = new List<HelpCoef>();
            foreach(var c in coefs)
            {
                coefficients_serialize.Add(new HelpCoef() { Key = c.Key, Value = c.Value });
            }
        }

        public double GetCoeff(int variable)
        {
            if (Coefficients.ContainsKey(variable))
            {
                return Coefficients[variable];
            }
            return -1;
        }
        /*
        internal int[] RoundVariables()
        {
            int[] result = new int[Variables.Length];
            for( int i = 0; i < Variables.Length; i++)
            {
                result[i] = (int)Math.Round(Variables[i]);
            }
            return result;
        }
        */
        internal int? HasDoubleVariables(Dictionary<int,bool> isInteger)
        {
            foreach(var v in Coefficients)
            {
                if (isInteger[v.Key] && !HelpUtils.IsInteger(v.Value))
                    return v.Key;
            }
            return null;
        }
    }
}
