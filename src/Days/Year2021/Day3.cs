using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(3, "Binary Diagnostic")]
    public sealed class Day3 : IDay
    {
        public void Run(string input)
        {
            var numbers = new List<int>();
            int bitCount = -1;

            var rawData = input.SplitLines();
            foreach (string line in rawData)
            {
                string binaryString;
                if (line[^1] != '\r')
                {
                    binaryString = line;
                }
                else
                {
                    binaryString = line[..^1];
                }

                if (bitCount < 0)
                {
                    bitCount = binaryString.Length;
                }
                else if (binaryString.Length != bitCount)
                {
                    throw new ArgumentException("The given numbers do not have a uniform bit count!");
                }

                int number = 0;
                for (int i = 0; i < bitCount; i++)
                {
                    int bit = int.Parse(binaryString[^(i + 1)].ToString());
                    if (bit < 0 || bit > 1)
                    {
                        throw new ArgumentException("Each bit must be either 0 or 1!");
                    }
                    number |= bit << i;
                }
                numbers.Add(number);
            }

            int gammaRate = 0;
            int epsilonRate = 0;

            // first pass goes right to left
            for (int i = 0; i < bitCount; i++)
            {
                int commonBit = GetMostCommonBit(numbers, i);

                int gammaBit = commonBit < 0 ? 1 : commonBit; // favor 1 over 0
                int epsilonBit = ~gammaBit & 0x1;

                gammaRate |= gammaBit << i;
                epsilonRate |= epsilonBit << i;
            }

            var remainingOxygenGeneratorRatings = new List<int>(numbers);
            var remainingCO2ScrubberRatings = new List<int>(numbers);

            // second pass goes left to right
            for (int i = bitCount - 1; i >= 0; i--)
            {
                int mask = 0x1 << i;

                bool oxygenGeneratorRatingFound = remainingOxygenGeneratorRatings.Count == 1;
                if (!oxygenGeneratorRatingFound)
                {
                    int commonBit = GetMostCommonBit(remainingOxygenGeneratorRatings, i);
                    int filter = (commonBit < 0 ? 1 : commonBit) << i;
                    remainingOxygenGeneratorRatings.RemoveAll(x => (x & mask) == filter);
                }

                bool co2ScrubberRatingFound = remainingCO2ScrubberRatings.Count == 1;
                if (!co2ScrubberRatingFound)
                {
                    int leastCommonBit = GetLeastCommonBit(remainingCO2ScrubberRatings, i);
                    int filter = (leastCommonBit < 0 ? 0 : leastCommonBit) << i;
                    remainingCO2ScrubberRatings.RemoveAll(x => (x & mask) == filter);
                }

                if (oxygenGeneratorRatingFound && co2ScrubberRatingFound)
                {
                    break;
                }
            }
            if (remainingOxygenGeneratorRatings.Count != 1)
            {
                throw new ArgumentException("Could not isolate an oxygen generator rating!");
            }
            if (remainingCO2ScrubberRatings.Count != 1)
            {
                throw new ArgumentException("Could not isolate a CO2 scrubber rating!");
            }

            Console.WriteLine($"Gamma rate: {gammaRate}");
            Console.WriteLine($"Epsilon rate: {epsilonRate}");
            Console.WriteLine($"Submarine power consumption: {gammaRate * epsilonRate}");

            int oxygenGeneratorRating = remainingOxygenGeneratorRatings[0];
            int co2ScrubberRating = remainingCO2ScrubberRatings[0];
            Console.WriteLine($"Oxygen generator rating: {oxygenGeneratorRating}");
            Console.WriteLine($"Carbon dioxide scrubber rating: {co2ScrubberRating}");
            Console.WriteLine($"Submarine life support rating: {oxygenGeneratorRating * co2ScrubberRating}");
        }
        private static int[] GetBitFrequencies(List<int> numbers, int currentBit)
        {
            var frequencies = new int[] { 0, 0 };
            foreach (int number in numbers)
            {
                int bit = (number >> currentBit) & 0x1;
                frequencies[bit]++;
            }
            return frequencies;
        }
        private static int GetMostCommonBit(List<int> numbers, int currentBit)
        {
            var frequencies = GetBitFrequencies(numbers, currentBit);

            int commonBit = -1;
            for (int i = 0; i < 2; i++)
            {
                if (frequencies[i] > frequencies[~i & 0x1])
                {
                    commonBit = i;
                }
            }
            return commonBit;
        }
        private static int GetLeastCommonBit(List<int> numbers, int currentBit)
        {
            var frequencies = GetBitFrequencies(numbers, currentBit);

            int leastCommonBit = -1;
            for (int i = 0; i < 2; i++)
            {
                if (frequencies[i] < frequencies[~i & 0x1])
                {
                    leastCommonBit = i;
                }
            }
            return leastCommonBit;
        }
    }
}
