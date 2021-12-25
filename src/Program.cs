using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
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
        private static IReadOnlyDictionary<int, IReadOnlyDictionary<int, DayInfo>> GetDays()
        {
            var days = new Dictionary<int, IReadOnlyDictionary<int, DayInfo>>();

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
                        days.Add(year, new SortedDictionary<int, DayInfo>());
                    }
                    var dayMap = (IDictionary<int, DayInfo>)days[year];
                    dayMap.Add(attribute.DayNumber, dayInfo);
                }
            }

            return days;
        }
        private static void Run(IDay instance, int selectedYear, int selectedDay)
        {
            // read input
            string path = $"input/{selectedYear}/{selectedDay}.txt";
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not open input file: {path}");
            }
            var stream = new FileStream(path, FileMode.Open);
            var reader = new StreamReader(stream);
            string input = reader.ReadToEnd();

            // print original challenge url and run
            Console.WriteLine($"Original challenge: https://adventofcode.com/{selectedYear}/day/{selectedDay}\n");
            instance.Run(input);
        }
        private static void Menu(int y, int d)
        {
            if (d >= 0 && y < 0)
            {
                throw new ArgumentException("If -d is specified, -y must also be present!");
            }

            // get days from the current assembly
            var days = GetDays();
            if (days.Count == 0)
            {
                throw new Exception("Could not find any code days!");
            }

            // if no year was specified on the command line, ask for year
            int selectedYear = y;
            if (selectedYear < 0)
            {
                Console.WriteLine("Years available:");
                foreach (int year in days.Keys)
                {
                    Console.WriteLine($"\t{year}");
                }
                selectedYear = Utilities.RequestInput("Please select a year");
                Console.Write('\n');
            }

            // verify that the selected year exists
            if (!days.ContainsKey(selectedYear))
            {
                throw new ArgumentException("The selected year does not exist!");
            }

            // if no day was specified on the command line, ask for day
            int selectedDay = d;
            if (selectedDay < 0)
            {
                Console.WriteLine("Days available:");
                foreach (int dayNumber in days[selectedYear].Keys)
                {
                    DayInfo dayInfo = days[selectedYear][dayNumber];
                    Console.WriteLine($"\t{dayNumber}: {dayInfo.Name}");
                }
                selectedDay = Utilities.RequestInput("Please select a day");
                Console.Write('\n');
            }

            // verify that the requested day exists
            if (!days[selectedYear].ContainsKey(selectedDay))
            {
                throw new ArgumentException("The selected day does not exist!");
            }

            // finally, run
            IDay instance = days[selectedYear][selectedDay].Instance;
            Run(instance, selectedYear, selectedDay);
        }
        public static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<int>(
                    new string[] { "-y", "--year" },
                    getDefaultValue: () => -1,
                    description: "Specify a year to automatically select."),

                new Option<int>(
                    new string[] { "-d", "--day" },
                    getDefaultValue: () => -1,
                    description: "Specify a day to automatically select. Must only be used with -y.")
            };
            rootCommand.Description = "Run a solution to the Advent of Code.";
            rootCommand.Handler = CommandHandler.Create<int, int>(Menu);
            return rootCommand.InvokeAsync(args).Result;
        }
    }
}