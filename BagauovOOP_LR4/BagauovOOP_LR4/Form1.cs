using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static BagauovOOP_LR4.Form1;

namespace BagauovOOP_LR4
{
    public partial class Form1 : Form
    {
        private ShapeContainer shapes = new ShapeContainer();
        
        private string selectedShapeType = "";

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            InitializeToolboxEvents();
        }

        private void InitializeToolboxEvents()
        {
            foreach (Control c in toolboxPanel.Controls)
            {
                if (c is PictureBox tool)
                {
                    tool.Click += (s, e) =>
                    {
                        selectedShapeType = tool.Tag.ToString();
                        foreach (Control ct in toolboxPanel.Controls)
                            ct.BackColor = Color.White;
                        tool.BackColor = Color.LightBlue;
                        this.Cursor = Cursors.Cross;
                    };

                    tool.MouseEnter += (s, e) => tool.BackColor = Color.LightGray;
                    tool.MouseLeave += (s, e) =>
                    {
                        if (tool.Tag.ToString() != selectedShapeType)
                            tool.BackColor = Color.White;
                    };
                }
            }
        }



        public abstract class CShape
        {
            public Point Position { get; set; }
            public Size Size { get; set; }
            public Color FillColor { get; set; } = Color.White;
            public bool IsSelected { get; set; }
            public string Name { get; set; } = "Фигура";

            // Абстрактные методы (должны быть реализованы в наследниках)
            public abstract void Draw(Graphics g);
            public abstract bool Contains(Point point);
            public abstract Rectangle GetBoundingBox();

            // Виртуальные методы (могут быть переопределены в наследниках)
            public virtual void Move(Point newPosition)
            {
                Position = newPosition;
            }

            public virtual void Resize(Size newSize)
            {
                Size = newSize;
            }

            public virtual void Select()
            {
                IsSelected = true;
                FillColor = Color.Red; // Выделение красной рамкой
                
            }

            public virtual void Deselect()
            {
                IsSelected = false;
                FillColor = Color.White;
                
            }

            // Общие методы для всех фигур
            public void ChangeFillColor(Color newColor)
            {
                FillColor = newColor;
            }


            public bool IntersectsWith(Rectangle rect)
            {
                return GetBoundingBox().IntersectsWith(rect);
            }

            public virtual void DrawSelection(Graphics g)
            {
                if (IsSelected)
                {
                    using (var pen = new Pen(Color.DarkBlue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                    {
                        var bounds = GetBoundingBox();
                        bounds.Inflate(5, 5);
                        g.DrawRectangle(pen, bounds);
                    }
                }
            }
        }

        /*public class CRectangle : CShape
        {

        }*/

        /*public class CSquare : CShape
        {

        }*/

        /*public class CTriangle : CShape
        {

        }*/

        /*public class CEllipse : CShape
        {

        }*/

        /*public class CLine : CShape
        {

        }*/

        public class CCircle : CShape
        {
            public int Radius
            {
                get { return Size.Width / 2; }
                private set { Size = new Size(value * 2, value * 2); }
            }

            public CCircle(int x, int y, int radius)
            {
                Position = new Point(x, y);
                Radius = radius;
                Name = "Круг";
                FillColor = Color.White; // Белая заливка по умолчанию
            }

            public override void Draw(Graphics g)
            {
                // Черный контур (как в исходном коде)
                Pen pen = new Pen(Color.Black, IsSelected ? 2 : 1);
                Brush brush = new SolidBrush(FillColor);

                int diameter = Size.Width;
                g.FillEllipse(brush, Position.X - Radius, Position.Y - Radius, diameter, diameter);
                g.DrawEllipse(pen, Position.X - Radius, Position.Y - Radius, diameter, diameter);

                pen.Dispose();
                brush.Dispose();

                if (IsSelected)
                {
                    // Синяя пунктирная рамка выделения
                    Pen selectionPen = new Pen(Color.Blue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                    var bounds = GetBoundingBox();
                    bounds.Inflate(5, 5);
                    g.DrawRectangle(selectionPen, bounds);
                    selectionPen.Dispose();
                }
            }

            public override bool Contains(Point point)
            {
                int dx = point.X - Position.X;
                int dy = point.Y - Position.Y;
                return dx * dx + dy * dy <= Radius * Radius;
            }

            public override Rectangle GetBoundingBox()
            {
                return new Rectangle(
                    Position.X - Radius,
                    Position.Y - Radius,
                    Radius * 2,
                    Radius * 2);
            }

            public void SetRadius(int newRadius)
            {
                Radius = newRadius; // Используем приватный сеттер
            }

            public override void Select()
            {
                base.Select();
                FillColor = Color.Red; // Красная заливка при выделении
            }

            public override void Deselect()
            {
                base.Deselect();
                FillColor = Color.White; // Белая заливка при снятии выделения
            }
        }

        public class ShapeContainer
        {
            private List<CShape> shapes = new List<CShape>();
            private int currentIndex = 0;

            // Добавление фигуры
            public void Add(CShape shape)
            {
                shapes.Add(shape);
            }

            // Удаление выделенных фигур
            public void RemoveSelected()
            {
                for (int i = shapes.Count - 1; i >= 0; i--)
                {
                    if (shapes[i].IsSelected)
                    {
                        shapes.RemoveAt(i);
                    }
                }
            }

            // Снятие выделения со всех фигур
            public void DeselectAll()
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    shapes[i].Deselect();
                }
            }

            // Выделение фигур в прямоугольной области
            public void SelectInRectangle(Rectangle rect)
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].IntersectsWith(rect))
                    {
                        shapes[i].Select();
                    }
                }
            }

            // Получение количества выделенных фигур
            public int GetSelectedCount()
            {
                int count = 0;
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (shapes[i].IsSelected)
                    {
                        count++;
                    }
                }
                return count;
            }

            // Методы для ручной итерации (как в оригинальном CircleContainer)
            public void First()
            {
                currentIndex = 0;
            }

            public bool EOL()
            {
                return currentIndex >= shapes.Count;
            }

            public void Next()
            {
                currentIndex++;
            }

            public CShape GetCurrent()
            {
                return shapes[currentIndex];
            }

            // Дополнительные базовые методы
            public int Count
            {
                get { return shapes.Count; }
            }

            public void Clear()
            {
                shapes.Clear();
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            // Проверяем, что клик был левой кнопкой мыши и выбран инструмент "Круг"
            if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Круг")
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);

                // Создаем новый круг с радиусом 30 пикселей
                CCircle newCircle = new CCircle(clickPoint.X, clickPoint.Y, 30);

                // Добавляем круг в контейнер
                shapes.Add(newCircle);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый круг в ({clickPoint.X}, {clickPoint.Y})");
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (shapes.First(); !shapes.EOL(); shapes.Next())
            {
                CShape currentShape = shapes.GetCurrent();

                // Проверяем, что фигура существует (защита от null)
                if (currentShape != null)
                {
                    try
                    {
                        currentShape.Draw(e.Graphics);
                    }
                    catch (Exception ex)
                    {
                        // Обработка ошибок рисования (можно заменить на логирование)
                        Console.WriteLine($"Ошибка при рисовании фигуры: {ex.Message}");
                    }
                }
            }
        }
    }
}