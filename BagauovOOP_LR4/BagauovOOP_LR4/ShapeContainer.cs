using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static BagauovOOP_LR4.Form1;


namespace BagauovOOP_LR4
{
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

        // Метод выделяет фигуры, чьи границы пересекаются с прямоугольником
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
        // Метод обновляет позиции всех фигур в соответствии с новым размером холста
        public void UpdateAllPositions(Size newCanvasSize)
        {
            foreach (CShape shape in shapes)
            {
                shape.UpdatePosition(newCanvasSize);
            }
        }

        public void TryCreateShape(string selectedShapeType, Point location, Color color, Color lineColor, bool isLineColorChanged)
        {
            CShape newShape = null;

            switch (selectedShapeType)
            {
                case "Круг":
                    newShape = new CCircle(location.X, location.Y, 30) { FillColor = color };
                    break;
                case "Прямоугольник":
                    newShape = new CRectangle(location.X, location.Y) { FillColor = color };
                    break;
                case "Треугольник":
                    newShape = new CTriangle(location.X, location.Y) { FillColor = color };
                    break;
                case "Эллипс":
                    newShape = new CEllipse(location.X, location.Y) { FillColor = color };
                    break;
                case "Линия":
                    var line = new CLine(location.X, location.Y);
                    if (isLineColorChanged)
                        line.FillColor = lineColor;
                    newShape = line;
                    break;
            }

            if (newShape != null)
                Add(newShape);
        }



        public bool HandleClick(Point clickPoint, string selectedShapeType, bool isColorMode, Color selectedColor, ref bool isLineColorChanged, ref Color currentLineColor)
        {
            for (First(); !EOL(); Next())
            {
                var shape = GetCurrent();

                if (shape.Contains(clickPoint))
                {
                    if (isColorMode)
                    {
                        if (shape is CLine)
                        {
                            shape.FillColor = selectedColor;
                            currentLineColor = selectedColor;
                            isLineColorChanged = true;
                        }
                        else
                        {
                            shape.FillColor = shape.FillColor == selectedColor ? Color.White : selectedColor;
                        }
                    }
                    else if (selectedShapeType == "Курсор")
                    {
                        if (!shape.IsSelected())
                        {
                            DeselectAll();
                            shape.Select();
                        }
                        else
                        {
                            shape.Deselect();
                        }
                    }

                    return true; // фигура была кликнута
                }
            }

            return false; // клик не по фигуре
        }

        public void SelectInRectangle(Rectangle selectionArea, bool isColorMode, Color selectedColor)
        {
            for (First(); !EOL(); Next())
            {
                var shape = GetCurrent();
                if (selectionArea.IntersectsWith(shape.GetBoundingBox()))
                {
                    if (isColorMode)
                        shape.FillColor = selectedColor;
                    else
                        shape.Select();
                }
                else
                {
                    shape.Deselect();
                }
            }
        }

        public bool HandleMouseDown(
            MouseEventArgs e,
            bool ctrlPressed,
            bool isColorMode,
            string selectedShapeType,
            out bool isResizing,
            out int resizeHandle,
            out Point resizeStartPoint,
            out bool isChangingPosition,
            out Point changePositionStartPoint)
        {
            isResizing = false;
            isChangingPosition = false;
            resizeHandle = -1;
            resizeStartPoint = Point.Empty;
            changePositionStartPoint = Point.Empty;

            if (e.Button == MouseButtons.Left && !ctrlPressed && !isColorMode)
            {
                // Сохраняем исходные параметры всех выбранных фигур
                for (First(); !EOL(); Next())
                {
                    var shape = GetCurrent();
                    if (shape.IsSelected())
                    {
                        if (shape is CLine line)
                            line.SaveOriginalPoints();
                        else
                            shape.SaveOriginalSizeAndPosition();
                    }
                }

                // Проверяем, тянем ли мы за ручку изменения размера
                for (First(); !EOL(); Next())
                {
                    var shape = GetCurrent();
                    if (shape.IsSelected())
                    {
                        int handle = shape.GetResizeHandle(e.Location);
                        if (handle != -1)
                        {
                            isResizing = true;
                            resizeHandle = handle;
                            resizeStartPoint = e.Location;
                            return true;
                        }
                    }
                }

                // Проверяем клик по выбранной фигуре для перемещения
                for (First(); !EOL(); Next())
                {
                    var shape = GetCurrent();
                    if (shape.IsSelected() && shape.Contains(e.Location))
                    {
                        isChangingPosition = true;
                        changePositionStartPoint = e.Location;
                        return true;
                    }
                }
            }

            return false;


        }

        public void DrawAll(Graphics g, bool rectangleSelection, Rectangle selectionRectangle)
        {
            // Рисуем все фигуры
            for (First(); !EOL(); Next())
            {
                CShape shape = GetCurrent();
                shape.Draw(g); // Рисуем фигуру
                shape.DrawSelection(g); //Рисуем рамку
                
            }

            // Рисуем прямоугольник выделения, если активен режим выделения
            if (rectangleSelection)
            {
                using (var pen = new Pen(Color.DarkBlue, 1) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash })
                {
                    g.DrawRectangle(pen, selectionRectangle);
                }
            }
        }

        public void HandleMouseMove(MouseEventArgs e, ref bool isResizing, ref int resizeHandle, Point resizeStartPoint,
                        ref bool isChangingPosition, ref bool wasChangingPosition, ref Point changePositionStartPoint,
                        ref bool rectangleSelection, bool ctrlPressed, Point selectionStartPoint,
                        ref Rectangle selectionRectangle, Size canvasSize)
        {
            //Если происходит изменение размера, то для каждой выбранной фигуры рассчитываются новые размеры и положение в зависимости от того, за какую ручку изменения размера мы тянем
            if (isResizing)
            {
                int dx = e.X - resizeStartPoint.X;
                int dy = e.Y - resizeStartPoint.Y;

                for (First(); !EOL(); Next())
                {
                    var shape = GetCurrent();
                    if (!shape.IsSelected()) 
                        continue;

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

                        double length = Math.Sqrt(Math.Pow(newEnd.X - newStart.X, 2) + Math.Pow(newEnd.Y - newStart.Y, 2));
                        if (length < 10)
                        {
                            if (resizeHandle == 0)
                            {
                                double angle = Math.Atan2(originalEnd.Y - newStart.Y, originalEnd.X - newStart.X);
                                newStart = new Point(
                                    (int)(originalEnd.X - 10 * Math.Cos(angle)),
                                    (int)(originalEnd.Y - 10 * Math.Sin(angle)));
                            }
                            else
                            {
                                double angle = Math.Atan2(newEnd.Y - originalStart.Y, newEnd.X - originalStart.X);
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
                            case 0:
                                newSize.Width = Math.Max(10, originalSize.Width - dx);
                                newSize.Height = Math.Max(10, originalSize.Height - dy);
                                newPosition = new Point(originalPosition.X + dx / 2, originalPosition.Y + dy / 2);
                                break;
                            case 1:
                                newSize.Width = Math.Max(10, originalSize.Width + dx);
                                newSize.Height = Math.Max(10, originalSize.Height - dy);
                                newPosition = new Point(originalPosition.X, originalPosition.Y + dy / 2);
                                break;
                            case 2:
                                newSize.Width = Math.Max(10, originalSize.Width + dx);
                                newSize.Height = Math.Max(10, originalSize.Height + dy);
                                break;
                            case 3:
                                newSize.Width = Math.Max(10, originalSize.Width - dx);
                                newSize.Height = Math.Max(10, originalSize.Height + dy);
                                newPosition = new Point(originalPosition.X + dx / 2, originalPosition.Y);
                                break;
                        }

                        shape.Resize(newSize, canvasSize);
                        shape.Position = newPosition;
                    }
                }

                return;
            }

            if (isChangingPosition)
            {
                wasChangingPosition = true;
                int dx = e.X - changePositionStartPoint.X;
                int dy = e.Y - changePositionStartPoint.Y;

                for (First(); !EOL(); Next())
                {
                    var shape = GetCurrent();
                    if (shape.IsSelected())
                    {
                        shape.Move(dx, dy, canvasSize);
                    }
                }

                changePositionStartPoint = e.Location;
                return;
            }

            if (rectangleSelection && ctrlPressed)
            {
                int x = Math.Min(selectionStartPoint.X, e.X);
                int y = Math.Min(selectionStartPoint.Y, e.Y);
                int width = Math.Abs(e.X - selectionStartPoint.X);
                int height = Math.Abs(e.Y - selectionStartPoint.Y);

                selectionRectangle = new Rectangle(x, y, width, height);
            }
        }

        public void HandleMouseUp(MouseEventArgs e,
                      ref bool isResizing,
                      ref int resizeHandle,
                      ref bool isChangingPosition,
                      ref bool wasChangingPosition,
                      ref bool rectangleSelection,
                      bool ctrlPressed,
                      bool isColorMode,
                      Rectangle selectionRectangle,
                      Color selectedColor)
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

                if (rectangleSelection && ctrlPressed)
                {
                    Rectangle selectionArea = selectionRectangle;

                    for (First(); !EOL(); Next())
                    {
                        var shape = GetCurrent();

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

                    rectangleSelection = false;
                }
            }
        }

        public bool ChangeShapeColor(Point clickPoint, Color selectedColor)
        {
            for (First(); !EOL(); Next())
            {
                var shape = GetCurrent();

                if (shape.Contains(clickPoint))
                {
                    // Изменяем цвет фигуры на выбранный
                    shape.FillColor = selectedColor;
                    return true;  // Цвет был изменен
                }
            }

            return false;  // Клик не попал по фигуре
        }


    }
}
