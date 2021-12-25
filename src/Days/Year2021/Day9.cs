using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(9, "Smoke Basin")]
    public sealed class Day9 : IDay
    {
        private class Map
        {
            public Map(int width, int height)
            {
                Width = width;
                Height = height;
                mData = new int[width, height];
            }
            public int this[int x, int y]
            {
                get => mData[x, y];
                set => mData[x, y] = value;
            }
            public int this[Vector position]
            {
                get => mData[position.X, position.Y];
                set => mData[position.X, position.Y] = value;
            }
            public int Width { get; }
            public int Height { get; }
            public bool IsOutOfBounds(Vector position)
            {
                return position.X < 0 || position.X >= Width ||
                    position.Y < 0 || position.Y >= Height;
            }
            private readonly int[,] mData;
        }
        static Day9()
        {
            mAdjacentOffsets = new Vector[]
            {
                ( 1,  0),
                (-1,  0),
                ( 0,  1),
                ( 0, -1)
            };
        }
        private static readonly Vector[] mAdjacentOffsets;
        public Day9()
        {
            mScannedPoints = new HashSet<Vector>();
        }
        public void Run(string input)
        {
            // parse input into row form
            var lines = new List<string>(input.Split('\r', '\n'));
            var rows = lines.FindAll(l => l.Length > 0);
            if (rows.Count == 0)
            {
                throw new ArgumentException("No input was given!");
            }

            // create map
            var map = new Map(rows[0].Length, rows.Count);

            // parse rows into map form
            for (int y = 0; y < map.Height; y++)
            {
                string row = rows[y];
                for (int x = 0; x < map.Width; x++)
                {
                    char character = row[x];
                    int positionValue = int.Parse(new string(new char[] { character }));
                    map[x, y] = positionValue;
                }
            }

            // find all low points
            var lowPoints = new List<Vector>();
            for (int y = 0; y < map.Height; y++)
            {
                for (int x = 0; x < map.Width; x++)
                {
                    Vector position = (x, y);
                    int positionValue = map[position];

                    bool lowPoint = true;
                    foreach (var offset in mAdjacentOffsets)
                    {
                        var adjacentPosition = position + offset;
                        if (map.IsOutOfBounds(adjacentPosition))
                        {
                            continue;
                        }

                        if (map[position + offset] <= positionValue)
                        {
                            lowPoint = false;
                            break;
                        }
                    }
                    if (lowPoint)
                    {
                        lowPoints.Add(position);
                    }
                }
            }
            if (lowPoints.Count == 0)
            {
                throw new ArgumentException("Could not find any low points!");
            }

            // iterate through all low points
            int riskLevelSum = 0;
            var basinSizes = new List<ulong>();
            mScannedPoints.Clear();
            foreach (var position in lowPoints)
            {
                riskLevelSum += map[position] + 1;

                ulong basinSize = IterateBasin(position, map);
                if (basinSize > 0)
                {
                    basinSizes.Add(basinSize);
                }
            }

            basinSizes.Sort((a, b) => a > b ? -1 : 1);
            if (basinSizes.Count < 3)
            {
                throw new ArgumentException("Please describe at least 3 basins.");
            }

            ulong basinSizeProduct = 1;
            for (int i = 0; i < 3; i++)
            {
                basinSizeProduct *= basinSizes[i];
            }

            Console.WriteLine($"The sum of all low point risk levels is {riskLevelSum}.");
            Console.WriteLine($"The product of the largest 3 basin sizes is {basinSizeProduct}.");
        }
        private ulong IterateBasin(Vector position, Map map)
        {
            if (mScannedPoints.Contains(position))
            {
                return 0;
            }
            mScannedPoints.Add(position);

            ulong basinSize = 1;
            foreach (var offset in mAdjacentOffsets)
            {
                Vector adjacentPosition = position + offset;
                if (map.IsOutOfBounds(adjacentPosition))
                {
                    continue;
                }

                if (map[adjacentPosition] < 9)
                {
                    ulong size = IterateBasin(adjacentPosition, map);
                    if (size > 0)
                    {
                        basinSize += size;
                    }
                }
            }
            return basinSize;
        }
        private readonly ISet<Vector> mScannedPoints;
    }
}
