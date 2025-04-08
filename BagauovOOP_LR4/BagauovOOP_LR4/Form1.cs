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
        private bool ctrlPressed = false;
        private Color currentColorBeforeCursor;
        
        private bool RectangleSelection = false;  // Флаг для отслеживания процесса выделения
        private Point selectionStartPoint; // Начальная точка выделения
        private Rectangle selectionRectangle; // Прямоугольник выделения
        private bool isChangingPosition = false;
        private Point changePositionStartPoint;
        private bool wasChangingPosition = false;
        private bool isResizing = false;
        private Point resizeStartPoint;
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






        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (wasChangingPosition)
            {
                wasChangingPosition = false;
                return;
            }

            if (e.Button == MouseButtons.Left && !RectangleSelection)
            {
                Point clickPoint = this.PointToClient(Cursor.Position);

                // Обработка клика по фигуре (цвет/выделение)
                bool shapeClicked = shapes.HandleClick(e.Location, selectedShapeType, isColorMode, selectedColor, ref isLineColorChanged, ref currentLineColor);

                // Если не попали по фигуре
                if (!shapeClicked && !RectangleSelection && !isColorMode)
                {
                    shapes.DeselectAll();
                }

                // Если выбран цвет, и мы в режиме изменения цвета, меняем цвет фигуры
                if (isColorMode)
                {
                    bool changedColor = shapes.ChangeShapeColor(clickPoint, selectedColor);
                    if (changedColor)
                    {
                        this.Invalidate();
                    }
                }
                else
                {
                    // Попытка создать фигуру
                    if (selectedShapeType != "Курсор")
                    {
                        shapes.TryCreateShape(selectedShapeType, clickPoint, selectedColor, currentLineColor, isLineColorChanged);
                    }
                }

                Invalidate();
            }
        }





        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            shapes.DrawAll(e.Graphics, RectangleSelection, selectionRectangle);
        }



        private void Cursortool_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
            isColorMode = false;  
            selectedShapeType = "Курсор";
            currentColorBeforeCursor = selectedColor;  // Сохраняем текущий цвет для курсора
                                                      
        }

        private void ColorTool_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedColor = colorDialog.Color;
                    currentLineColor = selectedColor; // Обновляем текущий цвет линии
                    isLineColorChanged = true;  // Устанавливаем флаг, что цвет линии был изменен

                    // Переключаемся в режим изменения цвета, но остаемся в режиме рисования фигур
                    isColorMode = true;  // Включаем режим изменения цвета
                    this.Invalidate();  // Обновляем форму
                }
            }
        }








        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            bool handled = shapes.HandleMouseDown(
                e,
                ctrlPressed,
                isColorMode,
                selectedShapeType,
                out isResizing,
                out resizeHandle,
                out resizeStartPoint,
                out isChangingPosition,
                out changePositionStartPoint
            );

            if (handled)
                return;

            // Прямоугольное выделение
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
            shapes.HandleMouseMove(
                e,
                ref isResizing,
                ref resizeHandle,
                resizeStartPoint,
                ref isChangingPosition,
                ref wasChangingPosition,
                ref changePositionStartPoint,
                ref RectangleSelection,
                ctrlPressed,
                selectionStartPoint,
                ref selectionRectangle,
                this.ClientSize);

            this.Invalidate();
        }


        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            shapes.HandleMouseUp(
            e,
            ref isResizing,
            ref resizeHandle,
            ref isChangingPosition,
            ref wasChangingPosition,
            ref RectangleSelection,
            ctrlPressed,
            isColorMode,
            selectionRectangle,
            selectedColor);

            this.Invalidate();
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

        private void ellipseTool_Click(object sender, EventArgs e)
        {
            isColorMode = false;
        }

        private void rectangleTool_Click(object sender, EventArgs e)
        {
            isColorMode = false;
        }

        private void circleTool_Click(object sender, EventArgs e)
        {
            isColorMode = false;
        }

        private void triangleTool_Click(object sender, EventArgs e)
        {
            isColorMode = false;
        }

        private void lineTool_Click(object sender, EventArgs e)
        {
            isColorMode = false;
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