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
            return row.ToString() + hashSeparator + coll.ToString();
        }

        public static void HashToCoords(string hash, ref int row, ref int coll)
        {
            try
            {
                string[] coords = hash.Split(hashSeparator);
                row = int.Parse(coords[0]);
                coll = int.Parse(coords[1]);
            }
            catch (Exception)
            {
                throw new GameException(EMessage.ERROR_PARSE_HASH);
            }
        }
        public static void Trasfer<T>(List<T> from, List<T> to)
        {
            foreach (T element in from)
            {
                to.Add(element);
            }
        }
        public static void CreateMatrix<T>(ref T[,] matrix, int rows, int colls, T val)
        {
            matrix = new T[rows, colls];
            FillMatrix(ref matrix, val);
        }
        public static void FillMatrix<T>(ref T[,] matrix, T value)
        {
            int rowCount = matrix.GetLength(0);
            int collCount = matrix.GetLength(1);

            for (int i = 0; i < rowCount; i++)
            {
                for (int j = 0; j < collCount; j++)
                {
                    matrix[i, j] = value;
                }
            }
        }

        public static char hashSeparator { get { return '|'; } }
    }
}
