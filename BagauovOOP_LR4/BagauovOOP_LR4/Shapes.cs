using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BagauovOOP_LR4
{
    public interface IShape
    {
        // Методы, которые должны быть реализованы в классе
        void Draw(Graphics g); // метод для рисования фигуры
        bool Contains(Point point); // проверяет, содержится ли точка внутри фигуры
        Rectangle GetBoundingBox(); // возвращает прямоугольную рамку, которая ограничивает фигуру

        void Select(); // метод для выделения фигуры рамкой
        void Deselect(); // методы снятия выделения с фигуры
        void Resize(Size newSize, Size canvasSize); // изменение размера фигуры, и проверка, чтобы она не выходила за пределы формы
        void Move(int dx, int dy, Size canvasSize); // перемещение фигуры по осям, проверка, чтобы она не выходила за границы
        int GetResizeHandle(Point point); // возвращает индекс углового маркера для изменения размера
        void SaveOriginalSizeAndPosition(); // сохраняет начальные размеры и положение
        Size GetOriginalSize(); // возвращает исходные размеры фигуры
        Point GetOriginalPosition();  // возвращает исходное положение фигуры
        bool IsWithinBounds(Rectangle bounds, Size canvasSize); // // проверка, укладывается ли фигура в заданные границы
        void UpdatePosition(Size newCanvasSize); // обновление позиции фигуры с учетом нового размера формы
    }



    public abstract class CShape : IShape
    {
        public Point Position { get; set; }
        public Size Size { get; set; }
        public Color FillColor { get; set; } = Color.White;
        public string Name { get; set; } = "Фигура";

        public bool isSelected = false;


        public Size _originalSize;
        public Point _originalPosition;

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
            newBoundingBox.Offset(dx, dy); // сдвигает прямоугольник на заданное количество пикселей

            // проверка, чтобы убедиться, что фигура не выйдет за пределы области формы
            if (IsWithinBounds(newBoundingBox, canvasSize))
            {
                Position = new Point(Position.X + dx, Position.Y + dy);
            }
        }

        // Метод проверки на выход за границы
        public bool IsWithinBounds(Rectangle bounds, Size canvasSize)
        {
            return
                bounds.Left >= 0 && // Левая граница прямоугольника не выходит за левый край канваса
                bounds.Right <= canvasSize.Width && // Правая граница прямоугольника не выходит за правый край канваса
                bounds.Top >= 0 && // Верхняя граница прямоугольника не выходит за верхний край канваса
                bounds.Bottom <= canvasSize.Height; // Нижняя граница прямоугольника не выходит за нижний край канваса
        }
        public virtual void Resize(Size newSize, Size canvasSize)
        {
            // Получаем существующую границу
            Rectangle newBoundingBox = GetBoundingBox();

            // Изменяем размер
            newBoundingBox.Size = newSize;

            // проверка, чтобы убедиться, что фигура не выйдет за пределы области формы
            if (IsWithinBounds(newBoundingBox, canvasSize))
            {
                Size = newSize;
            }
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


            g.FillPolygon(brush, _points);
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

            if ((d1 > 0 && d2 > 0 && d3 > 0) || (d1 < 0 && d2 < 0 && d3 < 0))
            {
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
            int left = Math.Min(_startPoint.X, _endPoint.X);
            int top = Math.Min(_startPoint.Y, _endPoint.Y);
            int right = Math.Max(_startPoint.X, _endPoint.X);
            int bottom = Math.Max(_startPoint.Y, _endPoint.Y);

            return new Rectangle(left, top, right - left, bottom - top);
        }

        public override int GetResizeHandle(Point point)
        {
            if (!isSelected) 
                return -1;

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

            if (startHandle.Contains(point)) 
                
                return 0; // Начальная точка
           
            if (endHandle.Contains(point)) 
                
                return 1;   // Конечная точка

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

       

    }
}
