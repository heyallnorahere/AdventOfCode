using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(8, "Seven Segment Search")]
    public sealed class Day8 : IDay
    {
        private static Dictionary<int, int> mSegmentCountMap;
        static Day8()
        {
            mSegmentCountMap = new Dictionary<int, int>
            {
                [2] = 1,
                [4] = 4,
                [3] = 7,
                [7] = 8,
            };
        }
        public void Run(string input)
        {
            int simpleDigitCount = 0;
            int outputValueSum = 0;

            var lines = input.SplitLines();
            foreach (string line in lines)
            {
                string[] sections = line.Split('|');
                if (sections.Length != 2)
                {
                    throw new ArgumentException("Malformed input!");
                }

                var digits = new List<string>();

                var internalDigits = new List<string>(sections[0].Split(' '));
                internalDigits.ForEach(digit =>
                {
                    if (digit.Length > 0)
                    {
                        digits.Add(digit);
                    }
                });

                var outputDigits = new List<string>(sections[1].Split(' '));
                outputDigits.ForEach(digit =>
                {
                    if (digit.Length > 0)
                    {
                        digits.Add(digit);
                    }
                });

                var digitSegmentMap = new Dictionary<int, string>();
                foreach (string digitData in digits)
                {
                    if (digitData.Length == 0)
                    {
                        continue;
                    }

                    if (!mSegmentCountMap.ContainsKey(digitData.Length))
                    {
                        continue;
                    }

                    int digit = mSegmentCountMap[digitData.Length];
                    if (digitSegmentMap.ContainsKey(digit))
                    {
                        continue;
                    }

                    digitSegmentMap.Add(digit, digitData);
                }

                var segmentMap = CreateSegmentMap(digitSegmentMap, digits);
                int outputValue = 0;
                for (int i = 0; i < outputDigits.Count; i++)
                {
                    string outputDigit = outputDigits[i];
                    if (outputDigit.Length == 0)
                    {
                        continue;
                    }

                    if (mSegmentCountMap.ContainsKey(outputDigit.Length))
                    {
                        simpleDigitCount++;
                    }

                    int digitValue = GetDigit(outputDigit, segmentMap);
                    outputValue += digitValue * (int)Math.Pow(10, outputDigits.Count - (i + 1));
                }
                outputValueSum += outputValue;
            }

            Console.WriteLine($"There are {simpleDigitCount} appearances of 1, 4, 7, or 8 in the output values.");
            Console.WriteLine($"The total sum of all output values is {outputValueSum}.");
        }
        private const string fullDisplay = "abcdefg";
        private static Dictionary<char, char> CreateSegmentMap(Dictionary<int, string> digitSegmentMap, List<string> digits)
        {
            var possibilityMap = new Dictionary<char, string>();
            foreach (char segment in fullDisplay)
            {
                possibilityMap.Add(segment, fullDisplay);
            }

            // isolate C and F
            string oneSegments = digitSegmentMap[1];
            Isolate("cf", oneSegments, possibilityMap);

            // isolate B and D
            string fourSegments = digitSegmentMap[4];
            string segmentBD = Eliminate(fourSegments, oneSegments);
            Isolate("bd", segmentBD, possibilityMap);

            // isolate A
            string sevenSegments = digitSegmentMap[7];
            string segmentA = Eliminate(sevenSegments, oneSegments);
            Isolate("a", segmentA, possibilityMap);

            // find groups of digits with segment counts of 5 and 6
            var group235 = new HashSet<string>();
            var group069 = new HashSet<string>();
            foreach (string digit in digits)
            {
                switch (digit.Length)
                {
                case 5:
                    group235.Add(digit);
                    break;
                case 6:
                    group069.Add(digit);
                    break;
                }
            }

            // identify common segments among 2, 3, and 5
            var commonSegments = new List<char>(fullDisplay);
            foreach (string fiveSegmentDigit in group235)
            {
                var segments = new List<char>(fiveSegmentDigit);
                commonSegments.RemoveAll(s => !segments.Contains(s));
            }

            // isolate D, which also isolates B
            foreach (char fourSegment in fourSegments)
            {
                if (commonSegments.Contains(fourSegment))
                {
                    Isolate("d", $"{fourSegment}", possibilityMap);
                    break;
                }
            }

            // isolate G
            string commonSegmentString = new string(commonSegments.ToArray());
            string segmentD = possibilityMap['d'];
            string segmentG = Eliminate(commonSegmentString, segmentA + segmentD);
            Isolate("g", segmentG, possibilityMap);

            // isolate E
            string segmentB = possibilityMap['b'];
            string requiredZeroSegments = segmentA + segmentB + oneSegments + segmentG;
            var zero = FindDigit(group069, requiredZeroSegments, segmentD);
            if (zero == null)
            {
                throw new ArgumentException("0 was not included in the provided digits!");
            }
            string segmentE = Eliminate(zero, requiredZeroSegments);
            Isolate("e", segmentE, possibilityMap);

            // isolate F, also isolating C
            string requiredSixSegments = segmentA + segmentBD + segmentE + segmentG;
            var six = FindDigit(group069, requiredSixSegments, string.Empty);
            if (six == null)
            {
                throw new ArgumentException("6 was not included in the provided digits!");
            }
            string segmentF = Eliminate(six, requiredSixSegments);
            Isolate("f", segmentF, possibilityMap);

            var segmentMap = new Dictionary<char, char>();
            foreach (char visible in possibilityMap.Keys)
            {
                segmentMap.Add(possibilityMap[visible][0], visible);
            }
            return segmentMap;
        }
        private static string? FindDigit(IEnumerable<string> possibilities, string requiredSegments, string forbiddenSegments)
        {
            foreach (string digit in possibilities)
            {
                bool isCandidate = true;
                var segmentList = new List<char>(digit);
                foreach (char requiredSegment in requiredSegments)
                {
                    if (!segmentList.Contains(requiredSegment))
                    {
                        isCandidate = false;
                        break;
                    }
                }
                foreach (char forbiddenSegment in forbiddenSegments)
                {
                    if (segmentList.Contains(forbiddenSegment))
                    {
                        isCandidate = false;
                        break;
                    }
                }
                if (isCandidate)
                {
                    return digit;
                }
            }
            return null;
        }
        private static void Isolate(string segments, string possibilties, Dictionary<char, string> map)
        {
            var segmentList = new List<char>(segments);
            foreach (char segment in map.Keys)
            {
                if (segmentList.Contains(segment))
                {
                    string removalMask = Eliminate(fullDisplay, possibilties);
                    map[segment] = Eliminate(map[segment], removalMask);
                }
                else
                {
                    map[segment] = Eliminate(map[segment], possibilties);
                }
            }
        }
        private static string Eliminate(string possibilities, string segments)
        {
            var remainingPossibilities = new List<char>(possibilities);

            var segmentList = new List<char>(segments);
            int removedCount = remainingPossibilities.RemoveAll(s => segmentList.Contains(s));
            return new string(remainingPossibilities.ToArray());
        }
        private static int GetDigit(string digitData, Dictionary<char, char> segmentMap)
        {
            var translatedSegments = new SortedSet<char>();
            foreach (char segment in digitData)
            {
                translatedSegments.Add(segmentMap[segment]);
            }

            var segments = new char[digitData.Length];
            translatedSegments.CopyTo(segments);
            var translatedData = new string(segments);

            return translatedData switch
            {
                "abcefg" => 0,
                "cf" => 1,
                "acdeg" => 2,
                "acdfg" => 3,
                "bcdf" => 4,
                "abdfg" => 5,
                "abdefg" => 6,
                "acf" => 7,
                "abcdefg" => 8,
                "abcdfg" => 9,
                _ => -1,
            };
        }
    }
}