using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    [Day(10, "Syntax Scoring")]
    public sealed class Day10 : IDay
    {
        static Day10()
        {
            mBlockMap = new Dictionary<char, char>
            {
                ['('] = ')',
                ['['] = ']',
                ['{'] = '}',
                ['<'] = '>',
            };
        }
        private static Dictionary<char, char> mBlockMap;
        private struct Block
        {
            public Block(char opening)
            {
                Opening = opening;
                Closing = mBlockMap[Opening];
            }
            public char Opening;
            public char Closing;
        }
        private class Line
        {
            public Line(string data)
            {
                Data = data;
            }
            public string Data { get; }

            public int ErrorScore
            {
                get
                {
                    var result = Parse((b, c) => {
                        if (c != b.Closing)
                        {
                            return c switch
                            {
                                ')' => 3,
                                ']' => 57,
                                '}' => 1197,
                                '>' => 25137,
                                _ => throw new ArgumentException("Invalid closing character!")
                            };
                        }
                        return null;
                    });
                    return result.GetReturnedData<int>() ?? 0;
                }
            }

            public ulong AutoCompleteScore
            {
                get
                {
                    var blockStack = Parse(null).BlockStack;
                    ulong score = 0;
                    while (blockStack.Count > 0)
                    {
                        var block = blockStack.Pop();
                        score *= 5;
                        score += block.Closing switch
                        {
                            ')' => 1,
                            ']' => 2,
                            '}' => 3,
                            '>' => 4,
                            _ => throw new ArgumentException("Invalid closing character!")
                        };
                    }
                    return score;
                }
            }

            private struct ParseResult
            {
                public T? GetReturnedData<T>() where T : struct => (T?)ReturnedCallbackData;
                public object? ReturnedCallbackData;
                public Stack<Block> BlockStack;
            }
            private ParseResult Parse(Func<Block, char, object?>? closeCallback)
            {
                object? returnedData = null;
                var blockStack = new Stack<Block>();
                foreach (char character in Data)
                {
                    if (mBlockMap.ContainsKey(character))
                    {
                        blockStack.Push(new Block(character));
                    }
                    else
                    {
                        var block = blockStack.Pop();
                        if (closeCallback != null && returnedData == null)
                        {
                            object? returned = closeCallback(block, character);
                            if (returned != null)
                            {
                                returnedData = returned;
                            }
                        }
                    }
                }
                return new ParseResult
                {
                    ReturnedCallbackData = returnedData,
                    BlockStack = blockStack
                };
            }
        }
        public void Run(string input)
        {
            var lines = input.SplitLines().ConvertAll(l => new Line(l));

            int totalErrorScore = 0;
            var autoCompleteScores = new List<ulong>();
            foreach (Line line in lines)
            {
                int errorScore = line.ErrorScore;
                if (errorScore > 0)
                {
                    totalErrorScore += errorScore;
                    continue;
                }
                autoCompleteScores.Add(line.AutoCompleteScore);
            }
            autoCompleteScores.Sort();

            ulong medianAutoCompleteScore;
            if (autoCompleteScores.Count % 2 != 0)
            {
                int index = (autoCompleteScores.Count - 1) / 2;
                medianAutoCompleteScore = autoCompleteScores[index];
            }
            else
            {
                throw new ArgumentException("Day 10: " +
                    "\"There will always be an oed number of scores to consider.\"");
            }

            Console.WriteLine($"Total error score: {totalErrorScore}");
            Console.WriteLine($"Median auto complete score: {medianAutoCompleteScore}");
        }
    }
}