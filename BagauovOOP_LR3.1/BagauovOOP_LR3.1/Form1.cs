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
            if (sender is NumericUpDown numericUpDown)
            {
                // Используем метод модели для обработки изменения значения
                model.HandleNumericUpDownChanged(numericUpDown, UpdateAllControls);
            }
        }

        // Обработчик изменения значения в trackBar
        private void trackBar_Scroll(object sender, EventArgs e)
        {
            if (sender is TrackBar trackBar)
            {
                // Используем метод модели для обработки изменения значения
                model.HandleTrackBarScroll(trackBar, UpdateAllControls);
            }
        }

        // Обработчик изменения текста в textBox
        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Используем метод модели для обработки изменения текста
                model.HandleTextChanged(textBox, UpdateAllControls);
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
            if (sender is TextBox textBox)
            {
                // Используем метод модели для выполнения всех проверок
                e.Handled = model.HandleKeyPress(textBox, e.KeyChar);
            }
        }

        private void Form_Click(object sender, EventArgs e)
        {
            // Восстанавливаем значения в TextBox с использованием метода модели
            textBox1.Text = model.getA().ToString();
            textBox2.Text = model.getB().ToString();
            textBox3.Text = model.getC().ToString();
            
            numericUpDown1.Value = model.getA();
            numericUpDown2.Value = model.getB();
            numericUpDown3.Value = model.getC();

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

    public string GetRestoredValue(string text, int valueType)
    {
        if (string.IsNullOrEmpty(text))
        {
            switch (valueType)
            {
                case 1: // Для A
                    return A.ToString();
                case 2: // Для B
                    return B.ToString();
                case 3: // Для C
                    return C.ToString();
                default:
                    throw new ArgumentException("Invalid value type");
            }
        }
        return text;
    }

    private bool IsValidKey(char keyChar)
    {
        // Разрешаем только цифры и управляющие клавиши (например, Backspace)
        return char.IsDigit(keyChar) || char.IsControl(keyChar);
    }

    // Проверка, не превышает ли новое значение лимит
    private bool IsValueWithinLimit(string text, char newChar, int limit)
    {
        string newText = text + newChar; // Предполагаемый текст после ввода
        if (int.TryParse(newText, out int newValue))
        {
            return newValue <= limit;
        }
        return true; // Если текст не является числом, считаем его допустимым
    }

    // Основной метод для обработки KeyPress
    public bool HandleKeyPress(TextBox textBox, char keyChar)
    {
        // Получаем текущий текст и лимит (например, 100)
        string currentText = textBox.Text;
        int limit = 100;

        // Если символ недопустим, блокируем ввод
        if (!IsValidKey(keyChar))
        {
            return true; // Блокировать ввод
        }

        // Если символ допустим и это цифра, проверяем лимит
        if (char.IsDigit(keyChar) && !IsValueWithinLimit(currentText, keyChar, limit))
        {
            return true; // Блокировать ввод
        }

        return false; // Разрешить ввод
    }

    public bool HandleTextChanged(TextBox textBox, Action updateControls)
    {
        if (textBox == null || string.IsNullOrEmpty(textBox.Text))
        {
            return false; // Если TextBox пустой, ничего не делаем
        }

        if (int.TryParse(textBox.Text, out int value))
        {
            if (textBox.Name == "textBox1") // Для A
            {
                setA(value);
            }
            else if (textBox.Name == "textBox2") // Для B
            {
                setB(value);
            }
            else if (textBox.Name == "textBox3") // Для C
            {
                setC(value);
            }

            // Вызываем метод для обновления элементов управления
            updateControls?.Invoke();
            return true; // Успешно обработано
        }

        return false; // Если текст не является числом, ничего не делаем
    }

    public void HandleNumericUpDownChanged(NumericUpDown numericUpDown, Action updateControls)
    {
        if (numericUpDown == null)
        {
            return; // Если NumericUpDown пустой, ничего не делаем
        }

        int value = (int)numericUpDown.Value;

        if (numericUpDown.Name == "numericUpDown1") // Для A
        {
            setA(value);
        }
        else if (numericUpDown.Name == "numericUpDown2") // Для B
        {
            setB(value);
        }
        else if (numericUpDown.Name == "numericUpDown3") // Для C
        {
            setC(value);
        }

        // Вызываем метод для обновления элементов управления
        updateControls?.Invoke();
    }

    // Метод для обработки изменения значения в TrackBar
    public void HandleTrackBarScroll(TrackBar trackBar, Action updateControls)
    {
        if (trackBar == null)
        {
            return; // Если TrackBar пустой, ничего не делаем
        }

        int value = trackBar.Value;

        if (trackBar.Name == "trackBar1") // Для A
        {
            setA(value);
        }
        else if (trackBar.Name == "trackBar2") // Для B
        {
            setB(value);
        }
        else if (trackBar.Name == "trackBar3") // Для C
        {
            setC(value);
        }

        // Вызываем метод для обновления элементов управления
        updateControls?.Invoke();
    }

    

}
