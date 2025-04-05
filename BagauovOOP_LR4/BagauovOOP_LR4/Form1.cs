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
        private Color selectedColor = Color.White; // Цвет по умолчанию
        private Color currentLineColor = Color.Black;
        private bool isLineColorChanged = false;
        private bool isColorMode = false;
        private bool RectangleSelection = false;  // Флаг для отслеживания процесса выделения
        private Point selectionStartPoint; // Начальная точка выделения
        private Rectangle selectionRectangle; // Прямоугольник выделения
        private bool ctrlPressed = false;
        private Color currentColorBeforeCursor;
        private bool isChangingPosition = false;
        private Point changePositionStartPoint;
        private bool wasChangingPosition = false;
        private bool isResizing = false;
        private Point resizeStartPoint;
        private Size originalSize;
        private int resizeHandle = -1;


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
        void Resize(Size newSize);
        void Move(int dx, int dy);
        int GetResizeHandle(Point point);
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
           //FillColor = Color.White;
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
       public virtual void Move(int dx, int dy)
       {
           Position = new Point(Position.X + dx, Position.Y + dy);
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

                        // Рисуем угловые маркеры
                        int handleSize = 8;
                        Brush brush = Brushes.White;

                        // Левый верхний
                        g.FillRectangle(brush, bounds.Left - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize);
                        g.DrawRectangle(Pens.Black, bounds.Left - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize);

                        // Правый верхний
                        g.FillRectangle(brush, bounds.Right - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize);
                        g.DrawRectangle(Pens.Black, bounds.Right - handleSize / 2, bounds.Top - handleSize / 2, handleSize, handleSize);

                        // Правый нижний
                        g.FillRectangle(brush, bounds.Right - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize);
                        g.DrawRectangle(Pens.Black, bounds.Right - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize);

                        // Левый нижний
                        g.FillRectangle(brush, bounds.Left - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize);
                        g.DrawRectangle(Pens.Black, bounds.Left - handleSize / 2, bounds.Bottom - handleSize / 2, handleSize, handleSize);
                    }
                }
            }

       public virtual int GetResizeHandle(Point point)
            {
                if (!isSelected) return -1;

                var bounds = GetBoundingBox();
                bounds.Inflate(5, 5); // То же самое, что и при отрисовке рамки

                int handleSize = 8; // Размер углового маркера
                Rectangle[] handles = new Rectangle[4]
                {
        // Левый верхний, правый верхний, правый нижний, левый нижний
                    new Rectangle(bounds.Left - handleSize/2, bounds.Top - handleSize/2, handleSize, handleSize),
                    new Rectangle(bounds.Right - handleSize/2, bounds.Top - handleSize/2, handleSize, handleSize),
                    new Rectangle(bounds.Right - handleSize/2, bounds.Bottom - handleSize/2, handleSize, handleSize),
                    new Rectangle(bounds.Left - handleSize/2, bounds.Bottom - handleSize/2, handleSize, handleSize)
                };

                for (int i = 0; i < handles.Length; i++)
                {
                    if (handles[i].Contains(point))
                        return i;
                }

                return -1;
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
            public override void Move(int dx, int dy)
            {
                base.Move(dx, dy);         // смещаем Position
                CalculatePoints();         // пересчитываем точки треугольника
            }

            public override void Resize(Size newSize)
            {
                base.Resize(newSize);
                CalculatePoints(); // Пересчитываем точки треугольника при изменении размера
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


        }

        public class CLine : CShape
        {
        public CLine(int x, int y)
            {
                Position = new Point(x, y);
                Name = "Линия";
                Size = new Size(100, 0);
                FillColor = Color.Black; // Черная заливка по умолчанию
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

            public override int GetResizeHandle(Point point)
            {
                if (!isSelected) return -1;

                var bounds = GetBoundingBox();
                int handleSize = 8;

                // Только левая и правая точки
                Rectangle[] handles = new Rectangle[2]
                {
            new Rectangle(bounds.Left - handleSize/2, bounds.Top + bounds.Height/2 - handleSize/2, handleSize, handleSize),
            new Rectangle(bounds.Right - handleSize/2, bounds.Top + bounds.Height/2 - handleSize/2, handleSize, handleSize)
                };

                for (int i = 0; i < handles.Length; i++)
                {
                    if (handles[i].Contains(point))
                        return i;
                }

                return -1;
            }

            public override void DrawSelection(Graphics g)
            {
                if (isSelected)
                {
                    using (var pen = new Pen(Color.DarkBlue, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                    {
                        var bounds = GetBoundingBox();
                        bounds.Inflate(5, 5);
                        g.DrawRectangle(pen, bounds);

                        // Рисуем только левый и правый маркеры
                        int handleSize = 8;
                        Brush brush = Brushes.White;

                        // Левый маркер
                        g.FillRectangle(brush, bounds.Left - handleSize / 2, bounds.Top + bounds.Height / 2 - handleSize / 2, handleSize, handleSize);
                        g.DrawRectangle(Pens.Black, bounds.Left - handleSize / 2, bounds.Top + bounds.Height / 2 - handleSize / 2, handleSize, handleSize);

                        // Правый маркер
                        g.FillRectangle(brush, bounds.Right - handleSize / 2, bounds.Top + bounds.Height / 2 - handleSize / 2, handleSize, handleSize);
                        g.DrawRectangle(Pens.Black, bounds.Right - handleSize / 2, bounds.Top + bounds.Height / 2 - handleSize / 2, handleSize, handleSize);
                    }
                }
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
            if (wasChangingPosition)
            {
                wasChangingPosition = false;
                return; // не создаем новую фигуру
            }
            // Проверяем, что клик был левой кнопкой мыши и выбран инструмент "Круг"
            if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Круг" && !RectangleSelection)
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);

                // Создаем новый круг с радиусом 30 пикселей
                CCircle newCircle = new CCircle(clickPoint.X, clickPoint.Y, 30);
                newCircle.FillColor = currentColorBeforeCursor;

                // Добавляем круг в контейнер
                shapes.Add(newCircle);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый круг в ({clickPoint.X}, {clickPoint.Y})");
            }

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Прямоугольник" && !RectangleSelection)
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);
                CRectangle newRectangle = new CRectangle(clickPoint.X, clickPoint.Y);
                newRectangle.FillColor = currentColorBeforeCursor;
                // Добавляем прямоугольник в контейнер
                shapes.Add(newRectangle);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый прямоугольник в ({clickPoint.X}, {clickPoint.Y})");
            }

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Треугольник" && !RectangleSelection)
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);

                CTriangle newTriangle = new CTriangle(clickPoint.X, clickPoint.Y);
                newTriangle.FillColor = currentColorBeforeCursor;
                // Добавляем треугольник в контейнер
                shapes.Add(newTriangle);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый треугольник в ({clickPoint.X}, {clickPoint.Y})");
            }

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Эллипс" && !RectangleSelection)
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);

                CEllipse newEllipse = new CEllipse(clickPoint.X, clickPoint.Y);
                newEllipse.FillColor = currentColorBeforeCursor;
                // Добавляем эллипс в контейнер
                shapes.Add(newEllipse);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создан новый эллипс в ({clickPoint.X}, {clickPoint.Y})");
            }

            else if (((MouseEventArgs)e).Button == MouseButtons.Left && selectedShapeType == "Линия" && !RectangleSelection)
            {
                // Получаем координаты клика
                Point clickPoint = this.PointToClient(Cursor.Position);

                CLine newLine = new CLine(clickPoint.X, clickPoint.Y);
                if (isLineColorChanged)
                {
                    newLine.FillColor = currentLineColor;
                }
                // Добавляем линию в контейнер
                shapes.Add(newLine);

                // Перерисовываем форму
                this.Invalidate();

                // Выводим информацию в консоль (для отладки)
                Console.WriteLine($"Создана новая линия в ({clickPoint.X}, {clickPoint.Y})");
            }

            Point clickPoint_Shape = e.Location;
            bool shapeClicked = false;

            // Проверяем, была ли кликнута фигура
            for (shapes.First(); !shapes.EOL(); shapes.Next())
            {
                CShape shape = shapes.GetCurrent();

                if (shape.Contains(clickPoint_Shape))
                {
                    shapeClicked = true;

                    // В режиме изменения цвета
                    if (isColorMode)
                    {
                        if (shape is CLine)
                        {
                            // Всегда применяем выбранный цвет к линии
                            shape.FillColor = selectedColor;
                            currentLineColor = selectedColor;
                            isLineColorChanged = true;
                        }
                        else
                        {
                            // Логика для других фигур
                            shape.FillColor = shape.FillColor == selectedColor ? Color.White : selectedColor;
                        }
                        this.Invalidate();
                    }
                    else
                    {
                        // Режим курсора: выделение/снятие выделения рамкой
                        if (!shape.IsSelected())
                        {
                            shapes.DeselectAll();  // Снимаем выделение с других фигур
                            shape.Select();  // Выделяем текущую фигуру
                        }
                        else
                        {
                            shape.Deselect();  // Снимаем выделение с фигуры
                        }
                    }

                    break;
                }
            }

            // Если не попали по фигуре, снимаем выделение со всех фигур (клик по пустому месте)
            if (!shapeClicked)
            {
                // Если не в режиме прямоугольного выделения и не в режиме изменения цвета, снимаем выделение со всех фигур
                if (!RectangleSelection && !isColorMode)
                {
                    shapes.DeselectAll();  // Снимаем выделение с всех фигур
                }
            }

            // Перерисовываем форму
            this.Invalidate();

            
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Рисуем все фигуры
            for (shapes.First(); !shapes.EOL(); shapes.Next())
            {
                CShape shape = shapes.GetCurrent();
                shape.Draw(g); // Рисуем фигуру

                // Рисуем рамку выделения, если фигура выбрана
                if (shape.IsSelected())
                {
                    shape.DrawSelection(g);
                }
            }

            // Рисуем прямоугольник выделения, если активен режим выделения
            if (RectangleSelection)
            {
                using (var pen = new Pen(Color.DarkBlue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    e.Graphics.DrawRectangle(pen, selectionRectangle);
                }
            }
        }


        private void Cursortool_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            isColorMode = false;
            selectedShapeType = "";
            currentColorBeforeCursor = selectedColor;
            // Не сбрасываем цвет, так как это не должно повлиять на дальнейшие действия
        }

        private void ColorTool_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    currentLineColor = selectedColor; // Всегда обновляем текущий цвет линии
                    isLineColorChanged = true;
                    isColorMode = true;
                    this.Invalidate();
                }
            }
        }



        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !ctrlPressed && !isColorMode)
            {
                // Проверяем, нажал ли пользователь на угол рамки выделенной фигуры
                for (shapes.First(); !shapes.EOL(); shapes.Next())
                {
                    var shape = shapes.GetCurrent();
                    if (shape.IsSelected())
                    {
                        int handle = shape.GetResizeHandle(e.Location);
                        if (handle != -1)
                        {
                            isResizing = true;
                            resizeHandle = handle;
                            resizeStartPoint = e.Location;
                            originalSize = shape.Size;
                            return;
                        }
                    }
                }

                // Проверяем, нажал ли пользователь на выделенную фигуру (для перемещения)
                for (shapes.First(); !shapes.EOL(); shapes.Next())
                {
                    var shape = shapes.GetCurrent();
                    if (shape.IsSelected() && shape.Contains(e.Location))
                    {
                        isChangingPosition = true;
                        changePositionStartPoint = e.Location;
                        break;
                    }
                }
            }

            // Остальной код остаётся без изменений
            if (e.Button == MouseButtons.Left && ctrlPressed)
            {
                selectionStartPoint = e.Location;
                RectangleSelection = true;
                selectionRectangle = new Rectangle(selectionStartPoint.X, selectionStartPoint.Y, 0, 0);
                this.Invalidate();
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                int dx = e.X - resizeStartPoint.X;
                int dy = e.Y - resizeStartPoint.Y;

                for (shapes.First(); !shapes.EOL(); shapes.Next())
                {
                    var shape = shapes.GetCurrent();
                    if (shape.IsSelected())
                    {
                        if (shape is CLine line)
                        {
                            // Обработка линии (только изменение длины)
                            int newWidth = line.Size.Width;
                            if (resizeHandle == 0) // Левая точка
                            {
                                newWidth = originalSize.Width - dx;
                                line.Position = new Point(line.Position.X + dx / 2, line.Position.Y);
                            }
                            else if (resizeHandle == 1) // Правая точка
                            {
                                newWidth = originalSize.Width + dx;
                                line.Position = new Point(line.Position.X + dx / 2, line.Position.Y);
                            }
                            line.Size = new Size(Math.Max(10, newWidth), line.Size.Height);
                        }
                        else
                        {
                            // Обработка всех остальных фигур
                            Size newSize = originalSize;

                            switch (resizeHandle)
                            {
                                case 0: // Левый верхний
                                    newSize.Width = Math.Max(10, originalSize.Width - dx);
                                    newSize.Height = Math.Max(10, originalSize.Height - dy);
                                    break;
                                case 1: // Правый верхний
                                    newSize.Width = Math.Max(10, originalSize.Width + dx);
                                    newSize.Height = Math.Max(10, originalSize.Height - dy);
                                    break;
                                case 2: // Правый нижний
                                    newSize.Width = Math.Max(10, originalSize.Width + dx);
                                    newSize.Height = Math.Max(10, originalSize.Height + dy);
                                    break;
                                case 3: // Левый нижний
                                    newSize.Width = Math.Max(10, originalSize.Width - dx);
                                    newSize.Height = Math.Max(10, originalSize.Height + dy);
                                    break;
                            }

                            shape.Resize(newSize);
                        }
                    }
                }

                this.Invalidate();
                return;
            }

            // Остальной код остаётся без изменений
            if (isChangingPosition)
            {
                wasChangingPosition = true;
                int dx = e.X - changePositionStartPoint.X;
                int dy = e.Y - changePositionStartPoint.Y;

                for (shapes.First(); !shapes.EOL(); shapes.Next())
                {
                    var shape = shapes.GetCurrent();
                    if (shape.IsSelected())
                    {
                        shape.Move(dx, dy);
                    }
                }

                changePositionStartPoint = e.Location;
                this.Invalidate();
            }

            if (RectangleSelection && ctrlPressed)
            {
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
                if (isResizing)
                {
                    isResizing = false;
                    resizeHandle = -1;
                    return;
                }

                isChangingPosition = false;

                if (wasChangingPosition)
                {
                    wasChangingPosition = false;
                    return;
                }

                // Остальной код остаётся без изменений
                if (RectangleSelection && ctrlPressed)
                {
                    Rectangle selectionArea = selectionRectangle;

                    for (shapes.First(); !shapes.EOL(); shapes.Next())
                    {
                        var shape = shapes.GetCurrent();

                        if (selectionArea.IntersectsWith(shape.GetBoundingBox()))
                        {
                            if (isColorMode)
                            {
                                shape.FillColor = selectedColor;
                            }
                            else
                            {
                                shape.Select();
                            }
                        }
                        else
                        {
                            shape.Deselect();
                        }
                    }

                    RectangleSelection = false;
                    this.Invalidate();
                }
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