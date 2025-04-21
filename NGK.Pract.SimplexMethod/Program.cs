
class SimplexMethod // Основной класс программы
{
    static void Main() // Главный метод, с которого начинается выполнение программы
    {
        // ==================== ЗАДАЧА 1 (max) ====================
        Console.WriteLine("\n=== Задача 1 (max) ===");

        // Матрица ограничений 
        // канонический вид
        double[,] constraints1 = {
            //x1 x2  x3 x4 x5  пч
            { 4, 7, 1, 0, 0, 49 },   // 4x1 + 7x2 + x3 = 49
            { 8, 3, 0, 1, 0, 51 },   // 8x1 + 3x2 + x4 = 51
            { 9, 5, 0, 0, 1, 45 }    // 9x1 + 5x2 + x5 = 45
        };

        // Целевая функция: F = 6x1 + 5x2 → max
        // Для симплекс-метода преобразуем к виду: -6x1 - 5x2 → max
        double[] objective1 = { -6, -5, 0, 0, 0, 0 };

        // Вызываем метод решения (true - поиск максимума, 2 - выводим x1 и x2)
        SolveSimplex(constraints1, objective1, true, 2);

        // ==================== ЗАДАЧА 2 (min) ====================
        Console.WriteLine("\n=== Задача 2 (min) ===");

        // Матрица ограничений 
        // канонический вид
        double[,] constraints2 = {
            //x1  x2  x3  x4 x5 x6  пч
            {  2, -1,  1, 1, 0, 0, 1 },   // 2x1 - x2 + x3 + x4 = 1
            { -4,  2, -1, 0, 1, 0, 2 },   // -4x1 + 2x2 - x3 + x5 = 2
            {  3,  0,  1, 0, 0, 1, 5 }     // 3x1 + x3 + x6 = 5
        };

        // целевая функция: x1 - x2 - 3x3 → max
        double[] objective2 = { 1, -1, -3, 0, 0, 0, 0 };

        // Вызываем метод решения (false - поиск минимума, 3 - выводим x1, x2, x3)
        SolveSimplex(constraints2, objective2, false, 3);
    }

    // ==================== МЕТОД РЕШЕНИЯ СИМПЛЕКС-МЕТОДОМ ====================
    static void SolveSimplex(double[,] constraints, double[] objectiveFunction,
                           bool findMax, int mainVariablesCount)
    {
        // Получаем размеры матрицы ограничений
        int rows = constraints.GetLength(0); // Количество уравнений
        int cols = constraints.GetLength(1); // Количество переменных + правая часть

        // Создаем симплекс-таблицу (добавляем строку для целевой функции)
        double[,] simplexTable = new double[rows + 1, cols];

        // Копируем ограничения в симплекс-таблицу
        for (int i = 0; i < rows; i++)
            for (int j = 0; j < cols; j++)
                simplexTable[i, j] = constraints[i, j];

        // Добавляем целевую функцию в последнюю строку таблицы
        for (int j = 0; j < cols - 1; j++)
            simplexTable[rows, j] = objectiveFunction[j];

        // ========== ОСНОВНОЙ ЦИКЛ СИМПЛЕКС-МЕТОДА ==========
        while (true)
        {
            // Шаг 1: Находим разрешающий столбец (с наименьшим отрицательным элементом в целевой строке)
            int pivotCol = -1;
            double minVal = 0;
            for (int j = 0; j < cols - 1; j++)
            {
                if (simplexTable[rows, j] < minVal)
                {
                    minVal = simplexTable[rows, j];
                    pivotCol = j;
                }
            }

            // Если все элементы целевой строки ≥ 0 - решение найдено
            if (pivotCol == -1) break;

            // Шаг 2: Находим разрешающую строку (минимальное отношение правой части к элементу столбца)
            int pivotRow = -1;
            double minRatio = double.MaxValue;
            for (int i = 0; i < rows; i++)
            {
                if (simplexTable[i, pivotCol] > 0)
                {
                    double ratio = simplexTable[i, cols - 1] / simplexTable[i, pivotCol];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                }
            }

            // Если не нашли разрешающую строку - задача не имеет решения
            if (pivotRow == -1)
            {
                Console.WriteLine("Решения не существует");
                return;
            }

            // Шаг 3: Нормализуем разрешающую строку (делим на разрешающий элемент)
            double pivot = simplexTable[pivotRow, pivotCol];
            for (int j = 0; j < cols; j++)
                simplexTable[pivotRow, j] /= pivot;

            // Шаг 4: Обнуляем остальные элементы разрешающего столбца
            for (int i = 0; i < rows + 1; i++)
            {
                if (i != pivotRow && simplexTable[i, pivotCol] != 0)
                {
                    double coeff = simplexTable[i, pivotCol];
                    for (int j = 0; j < cols; j++)
                        simplexTable[i, j] -= simplexTable[pivotRow, j] * coeff;
                }
            }
        }

        // ========== ВЫВОД РЕЗУЛЬТАТОВ ==========
        // Массив для хранения значений переменных
        double[] solution = new double[mainVariablesCount];

        // Находим значения основных переменных
        for (int j = 0; j < mainVariablesCount; j++)
        {
            bool isBasic = true;
            int basicRow = -1;

            // Проверяем, является ли переменная базисной
            for (int i = 0; i < rows; i++)
            {
                if (simplexTable[i, j] == 1)
                {
                    if (basicRow == -1)
                        basicRow = i;
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
            solution[j] = isBasic && basicRow != -1 ? simplexTable[basicRow, cols - 1] : 0;
        }

        // Вычисляем значение целевой функции
        double objectiveValue = simplexTable[rows, cols - 1];
        if (!findMax)
            objectiveValue = -objectiveValue; // Меняем знак для минимизации

        // Выводим результаты
        Console.WriteLine($"Значение целевой функции: {objectiveValue:F3}");
        Console.WriteLine("Значения переменных:");
        for (int i = 0; i < solution.Length; i++)
            Console.WriteLine($"x{i + 1} = {solution[i]:F3}");
    }
}