using System;
using System.Collections.Generic;

namespace AdventOfCode.Days.Year2021
{
    public sealed class BingoBoard
    {
        public const int Size = 5;
        public BingoBoard(int[,] data)
        {
            mData = data;
        }

        public bool Mark(Vector pos)
        {
            if (pos.X >= Size || pos.Y >= Size)
            {
                return false;
            }
            if (mMarked.Add(pos))
            {
                mFinished = null;
                mLastCalled = mData[pos.X, pos.Y];
                return true;
            }
            return false;
        }

        public bool Marked(Vector pos) => mMarked.Contains(pos);

        public Vector? FindNumber(int number)
        {
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    if (mData[x, y] == number)
                    {
                        return (x, y);
                    }
                }
            }
            return null;
        }

        public bool Finished()
        {
            if (mFinished != null)
            {
                return mFinished.Value;
            }
            // apparently diagonals dont count?
            for (int y = 0; y < Size; y++)
            {
                if (Row(y))
                {
                    mFinished = true;
                }
            }

            for (int x = 0; x < Size; x++)
            {
                if (Column(x))
                {
                    mFinished = true;
                }
            }

            if (mFinished == null)
            {
                mFinished = false;
            }
            return mFinished.Value;
        }

        private bool Row(int y)
        {
            for (int x = 0; x < Size; x++)
            {
                if (!mMarked.Contains((x, y)))
                {
                    return false;
                }
            }
            return true;
        }

        private bool Column(int x)
        {
            for (int y = 0; y < Size; y++)
            {
                if (!mMarked.Contains((x, y)))
                {
                    return false;
                }
            }
            return true;
        }

        public int Score
        {
            get
            {
                int unmarkedSum = 0;
                for (int x = 0; x < Size; x++)
                {
                    for (int y = 0; y < Size; y++)
                    {
                        if (!mMarked.Contains((x, y)))
                        {
                            unmarkedSum += mData[x, y];
                        }
                    }
                }
                return unmarkedSum * mLastCalled;
            }
        }

        private int mLastCalled = 0;
        private bool? mFinished = null;
        private readonly int[,] mData;
        private readonly HashSet<Vector> mMarked = new();
    }
    public sealed class BoardFactory
    {
        public IReadOnlyList<BingoBoard> CreatedBoards => mBoards;
        public void Submit(int number)
        {
            if (mCurrentData == null)
            {
                mCurrentData = new int[BingoBoard.Size, BingoBoard.Size];
            }

            mCurrentData[mCurrentPos.X, mCurrentPos.Y] = number;
            mCurrentPos.X++;

            if (mCurrentPos.X >= BingoBoard.Size)
            {
                mCurrentPos.X = 0;
                mCurrentPos.Y++;
            }

            if (mCurrentPos.Y >= BingoBoard.Size)
            {
                mCurrentPos.Y = 0;
                var board = new BingoBoard(mCurrentData);
                mCurrentData = null;
                mBoards.Add(board);
            }
        }
        public bool IncompleteBoard() => mCurrentPos.X > 0 || mCurrentPos.Y > 0;
        private int[,]? mCurrentData = null;
        private Vector mCurrentPos = (0, 0);
        private readonly List<BingoBoard> mBoards = new();
    }
    [Day(4, "Giant Squid")]
    public sealed class Day4 : IDay
    {
        public void Run(string input)
        {
            var lines = input.SplitLines();
            if (lines.Count == 0)
            {
                throw new ArgumentException("Cannot play bingo with no input!");
            }

            var numbersToDraw = new List<int>();
            foreach (string number in lines[0].Split(','))
            {
                numbersToDraw.Add(int.Parse(number));
            }

            var factory = new BoardFactory();
            for (int i = 1; i < lines.Count; i++)
            {
                var sections = lines[i].Split(' ');
                foreach (string section in sections)
                {
                    if (section.Length == 0)
                    {
                        continue;
                    }

                    int number = int.Parse(section);
                    factory.Submit(number);
                }
            }

            if (factory.IncompleteBoard())
            {
                throw new ArgumentException("An incomplete board was described!");
            }
            var boards = new List<BingoBoard>(factory.CreatedBoards);

            var completedBoards = new SortedDictionary<int, BingoBoard>();
            foreach (int toDraw in numbersToDraw)
            {
                foreach (var board in boards)
                {
                    if (board.Finished())
                    {
                        continue;
                    }

                    var position = board.FindNumber(toDraw);
                    if (position == null || board.Marked(position.Value))
                    {
                        continue;
                    }
                    board.Mark(position.Value);
                    if (board.Finished())
                    {
                        completedBoards.Add(completedBoards.Count, board);
                    }
                }
                if (completedBoards.Count == boards.Count)
                {
                    break;
                }
            }

            if (completedBoards.Count > 0)
            {
                Console.WriteLine($"Winning score: {completedBoards[0].Score}");
                Console.WriteLine($"Losing score: {completedBoards[completedBoards.Count - 1].Score}");
            }
            else
            {
                Console.WriteLine("No board won.");
            }
        }
    }
}
