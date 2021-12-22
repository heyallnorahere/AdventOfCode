using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DayAttribute : Attribute
    {
        public DayAttribute(int dayNumber, string name)
        {
            DayNumber = dayNumber;
            Name = name;
        }

        public int DayNumber { get; }
        public string Name { get; }
    }

    public interface IDay
    {
        void Run();
    }
}
