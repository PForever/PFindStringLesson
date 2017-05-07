using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PFindStringCS
{
    class Program
    {
        static void Main(string[] args)
        {
            string strS1 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab";
            string strS2 = "aaaaaaab";

            //Console.WriteLine(SimpleSearch(strS1, strS2 ));
            int result;
            Console.WriteLine("1) " + Tester(strS1, strS2, SimpleSearch, out result) + "\ni = {0}", result);
            Console.WriteLine("2) " + Tester(strS1, strS2, KMPSearch, out result) + "\ni = {0}", result);
            Console.WriteLine("3) " + Tester(strS1, strS2, RKSearch, out result) + "\ni = {0}", result);

            Console.ReadLine();
        }

        public static TimeSpan Tester(string strHaystack, string strNeedle, Func<string, string, int> seacher, out int result)
        {
            var tmr = Stopwatch.StartNew();
            result = seacher(strHaystack, strNeedle);
            return tmr.Elapsed;
        }

        //==============================================================
        // алгоритм КМП
        public static int KMPSearch(string strHaystack, string strNeedle)
        {
            var arrPrefix = GetPrefix(strNeedle);
            var intHayLength = strHaystack.Length;
            var intNeedLength = strNeedle.Length;
            int k = 0;
            for (int i = 0; i < intHayLength; i++)
            {
                while (true)
                {
                    if (strNeedle[k] == strHaystack[i])
                    {
                        if (++k == intNeedLength) return i - intNeedLength + 1;
                        break;
                    }

                    if (k > 0) k = arrPrefix[k - 1];
                    else break;
                }

            }
            return -1;
        }


        //==============================================================
        // Префикс-функция для алгоритма КМП
        private static int[] GetPrefix(string strText)
        {
            var intLength = strText.Length;
            int[] arrPrefix = new int[strText.Length];
            arrPrefix[0] = 0;
            int k = 0;
            for (int i = 1; i < intLength; i++)
            {
                while (true)
                {
                    if (strText[i] == strText[k + 1])
                    {
                        arrPrefix[i]++;
                        break;
                    }
                    if (k == 0)
                    {
                        arrPrefix[i] = 0;
                        break;
                    }
                    k = arrPrefix[k];
                }
            }

            return arrPrefix;
        }

        //==============================================================
        // алгоритм Рабина-Карпа
        public static int RKSearch(string strHaystack, string strNeedle)
        {
            int intHayLength = strHaystack.Length;
            int intNeedLength = strNeedle.Length;
            if (intHayLength < intNeedLength) return -1;
            byte[] arrHashHay = Hasher(strHaystack).ToArray();
            byte[] arrHashNeed = Hasher(strNeedle).ToArray();
            byte hashHay = 0;
            byte hashNeed = 0;
            int maxIndex = intHayLength - intNeedLength;

            foreach (var hash in arrHashNeed) hashNeed += hash;
            for (var i = 0; i < intNeedLength; i++) hashHay += arrHashHay[i];
            try
            {
                for (int i = 0; i <= maxIndex; hashHay += (byte)(arrHashHay[i + intNeedLength] - arrHashHay[i]), i++)
                    if (hashHay == hashNeed)
                    {
                        var t = i;
                        foreach (var ch in strNeedle)
                            if (ch != strHaystack[t++]) break;
                        if (t == i + intNeedLength) return i;
                    }
            }
            catch (IndexOutOfRangeException)
            {
            }
            return -1;
        }

        public static IEnumerable<byte> Hasher(string source)
        {
            foreach (byte ch in source)
            {
                yield return ch;
            }
        }

        //===================================================================
        // алгоритм наивного поиска
        public static int SimpleSearch(string strHaystack, string strNeedle)
        {
            int intHLen = strHaystack.Length;
            int intNLen = strNeedle.Length;
            bool blnSuccess = false;

            for (int intC1 = 0; intC1 <= intHLen - intNLen; intC1++)
            {
                int intC2= 0;
                while (strNeedle[intC2] == strHaystack[intC1+intC2])
                {
                    if (intC2 == intNLen - 1)
                    { 
                        blnSuccess = true;
                        break;
                    }
                    intC2++;
                }
                if(blnSuccess) return(intC1);
            }
            return -1;
        }

       
    }
}
