using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        string inputFile = "TOG.txt";
        if (!File.Exists(inputFile))
        {
            // If not in the current directory, check the parent directory.
            inputFile = Path.Combine("..", inputFile);
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Файл не найден: {inputFile}");
                return;
            }
        }

        // Чтение цветов из файла (книги)
        var russianColors = new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase)
        {
            { "Красный", Color.Red },
            { "Оранжевый", Color.Orange },
            { "Желтый", Color.Yellow },
            { "Зеленый", Color.Green },
            { "Голубой", Color.Cyan },
            { "Синий", Color.Blue },
            { "Фиолетовый", Color.Violet },
            { "Белый", Color.White },
            { "Черный", Color.Black },
            { "Серый", Color.Gray },
            { "Розовый", Color.Pink },
            { "Коричневый", Color.Brown }
        };

        List<Color> colors = new List<Color>();
        string[] lines = File.ReadAllLines(inputFile);
        foreach (string line in lines)
        {
            string[] colorEntries = line.Split(new[] { ' ', '	', ',', '.', ';', ':', '!', '?', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string entry in colorEntries)
            {
                string trimmed = entry.Trim();
                if (string.IsNullOrEmpty(trimmed)) continue;

                try
                {
                    Color color;
                    if (trimmed.StartsWith("#"))
                    {
                        // HEX формат
                        color = ColorTranslator.FromHtml(trimmed);
                    }
                    else if (russianColors.TryGetValue(trimmed, out color))
                    {
                        // Russian color name
                    }
                    else
                    {
                        // Название цвета
                        color = Color.FromName(trimmed);
                        // Color.FromName returns a color with ARGB(0,0,0,0) if the name is not found.
                        if (color.ToArgb() == 0 && !string.Equals(trimmed, "Transparent", StringComparison.OrdinalIgnoreCase))
                        {
                            // Если не стандартное имя, попробуем как HEX без #
                            color = ColorTranslator.FromHtml("#" + trimmed);
                        }
                    }
                    colors.Add(color);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось распознать цвет: {trimmed} ({ex.Message})");
                }
            }
        }

        if (colors.Count == 0)
        {
            Console.WriteLine("Не найдено ни одного валидного цвета.");
            return;
        }

        // Вычисление размеров изображения (квадратная сетка, ближайшая к sqrt)
        int side = (int)Math.Ceiling(Math.Sqrt(colors.Count));
        int totalPixels = side * side;
        // Дополним цвета, если нужно, повторяя последний
        while (colors.Count < totalPixels)
        {
            colors.Add(colors.Last());
        }

        // Создание bitmap
        using (Bitmap bitmap = new Bitmap(side, side))
        {
            for (int y = 0; y < side; y++)
            {
                for (int x = 0; x < side; x++)
                {
                    int index = y * side + x;
                    bitmap.SetPixel(x, y, colors[index]);
                }
            }

            // Сохранение как PNG
            string outputFileName = "colors.png";
            string outputPath = Path.Combine(Path.GetDirectoryName(inputFile), outputFileName);
            try
            {
                bitmap.Save(outputPath, ImageFormat.Png);
                Console.WriteLine($"Изображение сохранено: {outputPath} ({side}x{side} пикселей)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при сохранении изображения: {e.Message}");
            }
        }
    }
}