using System;
using System.Collections.Generic;
using System.Reflection;

namespace AdventOfCode
{
    public static class Program
    {
        private struct DayInfo
        {
            public string Name { get; set; }
            public IDay Instance { get; set; }
        };
        private static bool IsDayDerivative(Type type)
        {
            Type[] interfaces = type.GetInterfaces();

            foreach (Type interfaceType in interfaces)
            {
                if (interfaceType == typeof(IDay))
                {
                    return true;
                }
            }

            return false;
        }
        private static Dictionary<int, DayInfo> GetDays()
        {
            var days = new Dictionary<int, DayInfo>();

            var assembly = typeof(Program).Assembly;
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                var attribute = type.GetCustomAttribute<DayAttribute>();
                if (attribute != null)
                {
                    if (days.ContainsKey(attribute.DayNumber))
                    {
                        throw new ArgumentException($"Day {attribute.DayNumber} already exists!");
                    }

                    if (!IsDayDerivative(type))
                    {
                        throw new Exception($"Type {type.FullName} does not extend {typeof(IDay).FullName}!");
                    }

                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor == null)
                    {
                        throw new Exception($"Could not find a constructor for {type.FullName} that does not have any parameters!");
                    }

                    IDay instance = (IDay)constructor.Invoke(null);
                    var dayInfo = new DayInfo
                    {
                        Name = attribute.Name,
                        Instance = instance
                    };
                    days.Add(attribute.DayNumber, dayInfo);
                }
            }

            return days;
        }
        public static void Main(string[] args)
        {
            // get days from the current assembly
            var days = GetDays();

            if (days.Count == 0)
            {
                throw new Exception("Could not find any code days!");
            }

            // ask for user input
            Console.WriteLine("Pick a day to run:");
            foreach (int dayNumber in days.Keys)
            {
                DayInfo dayInfo = days[dayNumber];
                Console.WriteLine($"\t{dayNumber}: {dayInfo.Name}");
            }
            Console.Write("Please insert a number corresponding to the day: ");

            // verify that the user did not immediately hit enter
            string? input = Console.ReadLine();
            if (input == null || input.Length == 0)
            {
                throw new ArgumentException("Cannot operate on no input!");
            }

            // parse the input and verify that the requested day exists
            int selectedDay = int.Parse(input);
            if (!days.ContainsKey(selectedDay))
            {
                throw new ArgumentException("The selected day does not exist!");
            }

            // finally, clear the console and run
            Console.Clear();
            days[selectedDay].Instance.Run();
        }
    }
}