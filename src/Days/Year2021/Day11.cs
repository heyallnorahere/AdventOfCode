using AdventOfCode.Structures;
using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(11, "Dumbo Octopus")]
    public sealed class Day11 : IDay
    {
        static Day11()
        {
            mAdjacentOffsets = new List<Vector>
            {
                (1, 0),
                (-1, 0),

                (0, 1),
                (0, -1),

                (1, 1),
                (-1, 1),

                (1, -1),
                (-1, -1),
            };
        }
        private static readonly List<Vector> mAdjacentOffsets; 
        public void Run(string input)
        {
            var map = ReadInput(input);

            int simulationLength =
                Utilities.RequestInput("Input how many ticks the simulation should last for");
            if (simulationLength <= 0)
            {
                throw new ArgumentException("Please insert a positive integer.");
            }

            int totalFlashCount = 0;
            int firstCompleteFlash = -1;
            for (int tick = 0; tick < simulationLength; tick++)
            {
                // increment each octopus' energy level
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        map[x, y]++;
                    }
                }

                var flashed = new HashSet<Vector>();
                int flashCount = Flash(map, flashed);
                totalFlashCount += flashCount;
                if (flashCount == map.Width * map.Height && firstCompleteFlash < 0)
                {
                    firstCompleteFlash = tick;
                }

                foreach (Vector squid in flashed)
                {
                    map[squid] = 0;
                }
            }

            Console.WriteLine(
                $"After {simulationLength} ticks, there have been {totalFlashCount} flashes,");
            if (firstCompleteFlash < 0)
            {
                Console.WriteLine("and no \"blackouts.\"");
            }
            else
            {
                Console.WriteLine($"and a \"blackout\" on tick {firstCompleteFlash + 1}.");
            }
        }
        private static int Flash(Map map, HashSet<Vector> flashed, IEnumerable<Vector>? selection = null)
        {
            IEnumerable<Vector>? squidSelection = selection;
            if (squidSelection == null)
            {
                var squidSet = new HashSet<Vector>();
                for (int x = 0; x < map.Width; x++)
                {
                    for (int y = 0; y < map.Height; y++)
                    {
                        squidSet.Add((x, y));
                    }
                }
                squidSelection = squidSet;
            }

            int flashCount = 0;
            foreach (Vector squid in squidSelection)
            {
                if (map[squid] <= 9 || flashed.Contains(squid))
                {
                    continue;
                }
                flashed.Add(squid);

                var newSelection = new HashSet<Vector>();
                foreach (Vector offset in mAdjacentOffsets)
                {
                    Vector toFlash = squid + offset;
                    if (map.IsOutOfBounds(toFlash))
                    {
                        continue;
                    }

                    map[toFlash]++;
                    newSelection.Add(toFlash);
                }
                flashCount += Flash(map, flashed, newSelection) + 1;
            }
            return flashCount;
        }
        private static Map ReadInput(string input)
        {
            var lines = input.SplitLines();
            int width = lines[0].Length;
            int height = lines.Count;
            var map = new Map(width, height);

            for (int y = 0; y < lines.Count; y++)
            {
                string line = lines[y];
                for (int x = 0; x < line.Length; x++)
                {
                    char digit = line[x];
                    int energyLevel = int.Parse($"{digit}");
                    map[x, y] = energyLevel;
                }
            }

            return map;
        }
    }
}