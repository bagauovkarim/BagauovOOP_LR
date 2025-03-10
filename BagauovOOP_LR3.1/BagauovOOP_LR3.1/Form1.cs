using System;
using System.Windows.Forms;

namespace BagauovOOP_LR3._1
{
    public partial class Form1 : Form
    {
        Model model;

        public Form1()
        {
            InitializeComponent();
            model = new Model();
            
            
        }

        // Обработчик изменения значения в numericUpDown
        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (sender == numericUpDown1)
            {
                model.setA((int)numericUpDown1.Value);
                UpdateAllControls(); // Обновляем все элементы управления
            }
            else if (sender == numericUpDown2)
            {
                model.setB((int)numericUpDown2.Value);
                UpdateAllControls(); // Обновляем только элементы управления для B
            }
            else if (sender == numericUpDown3)
            {
                model.setC((int)numericUpDown3.Value);
                UpdateAllControls(); // Обновляем все элементы управления
            }
        }

        // Обработчик изменения значения в trackBar
        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if (sender == trackBar1)
            {
                model.setA(trackBar1.Value);
                UpdateAllControls(); // Обновляем все элементы управления
            }
            else if (sender == trackBar2)
            {
                model.setB(trackBar2.Value);
                UpdateAllControls(); // Обновляем только элементы управления для B
            }
            else if (sender == trackBar3)
            {
                model.setC(trackBar3.Value);
                UpdateAllControls(); // Обновляем все элементы управления
            }
        }

        // Обработчик изменения текста в textBox
        private void textBox_TextChanged(object sender, EventArgs e)
        {   

            if (sender == textBox1 && int.TryParse(textBox1.Text, out int valueA))
            {
                model.setA(valueA);
                UpdateAllControls(); // Обновляем все элементы управления
            }
            else if (sender == textBox2 && int.TryParse(textBox2.Text, out int valueB))
            {
                model.setB(valueB);
                UpdateAllControls(); // Обновляем только элементы управления для B
            }
            else if (sender == textBox3 && int.TryParse(textBox3.Text, out int valueC))
            {
                model.setC(valueC);
                UpdateAllControls(); // Обновляем все элементы управления
            }
        }

        // Обновление всех элементов управления
        private void UpdateAllControls()
        {
            // Обновляем элементы управления для A
            numericUpDown1.Value = model.getA();
            trackBar1.Value = model.getA();
            textBox1.Text = model.getA().ToString();

            // Обновляем элементы управления для B
            numericUpDown2.Value = model.getB();
            trackBar2.Value = model.getB();
            textBox2.Text = model.getB().ToString();

            // Обновляем элементы управления для C
            numericUpDown3.Value = model.getC();
            trackBar3.Value = model.getC();
            textBox3.Text = model.getC().ToString();
        }

        // Обновление элементов управления для B
        
        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Разрешаем ввод только цифр и управляющих клавиш (Backspace)
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // Блокируем ввод
            }
            if (sender is TextBox textBox && char.IsDigit(e.KeyChar))
            {
                string currentText = textBox.Text + e.KeyChar; // Предполагаемый текст после ввода
                if (int.TryParse(currentText, out int newValue) && newValue > 100)
                    {
                        e.Handled = true; // Если новое значение больше 100, блокируем ввод
                    }   
            }
        }
    }
}

class Model
{
    private int A;
    private int B;
    private int C;

    // Геттеры
    public int getA()
    {
        return A;
    }
    public int getB()
    {
        return B;
    }
    public int getC()
    {
        return C;
    }


    // Сеттер для A (разрешающее поведение)
    public void setA(int value)
    {
        if (value > C )
        {
            // Если A > C, увеличиваем C до значения A
            C = value;
        }
        A = value;

        // Корректируем B, если оно вышло за пределы
        if (B < A)
        {
            B = A;
        }

        if (B > C)
        {
            B = C;
        }

    }

    // Сеттер для B (запрещающее поведение)
    public void setB(int value)
    {
        if (value >= A && value <= C)
        {
            B = value; // Принимаем значение, если оно корректно
        }
        // Иначе игнорируем (откатываем изменение)
    }

    // Сеттер для C (разрешающее поведение)
    public void setC(int value)
    {
        if (value < A)
        {
            // Если C < A, уменьшаем A до значения C
            A = value;
        }
        C = value;

        // Корректируем B, если оно вышло за пределы
        if (B < A)
        {
            B = A;
        }

        if (B > C)
        {
            B = C;
        }
    }
}

