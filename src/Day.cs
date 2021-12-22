using System;

namespace AdventOfCode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DayAttribute : Attribute
    {
        public DayAttribute(int dayNumber, string name)
        {
            DayNumber = dayNumber;
            Name = name;
        }

        public int DayNumber { get; }
        public int Year { get; set; } = -1;
        public string Name { get; }
    }

    public interface IDay
    {
        void Run(string input);
    }
}
