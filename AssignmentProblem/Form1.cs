using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AssignmentProblem
{
    public partial class Form1 : Form
    {
        int size;
        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndexChanged += new EventHandler(ComboBox1_SelectedIndexChanged);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            clearNumericUpDown();
            // Получаем размерность из ComboBox (например, "2x2" -> 2)
            string selectedSize = comboBox1.SelectedItem.ToString();
            size = int.Parse(selectedSize[0].ToString());

            // Скрываем все GroupBox и NumericUpDown
            foreach (Control control in this.Controls)
            {
                if (control is GroupBox && control.Name.StartsWith("groupBoxA"))
                {
                    control.Visible = false;

                    // Скрываем все NumericUpDown внутри GroupBox
                    foreach (Control groupBoxControl in control.Controls)
                    {
                        if (groupBoxControl is NumericUpDown)
                        {
                            groupBoxControl.Visible = false;
                        }
                    }
                }
            }

            // Показываем только нужные GroupBox и NumericUpDown по размерности
            for (int i = 1; i <= size; i++)
            {
                for (int j = 1; j <= size; j++)
                {
                    string groupBoxName = $"groupBoxA{i}xB{j}";
                    Control groupBox = this.Controls.Find(groupBoxName, true).FirstOrDefault();
                    if (groupBox != null)
                    {
                        groupBox.Visible = true;

                        // Показываем все NumericUpDown внутри найденного GroupBox
                        foreach (Control groupBoxControl in groupBox.Controls)
                        {
                            if (groupBoxControl is NumericUpDown)
                            {
                                groupBoxControl.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        private void buttonRandom_Click(object sender, EventArgs e)
        {
            // Создаем генератор случайных чисел
            Random random = new Random();

            // Проходим по всем элементам формы
            foreach (Control control in this.Controls)
            {
                // Ищем только те GroupBox, которые видимы
                if (control is GroupBox groupBox && groupBox.Visible)
                {
                    // Находим NumericUpDown внутри GroupBox
                    foreach (Control innerControl in groupBox.Controls)
                    {
                        if (innerControl is NumericUpDown numericUpDown && numericUpDown.Visible)
                        {
                            // Устанавливаем случайное значение в диапазоне от 1 до 100
                            numericUpDown.Value = random.Next(1, 100);
                            numericUpDown.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            clearNumericUpDown();
        }

        private void clearNumericUpDown()
        {
            // Проходим по всем элементам на форме
            foreach (Control control in this.Controls)
            {
                // Ищем все GroupBox на форме
                if (control is GroupBox groupBox)
                {
                    // Находим NumericUpDown внутри каждого GroupBox
                    foreach (Control innerControl in groupBox.Controls)
                    {
                        if (innerControl is NumericUpDown numericUpDown)
                        {
                            // Устанавливаем значение 0
                            numericUpDown.Value = 0;
                            numericUpDown.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void buttonCalculateMoment_Click(object sender, EventArgs e)
        {
            MinimizeRows(size);
            MinimizeColumns(size);
            CoverZerosWithLines(size, out bool[] coveredRows, out bool[] coveredColumns, out bool allZerosCovered);

            if (allZerosCovered)
            {
                // Шаг 6: Назначение задач
                List<Tuple<int, int>> assignments = AssignTasks(size);

                // Вывод результатов в MessageBox
                StringBuilder message = new StringBuilder();
                foreach (var assignment in assignments)
                {
                    message.AppendLine($"Работник {assignment.Item1} -> Задача {assignment.Item2}");
                }
                MessageBox.Show(message.ToString(), "Результаты назначения", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Шаг 5: Изменение матрицы
                AdjustMatrix(size, coveredRows, coveredColumns);
            }
        }

        private void MinimizeRows(int size)
        {
            // Проход по каждой строке матрицы
            for (int i = 1; i <= size; i++)
            {
                int minValue = int.MaxValue;

                // Найти минимальное значение в строке
                for (int j = 1; j <= size; j++)
                {
                    string numericUpDownName = $"numericUpDownA{i}xB{j}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null)
                    {
                        minValue = Math.Min(minValue, (int)numericUpDown.Value);
                    }
                }

                // Вычесть минимальное значение из всех элементов строки и обновить их
                for (int j = 1; j <= size; j++)
                {
                    string numericUpDownName = $"numericUpDownA{i}xB{j}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null)
                    {
                        numericUpDown.Value -= minValue;
                    }
                }
            }
        }

        private void MinimizeColumns(int size)
        {
            // Проход по каждому столбцу матрицы
            for (int j = 1; j <= size; j++)
            {
                int minValue = int.MaxValue;

                // Найти минимальное значение в столбце
                for (int i = 1; i <= size; i++)
                {
                    string numericUpDownName = $"numericUpDownA{i}xB{j}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null)
                    {
                        minValue = Math.Min(minValue, (int)numericUpDown.Value);
                    }
                }

                // Вычесть минимальное значение из всех элементов столбца и обновить их
                for (int i = 1; i <= size; i++)
                {
                    string numericUpDownName = $"numericUpDownA{i}xB{j}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null)
                    {
                        numericUpDown.Value -= minValue;
                    }
                }
            }
        }

        private void CoverZerosWithLines(int size, out bool[] coveredRows, out bool[] coveredColumns, out bool allZerosCovered)
        {
            coveredRows = new bool[size];
            coveredColumns = new bool[size];
            allZerosCovered = false;

            // Найти строки и столбцы с единственными нулями и отметить их
            bool[,] zeroCovered = new bool[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string numericUpDownName = $"numericUpDownA{i + 1}xB{j + 1}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null && numericUpDown.Value == 0 && !coveredRows[i] && !coveredColumns[j])
                    {
                        zeroCovered[i, j] = true;
                        coveredRows[i] = true;
                        coveredColumns[j] = true;
                    }
                }
            }

            // Сброс покрытий строк и столбцов для последующих шагов
            coveredRows = new bool[size];
            coveredColumns = new bool[size];

            // Покрыть строки и столбцы, содержащие нули
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (zeroCovered[i, j])
                    {
                        coveredRows[i] = true;
                        coveredColumns[j] = true;
                    }
                }
            }

            // Проверить, перекрыты ли все нули
            int coveredLines = 0;
            for (int i = 0; i < size; i++)
            {
                if (coveredRows[i]) coveredLines++;
                if (coveredColumns[i]) coveredLines++;
            }

            allZerosCovered = coveredLines >= size;
        }

        private void AdjustMatrix(int size, bool[] coveredRows, bool[] coveredColumns)
        {
            int minValue = int.MaxValue;

            // Найти наименьший элемент среди не покрытых линиями элементов
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string numericUpDownName = $"numericUpDownA{i + 1}xB{j + 1}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null && !coveredRows[i] && !coveredColumns[j])
                    {
                        minValue = Math.Min(minValue, (int)numericUpDown.Value);
                    }
                }
            }

            // Вычесть наименьшее значение из всех не покрытых элементов и добавить его к покрытым элементам
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    string numericUpDownName = $"numericUpDownA{i + 1}xB{j + 1}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null)
                    {
                        if (!coveredRows[i] && !coveredColumns[j])
                        {
                            numericUpDown.Value -= minValue;
                        }
                        else if (coveredRows[i] && coveredColumns[j])
                        {
                            numericUpDown.Value += minValue;
                        }
                    }
                }
            }
        }

        // Шаг 6: Назначение задач
        private List<Tuple<int, int>> AssignTasks(int size)
        {
            List<Tuple<int, int>> assignments = new List<Tuple<int, int>>();
            bool[] rowCovered = new bool[size];
            bool[] columnCovered = new bool[size];

            for (int i = 1; i <= size; i++)
            {
                for (int j = 1; j <= size; j++)
                {
                    string numericUpDownName = $"numericUpDownA{i}xB{j}";
                    NumericUpDown numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;
                    if (numericUpDown != null && numericUpDown.Value == 0 && !rowCovered[i - 1] && !columnCovered[j - 1])
                    {
                        assignments.Add(new Tuple<int, int>(i, j));
                        rowCovered[i - 1] = true;
                        columnCovered[j - 1] = true;
                        break;
                    }
                }
            }
            return assignments;
        }

        Stack<int[,]> matrixStates = new Stack<int[,]>();

        private void SaveMatrixState()
        {
            int[,] state = new int[size, size];
            for (int i = 1; i <= size; i++)
            {
                for (int j = 1; j <= size; j++)
                {
                    string numericUpDownName = $"numericUpDownA{i}xB{j}";
                    var numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;

                    if (numericUpDown != null)
                    {
                        state[i - 1, j - 1] = (int)numericUpDown.Value;
                    }
                }
            }
            matrixStates.Push(state); // Добавляем состояние в стек
        }

        private void RestoreMatrixState()
        {
            if (matrixStates.Count > 0)
            {
                int[,] state = matrixStates.Pop(); // Извлекаем последнее состояние

                for (int i = 1; i <= size; i++)
                {
                    for (int j = 1; j <= size; j++)
                    {
                        string numericUpDownName = $"numericUpDownA{i}xB{j}";
                        var numericUpDown = this.Controls.Find(numericUpDownName, true).FirstOrDefault() as NumericUpDown;

                        if (numericUpDown != null)
                        {
                            numericUpDown.Value = state[i - 1, j - 1];
                        }
                    }
                }
            }
        }


        int currentStep = 0;
        private void buttonCalculateTime_Click(object sender, EventArgs e)
        {
            currentStep = 1; // Начинаем с первого шага
            matrixStates.Clear(); // Очищаем стек состояний

            SaveMatrixState(); // Сохраняем первоначальное состояние матрицы перед минимизацией строк

            buttonForward.Visible = true;
            buttonBack.Visible = true;
            buttonForward.Enabled = true;
            buttonBack.Enabled = true; // Делаем кнопку "Назад" доступной сразу

            ExecuteStep(currentStep);
        }


        private void buttonForward_Click(object sender, EventArgs e)
        {
            if (currentStep < 5)
            {
                SaveMatrixState(); // Сохраняем текущее состояние перед выполнением следующего шага
                currentStep++;
                ExecuteStep(currentStep);
            }

            buttonBack.Enabled = currentStep > 1;
            buttonForward.Enabled = currentStep < 6;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            if (currentStep == 1 && matrixStates.Count > 0) // Если это шаг минимизации строк
            {
                RestoreMatrixState(); // Восстанавливаем матрицу до минимизации строк
                buttonBack.Enabled = false; // Делаем кнопку "Назад" недоступной, т.к. это первый шаг
            }
            else if (matrixStates.Count > 0) // Для других шагов
            {
                RestoreMatrixState(); // Восстанавливаем предыдущее состояние
                currentStep--; // Уменьшаем номер текущего шага
            }

            buttonForward.Enabled = true; // Разрешаем двигаться вперед
        }



        private void ExecuteStep(int step)
        {
            switch (step)
            {
                case 1:
                    MinimizeRows(size);
                    MessageBox.Show("Шаг 1: Минимизация строк выполнена.");
                    break;

                case 2:
                    MinimizeColumns(size);
                    MessageBox.Show("Шаг 2: Минимизация столбцов выполнена.");
                    break;

                case 3:
                    CoverZerosWithLines(size, out _, out _, out _);
                    MessageBox.Show("Шаг 3: Покрытие нулей линиями выполнено.");
                    break;

                case 4:
                    AdjustMatrix(size, new bool[size], new bool[size]);
                    MessageBox.Show("Шаг 4: Матрица изменена для нового покрытия.");
                    break;

                case 5:
                    var assignments = AssignTasks(size);
                    StringBuilder result = new StringBuilder("Назначение задач:\n");
                    foreach (var assignment in assignments)
                    {
                        result.AppendLine($"Работник {assignment.Item1} -> Задача {assignment.Item2}");
                    }
                    MessageBox.Show(result.ToString(), "Результаты назначения", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;

                default:
                    MessageBox.Show("Шаг не определен!");
                    break;
            }
        }

    }
}
