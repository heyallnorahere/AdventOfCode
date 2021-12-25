using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(5, "Hydrothermal venture")]
    public sealed class Day5 : IDay
    {
        private struct Line
        {
            public Vector Point0;
            public Vector Point1;
            public bool IsVertical => Point0.X == Point1.X;
            public bool IsHorizontal => Point0.Y == Point1.Y;
            public bool IsPerfectlyDiagonal => Math.Abs(Point0.X - Point1.X) == Math.Abs(Point0.Y - Point1.Y);
        }
        public void Run(string input)
        {
            var inputData = input.SplitLines();
            var lines = new List<Line>();
            foreach (string line in inputData)
            {
                var points = new List<Vector>();
                foreach (string pointData in line.Split(" -> "))
                {
                    var coordinates = pointData.Split(',');
                    if (coordinates.Length != 2)
                    {
                        throw new ArgumentException("Each point must have exactly two coordinates!");
                    }

                    points.Add(new Vector
                    {
                        X = int.Parse(coordinates[0]),
                        Y = int.Parse(coordinates[1])
                    });
                }

                if (points.Count != 2)
                {
                    throw new ArgumentException("Each line must have exactly 2 points!");
                }
                lines.Add(new Line
                {
                    Point0 = points[0],
                    Point1 = points[1],
                });
            }

            var lineOverlaps = new Dictionary<Vector, int>();
            Action<Vector> hitTile = pos =>
            {
                if (!lineOverlaps.ContainsKey(pos))
                {
                    lineOverlaps.Add(pos, 0);
                }
                lineOverlaps[pos]++;
            };

            foreach (var line in lines)
            {
                if (line.IsHorizontal)
                {
                    int startingPoint, endingPoint;
                    if (line.Point1.X > line.Point0.X)
                    {
                        startingPoint = line.Point0.X;
                        endingPoint = line.Point1.X;
                    }
                    else
                    {
                        startingPoint = line.Point1.X;
                        endingPoint = line.Point0.X;
                    }
                    for (int x = startingPoint; x <= endingPoint; x++)
                    {
                        hitTile((x, line.Point0.Y));
                    }
                }

                if (line.IsVertical)
                {
                    int startingPoint, endingPoint;
                    if (line.Point1.Y > line.Point0.Y)
                    {
                        startingPoint = line.Point0.Y;
                        endingPoint = line.Point1.Y;
                    }
                    else
                    {
                        startingPoint = line.Point1.Y;
                        endingPoint = line.Point0.Y;
                    }
                    for (int y = startingPoint; y <= endingPoint; y++)
                    {
                        hitTile((line.Point0.X, y));
                    }
                }

                if (line.IsPerfectlyDiagonal)
                {
                    Vector difference = line.Point1 - line.Point0;
                    int xSign = difference.X / Math.Abs(difference.X);
                    int ySign = difference.Y / Math.Abs(difference.Y);

                    for (int i = 0; i <= Math.Abs(line.Point0.X - line.Point1.X); i++)
                    {
                        hitTile((new Vector(xSign, ySign) * i) + line.Point0);
                    }
                }
            }

            int overlappedPointCount = 0;
            foreach (int overlapCount in lineOverlaps.Values)
            {
                if (overlapCount > 1)
                {
                    overlappedPointCount++;
                }
            }
            Console.WriteLine($"Lines overlap at {overlappedPointCount} tiles.");
        }
    }
}