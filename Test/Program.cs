using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Test
{
    class Program
    {
        public static double Way(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        public static int[] changePath(int[] path)
        {
            Random rnd = new Random();
            int[] newPath = new int[path.Length];
            path.CopyTo(newPath, 0);
            int first = 0, second = 0;
            while (first == second)
            {
                first = rnd.Next(1, path.Length - 1);
                second = rnd.Next(1, path.Length - 1);
            }
            int cont = newPath[first];
            newPath[first] = newPath[second];
            newPath[second] = cont;
            return newPath;
        }
        public static double fullPath(int[] path, int[] xs, int[] ys)
        {
            double fullPath = 0;
            for (int i = 1; i < path.Length; i++)
            {
                fullPath += Way(xs[path[i]], ys[path[i]], xs[path[i - 1]], ys[path[i - 1]]);
            }
            return fullPath;
        }
        public static bool checkTemper(double s, double t)
        {
            Random rnd = new Random();
            double prob = 100 * Math.Pow(Math.E, -(s / t));
            if (prob > rnd.Next(1, 100)) return true;
            else return false;
        }
        public static bool checkList(List<int[]> arList, int[] path)
        {
            bool check = true;
            for (int i = 0; i < arList.Count; i++)
            {
                if (path.SequenceEqual(arList[i]))
                {
                    check = false;
                    break;
                }
            }
            return check;
        }

        static void Main(string[] args)
        {
            double a = 0.9, t = 100, shortPath;
            int[] path = new int[] { 0, 1, 2, 3, 0 };
            int[] xs = new int[] { 1, 25, 56, 44, 1 };
            int[] ys = new int[] { 61, 12, 33, 88, 61 };
            List<int[]> arList = new List<int[]>();
            arList.Add(path);
            while (t > 1)
            {
                shortPath = fullPath(path, xs, ys);
                int[] newpath = new int[path.Length];
                path.CopyTo(newpath, 0);
                while (!checkList(arList, newpath))
                    newpath = changePath(newpath);
                arList.Add(newpath);
                if (fullPath(newpath, xs, ys) <= shortPath)
                {
                    shortPath = fullPath(newpath, xs, ys);
                    path = newpath;
                }
                else
                {
                    if (checkTemper(fullPath(newpath, xs, ys) - shortPath, t))
                    {
                        shortPath = fullPath(newpath, xs, ys);
                        path = newpath;
                    }
                }
                for (int i = 0; i < path.Length; i++) Console.Write(path[i] + " ");
                Console.WriteLine(fullPath(path, xs, ys) + " > " + t + " ");
                t = a * t;
            }

            Console.ReadKey();
        }
    }
}
