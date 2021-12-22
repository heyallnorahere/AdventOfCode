using System;
using System.Collections.Generic;
using System.IO;
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
        private static Dictionary<int, Dictionary<int, DayInfo>> GetDays()
        {
            var days = new Dictionary<int, Dictionary<int, DayInfo>>();

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

                    int year = attribute.Year;
                    if (year < 0)
                    {
                        string? namespaceName = type.Namespace;
                        if (namespaceName != null)
                        {
                            const string yearDeclaration = "Year";
                            if (namespaceName.Contains(yearDeclaration))
                            {
                                int yearStart = namespaceName.IndexOf(yearDeclaration) + yearDeclaration.Length;
                                if (yearStart >= 0)
                                {
                                    int yearEnd = namespaceName.IndexOf('.', yearStart);
                                    if (yearEnd < 0)
                                    {
                                        yearEnd = namespaceName.Length;
                                    }

                                    string yearString = namespaceName.Substring(yearStart, yearEnd - yearStart);
                                    if (int.TryParse(yearString, out int parsedYear))
                                    {
                                        year = parsedYear;
                                    }
                                }
                            }
                        }
                    }
                    if (year < 0)
                    {
                        throw new ArgumentException("Could not determine year!");
                    }

                    if (!days.ContainsKey(year))
                    {
                        days.Add(year, new Dictionary<int, DayInfo>());
                    }
                    days[year].Add(attribute.DayNumber, dayInfo);
                }
            }

            return days;
        }
        private static int RequestInput(string prompt)
        {
            Console.Write($"{prompt}: ");

            // verify that the user did not immediately hit enter
            string? inputString = Console.ReadLine();
            if (inputString == null || inputString.Length == 0)
            {
                throw new ArgumentException("Cannot operate on no input!");
            }

            return int.Parse(inputString);
        }
        private static void Run(IDay instance, int year, int dayNumber)
        {
            // read input
            string path = $"input/{year}/{dayNumber}.{instance.InputExtension}";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not open input file: {path}");
            }
            var stream = new FileStream(path, FileMode.Open);
            var reader = new StreamReader(stream);
            string input = reader.ReadToEnd();

            // clear the console
            Console.Clear();

            // run code
            instance.Run(input);
        }
        public static void Main(string[] args)
        {
            // get days from the current assembly
            var days = GetDays();
            if (days.Count == 0)
            {
                throw new Exception("Could not find any code days!");
            }

            // ask for year
            Console.WriteLine("Years available:");
            foreach (int year in days.Keys)
            {
                Console.WriteLine($"\t{year}");
            }
            int selectedYear = RequestInput("Please select a year");

            // verify that the selected year exists
            if (!days.ContainsKey(selectedYear))
            {
                throw new ArgumentException("The selected year does not exist!");
            }

            // ask for day
            Console.WriteLine("Days available:");
            foreach (int dayNumber in days[selectedYear].Keys)
            {
                DayInfo dayInfo = days[selectedYear][dayNumber];
                Console.WriteLine($"\t{dayNumber}: {dayInfo.Name}");
            }
            int selectedDay = RequestInput("Please select a day");


            // verify that the requested day exists
            if (!days[selectedYear].ContainsKey(selectedDay))
            {
                throw new ArgumentException("The selected day does not exist!");
            }

            // finally, run
            IDay instance = days[selectedYear][selectedDay].Instance;
            Run(instance, selectedYear, selectedDay);
        }
    }
}