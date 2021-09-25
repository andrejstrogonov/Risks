using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerLibrary
{
    public class Code
    {
            public static char[] En_Alphabet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z','1','2','3','4','5','6','7','8','9','0' };

            public static Dictionary<char, int> En_Alphabet_Dict = new Dictionary<char, int>();

            public static void Init()
            {
                
                for (int i = 0; i < En_Alphabet.Count(); i++)
                {
                    En_Alphabet_Dict[En_Alphabet[i]] = i;
                    En_Alphabet_Dict[En_Alphabet[i].ToString().ToUpper()[0]] = i;
                }
            }

            public static string FromNumsToStr_En(List<int> nums)
            {
                string result = "";
                foreach (var n in nums)
                {
                    var index = n % 26;
                    if (index < 0)
                    {
                        index = 26 + index;
                    }
                    result += En_Alphabet[index];
                }
                return result;
            }
            public static List<int> FromStrToNums_En(string word)
            {
                List<int> result = new List<int>();
                foreach (var c in word)
                {
                    result.Add(En_Alphabet_Dict[c]);
                }
                return result;
            }

            

            public static List<BigInteger> FromNumToNums_10(BigInteger num)
            {
                List<BigInteger> res = new List<BigInteger>();
                while (num > 0)
                {
                    BigInteger n = num % 100;
                    res.Insert(0, n);
                    num = num / 100;
                }
                return res;
            }
            public static string Execute_En(string word, string key, bool code)
            {
                Init();
                int sign = code ? 1 : -1;
                var nums = FromStrToNums_En(word);
                int pos = 0;
                List<int> result = new List<int>();
                List<int> baseG = FromStrToNums_En(key);
                for (int i = 0; i < nums.Count(); i++)
                {
                    if (pos >= baseG.Count())
                    {
                        pos = 0;
                    }
                    result.Add(nums[i] + sign * baseG[pos]);
                    pos++;
                }
                return FromNumsToStr_En(result);
            }
        }
}
