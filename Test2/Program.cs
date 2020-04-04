using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2
{
    class Program
    {
        public static double Way(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        public static double[,] distanceMatrix(int[] xs, int[] ys)
        {
            double[,] arrPoints = new double[xs.Length, xs.Length];
            for (int i = 0; i < xs.Length; i++)
                for (int j = 0; j < xs.Length; j++)
                    arrPoints[i, j] = arrPoints[j, i] = Way(xs[i], ys[i], xs[j], ys[j]);
            return arrPoints;
        }
        public static int nextPick(double a, double b, double[,] distances, double[,] pheromones, int[] path, int currentPick)
        {
            double[] vars = new double[path.Length];
            double sum = 0;
            vars[currentPick] = 0;
            for (int j = 0; j < vars.Length; j++)
                if (currentPick != j)
                    sum += Math.Pow((1 / distances[currentPick, j]), a) * Math.Pow(pheromones[currentPick, j], b);
            for (int i = 0; i < vars.Length; i++)
                if (i != currentPick)
                    vars[i] = (Math.Pow((1 / distances[i, currentPick]), a) * Math.Pow(pheromones[i, currentPick], b)) / sum;
            Random rnd = new Random();
            double y = (double)rnd/*.NextDouble()*/.Next(1, 999) / 1000;
            sum = vars[0];
            int answ = 0;
            for (int i = 1; i < vars.Length; i++)
            {
                
                if (y < sum)
                {
                    answ = i;
                    break;
                }
                sum += vars[i];
            }
            return answ;
        }
        public static double[,] refreshPheromones(double[,] pheromones, double P, int[] path, double[,] distances)
        {
            double[,] newPheromones = pheromones;
            int k = 0;
            string check = "";
            for (int i = 0; i < path.Length; i++)
                check += path[i].ToString();
            for (int i = 0; i < pheromones.GetLength(0); i++)
            {
                for (int j = 0; j < pheromones.GetLength(1); j++)
                {
                    if (check.Contains($"{i}{j}") || check.Contains($"{j}{i}"))
                        newPheromones[i, j] = (1 - P) * pheromones[i, j] + 1 / distances[i, j];
                    else newPheromones[i, j] = (1 - P) * newPheromones[i, j];
                }
                k++;
            }
            return newPheromones;
        }
        static void Main(string[] args)
        {
            int[] path = new int[] { 0, 1, 2, 3, 4 };
            int[] xs = new int[] { 1, 2, 3, 4, 5 };
            int[] ys = new int[] { 5, 4, 3, 2, 1 };
            double[,] distances = distanceMatrix(xs, ys);
            double[,] pheromones = new double[distances.GetLength(0), distances.GetLength(1)];
            /*for (int i = 0; i < distances.GetLength(0); i++)
            {
                for (int j = 0; j < distances.GetLength(1); j++)

                {
                    Console.Write($"{distances[i, j]:f2}/");
                }
                Console.WriteLine();
            }*/
            
            for (int i = 0; i < pheromones.GetLength(0); i++)
            {
                for (int j = 0; j < pheromones.GetLength(1); j++)
                    if (j != i)
                        pheromones[i, j] = 10;
            }

            int[] new_path = new int[path.Length + 1];
            new_path[0] = path[0];
            int next_pick = 0;
            Random rnd = new Random();
            for (int k = 0; k < 100; k++)
            {
                for (int i = 0; i < path.Length; i++)
                    new_path[i] = 0;
                for (int i = 1; i < path.Length; i++)
                {
                    do
                    {
                        next_pick = nextPick(0, 2, distances, pheromones, path, path[i]);
                    } while (new_path.Contains(next_pick));
                    new_path[i] = next_pick;
                    //Console.WriteLine(new_path[i]);
                    
                }
                pheromones = refreshPheromones(pheromones, 0.2, new_path, distances);
                
                Console.WriteLine(k);
            }
            for (int i = 0; i < path.Length; i++)
                Console.WriteLine("final" + new_path[i]);
            
            Console.ReadKey();
        }
    }
}
