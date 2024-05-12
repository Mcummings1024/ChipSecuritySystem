using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChipSecuritySystem
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var chips = new List<ColorChip>();
            Console.WriteLine("Do you have a list of chips? (y/N)");
            var hasList = Console.ReadLine();

            try
            {
                if (hasList?.ToUpper() == "Y")
                {
                    Console.WriteLine("Please enter the list as comma-separated, pipe-delimited pairs (i.e. Blue, Green | Red, Yellow etc.):");
                    var list = Console.ReadLine()?.Replace(" ", "").Split('|');
                    // Todo: improve list validation
                    if (list?.Length > 1) chips = ParseList(list.ToList());
                }

                if (chips.Count == 0)
                {
                    Console.WriteLine("Generating random list.");
                    chips = GenerateChips();
                }

                Console.Write("List is: ");
                chips.ForEach(c => Console.Write("[" + c + "] "));
                Console.WriteLine();
                
                // Check for trivial cases
                if (!(chips.Any(c => c.StartColor == Color.Blue) &&
                      chips.Any(c => c.EndColor == Color.Green)))
                {
                    Console.WriteLine(Constants.ErrorMessage);
                    return;
                }

                // Start with blue
                var bestSolution = FindLongestPath(Color.Blue, chips, new List<ColorChip>(), new List<ColorChip>());

                if (bestSolution.Count > 0)
                {
                    Console.WriteLine("The longest solution is " + bestSolution.Count + ": ");
                    bestSolution.ForEach(c => Console.Write("[" + c + "] "));
                }
                else
                {
                    Console.WriteLine(Constants.ErrorMessage);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static List<ColorChip> FindLongestPath(Color color, List<ColorChip> remainingChips, List<ColorChip> solution, List<ColorChip> bestSolution)
        {
            var nextChips = remainingChips.Where(c => c.StartColor == color).ToList();

            foreach (var chip in nextChips)
            {
                solution.Add(chip);
                remainingChips.Remove(chip);
                var endColor = chip.EndColor;

                if (endColor == Color.Green && bestSolution.Count < solution.Count)
                {
                    bestSolution = new List<ColorChip>(solution);
                }

                if (remainingChips.Any(c => c.StartColor == endColor))
                {
                    bestSolution = FindLongestPath(endColor, remainingChips, solution, bestSolution);
                }

                solution.Remove(chip);
                remainingChips.Add(chip);
            }

            return bestSolution;
        }

        private static List<ColorChip> ParseList(List<string> list)
        {
            return list.Select(chip => chip.Replace("[", "").Replace("]", "").Split(","))
                .Select(pair => new ColorChip(ParseColor(pair[0]), ParseColor(pair[1]))).ToList();
        }

        private static Color ParseColor(string colorName)
        {
            switch (colorName)
            {
                case "Red": return Color.Red;
                case "Orange": return Color.Orange;
                case "Yellow": return Color.Yellow;
                case"Green": return Color.Green;
                case "Blue": return Color.Blue;
                default: return Color.Purple;
            }
        }

        private static List<ColorChip> GenerateChips()
        {
            var chips = new List<ColorChip>();
            var rng = new Random();
            var size  = rng.Next(1, 11); // 1 to 10

            for (var i = 1; i <= size; i++)
            {
                chips.Add(new ColorChip((Color)rng.Next(6), (Color)rng.Next(6)));
            }

            return chips;
        }
    }
}
