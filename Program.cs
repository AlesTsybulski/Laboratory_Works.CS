
using System.Drawing;
using System.Drawing.Imaging;


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
                Console.WriteLine($"File not found: {inputFile}");
                return;
            }
        }

        // Read colors from the file
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
                        // HEX format
                        color = ColorTranslator.FromHtml(trimmed);
                    }
                    else
                    {
                        // Color name
                        color = Color.FromName(trimmed);
                        // Color.FromName returns a color with ARGB(0,0,0,0) if the name is not found.
                        if (color.ToArgb() == 0 && !string.Equals(trimmed, "Transparent", StringComparison.OrdinalIgnoreCase))
                        {
                            // If it's not a standard name, try it as HEX without #
                            color = ColorTranslator.FromHtml("#" + trimmed);
                        }
                    }
                    colors.Add(color);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to recognize color: {trimmed} ({ex.Message})");
                }
            }
        }

        if (colors.Count == 0)
        {
            Console.WriteLine("No valid colors found.");
            return;
        }

        // Calculate image dimensions (square grid, closest to sqrt)
        int side = (int)Math.Ceiling(Math.Sqrt(colors.Count));
        int totalPixels = side * side;
        // Add more colors if needed, repeating the last one
        while (colors.Count < totalPixels)
        {
            colors.Add(colors.Last());
        }

        // Create bitmap
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

            // Save as PNG
            string outputFileName = "colors.png";
            string outputPath = Path.Combine(Path.GetDirectoryName(inputFile), outputFileName);
            try
            {
                bitmap.Save(outputPath, ImageFormat.Png);
                Console.WriteLine($"Image saved: {outputPath} ({side}x{side} pixels)");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error saving image: {e.Message}");
            }
        }
    }
}