using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static BagauovOOP_LR4.Form1;



namespace BagauovOOP_LR4
{

    
    public partial class Form1 : DoubleBufferedForm
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
        private Point originalLeftPoint; // Для хранения начальной левой точки
        private Point originalRightPoint; // Для хранения начальной правой точки
        private Point originalPosition;
        private CShape resizeTargetShape = null;

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
        void Resize(Size newSize, Size canvasSize);
        void Move(int dx, int dy, Size canvasSize);
        int GetResizeHandle(Point point);
        void SaveOriginalSizeAndPosition();
        Size GetOriginalSize();
        Point GetOriginalPosition();
        bool IsWithinBounds(Rectangle bounds, Size canvasSize);
        void UpdatePosition(Size newCanvasSize);
    }



    public abstract class CShape: IShape
    {
       public Point Position { get; set; }
       public Size Size { get; set; }
       public Color FillColor { get; set; } = Color.White;
       public string Name { get; set; } = "Фигура";
            
       public bool isSelected = false;


       protected Size _originalSize;
       protected Point _originalPosition;

            public virtual void SaveOriginalSizeAndPosition()
            {
                _originalSize = Size;
                _originalPosition = Position;
            }

            public Size GetOriginalSize() => _originalSize;
            public Point GetOriginalPosition() => _originalPosition;


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
            public virtual void Move(int dx, int dy, Size canvasSize)
            {
                // Получаем новые границы фигуры
                Rectangle newBoundingBox = GetBoundingBox();
                newBoundingBox.Offset(dx, dy); // Сдвигаем пределы

                // Проверяем, можно ли переместить фигуру
                if (IsWithinBounds(newBoundingBox, canvasSize))
                {
                    Position = new Point(Position.X + dx, Position.Y + dy);
                }
            }

            // Метод проверки на выход за границы
            public bool IsWithinBounds(Rectangle bounds, Size canvasSize)
            {
                return
                    bounds.Left >= 0 &&
                    bounds.Right <= canvasSize.Width &&
                    bounds.Top >= 0 &&
                    bounds.Bottom <= canvasSize.Height;
            }
            public virtual void Resize(Size newSize, Size canvasSize)
            {
                // Получаем существующую границу
                Rectangle newBoundingBox = GetBoundingBox();

                // Изменяем размер
                newBoundingBox.Size = newSize;

                // Проверяем, чтобы не выйти за границы
                if (IsWithinBounds(newBoundingBox, canvasSize))
                {
                    Size = newSize;
                }
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
            public virtual void UpdatePosition(Size newCanvasSize)
            {
                // Проверка, что фигура выходит за пределы новой области
                Rectangle boundingBox = GetBoundingBox();

                // Определяем, насколько необходимо сдвинуть фигуру
                if (boundingBox.Right > newCanvasSize.Width)
                {
                    Position = new Point(Position.X - (boundingBox.Right - newCanvasSize.Width), Position.Y);
                }
                if (boundingBox.Left < 0)
                {
                    Position = new Point(Position.X - boundingBox.Left, Position.Y);
                }
                if (boundingBox.Bottom > newCanvasSize.Height)
                {
                    Position = new Point(Position.X, Position.Y - (boundingBox.Bottom - newCanvasSize.Height));
                }
                if (boundingBox.Top < 0)
                {
                    Position = new Point(Position.X, Position.Y - boundingBox.Top);
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
            public override void Move(int dx, int dy, Size canvasSize)
            {
                base.Move(dx, dy, canvasSize);         // смещаем Position
                CalculatePoints();         // пересчитываем точки треугольника
            }

            public override void Resize(Size newSize, Size canvasSize)
            {
                base.Resize(newSize, canvasSize);
                CalculatePoints(); // Пересчитываем точки треугольника при изменении размера
            }

            public override void UpdatePosition(Size newCanvasSize)
            {
                base.UpdatePosition(newCanvasSize); // Вызываем родительский метод для обновления позиции
                CalculatePoints(); // Пересчитываем вершины после изменения позиции
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
            private Point _startPoint;
            private Point _endPoint;

            public CLine(int x, int y)
            {
                _startPoint = new Point(x - 50, y); // Начальная точка слева от центра
                _endPoint = new Point(x + 50, y);   // Конечная точка справа от центра
                UpdatePositionAndSize();
                Name = "Линия";
                FillColor = Color.Black;
            }

            public Point StartPoint => _startPoint;
            public Point EndPoint => _endPoint;

            private Point _originalStart;
            private Point _originalEnd;

            public void SaveOriginalPoints()
            {
                _originalStart = _startPoint;
                _originalEnd = _endPoint;
            }

            public Point GetOriginalStart() => _originalStart;
            public Point GetOriginalEnd() => _originalEnd;



            public void SetPoints(Point start, Point end)
            {
                int buffer = 10; // Запас в 10 пикселей

                if (IsWithinBounds(start, Application.OpenForms[0].ClientSize, buffer) &&
                    IsWithinBounds(end, Application.OpenForms[0].ClientSize, buffer))
                {
                    _startPoint = start;
                    _endPoint = end;
                    UpdatePositionAndSize(); // обновите позицию и размер
                }
            }

            private void UpdatePositionAndSize()
            {
                // Центр линии - середина между точками
                Position = new Point(
                    (_startPoint.X + _endPoint.X) / 2,
                    (_startPoint.Y + _endPoint.Y) / 2);

                // Размер - разница между точками
                Size = new Size(
                    Math.Abs(_endPoint.X - _startPoint.X),
                    Math.Abs(_endPoint.Y - _startPoint.Y));
            }

            public override void Draw(Graphics g)
            {
                using (Pen pen = new Pen(FillColor, 3))
                {
                    g.DrawLine(pen, _startPoint, _endPoint);
                }
            }

            public override bool Contains(Point point)
            {
                // Проверка попадания точки на линию с учетом толщины
                return DistanceToLine(point, _startPoint, _endPoint) <= 3;
            }

            private double DistanceToLine(Point point, Point lineStart, Point lineEnd)
            {
                // Вычисление расстояния от точки до линии
                double A = point.X - lineStart.X;
                double B = point.Y - lineStart.Y;
                double C = lineEnd.X - lineStart.X;
                double D = lineEnd.Y - lineStart.Y;

                double dot = A * C + B * D;
                double len_sq = C * C + D * D;
                double param = (len_sq != 0) ? dot / len_sq : -1;

                double xx, yy;

                if (param < 0)
                {
                    xx = lineStart.X;
                    yy = lineStart.Y;
                }
                else if (param > 1)
                {
                    xx = lineEnd.X;
                    yy = lineEnd.Y;
                }
                else
                {
                    xx = lineStart.X + param * C;
                    yy = lineStart.Y + param * D;
                }

                double dx = point.X - xx;
                double dy = point.Y - yy;
                return Math.Sqrt(dx * dx + dy * dy);
            }

            public override Rectangle GetBoundingBox()
            {
                int left = Math.Min(_startPoint.X, _endPoint.X) - 5;
                int top = Math.Min(_startPoint.Y, _endPoint.Y) - 5;
                int right = Math.Max(_startPoint.X, _endPoint.X) + 5;
                int bottom = Math.Max(_startPoint.Y, _endPoint.Y) + 5;

                return new Rectangle(left, top, right - left, bottom - top);
            }

            public override int GetResizeHandle(Point point)
            {
                if (!isSelected) return -1;

                int handleSize = 8;
                Rectangle startHandle = new Rectangle(
                    _startPoint.X - handleSize / 2,
                    _startPoint.Y - handleSize / 2,
                    handleSize,
                    handleSize);

                Rectangle endHandle = new Rectangle(
                    _endPoint.X - handleSize / 2,
                    _endPoint.Y - handleSize / 2,
                    handleSize,
                    handleSize);

                if (startHandle.Contains(point)) return 0; // Начальная точка
                if (endHandle.Contains(point)) return 1;   // Конечная точка

                return -1;
            }

            public override void DrawSelection(Graphics g)
            {
                if (!isSelected) return;

                // Рисуем маркеры на концах линии
                int handleSize = 8;
                Brush brush = Brushes.White;

                // Маркер начальной точки
                g.FillRectangle(brush,
                    _startPoint.X - handleSize / 2,
                    _startPoint.Y - handleSize / 2,
                    handleSize,
                    handleSize);
                g.DrawRectangle(Pens.Black,
                    _startPoint.X - handleSize / 2,
                    _startPoint.Y - handleSize / 2,
                    handleSize,
                    handleSize);

                // Маркер конечной точки
                g.FillRectangle(brush,
                    _endPoint.X - handleSize / 2,
                    _endPoint.Y - handleSize / 2,
                    handleSize,
                    handleSize);
                g.DrawRectangle(Pens.Black,
                    _endPoint.X - handleSize / 2,
                    _endPoint.Y - handleSize / 2,
                    handleSize,
                    handleSize);
            }

            public override void Move(int dx, int dy, Size canvasSize)
            {
                // Получаем новые координаты начальной и конечной точки
                Point newStart = new Point(_startPoint.X + dx, _startPoint.Y + dy);
                Point newEnd = new Point(_endPoint.X + dx, _endPoint.Y + dy);

                int buffer = 10; // Запас в 10 пикселей

                // Проверяем, находятся ли новые точки в пределах области c запасом
                if (IsWithinBounds(newStart, canvasSize, buffer) && IsWithinBounds(newEnd, canvasSize, buffer))
                {
                    // Обновляем только если обе точки в пределах границ
                    _startPoint = newStart;
                    _endPoint = newEnd;
                    UpdatePositionAndSize(); // Обновите размер и позицию, если это необходимо
                }
            }

            // Метод проверки, что точка находится в пределах области
            private bool IsWithinBounds(Point point, Size canvasSize, int buffer)
            {
                return point.X >= +buffer && point.X <= canvasSize.Width - buffer &&
                       point.Y >= +buffer && point.Y <= canvasSize.Height - buffer;
            }

           
                public override void UpdatePosition(Size newCanvasSize)
            {
                // Проверяем, выходит ли начальная точка за границы
                if (_startPoint.X < 0)
                {
                    int offset = -_startPoint.X;
                    _startPoint.X += offset;
                    _endPoint.X += offset;
                }
                else if (_startPoint.X > newCanvasSize.Width)
                {
                    int offset = _startPoint.X - newCanvasSize.Width;
                    _startPoint.X -= offset;
                    _endPoint.X -= offset;
                }

                if (_startPoint.Y < 0)
                {
                    int offset = -_startPoint.Y;
                    _startPoint.Y += offset;
                    _endPoint.Y += offset;
                }
                else if (_startPoint.Y > newCanvasSize.Height)
                {
                    int offset = _startPoint.Y - newCanvasSize.Height;
                    _startPoint.Y -= offset;
                    _endPoint.Y -= offset;
                }

                // Проверяем, выходит ли конечная точка за границы
                if (_endPoint.X < 0)
                {
                    int offset = -_endPoint.X;
                    _startPoint.X += offset;
                    _endPoint.X += offset;
                }
                else if (_endPoint.X > newCanvasSize.Width)
                {
                    int offset = _endPoint.X - newCanvasSize.Width;
                    _startPoint.X -= offset;
                    _endPoint.X -= offset;
                }

                if (_endPoint.Y < 0)
                {
                    int offset = -_endPoint.Y;
                    _startPoint.Y += offset;
                    _endPoint.Y += offset;
                }
                else if (_endPoint.Y > newCanvasSize.Height)
                {
                    int offset = _endPoint.Y - newCanvasSize.Height;
                    _startPoint.Y -= offset;
                    _endPoint.Y -= offset;
                }

                // Обновляем позицию и размер
                UpdatePositionAndSize();
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
            public void UpdateAllPositions(Size newCanvasSize)
            {
                foreach (CShape shape in shapes)
                {
                    shape.UpdatePosition(newCanvasSize);
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
                    else if (selectedShapeType == "Курсор") // Режим курсора
                    {
                        if (!shape.IsSelected())
                        {
                            shapes.DeselectAll();  // Снимаем выделение с других фигур
                            shape.Select();  // Выделяем текущую фигуру (только в режиме курсора)
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
            selectedShapeType = "Курсор";
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
            // Если левая кнопка мыши, не режим цвета и не зажат Ctrl
        if (e.Button == MouseButtons.Left && !ctrlPressed && !isColorMode)
    {
        // Сохраняем исходные параметры всех выбранных фигур
        for (shapes.First(); !shapes.EOL(); shapes.Next())
        {
            var shape = shapes.GetCurrent();
            if (shape.IsSelected())
            {
                if (shape is CLine line)
                    line.SaveOriginalPoints();
                else
                    shape.SaveOriginalSizeAndPosition();
            }
        }

        // Проверяем, тянем ли мы за ручку изменения размера
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
                    return;
                }
            }
        }

        // Если не тянем за ручку — проверим, кликнули ли по выбранной фигуре для перемещения
        for (shapes.First(); !shapes.EOL(); shapes.Next())
        {
            var shape = shapes.GetCurrent();
            if (shape.IsSelected() && shape.Contains(e.Location))
            {
                isChangingPosition = true;
                changePositionStartPoint = e.Location;
                return;
            }
        }
    }

    // Прямоугольное выделение (если зажат Ctrl и активен курсор или режим цвета)
    if (e.Button == MouseButtons.Left && ctrlPressed &&
        (selectedShapeType == "Курсор" || selectedShapeType == "Цвет"))
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
                Size canvasSize = this.ClientSize;

                for (shapes.First(); !shapes.EOL(); shapes.Next())
                {
                    var shape = shapes.GetCurrent();
                    if (!shape.IsSelected()) continue;

                    if (shape is CLine line)
                    {
                        Point originalStart = line.GetOriginalStart();
                        Point originalEnd = line.GetOriginalEnd();

                        Point newStart = originalStart;
                        Point newEnd = originalEnd;

                        if (resizeHandle == 0)
                            newStart = new Point(originalStart.X + dx, originalStart.Y + dy);
                        else if (resizeHandle == 1)
                            newEnd = new Point(originalEnd.X + dx, originalEnd.Y + dy);

                        double length = Math.Sqrt(Math.Pow(newEnd.X - newStart.X, 2) +
                                                  Math.Pow(newEnd.Y - newStart.Y, 2));
                        if (length < 10)
                        {
                            if (resizeHandle == 0)
                            {
                                double angle = Math.Atan2(originalEnd.Y - newStart.Y,
                                                          originalEnd.X - newStart.X);
                                newStart = new Point(
                                    (int)(originalEnd.X - 10 * Math.Cos(angle)),
                                    (int)(originalEnd.Y - 10 * Math.Sin(angle)));
                            }
                            else
                            {
                                double angle = Math.Atan2(newEnd.Y - originalStart.Y,
                                                          newEnd.X - originalStart.X);
                                newEnd = new Point(
                                    (int)(originalStart.X + 10 * Math.Cos(angle)),
                                    (int)(originalStart.Y + 10 * Math.Sin(angle)));
                            }
                        }

                        line.SetPoints(newStart, newEnd);
                    }
                    else
                    {
                        Size originalSize = shape.GetOriginalSize();
                        Point originalPosition = shape.GetOriginalPosition();

                        Size newSize = originalSize;
                        Point newPosition = originalPosition;

                        switch (resizeHandle)
                        {
                            case 0: // Левый верхний
                                newSize.Width = Math.Max(10, originalSize.Width - dx);
                                newSize.Height = Math.Max(10, originalSize.Height - dy);
                                newPosition = new Point(originalPosition.X + dx / 2, originalPosition.Y + dy / 2);
                                break;
                            case 1: // Правый верхний
                                newSize.Width = Math.Max(10, originalSize.Width + dx);
                                newSize.Height = Math.Max(10, originalSize.Height - dy);
                                newPosition = new Point(originalPosition.X, originalPosition.Y + dy / 2);
                                break;
                            case 2: // Правый нижний
                                newSize.Width = Math.Max(10, originalSize.Width + dx);
                                newSize.Height = Math.Max(10, originalSize.Height + dy);
                                break;
                            case 3: // Левый нижний
                                newSize.Width = Math.Max(10, originalSize.Width - dx);
                                newSize.Height = Math.Max(10, originalSize.Height + dy);
                                newPosition = new Point(originalPosition.X + dx / 2, originalPosition.Y);
                                break;
                        }

                        shape.Resize(newSize, canvasSize);
                        shape.Position = newPosition;
                    }
                }

                this.Invalidate();
            }

            // Обработка перемещения фигуры
            if (isChangingPosition)
            {
                wasChangingPosition = true;
                int dx = e.X - changePositionStartPoint.X;
                int dy = e.Y - changePositionStartPoint.Y;

                Size canvasSize = this.ClientSize; // Получаем размер области рисования

                for (shapes.First(); !shapes.EOL(); shapes.Next())
                {
                    var shape = shapes.GetCurrent();
                    if (shape.IsSelected())
                    {
                        shape.Move(dx, dy, canvasSize); // Передаем размер холста
                    }
                }

                changePositionStartPoint = e.Location;
                this.Invalidate();
            }

            // Обработка прямоугольного выделения
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

        

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            shapes.UpdateAllPositions(this.ClientSize);
            this.Invalidate(); // Перерисовываем форму
        }



    }
    public class DoubleBufferedForm : Form
    {
        public DoubleBufferedForm()
        {
            // Включаем двойную буферизацию
            this.DoubleBuffered = true;
           

        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            e.Graphics.Clear(this.BackColor);
        }
    }
}