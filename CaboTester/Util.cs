using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaboTester
{
    static class Util
    {
        static Random rnd = new Random();
        public static T[] SelectCards<T>(ref T[] from, int needed)
        {
            T[] picked = new T[needed];
            T[] rest = new T[from.Length - needed];
            for (int i = 0; i < from.Length; i++) {
                if (rnd.NextDouble() < (double)needed / (from.Length - i)) {
                    picked[needed - 1] = from[i];
                    needed--;
                } else {
                    rest[i - picked.Length + needed] = from[i];
                }
            }
            from = rest;
            return picked.OrderBy(x => rnd.Next()).ToArray();
        }

        public static string CardsToString(Card[] array)
        {
            return "[" + string.Join(", ", array.Select(x => x.value.ToString() + " " + (x.isSeen ? "s" : "u"))) + "]";
        }
    }
}
