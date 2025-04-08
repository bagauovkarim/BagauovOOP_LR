namespace BagauovOOP_LR4
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.toolboxPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.Cursortool = new System.Windows.Forms.PictureBox();
            this.ColorTool = new System.Windows.Forms.PictureBox();
            this.ellipseTool = new System.Windows.Forms.PictureBox();
            this.rectangleTool = new System.Windows.Forms.PictureBox();
            this.circleTool = new System.Windows.Forms.PictureBox();
            this.triangleTool = new System.Windows.Forms.PictureBox();
            this.lineTool = new System.Windows.Forms.PictureBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.toolboxPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Cursortool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorTool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ellipseTool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rectangleTool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.circleTool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.triangleTool)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineTool)).BeginInit();
            this.SuspendLayout();
            // 
            // toolboxPanel
            // 
            this.toolboxPanel.AutoScroll = true;
            this.toolboxPanel.BackColor = System.Drawing.Color.LightGray;
            this.toolboxPanel.Controls.Add(this.Cursortool);
            this.toolboxPanel.Controls.Add(this.ColorTool);
            this.toolboxPanel.Controls.Add(this.ellipseTool);
            this.toolboxPanel.Controls.Add(this.rectangleTool);
            this.toolboxPanel.Controls.Add(this.circleTool);
            this.toolboxPanel.Controls.Add(this.triangleTool);
            this.toolboxPanel.Controls.Add(this.lineTool);
            this.toolboxPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.toolboxPanel.Location = new System.Drawing.Point(0, 0);
            this.toolboxPanel.Margin = new System.Windows.Forms.Padding(5);
            this.toolboxPanel.Name = "toolboxPanel";
            this.toolboxPanel.Padding = new System.Windows.Forms.Padding(5);
            this.toolboxPanel.Size = new System.Drawing.Size(70, 521);
            this.toolboxPanel.TabIndex = 0;
            // 
            // Cursortool
            // 
            this.Cursortool.BackColor = System.Drawing.Color.White;
            this.Cursortool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Cursortool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Cursortool.Image = ((System.Drawing.Image)(resources.GetObject("Cursortool.Image")));
            this.Cursortool.Location = new System.Drawing.Point(8, 8);
            this.Cursortool.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.Cursortool.Name = "Cursortool";
            this.Cursortool.Size = new System.Drawing.Size(50, 50);
            this.Cursortool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.Cursortool.TabIndex = 6;
            this.Cursortool.TabStop = false;
            this.Cursortool.Tag = "Курсор";
            this.Cursortool.Click += new System.EventHandler(this.Cursortool_Click);
            // 
            // ColorTool
            // 
            this.ColorTool.BackColor = System.Drawing.Color.White;
            this.ColorTool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ColorTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ColorTool.Image = ((System.Drawing.Image)(resources.GetObject("ColorTool.Image")));
            this.ColorTool.Location = new System.Drawing.Point(8, 71);
            this.ColorTool.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.ColorTool.Name = "ColorTool";
            this.ColorTool.Size = new System.Drawing.Size(50, 50);
            this.ColorTool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ColorTool.TabIndex = 4;
            this.ColorTool.TabStop = false;
            this.ColorTool.Tag = "Цвет";
            this.ColorTool.Click += new System.EventHandler(this.ColorTool_Click);
            // 
            // ellipseTool
            // 
            this.ellipseTool.BackColor = System.Drawing.Color.White;
            this.ellipseTool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ellipseTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ellipseTool.Image = ((System.Drawing.Image)(resources.GetObject("ellipseTool.Image")));
            this.ellipseTool.Location = new System.Drawing.Point(8, 134);
            this.ellipseTool.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.ellipseTool.Name = "ellipseTool";
            this.ellipseTool.Size = new System.Drawing.Size(50, 50);
            this.ellipseTool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.ellipseTool.TabIndex = 5;
            this.ellipseTool.TabStop = false;
            this.ellipseTool.Tag = "Эллипс";
            this.ellipseTool.Click += new System.EventHandler(this.ellipseTool_Click);
            // 
            // rectangleTool
            // 
            this.rectangleTool.BackColor = System.Drawing.Color.White;
            this.rectangleTool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rectangleTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rectangleTool.Image = ((System.Drawing.Image)(resources.GetObject("rectangleTool.Image")));
            this.rectangleTool.Location = new System.Drawing.Point(8, 197);
            this.rectangleTool.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.rectangleTool.Name = "rectangleTool";
            this.rectangleTool.Size = new System.Drawing.Size(50, 50);
            this.rectangleTool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.rectangleTool.TabIndex = 0;
            this.rectangleTool.TabStop = false;
            this.rectangleTool.Tag = "Прямоугольник";
            this.rectangleTool.Click += new System.EventHandler(this.rectangleTool_Click);
            // 
            // circleTool
            // 
            this.circleTool.BackColor = System.Drawing.Color.White;
            this.circleTool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.circleTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.circleTool.Image = ((System.Drawing.Image)(resources.GetObject("circleTool.Image")));
            this.circleTool.Location = new System.Drawing.Point(8, 260);
            this.circleTool.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.circleTool.Name = "circleTool";
            this.circleTool.Size = new System.Drawing.Size(50, 50);
            this.circleTool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.circleTool.TabIndex = 1;
            this.circleTool.TabStop = false;
            this.circleTool.Tag = "Круг";
            this.circleTool.Click += new System.EventHandler(this.circleTool_Click);
            // 
            // triangleTool
            // 
            this.triangleTool.BackColor = System.Drawing.Color.White;
            this.triangleTool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.triangleTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.triangleTool.Image = ((System.Drawing.Image)(resources.GetObject("triangleTool.Image")));
            this.triangleTool.Location = new System.Drawing.Point(8, 323);
            this.triangleTool.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.triangleTool.Name = "triangleTool";
            this.triangleTool.Size = new System.Drawing.Size(50, 50);
            this.triangleTool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.triangleTool.TabIndex = 2;
            this.triangleTool.TabStop = false;
            this.triangleTool.Tag = "Треугольник";
            this.triangleTool.Click += new System.EventHandler(this.triangleTool_Click);
            // 
            // lineTool
            // 
            this.lineTool.BackColor = System.Drawing.Color.White;
            this.lineTool.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lineTool.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lineTool.Image = ((System.Drawing.Image)(resources.GetObject("lineTool.Image")));
            this.lineTool.Location = new System.Drawing.Point(8, 386);
            this.lineTool.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.lineTool.Name = "lineTool";
            this.lineTool.Size = new System.Drawing.Size(50, 50);
            this.lineTool.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.lineTool.TabIndex = 3;
            this.lineTool.TabStop = false;
            this.lineTool.Tag = "Линия";
            this.lineTool.Click += new System.EventHandler(this.lineTool_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 521);
            this.Controls.Add(this.toolboxPanel);
            this.Name = "Form1";
            this.Text = "Графический редактор";
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseUp);
            this.toolboxPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Cursortool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColorTool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ellipseTool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rectangleTool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.circleTool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.triangleTool)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.lineTool)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel toolboxPanel;
        private System.Windows.Forms.PictureBox rectangleTool;
        private System.Windows.Forms.PictureBox circleTool;
        private System.Windows.Forms.PictureBox triangleTool;
        private System.Windows.Forms.PictureBox lineTool;
        private System.Windows.Forms.PictureBox ColorTool;
        private System.Windows.Forms.PictureBox ellipseTool;
        private System.Windows.Forms.PictureBox Cursortool;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}