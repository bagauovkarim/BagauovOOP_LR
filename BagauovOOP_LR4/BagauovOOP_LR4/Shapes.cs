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
        // Методы, которые должны быть реализованы в классе, поддерживающем этот интерфейс
        void Draw(Graphics g); // метод для рисования фигуры
        bool Contains(Point point); // проверяет, содержится ли точка внутри фигуры
        Rectangle GetBoundingBox(); // возвращает прямоугольник, который полностью ограничивает фигуру 

        void Select(); // метод для выделения фигуры рамкой
        void Deselect(); // методы снятия выделения с фигуры
        void Resize(Size newSize, Size canvasSize); // изменение размера фигуры, и проверка, чтобы она не выходила за пределы формы
        void Move(int dx, int dy, Size canvasSize); // перемещение фигуры по осям, проверка, чтобы она не выходила за границы
        int GetResizeHandle(Point point); // возвращает индекс рукоятки для изменения размера
        void SaveOriginalSize(); // сохраняет начальные размеры 
        Size GetSize(); // возвращает исходные размеры фигуры
        bool IsWithinBounds(Rectangle bounds, Size canvasSize); // // проверка, укладывается ли фигура в заданные границы
        void UpdatePosition(Size newCanvasSize); // обновление позиции фигуры с учетом нового размера формы
    }



    public abstract class CShape : IShape
    {
        protected Point Position;
        protected Size Size;
        protected Color FillColor;
        protected string Name = "Фигура";
        protected bool isSelected = false;
        protected Size _originalSize;
    

        public Color GetFillColor() => FillColor;
        
        public Point GetPosition() => Position;
        
        public Size GetSize() => Size;

        public Size GetOriginalSize() => _originalSize;
        public bool IsSelected()
        {
            return isSelected;  // Возвращаем состояние флага выделения
        }
        public void Select()
        {
            isSelected = true;  // Устанавливаем флаг выделения
        }

        // Метод для снятия выделения
        public void Deselect()
        {
            isSelected = false;  // Снимаем выделение

        }
        public void SetFillColor(Color _fillColor)
        {
            FillColor = _fillColor;
        }
        public void SetPosition(Point _position)
        {
            Position = _position;
        }

        public void SaveOriginalSize()
        {
            _originalSize = Size;
            
        }


        // Проверяет, не выходит ли фигура за пределы формы
        public bool IsWithinBounds(Rectangle bounds, Size canvasSize)
        {
            return
                bounds.Left >= 70 && // Левая граница прямоугольника не выходит за левый край панели инструментов
                bounds.Right <= canvasSize.Width && // Правая граница прямоугольника не выходит за правый край формы
                bounds.Top >= 0 && // Верхняя граница прямоугольника не выходит за верхний край формы
                bounds.Bottom <= canvasSize.Height; // Нижняя граница прямоугольника не выходит за нижний край формы
        }


        // Проверяет, пересекается ли фигура с заданным прямоугольником rect
        public bool IntersectsWith(Rectangle rect) 
        {
            return GetBoundingBox().IntersectsWith(rect);
        }

        // Абстрактные методы (должны быть реализованы в наследниках)
        public abstract void Draw(Graphics g);
        public abstract bool Contains(Point point);
        public abstract Rectangle GetBoundingBox();


        // Виртуальные методы (могут быть переопределены в наследниках)

        // Метод Move отвечает за перемещение фигуры по форме
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


        // Метод Resizr отвечает за изменение размера фигуры 
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


        // Метод рисует выделение прямоугольником
        public virtual void DrawSelection(Graphics g)
        {
            if (isSelected)
            {
                using (var pen = new Pen(Color.DarkBlue, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    var bounds = GetBoundingBox();
                    bounds.Inflate(5, 5);
                    g.DrawRectangle(pen, bounds);

                    // Рисуем рукоятки
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


        // Метод определяет, попал ли курсор мыши в одну из рукояток
        public virtual int GetResizeHandle(Point point)
        {
            if (!isSelected) 
                return -1;

            var bounds = GetBoundingBox();
            bounds.Inflate(5, 5); // Увеличиваем объект, чтобы корректно работать с рукоятками

            int handleSize = 8; // Размер рукоятки
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


        // Проверка, что фигура выходит за пределы новой области
        public virtual void UpdatePosition(Size newCanvasSize)
        {
            Rectangle boundingBox = GetBoundingBox();
            int dx = 0;
            int dy = 0;

            if (boundingBox.Right > newCanvasSize.Width)
            {
                dx = -(boundingBox.Right - newCanvasSize.Width);
            }
            if (boundingBox.Left < 70)
            {
                dx = 70 - boundingBox.Left;
            }
            if (boundingBox.Bottom > newCanvasSize.Height)
            {
                dy = -(boundingBox.Bottom - newCanvasSize.Height);
            }
            if (boundingBox.Top < 0)
            {
                dy = -boundingBox.Top;
            }

            Position = new Point(Position.X + dx, Position.Y + dy);
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
            // Черный контур 
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
            return new Rectangle(Position.X - Size.Width / 2, Position.Y - Size.Height / 2, Size.Width + 1, Size.Height + 1);

        }

        public override void Resize(Size newSize, Size canvasSize)
        {
            // Создаём новый bounding box с учётом позиции по центру
            var newBoundingBox = new Rectangle(
                Position.X - newSize.Width / 2 + 1,
                Position.Y - newSize.Height / 2,
                newSize.Width + 1,
                newSize.Height + 1
            );

            // проверка на выход за пределы холста
            if (IsWithinBounds(newBoundingBox, canvasSize))
            {
                Size = newSize;
            }
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

        // Векторное произведение векторов
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

            // Используется Sign, чтобы определить, находится ли точка внутри треугольника
            float d1 = Sign(point, A, B);
            float d2 = Sign(point, B, C);
            float d3 = Sign(point, C, A);

            // Проверяем, все ли знаки положительные или все отрицательные.

            if ((d1 > 0 && d2 > 0 && d3 > 0) || (d1 < 0 && d2 < 0 && d3 < 0))
            {
                return true;
            }
            else
            {
                return false; //Иначе точка не в треугольнике
            }
                


        }



        public override Rectangle GetBoundingBox()
        { 

            return new Rectangle(Position.X - Size.Width / 2, Position.Y - Size.Height / 2, Size.Width + 1, Size.Height + 1);

        }
        public override void Move(int dx, int dy, Size canvasSize)
        {
            base.Move(dx, dy, canvasSize);         
            CalculatePoints(); // Пересчитываем точки треугольника
        }

        public override void Resize(Size newSize, Size canvasSize)
        {
            // Получаем текущие границы треугольника
            var newBoundingBox = new Rectangle(
                Position.X - newSize.Width / 2 + 1,
                Position.Y - newSize.Height / 2,
                newSize.Width + 1,
                newSize.Height + 1
            );

            // Изменяем размер
            newBoundingBox.Size = newSize;

            // Проверка, чтобы убедиться, что фигура не выходит за пределы области формы
            if (IsWithinBounds(newBoundingBox, canvasSize))
            {
                CalculatePoints();
                Size = newSize; // Обновляем размер эллипса
            }
        }



        public override void UpdatePosition(Size newCanvasSize)
        {
            base.UpdatePosition(newCanvasSize); 
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
            double a = Size.Width / 2.0;  // Полуось по X
            double b = Size.Height / 2.0; // Полуось по Y
            double centerX = Position.X; // Центр эллипса по Х
            double centerY = Position.Y; // Центр эллипса по Y 

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
            return new Rectangle(Position.X - width / 2, Position.Y - height / 2, width + 1, height + 1);
        }

        public override void Resize(Size newSize, Size canvasSize)
        {
            // Новый размер эллипса, просто обновляем его ширину и высоту
            Size newEllipseSize = new Size(newSize.Width, newSize.Height);

            // Новый bounding box для эллипса
            Rectangle newBoundingBox = new Rectangle(
                Position.X - newEllipseSize.Width / 2,
                Position.Y - newEllipseSize.Height / 2,
                newEllipseSize.Width,
                newEllipseSize.Height);

            // Проверка, чтобы убедиться, что фигура не выходит за пределы области холста
            if (IsWithinBounds(newBoundingBox, canvasSize))
            {
                Size = newEllipseSize; // Обновляем размер эллипса
            }
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
            Name = "Линия";
            FillColor = Color.Black;
        }

        public Point GetStartPoint => _startPoint;
        public Point GetEndPoint => _endPoint;

        private Point _originalStart;
        private Point _originalEnd;

        public void SaveOriginalPoints()
        {
            _originalStart = _startPoint;
            _originalEnd = _endPoint;
        }

        public Point GetOriginalStart() => _originalStart;
        public Point GetOriginalEnd() => _originalEnd;



        public void SetPoints(Point start, Point end, Size canvasSize)
        {
            int buffer = 10; // Запас в 10 пикселей

            // Проверяем, не выходит ли начальная точка за границу канваса с запасом
            bool isStartWithinBounds = IsWithinBounds(start, canvasSize, buffer);
            // Проверяем, не выходит ли конечная точка за границу канваса с запасом
            bool isEndWithinBounds = IsWithinBounds(end, canvasSize, buffer);

            // Разрешаем изменить точки только если обе точки находятся в пределах границ канваса
            if (isStartWithinBounds && isEndWithinBounds)
            {
                _startPoint = start;
                _endPoint = end;
                UpdatePositionAndSizeFromPoints();
            }
            else
            {
                // Если хотя бы одна точка выходит за пределы, ограничиваем её
                if (!isStartWithinBounds)
                {
                    // Если начальная точка выходит за границу, ограничиваем её в пределах канваса
                    start = LimitPointWithinBounds(start, canvasSize);
                }

                if (!isEndWithinBounds)
                {
                    // Если конечная точка выходит за границу, ограничиваем её в пределах канваса
                    end = LimitPointWithinBounds(end, canvasSize);
                }

                _startPoint = start;
                _endPoint = end;
                UpdatePositionAndSizeFromPoints();
            }
        }

        private Point LimitPointWithinBounds(Point point, Size canvasSize)
        {
            // Ограничиваем точку в пределах канваса с учётом буфера
            int buffer = 10;

            // Ограничиваем по X и Y
            int x = Math.Max(buffer + 70, Math.Min(point.X, canvasSize.Width - buffer));
            int y = Math.Max(buffer, Math.Min(point.Y, canvasSize.Height - buffer));

            return new Point(x, y);
        }



        private void UpdatePositionAndSizeFromPoints()
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


        // Вычисляет расстояние от точки до прямой, заданной двумя точками: начальной (lineStart) и конечной (lineEnd)
        private double DistanceToLine(Point point, Point lineStart, Point lineEnd)
        {
            // Вычисление расстояния от точки до линии
            double A = point.X - lineStart.X; // Разница по оси X между точкой и началом линии
            double B = point.Y - lineStart.Y; // Разница по оси Y между точкой и началом линии
            double C = lineEnd.X - lineStart.X; // Разница по оси X между началом и концом линии
            double D = lineEnd.Y - lineStart.Y; // Разница по оси Y между началом и концом линии

            double dot = A * C + B * D; // Вычисление скалярного произведения
            double len_sq = C * C + D * D; // Вычисление квадратов длины линии
            double param = (len_sq != 0) ? dot / len_sq : -1; // Это параметр, который позволяет найти ближайшую точку на линии относительно точки point

            double xx, yy; // Параметры для вычисления ближайшей точки на линии

            if (param < 0) // Это означает, что проекция точки на линию лежит за пределами начальной точки линии, ближе к начальной точке (lineStart)
            {
                xx = lineStart.X;
                yy = lineStart.Y;
            }
            else if (param > 1) // Это означает, что проекция точки на линию лежит за пределами конечной точки линии, ближе к конечной точке (lineEnd)
            {
                xx = lineEnd.X;
                yy = lineEnd.Y;
            }
            else // Это означает, что проекция точки на линию находится на самой линии, между начальной и конечной точками
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
            int left = Math.Min(GetStartPoint.X, GetEndPoint.X);
            int top = Math.Min(GetStartPoint.Y, GetEndPoint.Y);
            int width = Math.Abs(GetEndPoint.X - GetStartPoint.X);
            int height = Math.Abs(GetEndPoint.Y - GetStartPoint.Y);

            return new Rectangle(left, top, width, height);
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

            // Рисуем рукоятки на концах линии
            int handleSize = 8;
            Brush brush = Brushes.White;

            // Рукоятка начальной точки
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

            // Рукоятка конечной точки
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
            Point newStart = new Point(_startPoint.X + dx, _startPoint.Y + dy);
            Point newEnd = new Point(_endPoint.X + dx, _endPoint.Y + dy);

            int buffer = 10;

            bool wasOutOfBounds =
                !IsWithinBounds(_startPoint, canvasSize, buffer) ||
                !IsWithinBounds(_endPoint, canvasSize, buffer);

            bool newPointsWithinBounds =
                IsWithinBounds(newStart, canvasSize, buffer) &&
                IsWithinBounds(newEnd, canvasSize, buffer);

            // Разрешаем перемещение, если:
            // 1. Новые точки внутри границ (обычный случай)
            // 2. Или фигура была вне границ и сдвигается внутрь
            if (newPointsWithinBounds || wasOutOfBounds)
            {
                _startPoint = newStart;
                _endPoint = newEnd;
                UpdatePositionAndSizeFromPoints();
            }
        }


        // Метод проверки, что точка находится в пределах области
        private bool IsWithinBounds(Point point, Size canvasSize, int buffer)
        {
            return point.X >= +buffer + 68 && point.X <= canvasSize.Width - buffer &&
                   point.Y >= +buffer && point.Y <= canvasSize.Height - buffer;
        }


        public override void UpdatePosition(Size newCanvasSize)
        {
            Rectangle boundingBox = GetBoundingBox();
            int dx = 0, dy = 0;

            if (boundingBox.Right > newCanvasSize.Width)
                dx = -(boundingBox.Right - newCanvasSize.Width);
            if (boundingBox.Left < 70)
                dx = 70 - boundingBox.Left;
            if (boundingBox.Bottom > newCanvasSize.Height)
                dy = -(boundingBox.Bottom - newCanvasSize.Height);
            if (boundingBox.Top < 0)
                dy = -boundingBox.Top;

            // Сдвигаем обе точки линии
            _startPoint = new Point(GetStartPoint.X + dx, GetStartPoint.Y + dy);
            _endPoint = new Point(GetEndPoint.X + dx, GetEndPoint.Y + dy);
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

        public override void Resize(Size newSize, Size canvasSize)
        {
            // Диаметр круга = Width (или Height), т.к. круг должен быть круглым
            int newDiameter = Math.Min(newSize.Width, newSize.Height);
            int newRadius = newDiameter / 2;

            // Новый bounding box, исходя из центра круга
            Rectangle newBoundingBox = new Rectangle(
                Position.X - newRadius,
                Position.Y - newRadius,
                newDiameter,
                newDiameter);

            // Проверка на выход за границы
            if (IsWithinBounds(newBoundingBox, canvasSize))
            {
                Size = new Size(newDiameter, newDiameter); // Обновляем размер
            }
        }


    }
}
