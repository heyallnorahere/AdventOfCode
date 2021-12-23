using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(7, "The Treachery of Whales")]
    public sealed class Day7 : IDay
    {
        public void Run(string input)
        {
            var positions = new List<int>();
            int closestPosition = -1;
            int farthestPosition = -1;
            foreach (string positionString in input.Split(','))
            {
                if (positionString.Length == 0)
                {
                    continue;
                }

                int position = int.Parse(positionString);
                if (position < closestPosition || closestPosition < 0)
                {
                    closestPosition = position;
                }
                if (position > farthestPosition || farthestPosition < 0)
                {
                    farthestPosition = position;
                }
                positions.Add(position);
            }
            if (positions.Count == 0)
            {
                throw new ArgumentException("Why are there no crab submarines?! Crab submarines are awesome!");
            }

            var newPositions = new SortedDictionary<ulong, List<int>>();
            for (int newPosition = closestPosition; newPosition <= farthestPosition; newPosition++)
            {
                ulong fuelCost = 0;
                foreach (int submarinePosition in positions)
                {
                    // slow but i couldnt get anything else to work
                    int distance = Math.Abs(submarinePosition - newPosition);
                    ulong submarineFuelCost = 0;
                    ulong fuelCostPerPositionMoved = 1;
                    for (int i = 0; i < distance; i++)
                    {
                        submarineFuelCost += fuelCostPerPositionMoved;
                        fuelCostPerPositionMoved++;
                    }
                    fuelCost += submarineFuelCost;
                }
                if (!newPositions.ContainsKey(fuelCost))
                {
                    newPositions.Add(fuelCost, new List<int>());
                }
                newPositions[fuelCost].Add(newPosition);
            }

            // hacky hacky hack hack
            var keys = newPositions.Keys;
            using var keyEnumerator = keys.GetEnumerator();
            keyEnumerator.MoveNext();
            ulong leastFuelCost = keyEnumerator.Current;

            var optimalPositions = newPositions[leastFuelCost];
            string positionsString = string.Empty;
            foreach (int position in optimalPositions)
            {
                if (positionsString.Length > 0)
                {
                    positionsString += ", ";
                }
                positionsString += position;
            }
            Console.WriteLine($"Positions [ {positionsString} ] cost {leastFuelCost} fuel.");
        }
    }
}