using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentAgent
{
    static class Utils
    {
        public static int GetIntrval(int first, int second)
        {
            if (first < second)
            {
                return Math.Abs(second - first);
            }

            return Math.Abs(first - second);
        }

        public static string CaveHash(int row, int coll)
        {
            return row.ToString() + coll.ToString();
        }
    }
}
