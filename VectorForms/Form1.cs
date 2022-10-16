using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
using System.Drawing.Drawing2D;

namespace VectorForms
{
    public partial class vectorform1 : Form
    {
        // Доступ к переменной
        public int PIX_IN_ONE { get; private set; } = 20;
        // Длина стрелки
        const int ARR_LEN = 10;
        // Половина длины pictureBox
        public int SizeX { get; private set; }
        // Половина высоты pictureBox
        public int SizeY { get; private set; }
        // Поиск максимума/минимума
        private bool Maximum=true;
        // Левая граница
        private int tbLeft = -3;
        // Правая граница
        private int tbRight=5;
        // Точность вычислений
        private double Accurate=1;

        //Начальные установки
        public vectorform1()
        {
            InitializeComponent();
            SizeX = pictureBox1.ClientSize.Width / 2;
            SizeY = pictureBox1.ClientSize.Height / 2;
            DrawCoordinateSystem();

            Bar1.Visible = false;
            Bar1.Value = 0;
            lAns.Visible = false;
            tbAns1.Visible = false;
            label6.Visible = false;
            tbAns2.Visible = false;
        }

        // Событие завершения работы программы
        private void bClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        // Событие выполнеия работы
        private void DoWork_Click(object sender, EventArgs e)
        {
            splitContainer1.Panel1.Enabled = false;

            if (tbResolution.Text!="")
            {
                PIX_IN_ONE = int.Parse(tbResolution.Text);
            }
            if (tbAcc.Text!="")
            {
                Accurate = double.Parse(tbAcc.Text);
            }
            if (tbL.Text!="")
            {
                tbLeft = int.Parse(tbL.Text);
            }
            if (tbR.Text!="")
            {
                tbRight = int.Parse(tbR.Text);
            }

            if (25 - Math.Pow(tbLeft, 2) < 0 || 25 - Math.Pow(tbRight, 2) < 0)
            {
                MessageBox.Show("Выход за ОДЗ!");
                splitContainer1.Panel1.Enabled = true;
                return;
            }
            if (tbLeft > tbRight)
            {
                MessageBox.Show("Неверный ввод границ!");
                splitContainer1.Panel1.Enabled = true;
                return;
            }

            if (Bar1.Visible == true)
            {
                Bar1.Visible = false;
                Bar1.Value = 0;
                lAns.Visible = false;
                tbAns1.Visible = false;
                label6.Visible = false;
                tbAns2.Visible = false;
            }

            Bar1.Visible = true;
            while (Bar1.Value!=Bar1.Maximum)
            {
                Bar1.PerformStep();
                System.Threading.Thread.Sleep(100);
            }

            Task();

            lAns.Visible = true;
            tbAns1.Visible = true;
            label6.Visible = true;
            tbAns2.Visible = true;

            splitContainer1.Panel1.Enabled = true;
        }

        //Выполнение работы
        private void Task()
        {           
            if (rbMethod1.Checked)
            {
                UniformSearch();
            }
            else if (rbMethod2.Checked)
            {
                DichotomyMethod();
            }
            else if (rbMethod3.Checked)
            {
                GoldenSectionMethod();
            }
            else if (rbMethod4.Checked)
            {
                FibonacciMethod();
            }
            else if (rbMethod5.Checked)
            {
                ChordMethod();
            }
            else if(rbMethod6.Checked)
            {
                TangentMethod();
            }
        }

        //Метод равномерного поиска
        private void UniformSearch()
        {
            List<double> Max = new List<double>();
            List<double> Min = new List<double>();
            List<double[]> x = new List<double[]>();

            double count = tbLeft;
            while (count <= tbRight)
            {
                x.Add(new double[] { Math.Round(count, 2), F(count) });
                count += Accurate;
            }

            Point[] points = new Point[x.Count];

            //Вывод значений функций и отображение
            tbAns2.Text = "   Результаты значения функции";
            for (int i = 0; i < x.Count; i++)
            {
                tbAns2.Text += Environment.NewLine + "x = " + x[i][0] + ": y = " + x[i][1];
                points[i] = new Point((int)(x[i][0] * PIX_IN_ONE + SizeX), (int)(x[i][1] * -PIX_IN_ONE + SizeY));
            }

            // Поиск Max/ Min
            Max.Add(0);Max.Add(x[0][1]);
            Min.Add(0);Min.Add(x[0][1]);
            for (int i = 1; i < x.Count; i++)
            {
                if (x[i][1] < Min[1])
                {
                    Min[0] = x[i][0];
                    Min[1] = x[i][1];
                }
                if (x[i][1] > Max[1])
                {
                    Max[0] = x[i][0];
                    Max[1] = x[i][1];
                }
            }

            if (Maximum)
            {
                tbAns1.Text = $"Максимум при x={Max[0]}: y ={Max[1]} при ε = {Accurate}";
            }
            else
            {
                tbAns1.Text = $"Минимум при x={Min[0]}: y ={Min[1]} при ε = {Accurate}";
            }

            FigureDisplay(points);
        }

        //Метод дихотомии 
        private void DichotomyMethod()
        {           
            double a= tbLeft, b= tbRight;
            double c = (a + b) / 2;

            int i = 0;

            tbAns2.Text = "   Результаты значения функции";
            //while (Math.Abs(Math.Round(F(c + Accurate) - F(c - Accurate), 4)) > Accurate)
            while (F(c+Accurate)!=F(c-Accurate))
            {
                tbAns2.Text += Environment.NewLine + $"x{i} = {c} +/- {Accurate}";
                tbAns2.Text += Environment.NewLine + $"     F+({c + Accurate}) = {F(c + Accurate)}";
                tbAns2.Text += Environment.NewLine + $"     F-({c - Accurate}) = {F(c - Accurate)}";

                if (F(c+Accurate)<F(c-Accurate))
                {
                    b = c;
                }
                else
                {
                    a = c;
                }

                c = (a + b) / 2;
                i++;
            }

            tbAns2.Text += Environment.NewLine + $"x{i} = {c} +/- {Accurate}";
            tbAns2.Text += Environment.NewLine + $"     F+({c + Accurate}) = {F(c + Accurate)}";
            tbAns2.Text += Environment.NewLine + $"     F-({c - Accurate}) = {F(c - Accurate)}";

            tbAns1.Text = $"Максимум при x={c}: y ={F(c)} при ε = {Accurate}";
        }

        //Функция f(x)
        static double F(double x)
        {
            return Math.Round(Math.Sqrt(25 - Math.Pow(x, 2)), 4);
        }

        //Метод золотого сечения
        private void GoldenSectionMethod()
        {
            throw new NotImplementedException();
        }

        //Метод Фибоначчи
        private void FibonacciMethod()
        {
            throw new NotImplementedException();
        }

        //Метод хорд
        private void ChordMethod()
        {
            throw new NotImplementedException();
        }

        //Метод касательных
        private void TangentMethod()
        {
            throw new NotImplementedException();
        }

        //Отрисовка графика
        private void FigureDisplay(Point[] points)
        {
            Graphics graphics = pictureBox1.CreateGraphics();
            Pen pen = new Pen(Color.FromArgb(33, 150, 243), 3f);

            graphics.DrawLines(pen, points);

            Pen pen1 = new Pen(Color.Red, 1f);
            graphics.DrawLine(pen1, tbLeft * PIX_IN_ONE + SizeX, -SizeY * PIX_IN_ONE, tbLeft * PIX_IN_ONE + SizeX, SizeY * PIX_IN_ONE);
            graphics.DrawLine(pen1, tbRight * PIX_IN_ONE + SizeX, -SizeY * PIX_IN_ONE, tbRight * PIX_IN_ONE + SizeX, SizeY * PIX_IN_ONE);
        }

        // Очистка системы координат
        private void bClear_Click(object sender, EventArgs e)
        {
            if (tbResolution.Text != "")
            {
                PIX_IN_ONE = int.Parse(tbResolution.Text);
            }

            Graphics graphics = pictureBox1.CreateGraphics();
            graphics.Clear(Color.Silver);

            DrawCoordinateSystem();
        }

        // Находим максимум
        private void bMax_Click(object sender, EventArgs e)
        {
            Maximum = true;
        }

        // Находим минимум
        private void bMin_Click(object sender, EventArgs e)
        {
            Maximum = false;
        }

        // Рисовка системы координат
        public void DrawCoordinateSystem()
        {
            Bitmap main_image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            Graphics gr = Graphics.FromImage(main_image);

            int w = pictureBox1.ClientSize.Width / 2;
            int h = pictureBox1.ClientSize.Height / 2;

            pictureBox1.Image = new Bitmap(w, h);

            //Смещение начала координат в центр PictureBox
            gr.TranslateTransform(w, h);
            DrawXAxis(new Point(-w, 0), new Point(w, 0), ref gr);
            DrawYAxis(new Point(0, h), new Point(0, -h), ref gr);
            //Центр координат
            gr.FillEllipse(Brushes.Purple, -2, -2, 4, 4);

            pictureBox1.Image= main_image;
        }

        // Рисование оси X
        private void DrawXAxis(Point start, Point end, ref Graphics g)
        {
            //Деления в положительном направлении оси
            for (int i = PIX_IN_ONE; i < end.X - ARR_LEN; i += PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, i, -5, i, 5);
                DrawText(new Point(i, 5), (i / PIX_IN_ONE).ToString(), ref g);
            }
            //Деления в отрицательном направлении оси
            for (int i = -PIX_IN_ONE; i > start.X; i -= PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, i, -5, i, 5);
                DrawText(new Point(i, 5), (i / PIX_IN_ONE).ToString(), ref g);
            }
            //Ось
            g.DrawLine(Pens.Black, start, end);
            //Стрелка направления осей
            g.DrawLines(Pens.Black, GetArrow(start.X, start.Y, end.X, end.Y, ARR_LEN));
        }

        // Рисование оси Y
        private void DrawYAxis(Point start, Point end, ref Graphics g)
        {
            //Деления в отрицательном направлении оси
            for (int i = PIX_IN_ONE; i < start.Y; i += PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, -5, i, 5, i);
                DrawText(new Point(5, i), (-i / PIX_IN_ONE).ToString(), ref g, true);
            }
            //Деления в положительном направлении оси
            for (int i = -PIX_IN_ONE; i > end.Y + ARR_LEN; i -= PIX_IN_ONE)
            {
                g.DrawLine(Pens.Black, -5, i, 5, i);
                DrawText(new Point(5, i), (-i / PIX_IN_ONE).ToString(), ref g, true);
            }
            //Ось
            g.DrawLine(Pens.Black, start, end);
            //Стрелка
            g.DrawLines(Pens.Black, GetArrow(start.X, start.Y, end.X, end.Y, ARR_LEN));
        }

        // Рисование текста
        private void DrawText(Point point, string text, ref Graphics g, bool isYAxis = false)
        {
            var f = new Font("FontFamily", 7); 
            var size = g.MeasureString(text, f);
            var pt = isYAxis
                ? new PointF(point.X + 1, point.Y - size.Height / 2)
                : new PointF(point.X - size.Width / 2, point.Y + 1);
            var rect = new RectangleF(pt, size);
            g.DrawString(text, f, Brushes.Black, rect);
        }

        // Вычисление стрелки оси
        private static PointF[] GetArrow(float x1, float y1, float x2, float y2, float len = 10, float width = 4)
        {
            PointF[] result = new PointF[3];
            //направляющий вектор отрезка
            var n = new PointF(x2 - x1, y2 - y1);
            //Длина отрезка
            var l = (float)Math.Sqrt(n.X * n.X + n.Y * n.Y);
            //Единичный вектор
            var v1 = new PointF(n.X / l, n.Y / l);
            //Длина стрелки
            n.X = x2 - v1.X * len;
            n.Y = y2 - v1.Y * len;
            result[0] = new PointF(n.X + v1.Y * width, n.Y - v1.X * width);
            result[1] = new PointF(x2, y2);
            result[2] = new PointF(n.X - v1.Y * width, n.Y + v1.X * width);
            return result;
        }

        //Тоже "Выполнить работу"
        private void tbAcc_KeyPress(object sender, KeyPressEventArgs ex)
        {
            if (ex.KeyChar == (char)13)
            {
                splitContainer1.Panel1.Enabled = false;

                if (tbResolution.Text != "")
                {
                    PIX_IN_ONE = int.Parse(tbResolution.Text);
                }
                if (tbAcc.Text != "")
                {
                    Accurate = double.Parse(tbAcc.Text);
                }
                if (tbL.Text != "")
                {
                    tbLeft = int.Parse(tbL.Text);
                }
                if (tbR.Text != "")
                {
                    tbRight = int.Parse(tbR.Text);
                }

                if (25 - Math.Pow(tbLeft, 2) < 0 || 25 - Math.Pow(tbRight, 2) < 0)
                {
                    MessageBox.Show("Выход за ОДЗ!");
                    splitContainer1.Panel1.Enabled = true;
                    return;
                }
                if (tbLeft > tbRight)
                {
                    MessageBox.Show("Неверный ввод границ!");
                    splitContainer1.Panel1.Enabled = true;
                    return;
                }

                if (Bar1.Visible == true)
                {
                    Bar1.Visible = false;
                    Bar1.Value = 0;
                    lAns.Visible = false;
                    tbAns1.Visible = false;
                    label6.Visible = false;
                    tbAns2.Visible = false;
                }

                Bar1.Visible = true;
                while (Bar1.Value != Bar1.Maximum)
                {
                    Bar1.PerformStep();
                    System.Threading.Thread.Sleep(100);
                }

                Task();

                lAns.Visible = true;
                tbAns1.Visible = true;
                label6.Visible = true;
                tbAns2.Visible = true;

                splitContainer1.Panel1.Enabled = true;
            }
        }
    }
}
