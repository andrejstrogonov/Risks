using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexPlan
{
    public class Limitation
    {
        private Dictionary<int,double> coefficientsOfVariables;
        public bool Sign { get; set; }//true - >=, false - <=
        public double Limit { get; set; }

        public Limitation()
        {
            coefficientsOfVariables = new Dictionary<int, double>();
            Sign = false;
            Limit = 0;
        }

        public Limitation(double[] line, bool sign=false)
        {
            coefficientsOfVariables = new Dictionary<int, double>();
            for(int i = 0; i < line.Length-1; i++)
            {
                coefficientsOfVariables[i] = line[i];
            }
            Limit = line[line.Length - 1];
            Sign = sign;
        }

        public Limitation(Dictionary<int,double> COV, double _limit=0, bool _sign=false)
        {
            coefficientsOfVariables = COV;
            Limit = _limit;
            Sign = _sign;
        }

        public Limitation(int variable, double limit, bool MoreThan)
        {
            Sign = MoreThan;
            coefficientsOfVariables = new Dictionary<int, double>();
            coefficientsOfVariables[variable] = 1;
            Limit = limit;
        }

        public Limitation(int variable, int stockvar, double limit, bool MoreThan)
        {
            Sign = MoreThan;
            coefficientsOfVariables = new Dictionary<int, double>();
            coefficientsOfVariables[variable] = 1;
            coefficientsOfVariables[stockvar] = 1;
            Limit = limit;
        }


        public void AddOrChangeVariable(int NumOfVar, double coef)
        {
            coefficientsOfVariables[NumOfVar] = coef;
        }

        public void ChangeLimit(double _limit)
        {
            Limit = _limit;
        }

        public void ChangeSing(bool _sign)
        {
            Sign = _sign;
        }

        public double[] ToArray(List<int> listOfVariables)
        {
            double[] result = new double[listOfVariables.Count+1];
            for(int i=0;i<listOfVariables.Count;i++)
            {
                if (coefficientsOfVariables.ContainsKey(listOfVariables[i]))
                {
                    result[i] = coefficientsOfVariables[listOfVariables[i]] *(Sign?-1:1);
                }
                else
                {
                    result[i] = 0;
                }
            }
            result[listOfVariables.Count] = Limit * (Sign ? -1 : 1);
            return result;
        }

        public double[] ToArray()
        {
            double[] result = new double[coefficientsOfVariables.Count + 1];
            int i = 0;
            foreach (var cov in coefficientsOfVariables)
            {
                result[i] = cov.Value * (Sign ? -1 : 1);
                i++;
            }
            result[i] = Limit * (Sign ? -1 : 1);
            return result;
        }

        public override string ToString()
        {
            string result = "";
            foreach(var cov in coefficientsOfVariables)
            {
                if (cov.Value!=0)
                    result += cov.Value.ToString() + "x" + cov.Key.ToString() + " + ";
            }
            result += Sign ? ">" : "<";
            result += " " + Limit.ToString();
            return result;
        }

        public SortedSet<int> GetVariables()
        {
            SortedSet<int> result = new SortedSet<int>(); 
            foreach(var cov in coefficientsOfVariables)
            {
                result.Add(cov.Key);
            }
            return result;
        }

        public bool DeleteVariable(int variable)
        {
            if (coefficientsOfVariables.ContainsKey(variable))
            {
                coefficientsOfVariables.Remove(variable);
            }
            return coefficientsOfVariables.Count != 0;
        }

        public Limitation Copy()
        {
            Limitation limitation = new Limitation();
            limitation.coefficientsOfVariables = new Dictionary<int, double>();
            foreach (var pair in coefficientsOfVariables)
            {
                limitation.coefficientsOfVariables[pair.Key] = pair.Value;
            }
            limitation.Sign = Sign;
            limitation.Limit = Limit;
            return limitation;
        }
    }
}
