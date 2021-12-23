using System;

namespace AdventOfCode.Days.Year2021
{
    [Day(2, "Dive!")]
    public sealed class Day2 : IDay
    {
        public void Run(string input)
        {
            int horizontalPosition = 0;
            int depth = 0;
            int aim = 0;

            int part = Utilities.RequestInput("Select a part (default=2)", 2);
            if (part < 1 || part > 2)
            {
                throw new ArgumentException("The requested part must be either 1 or 2!");
            }

            var commands = input.Split('\r', '\n');
            foreach (string command in commands)
            {
                if (command.Length == 0)
                {
                    continue;
                }

                string[] tokens = command.Split(' ');
                if (tokens.Length < 2)
                {
                    throw new ArgumentException("A command must have at least 2 tokens: action and value.");
                }

                int value = int.Parse(tokens[1]);
                switch (tokens[0])
                {
                    case "forward":
                        horizontalPosition += value;
                        if (part == 2)
                        {
                            depth += aim * value;
                        }
                        break;
                    case "up":
                        if (part == 1)
                        {
                            depth -= value;
                        }
                        else
                        {
                            aim -= value;
                        }
                        break;
                    case "down":
                        if (part == 1)
                        {
                            depth += value;
                        }
                        else
                        {
                            aim += value;
                        }
                        break;
                    default:
                        throw new ArgumentException("The specified command must be either forward, up, or down!");
                }
            }

            Console.WriteLine($"Final horizontal position: {horizontalPosition}");
            Console.WriteLine($"Final depth: {depth}");
            Console.WriteLine($"Product of the two: {horizontalPosition * depth}");
        }
    }
}
