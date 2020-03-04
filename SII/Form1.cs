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
        public Pen brush = new Pen(Color.Black, 3);
        double t, a, shortPath;
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
            count = Convert.ToInt32(numericUpDown1.Value);
            t = Convert.ToDouble(textBox2.Text);
            a = Convert.ToDouble(textBox3.Text);
            List<int[]> arList = new List<int[]>();
            int[] path = new int[count+1];
            path[0] = 0;
            points[count] = points[0];
            path[count] = path[0];
            for (int i = 0; i < count; i++)
                path[i] = i;
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
                //для вывода промежуточного результата. Можно удалить вместе с textBox
                textBox1.Text += ($"Parh: {fullPath(path, points):f2} > Temp: {t:f2}") + Environment.NewLine;
                
            }
            textBox4.Text = ($"{fullPath(path, points):f2}") + Environment.NewLine;
            for (int i = 1; i < path.Length; i++)
            {
                g.DrawLine(brush, points[path[i - 1]].X, points[path[i - 1]].Y, points[path[i]].X, points[path[i]].Y);
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
