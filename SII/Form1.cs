using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SII
{
    
    public partial class Form1 : Form
    {
        #region Variables
        int clicksMD = 0;
        Point[] points = new Point[11];
        bool MD = true;
        Graphics g;
        int count;
        public Pen brush = new Pen(Color.Black, 4);
        public Pen brush2 = new Pen(Color.Black, 2);
        public Pen brush3 = new Pen(Color.Red, 2);
        double t, a, shortPath;
        float A, B, P;
        #endregion
        #region Functions
        //поиск расстояния между 2мя точками
        public static double Way(float x1, float y1, float x2, float y2)
        {   
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
        //нарисовать точку
        private void Point(float x, float y)
        {
            g.DrawEllipse(brush, x, y, brush.Width, brush.Width);
            
        }
        //изменение последовательности точек
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
        //сумма всего пути
        public static double fullPath(int[] path, Point[] points)
        {
            double fullPath = 0;
            for (int i = 1; i < path.Length; i++)
            {
                fullPath += Way(points[path[i]].X, points[path[i]].Y, points[path[i - 1]].X, points[path[i - 1]].Y);
            }
            return fullPath;
        }
        //сравнение P* и случайного числа
        public static bool checkTemper(double s, double t)
        {
            Random rnd = new Random();
            double prob = 100 * Math.Pow(Math.E, -(s / t));
            if (prob > rnd.Next(1, 100)) return true;
            else return false;
        }
        //проверка повторяющихся последовательностей точек
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
        //построение маршрута
        public static int[] buildPath(int count)
        {
            int[] path = new int[count + 1];
            for (int i = 0; i < count; i++)
                path[i] = i;
            path[0] = 0;
            path[path.Length - 1] = path[0];
            return path;
        }
        //функции для муравьиного алгоритма
        //построение маршрута муравьев
        public static int[] buildPathAnt(int count)
        {
            int[] path = new int[count];
            for (int i = 0; i < count; i++)
                path[i] = i;
            path[0] = 0;
            path[path.Length - 1] = path[0];
            return path;
        }
        //создание матрицы расстояний
        public static float[,] distanceMatrix(Point[] points, int count)
        {
            float[,] arrPoints = new float[/*points.Length, points.Length*/ count, count];
            for (int i = 0; i < count; i++)
                for (int j = 0; j < count; j++)
                    arrPoints[i, j] = arrPoints[j, i] = (float)Way(points[i].X, points[i].Y, points[j].X, points[j].Y);
            return arrPoints;
        }
        //следующая вершина
        public static int nextPick(float a, float b, float[,] distances, float[,] pheromones, int[] path, int currentPick)
        {
            float[] vars = new float[path.Length];
            float sum = 0;
            vars[currentPick] = 0;
            for (int j = 0; j < vars.Length; j++)
                if (currentPick != j)
                    sum += (float)(Math.Pow((1 / distances[currentPick, j]), a) * Math.Pow(pheromones[currentPick, j], b));
            for (int i = 0; i < vars.Length; i++)
                if (i != currentPick)
                    vars[i] = (float)(Math.Pow((1 / distances[i, currentPick]), a) * Math.Pow(pheromones[i, currentPick], b)) / sum;
            Random rnd = new Random();
            float y = (float)rnd.NextDouble();
            sum = vars[0];
            int answ = 1;
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
        //обновление феромонов
        public static float[,] refreshPheromones(float[,] pheromones, float P, int[] path, float[,] distances)
        {
            float[,] newPheromones = pheromones;
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
        #endregion
        public Form1()
        {
            InitializeComponent();
            g = pictureBox1.CreateGraphics();
            count = Convert.ToInt32(numericUpDown1.Value);    
        }

        //очитска всех полей, выводящих информацию
        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
            MD = true;
            clicksMD = 0;
            textBox1.Text = "";
            button3.Enabled = true;
            textBox4.Text = "";
            Array.Clear(points, 0, points.Length);
        }
        //расстановка точек вручную
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (MD == true)
            {
                Point(e.X, e.Y);
                points[clicksMD].X = e.X;
                points[clicksMD].Y = e.Y;
                clicksMD++;
                if (clicksMD >= count) MD = false;
            }
        }



        private void button2_Click(object sender, EventArgs e)
        {
            t = Convert.ToDouble(textBox2.Text);
            a = Convert.ToDouble(textBox3.Text);
            List<int[]> arList = new List<int[]>();
            int[] path = buildPath((int)numericUpDown1.Value);
            
            points[path.Length - 1] = points[0];
            path = changePath(path);
            arList.Add(path);
            while (t > 1)
            {
                shortPath = fullPath(path, points);
                int[] newpath = new int[path.Length];
                path.CopyTo(newpath, 0);
                while (!checkList(arList, newpath))
                    newpath = changePath(newpath);
                arList.Add(newpath);
                if (fullPath(newpath, points) <= shortPath)
                {
                    shortPath = fullPath(newpath, points);
                    path = newpath;
                }
                else
                {
                    if (checkTemper(fullPath(newpath, points) - shortPath, t))
                    {
                        shortPath = fullPath(newpath, points);
                        path = newpath;
                    }

                }
                t = a * t;          
            }

            textBox4.Text += ($"Метод отжига: {fullPath(path, points):f2}") + Environment.NewLine;

            for (int i = 1; i < path.Length; i++)
            {
                g.DrawLine(brush2, points[path[i - 1]].X, points[path[i - 1]].Y, points[path[i]].X, points[path[i]].Y);
            }
        }

        //рандомное расставление точек
        private void button3_Click(object sender, EventArgs e)
        {
            MD = false;
            Random rnd = new Random();
            for (int i = 0; i < count; i++)
            {
                int x = rnd.Next(10, pictureBox1.Width-10);
                int y = rnd.Next(10, pictureBox1.Height-10);
                Point(x, y);
                points[i].X = x;
                points[i].Y = y;
            }
            button3.Enabled = false;
        }

        #region EnterFields
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            t = Convert.ToDouble(textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            A = (float)(numericUpDown2.Value);
            B = (float)(numericUpDown3.Value);
            P = (float)Convert.ToDouble(textBox5.Text);
            float[,] distances = distanceMatrix(points, (int)numericUpDown1.Value);
            float[,] pheromones = new float[distances.GetLength(0), distances.GetLength(1)];
            int[] path = buildPathAnt((int)numericUpDown1.Value);
            for (int i = 0; i < pheromones.GetLength(0); i++)
            {
                for (int j = 0; j < pheromones.GetLength(1); j++)
                    if (j != i)
                        pheromones[i, j] = 1;
            }

            int[] new_path = new int[path.Length + 1];
            new_path[0] = path[0];
            int next_pick = 0;
            for (int k = 0; k < (int)numericUpDown6.Value; k++)
            {
                for (int i = 0; i < path.Length; i++)
                    new_path[i] = 0;
                for (int i = 1; i < path.Length; i++)
                {
                    do
                    {
                        next_pick = nextPick(A, B, distances, pheromones, path, path[i]);
                    } while (new_path.Contains(next_pick));
                    new_path[i] = next_pick;
                }
                pheromones = refreshPheromones(pheromones, P, new_path, distances);
                new_path[new_path.Length - 1] = new_path[0];
            }
            for (int i = 1; i < new_path.Length; i++)
            {
                g.DrawLine(brush3, points[new_path[i - 1]].X + 1, points[new_path[i - 1]].Y + 1, points[new_path[i]].X + 1, points[new_path[i]].Y + 1);
            }
            textBox4.Text += ($"Муравьиный алгоритм: {fullPath(new_path, points):f2}") + Environment.NewLine;
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            a = Convert.ToDouble(textBox3.Text);
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            count = Convert.ToInt32(numericUpDown1.Value);
        }
        #endregion
    }
}
