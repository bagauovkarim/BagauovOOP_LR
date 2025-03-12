using BagauovOOP_LR3._1.Properties;
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

            model.LoadData();
            UpdateAllControls();
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

            label6.Text = model.GetNotificationCounter().ToString();
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

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Сохраняем данные при закрытии приложения
            model.SaveData();
        }

        
    }
}

class Model
{
    private int A;
    private int B;
    private int C;
    private int notificationCounter = 0; // Счетчик уведомлений
    private bool isInitialLoad = true;

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

    // Геттер для счетчика уведомлений
    public int GetNotificationCounter()
    {
        return notificationCounter;
    }

    // Метод для увеличения счетчика уведомлений
    private void IncrementNotificationCounter()
    {
        notificationCounter++;
    }

    // Метод для обновления значений с минимизацией уведомлений
    private void UpdateValues(int newA, int newB, int newC, Action updateControls)
    {
        bool isChanged = false; // Флаг для отслеживания изменений

        // Проверяем и обновляем A
        if (newA != A)
        {
            A = newA;
            isChanged = true;
        }

        // Проверяем и обновляем B
        if (newB != B)
        {
            B = newB;
            isChanged = true;
        }

        // Проверяем и обновляем C
        if (newC != C)
        {
            C = newC;
            isChanged = true;
        }

        // Если хотя бы одно значение изменилось, увеличиваем счетчик уведомлений и испускаем уведомление
        if (isChanged)
        {
            IncrementNotificationCounter();
            if (!isInitialLoad) // Не вызываем уведомление при начальной загрузке
            {
                updateControls?.Invoke();
            }
        }
    }

    // Сеттер для A (разрешающее поведение)
    public void setA(int value, Action updateControls)
    {
        int newA = value;
        int newB = B;
        int newC = C;

        if (newA > C)
        {
            // Если A > C, увеличиваем C до значения A
            newC = newA;
        }

        // Корректируем B, если оно вышло за пределы
        if (newB < newA)
        {
            newB = newA;
        }

        if (newB > newC)
        {
            newB = newC;
        }

        UpdateValues(newA, newB, newC, updateControls);
    }

    // Сеттер для B (запрещающее поведение)
    
    public void setB(int value, Action updateControls)
    {
        if (value >= A && value <= C)
        {
            // Если значение корректно, обновляем B
            UpdateValues(A, value, C, updateControls);
        }
        else
        {
            // Если значение некорректно, восстанавливаем предыдущее значение B
            UpdateValues(A, B, C, null); // Не вызываем уведомление
        }
    }

    // Сеттер для C (разрешающее поведение)
    public void setC(int value, Action updateControls)
    {
        int newC = value;
        int newA = A;
        int newB = B;

        if (newC < A)
        {
            // Если C < A, уменьшаем A до значения C
            newA = newC;
        }

        // Корректируем B, если оно вышло за пределы
        if (newB < newA)
        {
            newB = newA;
        }

        if (newB > newC)
        {
            newB = newC;
        }

        UpdateValues(newA, newB, newC, updateControls);
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
                setA(value, updateControls);
            }
            else if (textBox.Name == "textBox2") // Для B
            {
                // Проверяем, соответствует ли значение бизнес-правилам
                if (value >= getA() && value <= getC())
                {
                    setB(value, updateControls);
                }
                else
                {
                    // Если значение некорректно, восстанавливаем предыдущее значение
                    textBox.Text = getB().ToString();
                }
            }
            else if (textBox.Name == "textBox3") // Для C
            {
                setC(value, updateControls);
            }

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
            setA(value, updateControls);
        }
        else if (numericUpDown.Name == "numericUpDown2") // Для B
        {
            // Проверяем, соответствует ли значение бизнес-правилам
            if (value >= getA() && value <= getC())
            {
                setB(value, updateControls);
            }
            else
            {
                // Если значение некорректно, восстанавливаем предыдущее значение
                numericUpDown.Value = getB();
            }
        }
        else if (numericUpDown.Name == "numericUpDown3") // Для C
        {
            setC(value, updateControls);
        }
    }

    public void HandleTrackBarScroll(TrackBar trackBar, Action updateControls)
    {
        if (trackBar == null)
        {
            return; // Если TrackBar пустой, ничего не делаем
        }

        int value = trackBar.Value;

        if (trackBar.Name == "trackBar1") // Для A
        {
            setA(value, updateControls);
        }
        else if (trackBar.Name == "trackBar2") // Для B
        {
            // Проверяем, соответствует ли значение бизнес-правилам
            if (value >= getA() && value <= getC())
            {
                setB(value, updateControls);
            }
            else
            {
                // Если значение некорректно, восстанавливаем предыдущее значение
                trackBar.Value = getB();
            }
        }
        else if (trackBar.Name == "trackBar3") // Для C
        {
            setC(value, updateControls);
        }
    }

    public void SaveData()
    {
        Settings.Default.A = A;
        Settings.Default.B = B;
        Settings.Default.C = C;
        Settings.Default.NotificationCounter = notificationCounter;
        Settings.Default.Save(); // Сохраняем настройки
    }

    // Метод для загрузки данных из настроек
    public void LoadData()
    {
        A = Settings.Default.A;
        B = Settings.Default.B;
        C = Settings.Default.C;
        notificationCounter = Settings.Default.NotificationCounter;

        // Увеличиваем счетчик уведомлений при загрузке данных
        IncrementNotificationCounter();

        // После загрузки данных сбрасываем флаг
        isInitialLoad = false;
    }



}
