using System;

namespace AdventOfCode.Days.Year2021
{
    [Day(1, "Sonar Sweep")]
    public sealed class Day1 : IDay
    {
        public void Run(string input)
        {
            var lines = input.SplitLines();
            var inputData = lines.ConvertAll(l => int.Parse(l));

            int previousDepthMeasurement = 0;
            int depthIncreaseCount = 0;

            int previousSlidingWindowSum = 0;
            int slidingWindowSumIncreaseCount = 0;
            for (int i = 0; i < inputData.Count; i++)
            {

                // depth measurement
                int depthMeasurement = inputData[i];
                if (previousDepthMeasurement > 0 && depthMeasurement > previousDepthMeasurement)
                {
                    depthIncreaseCount++;
                }
                previousDepthMeasurement = depthMeasurement;

                // sliding window measurement
                if (i >= 2)
                {
                    int slidingWindowSum = 0;
                    for (int j = 0; j < 3; j++)
                    {
                        slidingWindowSum += inputData[i - j];
                    }

                    if (previousSlidingWindowSum > 0 && slidingWindowSum > previousSlidingWindowSum)
                    {
                        slidingWindowSumIncreaseCount++;
                    }
                    previousSlidingWindowSum = slidingWindowSum;
                }
            }

            Console.WriteLine($"The depth measurement increased {depthIncreaseCount} times.");
            Console.WriteLine($"The sliding window sum increased {slidingWindowSumIncreaseCount} times.");
        }
    }
}
