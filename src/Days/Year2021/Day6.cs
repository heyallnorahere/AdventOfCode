using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(6, "Lanternfish")]
    public sealed class Day6 : IDay
    {
        public const int TimerValueOnReset = 6;
        public const int BirthTimerValue = 8;
        public void Run(string input)
        {
            // when in doubt, throw a hashmap at the problem
            var lanternfish = new Dictionary<int, ulong>();
            ulong initialFishCount = 0;
            foreach (string timer in input.Split(','))
            {
                if (timer.Length == 0)
                {
                    continue;
                }

                int timerValue = int.Parse(timer);
                if (!lanternfish.ContainsKey(timerValue))
                {
                    lanternfish.Add(timerValue, 0);
                }
                lanternfish[timerValue]++;
                initialFishCount++;
            }
            if (initialFishCount == 0)
            {
                throw new ArgumentException("Please describe 1 or more lanternfish.");
            }

            Console.WriteLine($"Starting with {initialFishCount} lanternfish.");
            int simulationLength = Utilities.RequestInput("Please input how many days the simulation should run for");
            if (simulationLength <= 0)
            {
                throw new ArgumentException("Must input a positive integer!");
            }

            for (int day = 0; day < simulationLength; day++)
            {
                ulong birthingCount = 0;
                if (lanternfish.ContainsKey(0))
                {
                    birthingCount = lanternfish[0];
                }

                for (int tickCount = 1; tickCount <= BirthTimerValue; tickCount++)
                {
                    ulong fishCount = 0;
                    if (lanternfish.ContainsKey(tickCount))
                    {
                        fishCount = lanternfish[tickCount];
                    }

                    int newTickCount = tickCount - 1;
                    if (lanternfish.ContainsKey(newTickCount))
                    {
                        lanternfish[newTickCount] = fishCount;
                    }
                    else
                    {
                        lanternfish.Add(newTickCount, fishCount);
                    }
                }

                if (lanternfish.ContainsKey(BirthTimerValue))
                {
                    lanternfish[BirthTimerValue] = birthingCount;
                }
                else
                {
                    lanternfish.Add(BirthTimerValue, birthingCount);
                }

                if (lanternfish.ContainsKey(TimerValueOnReset))
                {
                    lanternfish[TimerValueOnReset] += birthingCount;
                }
                else
                {
                    lanternfish.Add(TimerValueOnReset, birthingCount);
                }
            }

            ulong finalFishCount = 0;
            foreach (ulong fishCount in lanternfish.Values)
            {
                finalFishCount += fishCount;
            }
            Console.WriteLine($"After {simulationLength}, there are {finalFishCount} lanternfish.");
        }
    }
}