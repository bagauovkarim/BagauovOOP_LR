using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BagauovOOP_LR3
{
    public partial class Form1 : Form
    {
        private List<CCircle> circles = new List<CCircle>(); // Список для хранения кругов
        private Point startPoint;
        private bool selecting = false;
        private Rectangle selectionRectangle;
        private List<CCircle> selectedCircles = new List<CCircle>();
        private bool mouseMoved = false;
        

        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(Form1_Paint_DrawCircles);
            this.MouseClick += new MouseEventHandler(Form1_MouseClick);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_Paint_DrawCircles(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем все круги
            for (int i = 0; i < circles.Count; i++)
            {
                CCircle circle = circles[i];

                if (selectedCircles.Contains(circle))
                {
                    // Рисуем выделенный круг
                    g.DrawEllipse(Pens.Blue, circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2);
                    g.FillEllipse(Brushes.Red, circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2);
                }
                else
                {
                    // Рисуем обычный круг
                    g.DrawEllipse(Pens.Black, circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2);
                }
            }

            // Рисуем прямоугольник выделения, если идет выделение
            if (selecting)
            {
                using (Pen pen = new Pen(Color.Blue))
                {
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            bool clickedOnCircle = false;


            for (int i = 0; i < circles.Count; i++)
            {
                CCircle circle = circles[i];
                Rectangle circleBounds = new Rectangle(circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2);

                if (circleBounds.Contains(e.Location))
                {
                    clickedOnCircle = true;

                    if (!selectedCircles.Contains(circle))
                    {
                        selectedCircles.Add(circle);
                    }


                break;
                }

            }
            if (!clickedOnCircle && !mouseMoved)
            {
                if (selectedCircles.Count == 0)
                {
                    circles.Add(new CCircle(e.X, e.Y, 30));
                    
                }
                
                
                selectedCircles.Clear();
                



            }
            this.Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = e.Location;
                selecting = true;
                mouseMoved = false; // Сбрасываем флаг mouseMoved
                selectionRectangle = new Rectangle(startPoint.X, startPoint.Y, 0, 0);
                this.Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selecting)
            {
                mouseMoved = true; // Устанавливаем флаг mouseMoved при движении мыши
                int x = Math.Min(startPoint.X, e.X);
                int y = Math.Min(startPoint.Y, e.Y);
                int width = Math.Abs(e.X - startPoint.X);
                int height = Math.Abs(e.Y - startPoint.Y);

                selectionRectangle = new Rectangle(x, y, width, height);
                this.Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selecting = false;

                if (mouseMoved)
                {
                    // Выделяем круги, которые пересекаются с прямоугольником выделения
                    selectedCircles.Clear();
                    for (int i = 0; i < circles.Count; i++)
                    {
                        CCircle circle = circles[i];
                        Rectangle circleBounds = new Rectangle(circle.X - circle.Radius, circle.Y - circle.Radius, circle.Radius * 2, circle.Radius * 2);
                        if (selectionRectangle.IntersectsWith(circleBounds))
                        {
                            selectedCircles.Add(circle);
                        }
                    }
                }

                // Сбрасываем флаг mouseMoved
                mouseMoved = false;

                this.Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                // Удаляем выделенные круг
                for (int i = 0; i < selectedCircles.Count; i++)
                {
                    circles.Remove(selectedCircles[i]);
                }

                // Очищаем список выделенных кругов
                selectedCircles.Clear();

                // Перерисовываем форму
                this.Invalidate();
            }
        }
    }

    public class CCircle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; private set; }

        public CCircle(int x, int y, int radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }
    }
}