using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace BagauovOOP_LR4
{
    public partial class Form1 : Form
    {
        private CircleContainer circles = new CircleContainer(); // Контейнер для кругов
        private Point startPoint;
        private bool selecting = false;
        private Rectangle selectionRectangle;
        private bool mouseMoved = false;
        private bool ctrlPressed = false;

        public Form1()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(Form1_Paint_DrawCircles);
            this.MouseClick += new MouseEventHandler(Form1_MouseClick);
            this.MouseDown += new MouseEventHandler(Form1_MouseDown);
            this.MouseMove += new MouseEventHandler(Form1_MouseMove);
            this.MouseUp += new MouseEventHandler(Form1_MouseUp);
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);
        }

        private void Form1_Paint_DrawCircles(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем все круги через контейнер
            for (circles.First(); !circles.EOL(); circles.Next())
            {
                circles.GetCurrent().Draw(g);
            }

            // Рисуем прямоугольник выделения, если идет выделение
            if (selecting)
            {
                Pen pen = new Pen(Color.Blue);
                e.Graphics.DrawRectangle(pen, selectionRectangle);
                pen.Dispose();

            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            bool clickedOnCircle = false;

            // Проверяем, попал ли клик на какой-либо круг
            for (circles.First(); !circles.EOL(); circles.Next())
            {
                if (circles.GetCurrent().Contains(e.Location))
                {
                    clickedOnCircle = true;

                    // Если круг уже выделен, снимаем выделение
                    if (circles.GetCurrent().IsCircleSelected())
                    {
                        circles.GetCurrent().Deselect();
                    }
                    else
                    {
                        // Если круг не выделен, выделяем его
                        circles.GetCurrent().Select();
                    }
                    break;
                }
            }

            // Если клик был вне всех кругов и мышь не двигалась
            if (!clickedOnCircle && !mouseMoved)
            {
                if (circles.GetSelectedCount() == 0)
                {
                    // Добавляем новый круг (не выделенный)
                    CCircle newCircle = new CCircle(e.X, e.Y, 30);
                    circles.Add(newCircle);
                }
                else
                {
                    circles.DeselectAll(); // Снимаем выделение со всех кругов
                }
            }

            this.Invalidate();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ctrlPressed)
            {
                startPoint = e.Location;
                selecting = true;
                mouseMoved = false;
                selectionRectangle = new Rectangle(startPoint.X, startPoint.Y, 0, 0);
                this.Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (selecting && ctrlPressed)
            {
                mouseMoved = true;
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
                    circles.SelectCirclesInRectangle(selectionRectangle); // Выделяем круги в прямоугольнике
                }

                mouseMoved = false;
                this.Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                circles.RemoveSelected(); // Удаляем выделенные круги
                this.Invalidate();
            }

            if (e.KeyCode == Keys.ControlKey)
            {
                ctrlPressed = true;
            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                ctrlPressed = false;

                if (selecting)
                {
                    selecting = false;
                    selectionRectangle = Rectangle.Empty;
                    this.Invalidate();
                }
            }
        }
    }

    // Класс-контейнер для кругов
    public class CircleContainer
    {
        private List<CCircle> circles = new List<CCircle>();
        private int currentIndex = 0;

        // Добавление круга
        public void Add(CCircle circle)
        {
            circles.Add(circle);
        }

        // Удаление выделенных кругов
        public void RemoveSelected()
        {
            for (int i = circles.Count - 1; i >= 0; i--)
            {
                if (circles[i].IsCircleSelected())
                {
                    circles.RemoveAt(i);
                }
            }
        }

        // Снятие выделения со всех кругов
        public void DeselectAll()
        {
            for (int i = 0; i < circles.Count; i++)
            {
                circles[i].Deselect();
            }
        }

        // Выделение кругов, пересекающихся с прямоугольником
        public void SelectCirclesInRectangle(Rectangle rect)
        {
            for (int i = 0; i < circles.Count; i++)
            {
                if (circles[i].IntersectsWith(rect))
                {
                    circles[i].Select();
                }
            }
        }

        // Получение количества выделенных кругов
        public int GetSelectedCount()
        {
            int count = 0;
            for (int i = 0; i < circles.Count; i++)
            {
                if (circles[i].IsCircleSelected())
                {
                    count++;
                }
            }
            return count;
        }

        // Методы для итерации по кругам
        public void First()
        {
            currentIndex = 0;
        }

        public bool EOL()
        {
            return currentIndex >= circles.Count;
        }

        public void Next()
        {
            currentIndex++;
        }

        public CCircle GetCurrent()
        {
            return circles[currentIndex];
        }
    }

    // Класс круга
    public class CCircle
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Radius { get; private set; }
        private bool isCircleSelected;

        public CCircle(int x, int y, int radius)
        {
            X = x;
            Y = y;
            Radius = radius;
            isCircleSelected = false;
        }

        // Проверка, попадает ли точка в круг
        public bool Contains(Point point)
        {
            int dx = point.X - X;
            int dy = point.Y - Y;
            return dx * dx + dy * dy <= Radius * Radius;
        }

        // Рисование круга
        public void Draw(Graphics g)
        {
            if (isCircleSelected)
            {
                g.DrawEllipse(Pens.Blue, X - Radius, Y - Radius, Radius * 2, Radius * 2);
                g.FillEllipse(Brushes.Red, X - Radius, Y - Radius, Radius * 2, Radius * 2);
            }
            else
            {
                g.DrawEllipse(Pens.Black, X - Radius, Y - Radius, Radius * 2, Radius * 2);
            }
        }

        // Проверка пересечения с прямоугольником
        public bool IntersectsWith(Rectangle rect)
        {
            Rectangle circleBounds = new Rectangle(X - Radius, Y - Radius, Radius * 2, Radius * 2);
            return rect.IntersectsWith(circleBounds);
        }

        // Управление выделением
        public void Select()
        {
            isCircleSelected = true;
        }

        public void Deselect()
        {
            isCircleSelected = false;
        }

        public bool IsCircleSelected()
        {
            return isCircleSelected;
        }
    }
}