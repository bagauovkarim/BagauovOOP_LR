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
        private Color selectedColor = Color.Black; // Цвет по умолчанию
        private bool isColorMode = false;
        bool clickedOnShape = false;
        private bool RectangleSelection = false;  // Флаг для отслеживания процесса выделения
        private Point selectionStartPoint; // Начальная точка выделения
        private Rectangle selectionRectangle; // Прямоугольник выделения
        private bool mouseMoved = false;
        private bool ctrlPressed = false;

        public Form1()
        {
            InitializeComponent();
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
                        tool.BackColor = Color.LightSkyBlue;
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


    public interface IShape
    {
        // Методы, которые должны быть реализованы в классе
        void Draw(Graphics g);
        bool Contains(Point point);
        Rectangle GetBoundingBox();

        void Select();
        void Deselect();
        void Move(Point newPosition);
        void Resize(Size newSize);
      
    }



    public abstract class CShape: IShape
    {
       public Point Position { get; set; }
       public Size Size { get; set; }
       public Color FillColor { get; set; } = Color.White;
       public string Name { get; set; } = "Фигура";
            
       public bool isSelected = false;
            
            
       // Метод для выделения фигуры
       public virtual void Select()
       {
            isSelected = true;  // Устанавливаем флаг выделения
       }

       // Метод для снятия выделения
       public virtual void Deselect()
       {
           isSelected = false;  // Снимаем выделение
           FillColor = Color.White;
       }

       // Метод для проверки, выделена ли фигура
       public bool IsSelected()
       {
           return isSelected;  // Возвращаем состояние флага выделения
       }



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

            

       // Общие методы для всех фигур
       public void SetColor(Color newColor)
       {
           FillColor = newColor;
       }


       public bool IntersectsWith(Rectangle rect)
       {
           return GetBoundingBox().IntersectsWith(rect);
       }

       public virtual void DrawSelection(Graphics g)
       {
           if (isSelected)
           {
               using (var pen = new Pen(Color.DarkBlue, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
               {
                   var bounds = GetBoundingBox();
                   bounds.Inflate(5, 5);
                   g.DrawRectangle(pen, bounds);
               }
           }
       }

        
            
    }

        public class CRectangle : CShape
        {

            public CRectangle(int x, int y)
            {
                Position = new Point(x, y);
                Name = "Прямоугольник";
                Size = new Size(60, 40);
                FillColor = Color.White; // Белая заливка по умолчанию
            }

            public override void Draw(Graphics g)
            {
                // Черный контур (как в исходном коде)
                Pen pen = new Pen(Color.Black);
                Brush brush = new SolidBrush(FillColor);

                int Height = Size.Height;
                int Width = Size.Width;
                g.FillRectangle(brush, Position.X - Width / 2, Position.Y - Height / 2, Width, Height);
                g.DrawRectangle(pen, Position.X - Width / 2, Position.Y - Height / 2, Width, Height);

                pen.Dispose();
                brush.Dispose();

               
            }

            public override bool Contains(Point point)
            {
                int height = Size.Height;
                int width = Size.Width;

                // Проверка попадания по прямоугольнику с учетом его смещения относительно центра
                if (point.X > Position.X - width / 2 && point.X < Position.X + width / 2 &&
                    point.Y > Position.Y - height / 2 && point.Y < Position.Y + height / 2)
                {
                    return true;
                }
                return false;
            }

            public override Rectangle GetBoundingBox()
            {
                return new Rectangle(Position.X - Size.Width / 2, Position.Y - Size.Height / 2, Size.Width, Size.Height);

            }

            public override void Select()
            {
                base.Select();
               
            }

            public override void Deselect()
            {
                base.Deselect();
               
            }
        }

        public class CTriangle : CShape
        {
            
            private PointF[] _points = new PointF[3];
            public CTriangle(int x, int y)
            {
                
                Position = new Point(x, y);
                Name = "Треугольник";
                Size = new Size(60, 60);
                FillColor = Color.White; // Белая заливка по умолчанию
                CalculatePoints();
            }

            private void CalculatePoints()
            {
                _points = new PointF[3];

                _points[0] = new PointF(Position.X - Size.Width / 2, Position.Y + Size.Height / 2);
                _points[1] = new PointF(Position.X + Size.Width / 2, Position.Y + Size.Height / 2);
                _points[2] = new PointF(Position.X, Position.Y - Size.Height / 2);
                
            }


            public override void Draw(Graphics g)
            {
                // Черный контур (как в исходном коде)
                Pen pen = new Pen(Color.Black);
                Brush brush = new SolidBrush(FillColor);

                
                g.FillPolygon(brush,_points);
                g.DrawPolygon(pen, _points);

                pen.Dispose();
                brush.Dispose();

               
            }


            private float Sign(PointF p1, PointF p2, PointF p3)
            {
                return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
            }

            public override bool Contains(Point point)
            {
                // Получаем координаты вершин треугольника
                PointF A = _points[0];
                PointF B = _points[1];
                PointF C = _points[2];

                // Вычисляем знаки для каждой стороны треугольника
                float d1 = Sign(point, A, B);
                float d2 = Sign(point, B, C);
                float d3 = Sign(point, C, A);

                // Проверяем, все ли знаки положительные или все отрицательные.
                
                if ((d1 > 0 && d2 > 0 && d3 > 0) || (d1 < 0 && d2 < 0 && d3 < 0)) {
                    return true;
                }
                else
                    return false;


            }

            

            public override Rectangle GetBoundingBox()
            {
                float minX = _points[0].X;
                float minY = _points[0].Y;
                float maxX = _points[0].X;
                float maxY = _points[0].Y;

                foreach (var point in _points)
                {
                    minX = Math.Min(minX, point.X);
                    minY = Math.Min(minY, point.Y);
                    maxX = Math.Max(maxX, point.X);
                    maxY = Math.Max(maxY, point.Y);
                }

                return new Rectangle((int)minX, (int)minY, (int)(maxX - minX), (int)(maxY - minY));

            }

            public override void Select()
            {
                base.Select();
                
            }

            public override void Deselect()
            {
                base.Deselect();
                
            }
        }

        public class CEllipse : CShape
        {

            public CEllipse(int x, int y)
            {
                Position = new Point(x, y);
                Name = "Эллипс";
                Size = new Size(70, 50);
                FillColor = Color.White; // Белая заливка по умолчанию
            }

            public override void Draw(Graphics g)
            {
                // Черный контур (как в исходном коде)
                Pen pen = new Pen(Color.Black);
                Brush brush = new SolidBrush(FillColor);

                g.FillEllipse(brush, Position.X - Size.Width / 2, Position.Y - Size.Height / 2, Size.Width, Size.Height);
                g.DrawEllipse(pen, Position.X - Size.Width / 2, Position.Y - Size.Height / 2, Size.Width, Size.Height);

                pen.Dispose();
                brush.Dispose();

                
            }

            public override bool Contains(Point point)
            {
                double a = Size.Width / 2.0;  // Радиус по X
                double b = Size.Height / 2.0; // Радиус по Y
                double centerX = Position.X;
                double centerY = Position.Y;

                double normalizedX = (point.X - centerX) * (point.X - centerX) / (a * a);
                double normalizedY = (point.Y - centerY) * (point.Y - centerY) / (b * b);

                if (normalizedX + normalizedY <= 1)
                {
                    return true;
                }
                else
                    return false;
            }

            public override Rectangle GetBoundingBox()
            {
                int width = Size.Width;
                int height = Size.Height;
                return new Rectangle(Position.X - width / 2, Position.Y - height / 2, width, height);
            }


            public override void Select()
            {
                base.Select();
                
            }

            public override void Deselect()
            {
                base.Deselect();
                
            }
        }

        public class CLine : CShape
        {
        public CLine(int x, int y)
            {
                Position = new Point(x, y);
                Name = "Линия";
                Size = new Size(100, 0);
                FillColor = Color.Black; // Белая заливка по умолчанию
            }

            public override void Draw(Graphics g)
            {
                // Черный контур (как в исходном коде)
                Pen pen = new Pen(FillColor, 3);
                Brush brush = new SolidBrush(FillColor);

                
                g.DrawLine(pen, Position.X - Size.Width / 2, Position.Y, Position.X + Size.Width / 2, Position.Y + Size.Height);

                pen.Dispose();
                brush.Dispose();

                
            }

            public override bool Contains(Point point)
            {
                int x1 = Position.X - Size.Width / 2; // Левая точка линии
                int x2 = Position.X + Size.Width / 2; // Правая точка линии
                int y = Position.Y; // Высота линии

                // Проверяем, находится ли точка на той же высоте и внутри диапазона X
                if ((point.Y >= y - 2 && point.Y <= y + 2) && (point.X >= x1 && point.X <= x2)) {
                    
                    return true;
                }
                else
                    return false;
            }

            public override Rectangle GetBoundingBox()
            {

                int left = Position.X - Size.Width / 2;
                int top = Position.Y - 2; // Высота линии будет 2 пикселя (толщина линии)
                int right = Position.X + Size.Width / 2;
                int bottom = Position.Y + 2;

                return new Rectangle(left, top, right - left, bottom - top);
            }


            public override void Select()
            {
                base.Select();
                
            }

            public override void Deselect()
            {
                base.Deselect();
                FillColor = Color.Black;
               
            }
        
        }

        public class CCircle : CShape
        {
            private int Radius
            {
                get { return Size.Width / 2; }
                set { Size = new Size(value * 2, value * 2); }
            }
            
            public CCircle(int x, int y, int radius)
            {
                Position = new Point(x, y);
                Name = "Круг";
                Radius = radius;
                FillColor = Color.White; // Белая заливка по умолчанию
            }

            public override void Draw(Graphics g)
            {
                // Черный контур (как в исходном коде)
                Pen pen = new Pen(Color.Black);
                Brush brush = new SolidBrush(FillColor);

                int diameter = Size.Width;
                g.FillEllipse(brush, Position.X - Radius, Position.Y - Radius, diameter, diameter);
                g.DrawEllipse(pen, Position.X - Radius, Position.Y - Radius, diameter, diameter);

                pen.Dispose();
                brush.Dispose();

                
            }

            public override bool Contains(Point point)
            {
                int dx = point.X - Position.X;
                int dy = point.Y - Position.Y;
                return dx * dx + dy * dy <= Radius * Radius;
            }

            public override Rectangle GetBoundingBox()
            {
                return new Rectangle(Position.X - Radius, Position.Y - Radius, Radius * 2, Radius * 2);
            }

            public void SetRadius(int newRadius)
            {
                Radius = newRadius; // Используем приватный сеттер
            }

            public override void Select()
            {
                base.Select();
                
            }

            public override void Deselect()
            {
                base.Deselect();
               
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
                    if (shapes[i].IsSelected())
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
                    if (shapes[i].IsSelected())
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

            public void SelectShapesInRectangle(Rectangle rect)
            {
                foreach (CShape shape in shapes)
                {
                    if (shape.GetBoundingBox().IntersectsWith(rect))
                    {
                        shape.Select();  // Выделяем фигуру
                    }
                }
            }
        }
       
        private void Form1_MouseClick(object sender, MouseEventArgs e)
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

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Прямоугольник")
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);
                CRectangle newRectangle = new CRectangle(clickPoint.X, clickPoint.Y);

                // Добавляем круг в контейнер
                shapes.Add(newRectangle);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый прямоугольник в ({clickPoint.X}, {clickPoint.Y})");
            }

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Треугольник")
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);


                CTriangle newTriangle = new CTriangle(clickPoint.X, clickPoint.Y);

                // Добавляем круг в контейнер
                shapes.Add(newTriangle);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый треугольник в ({clickPoint.X}, {clickPoint.Y})");
            }

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Эллипс")
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);


                CEllipse newEllipse = new CEllipse(clickPoint.X, clickPoint.Y);

                // Добавляем круг в контейнер
                shapes.Add(newEllipse);

                // Перерисовываем форму
                

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый эллипс в ({clickPoint.X}, {clickPoint.Y})");
            }

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Линия")
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);


                CLine newLine = new CLine(clickPoint.X, clickPoint.Y);

                // Добавляем круг в контейнер
                shapes.Add(newLine);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создана новая линия в ({clickPoint.X}, {clickPoint.Y})");
            }




            

            for (shapes.First(); !shapes.EOL(); shapes.Next())
            {
                CShape shape = shapes.GetCurrent();

                if (shape.Contains(e.Location))
                {
                    clickedOnShape = true;

                    // Если активен инструмент выбора цвета
                    if (isColorMode)
                    {
                        // Если фигура уже окрашена в выбранный цвет, сбрасываем в белый
                        if (shape.FillColor == selectedColor)
                        {
                            if (shape is CLine)
                            {
                                shape.FillColor = Color.Black;
                            }
                            else
                                shape.FillColor = Color.White;
                        }
                        else
                        {
                            shape.FillColor = selectedColor;
                        }
                    }
                    else
                    {
                        // Обычное выделение фигуры
                        if (shape.IsSelected())
                        {
                            shape.Deselect();
                        }
                        else
                        {
                            shape.Select();
                        }
                    }
                    break;
                }
            }

            if (!clickedOnShape)
            {
                if (shapes.GetSelectedCount() > 0)
                {
                    // Снимаем выделение со всех фигур
                    shapes.DeselectAll();
                }
                else
                {
                    // Добавляем новую фигуру
                    CShape newShape = null;
                    switch (selectedShapeType)
                    {
                        case "Круг":
                            newShape = new CCircle(e.X, e.Y, 30);
                            break;
                        case "Прямоугольник":
                            newShape = new CRectangle(e.X, e.Y);
                            break;
                        case "Треугольник":
                            newShape = new CTriangle(e.X, e.Y);
                            break;
                        case "Эллипс":
                            newShape = new CEllipse(e.X, e.Y);
                            break;
                        case "Линия":
                            newShape = new CLine(e.X, e.Y);
                            break;
                    }

                    if (newShape != null)
                    {
                        shapes.Add(newShape);
                    }
                }
            }
            


            this.Invalidate(); // Перерисовываем форму
        }
        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (shapes.First(); !shapes.EOL(); shapes.Next())
            {
                CShape currentShape = shapes.GetCurrent();
                currentShape.Draw(g);

                
            }

            if (RectangleSelection)
            {
                using (var pen = new Pen(Color.DarkBlue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }

            // Рисуем все фигуры
            for (shapes.First(); !shapes.EOL(); shapes.Next())
            {
                CShape shape = shapes.GetCurrent();
                shape.Draw(e.Graphics);

                // Рисуем рамку выделения, если фигура выбрана
                if (shape.IsSelected())
                {
                    shape.DrawSelection(e.Graphics);
                }
            }

        }

        private void ColorTool_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedColor = colorDialog.Color; // Сохраняем выбранный цвет
                    isColorMode = true; // Активируем режим изменения цвета
                }
            }
        }


        private void Cursortool_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            
        }

        

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ctrlPressed)
            {
                selectionStartPoint = e.Location;
                RectangleSelection = true;
                mouseMoved = false;
                selectionRectangle = new Rectangle(selectionStartPoint.X, selectionStartPoint.Y, 0, 0);
                this.Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (RectangleSelection && ctrlPressed)
            {
                mouseMoved = true;
                int x = Math.Min(selectionStartPoint.X, e.X);
                int y = Math.Min(selectionStartPoint.Y, e.Y);
                int width = Math.Abs(e.X - selectionStartPoint.X);
                int height = Math.Abs(e.Y - selectionStartPoint.Y);

                selectionRectangle = new Rectangle(x, y, width, height);
                this.Invalidate();
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                RectangleSelection = false;

                if (mouseMoved)
                {
                    shapes.SelectShapesInRectangle(selectionRectangle); // Выделяем фигуры в прямоугольнике
                }

                mouseMoved = false;
                this.Invalidate();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                shapes.RemoveSelected(); // Удаляем выделенные фигуры
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

                if (RectangleSelection)
                {
                    RectangleSelection = false;
                    selectionRectangle = Rectangle.Empty;
                    this.Invalidate();
                }
            }
        }

    }
}