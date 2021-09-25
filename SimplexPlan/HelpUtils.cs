using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexPlan
{
    public static class HelpUtils
    {
        public static double RoundDouble(double number, int n)
        {
            if (IsInteger(number))
                return Math.Round(number);
            return Math.Round(number, n);
        }

        public static double SmartRound(double number)
        {
            if (IsInteger(number,0.00000000001))
                return Math.Round(number);
            return number;
        }

        public static int RoundDouble(double number, bool high = false)
        {
            return (int)Math.Truncate(number) + (high ? 1 : 0);
        }

        public static bool IsInteger(double number, double eps=double.Epsilon)
        {
            int right = (int)Math.Round(number);
            return (Math.Abs(right - number) < eps);
        }

        internal class VariableLimited
        {
            internal int variable { get; set; }
            internal double min { get; set; }
            internal double max { get; set; }
        }



    }
}
