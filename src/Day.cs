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

    /// <summary>
    /// An IDay represents a day of code.
    /// </summary>
    public interface IDay
    {
        /// <summary>
        /// Runs the code associated with this day of the advent calendar.
        /// </summary>
        /// <param name="input">The input data.</param>
        void Run(string input);
    }
}
