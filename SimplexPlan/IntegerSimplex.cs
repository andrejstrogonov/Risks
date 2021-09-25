using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexPlan
{

     /*    
     maxResult =0;    
             >=0  >=0  >=0     0..1|  |2...+бесконесть
     1) 25 : 1,2  0,5   0   
        1) 1  -> >=0,  <=1  
        2) 2  ->  >=2, 
         
         
      минимизация,
      ветвление,
      шаблоны для уравнений ограничений,
      */

    public class IntegerSimplex
    {
        Queue<VariableLimites> queue;
        SystemOfLimitations SystemOfLimitations;
        double max;
        Decidion bestDecidion;

        public IntegerSimplex(SystemOfLimitations systemOfLimitations)
        {
            queue = new Queue<VariableLimites>();
            Decidion.Variables_id = systemOfLimitations.Variables;
            SystemOfLimitations = systemOfLimitations.Copy();
            bestDecidion = null;
            max = 0;
            queue.Enqueue(SystemOfLimitations.GetVariableLimites());
        }

        private Decidion FindDecidion(VariableLimites vl, bool min=false)
        {
            SystemOfLimitations.SetPersonalLimitations(vl);
            Simplex simplex = new Simplex(SystemOfLimitations);
            var dec = simplex.FindDecidion(min);
            return dec;
        }
        
        private int? NeedBranching(Decidion decidion, bool min=false) {
            if (decidion is null)
                return null;
            if (min?decidion.MaxResult<=max:decidion.MaxResult >= max)
            {
                int? index = decidion.HasDoubleVariables(SystemOfLimitations.GetDictionaryIsInteger());

                if (index is null)
                {
                    max = decidion.MaxResult;
                    bestDecidion = decidion;
                    return null;
                }
                return (int)index;
            }
            return null;
        }

        private void Branch(VariableLimites vl, int index,double meanOfVar)
        {
            VariableLimites vl1 = vl.Copy();
            VariableLimites vl2 = vl.Copy();
            int var = index;
            double newmax = HelpUtils.RoundDouble(meanOfVar);
            if (newmax >= vl1[var].Min)
            {
                vl1[var] = new Interval(vl1[var].Min, newmax);
                queue.Enqueue(vl1);
            }
            double newmin = HelpUtils.RoundDouble(meanOfVar, true);
            if (newmin <= vl2[var].Max)
            {
                vl2[var] = new Interval(newmin, vl2[var].Max);
                queue.Enqueue(vl2);
            }
        }

        private void Cycle(bool min=false)
        {
            max = min ? double.PositiveInfinity : double.NegativeInfinity;
            while (queue.Count > 0)
            {
                VariableLimites vl = queue.Dequeue();
                Decidion dec = FindDecidion(vl, min);
                int? index = NeedBranching(dec,min);
                if (!(index is null))
                {
                    Branch(vl, (int)index, dec.Coefficients[(int)index]);
                }
            }
        }

        public Decidion FindResult(bool min=false)
        {
            Cycle(min);
            return bestDecidion;
        }
    }
}
