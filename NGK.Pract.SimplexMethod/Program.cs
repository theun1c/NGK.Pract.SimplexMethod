
class SimplexMethod
{
    // главный метод
    static void Simplex_Method(double[,] canonicialTable, double[] objectiveFunction, bool findMax, int variablesCount)
    {
        // Получаем размеры матрицы ограничений
        int rows = canonicialTable.GetLength(0); // Количество уравнений
        int cols = canonicialTable.GetLength(1); // Количество переменных + правая часть

        // Создаем симплекс-таблицу (добавляем строку для целевой функции)
        double[,] simplexTable = new double[rows + 1, cols];

        // Копируем ограничения в симплекс-таблицу
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                simplexTable[i, j] = canonicialTable[i, j];
            }
        }

        // Добавляем целевую функцию в последнюю строку таблицы
        for (int i = 0; i < cols; i++)
        {
            simplexTable[rows, i] = objectiveFunction[i];
        }

        // цикл симплекс-метода
        while (true)
        {
            // Находим разрешающий столбец (с наименьшим отрицательным элементом в целевой строке)
            int mainCol = -1;
            double minVal = 0;
            for (int i = 0; i < cols - 1; i++)
            {
                if (simplexTable[rows, i] < minVal)
                {
                    minVal = simplexTable[rows, i];
                    mainCol = i;
                }
            }

            // Если все элементы целевой строки ≥ 0 - решение найдено
            if (mainCol == -1) 
            { 
                break; 
            }

            // Находим разрешающую строку (минимальное отношение правой части к элементу столбца)
            int mainRow = -1;
            double minRatio = double.MaxValue;
            for (int i = 0; i < rows; i++)
            {
                if (simplexTable[i, mainCol] > 0)
                {
                    double ratio = simplexTable[i, cols - 1] / simplexTable[i, mainCol];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        mainRow = i;
                    }
                }
            }

            // Если не нашли разрешающую строку - задача не имеет решения
            if (mainRow == -1)
            {
                Console.WriteLine("Решения не существует");
                return;
            }

            // Нормализуем разрешающую строку (делим на разрешающий элемент)
            double pivot = simplexTable[mainRow, mainCol];
            for (int j = 0; j < cols; j++)
            {
                simplexTable[mainRow, j] /= pivot;
            }

            // Обнуляем остальные элементы разрешающего столбца
            for (int i = 0; i < rows + 1; i++)
            {
                if (i != mainRow && simplexTable[i, mainCol] != 0)
                {
                    double coeff = simplexTable[i, mainCol];
                    for (int j = 0; j < cols; j++)
                    {
                        simplexTable[i, j] -= simplexTable[mainRow, j] * coeff;
                    }
                }
            }
        }

        // вывод результата
        // Массив для хранения значений переменных
        double[] solution = new double[variablesCount];

        // Находим значения основных переменных
        for (int j = 0; j < variablesCount; j++)
        {
            bool isBasic = true;
            int basicRow = -1;

            // Проверяем, является ли переменная базисной
            for (int i = 0; i < rows; i++)
            {
                if (simplexTable[i, j] == 1)
                {
                    if (basicRow == -1)
                    {
                        basicRow = i;
                    }
                    else
                    {
                        isBasic = false;
                        break;
                    }
                }
                else if (simplexTable[i, j] != 0)
                {
                    isBasic = false;
                    break;
                }
            }

            // Если переменная базисная - берем значение из правой части
            if(isBasic && basicRow != -1)
            {
                solution[j] = simplexTable[basicRow, cols - 1];
            }
            else
            {
                solution[j] = 0;
            }
        }

        // Вычисляем значение целевой функции
        double objectiveValue = simplexTable[rows, cols - 1];
        if (!findMax)
        {
            objectiveValue = -objectiveValue; // Меняем знак для минимизации
        }

        // Выводим результаты
        Console.WriteLine($"Значение целевой функции: {objectiveValue:F3}");
        Console.WriteLine("Значения переменных:");
        for (int i = 0; i < solution.Length; i++)
        {
            Console.WriteLine($"x{i + 1} = {solution[i]:F3}");
        }
    }
    static void Main() // Главный метод, с которого начинается выполнение программы
    {
        // Задача 1 на максимум
        Console.WriteLine("\nЗадача 1 (max)");

        // Матрица ограничений 
        // канонический вид
        double[,] canonicalTableForMax = {
            //x1 x2  x3 x4 x5  пч
            { 4, 7, 1, 0, 0, 49 },   // 4x1 + 7x2 + x3 = 49
            { 8, 3, 0, 1, 0, 51 },   // 8x1 + 3x2 + x4 = 51
            { 9, 5, 0, 0, 1, 45 }    // 9x1 + 5x2 + x5 = 45
        };

        // Целевая функция: F = 6x1 + 5x2 → max
        // Для симплекс-метода преобразуем к виду: -6x1 - 5x2 → max
        double[] function1 = { -6, -5, 0, 0, 0, 0 };

        // Вызываем метод решения (true - поиск максимума, 2 - выводим x1 и x2)
        Simplex_Method(canonicalTableForMax, function1, true, 2);

        // Задача 2 на минимум
        Console.WriteLine("\nЗадача 2 (min)");

        // Матрица ограничений 
        // канонический вид
        double[,] canonicialTableForMin = {
            //x1  x2  x3  x4 x5 x6  пч
            {  2, -1,  1, 1, 0, 0, 1 },   // 2x1 - x2 + x3 + x4 = 1
            { -4,  2, -1, 0, 1, 0, 2 },   // -4x1 + 2x2 - x3 + x5 = 2
            {  3,  0,  1, 0, 0, 1, 5 }     // 3x1 + x3 + x6 = 5
        };

        // целевая функция: x1 - x2 - 3x3 → max
        double[] function2 = { 1, -1, -3, 0, 0, 0, 0 };

        // Вызываем метод решения (false - поиск минимума, 3 - выводим x1, x2, x3)
        Simplex_Method(canonicialTableForMin, function2, false, 3);
    }
}