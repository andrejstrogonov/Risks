using DataProcessLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestForAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            Analys analys = new Analys();
            var result = analys.FindResult();
            foreach (var r in result)
            {
                Console.WriteLine(r.Key);
                Console.WriteLine(r.Value.Key.Print(r.Value.Value));
            }
        }
    }
}
