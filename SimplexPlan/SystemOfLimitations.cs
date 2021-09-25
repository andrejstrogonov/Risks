using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexPlan
{
    public class SystemOfLimitations
    {
        private List<Limitation> limitations;
        private Dictionary<int, Limitation> personalLimitationsMoreThan;
        private Dictionary<int, Limitation> personalLimitationsLessThan;

        private Dictionary<int, Limitation> stockLimitationLessThan;
        private Dictionary<int, Limitation> stockLimitationMoreThan;

        private Limitation budgetLimitation;
        private Limitation riskLimitation;

        //private readonly List<double[]> table;
        public List<int> Variables { get; private set; }
        private Limitation F;
        private Dictionary<int, bool> isInteger;
        public void ControlVariable(int variable)
        {
            if (!personalLimitationsMoreThan.ContainsKey(variable))
            {
                personalLimitationsMoreThan[variable] = new Limitation(variable,0,true);
            }
            if (!personalLimitationsLessThan.ContainsKey(variable))
            {
                personalLimitationsLessThan[variable] = new Limitation(variable, double.PositiveInfinity, false);
            }
            if (!isInteger.ContainsKey(variable))
            {
                isInteger[variable] = false;
            }
        }

        public void SetAllVariables(List<int> variables)
        {
            Variables = variables;
            foreach(var v in variables)
            {
                ControlVariable(v);
            }
        }

        public SystemOfLimitations(List<int> variables)
        {
            limitations = new List<Limitation>();
            personalLimitationsLessThan = new Dictionary<int, Limitation>();
            personalLimitationsMoreThan = new Dictionary<int, Limitation>();
            stockLimitationMoreThan = new Dictionary<int, Limitation>();
            stockLimitationLessThan = new Dictionary<int, Limitation>();
            budgetLimitation = new Limitation();
            isInteger = new Dictionary<int, bool>();
            F = new Limitation();
            //table = new List<double[]>();
            SetAllVariables(variables);
        }
        
        private void ControlPersonalLimitation()
        {
            foreach(var v in Variables)
            {
                ControlVariable(v);
            }

        }

        public void AddLimitation(Limitation limitation)
        {
            limitations.Add(limitation);
        }

        public void AddLimitation(Dictionary<int,double> coefsOfVars, double limit, bool sign)
        {
            Limitation limitation = new Limitation(coefsOfVars, limit, sign);
            limitations.Add(limitation);
        }

       
        internal double[,] ToTable()
        {
            int size = limitations.Count + personalLimitationsLessThan.Count + personalLimitationsMoreThan.Count + stockLimitationLessThan.Count + stockLimitationMoreThan.Count+2;
            double[,] result = new double[size, Variables.Count + 1];
            int i=0;
            foreach(var l in limitations)
            {
                var tmp = l.ToArray(Variables);
                for(int j = 0; j < tmp.Length; j++)
                {
                    result[i, j] = tmp[j];
                }
                i++;
            }
            foreach (var l in personalLimitationsMoreThan)
            {
                if (l.Value.Limit != 0)
                {
                    var tmp = l.Value.ToArray(Variables);
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        result[i, j] = tmp[j];
                    }
                    i++;
                }
            }
            foreach (var l in personalLimitationsLessThan)
            {
                if (l.Value.Limit < double.PositiveInfinity)
                {
                    var tmp = l.Value.ToArray(Variables);
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        result[i, j] = tmp[j];
                    }
                    i++;
                }
            }
            foreach (var l in stockLimitationMoreThan)
            {
                if (l.Value.Limit != 0)
                {
                    var tmp = l.Value.ToArray(Variables);
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        result[i, j] = tmp[j];
                    }
                    i++;
                }
            }
            foreach (var l in stockLimitationLessThan)
            {
                if (l.Value.Limit < double.PositiveInfinity)
                {
                    var tmp = l.Value.ToArray(Variables);
                    for (int j = 0; j < tmp.Length; j++)
                    {
                        result[i, j] = tmp[j];
                    }
                    i++;
                }
            }
            if (budgetLimitation.Limit < double.PositiveInfinity)
            {
                var tmp = budgetLimitation.ToArray(Variables);
                for (int j = 0; j < tmp.Length; j++)
                {
                    result[i, j] = tmp[j];
                }
                i++;
            }

            if (riskLimitation.Limit < double.PositiveInfinity)
            {
                var tmp = riskLimitation.ToArray(Variables);
                for (int j = 0; j < tmp.Length; j++)
                {
                    result[i, j] = tmp[j];
                }
                i++;
            }

            return result;
        }
        
        /*не понадобилась
        internal int[] ToArrayOfSigns()
        {
            int[] result = new int[limitations.Count + personalLimitationsLessThan.Count + personalLimitationsMoreThan.Count];
            int i = 0;
            foreach (var l in limitations)
            {
                result[i] = l.Sign?1:-1;
                i++;
            }
            foreach (var l in personalLimitationsMoreThan)
            {
                result[i] = l.Value.Sign ? 1 : -1;
                i++;
            }
            foreach (var l in personalLimitationsLessThan)
            {
                result[i] = l.Value.Sign ? 1 : -1;
                i++;
            }
            return result;
        }
        */
        public override string ToString()
        {
            string result = "";
            foreach(var lim in limitations)
            {
                result += lim.ToString() + '\n';
            }
            foreach (var lim in personalLimitationsMoreThan)
            {
                result += lim.Value.ToString() + '\n';
            }
            foreach (var lim in personalLimitationsLessThan)
            {
                result += lim.Value.ToString() + '\n';
            }
            foreach (var lim in stockLimitationLessThan)
            {
                result += lim.Value.ToString() + '\n';
            }
            foreach (var lim in stockLimitationMoreThan)
            {
                result += lim.Value.ToString() + '\n';
            }
            return result;
        }

        public void AddMainFunction(Dictionary<int,double> vars, double freeelem=0)
        {
            F = new Limitation(vars, freeelem);
        }

        public void AddMainFunction(Limitation limitation)
        {
            F = limitation;
        }

        public void SetBudgetLimitation(Limitation limitation)
        {
            budgetLimitation = limitation;
        }

        public void SetRiskLimitation(Limitation limitation)
        {
            riskLimitation = limitation;
        }

        /*public void AddVariableToF(int variable, double coefficient)
        {
            F.AddOrChangeVariable(variable, coefficient);
        }*/

        internal double[] GetMainFunction()
        {
            return  F.ToArray(Variables);
        }

        public bool SetVariableIsInteger(int variable, bool _isInteger = true)
        {
            if (!Variables.Contains(variable))
                return false;
            isInteger[variable] = _isInteger;
            return true;
        }

        public bool SetPersonalLimitationMoreThan(int variable, double min)
        {
            if (!Variables.Contains(variable))
                return false;
            if (personalLimitationsMoreThan.ContainsKey(variable))
                personalLimitationsMoreThan[variable].Limit = min;
            else
                personalLimitationsMoreThan[variable] = new Limitation(variable,  min, true);
            return true;
        }

        public bool SetPersonalLimitationLessThan(int variable, double max)
        {
            if (!Variables.Contains(variable))
                return false;
            if (personalLimitationsLessThan.ContainsKey(variable))
                personalLimitationsLessThan[variable].Limit = max;
            else
                personalLimitationsLessThan[variable] = new Limitation(variable, max, false);
            return true;
        }

        public bool SetStockLimitationMoreThan(int key, int var2, double min)
        {
            if (!Variables.Contains(key))
                return false;
            if (stockLimitationMoreThan.ContainsKey(key))
                stockLimitationMoreThan[key].Limit = min;
            else
                stockLimitationMoreThan[key] = new Limitation(key,var2, min, true);
            return true;
        }

        public bool SetStockLimitationLessThan(int key, int var2, double max)
        {
            if (!Variables.Contains(key))
                return false;
            if (stockLimitationLessThan.ContainsKey(key))
                stockLimitationLessThan[key].Limit = max;
            else
                stockLimitationLessThan[key] = new Limitation(key, var2, max, false);
            return true;
        }

        internal void SetPersonalLimitations(VariableLimites variableLimites)
        {
            foreach(KeyValuePair<int,Interval> vl in variableLimites)
            {
                SetPersonalLimitationMoreThan(vl.Key, vl.Value.Min);
                SetPersonalLimitationLessThan(vl.Key, vl.Value.Max);
            }
        }

        public bool SetPersonalLimitation(int variable, double min, double max, bool _isInteger=false)
        {
            return SetPersonalLimitationLessThan(variable, max) && SetPersonalLimitationMoreThan(variable, min) && SetVariableIsInteger(variable,_isInteger);
        }

        internal bool IsVariableInteger(int index)
        {
            if (isInteger.ContainsKey(index))
                return isInteger[index];
            return false;
        }

        internal Dictionary<int,bool> GetDictionaryIsInteger()
        {
            return isInteger;
        }

        internal VariableLimites GetVariableLimites()
        {
            VariableLimites limites = new VariableLimites();
            foreach(var v in Variables)
            {
                double min = 0;
                double max = double.PositiveInfinity;
                if (personalLimitationsMoreThan.ContainsKey(v))
                {
                    min = personalLimitationsMoreThan[v].Limit;
                }
                if (personalLimitationsLessThan.ContainsKey(v))
                {
                    max = personalLimitationsLessThan[v].Limit;
                }
                Interval interval = new Interval(min, max);
                limites[v] = interval;
            }
            return limites;
        }

       
        public SystemOfLimitations Copy()
        {
            SystemOfLimitations result = new SystemOfLimitations(new List<int>());
            result.limitations = new List<Limitation>();
            foreach(var l in limitations){
                result.limitations.Add(l.Copy());
            }
            result.personalLimitationsMoreThan = new Dictionary<int, Limitation>();
            foreach(var l in personalLimitationsMoreThan)
            {
                result.personalLimitationsMoreThan[l.Key] = l.Value.Copy();
            }
            result.personalLimitationsLessThan = new Dictionary<int, Limitation>();
            foreach (var l in personalLimitationsLessThan)
            {
                result.personalLimitationsLessThan[l.Key] = l.Value.Copy();
            }

            foreach (var l in stockLimitationLessThan)
            {
                result.stockLimitationLessThan[l.Key] = l.Value.Copy();
            }

            foreach (var l in stockLimitationMoreThan)
            {
                result.stockLimitationMoreThan[l.Key] = l.Value.Copy();
            }

            result.Variables = new List<int>();
            foreach(var v in Variables)
            {
                result.Variables.Add(v);
            }
            if (F is null)
                result.F = null;
            else
                result.F = F.Copy();
            result.isInteger = new Dictionary<int, bool>();
            foreach(var i in isInteger)
            {
                result.isInteger[i.Key] = i.Value;
            }

            if (!(budgetLimitation is null))
                result.budgetLimitation = budgetLimitation.Copy();

            if (!(riskLimitation is null))
                result.riskLimitation = riskLimitation.Copy();
            return result;
        }
    }
}
