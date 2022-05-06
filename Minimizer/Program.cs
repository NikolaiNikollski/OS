using Minimizer.Machines;
using System;
using System.IO;

namespace Minimizer
{
    class Program
    {
        const string Path = "ml2.txt";
        static void Main(string[] args)
        {
            Machine machine = LoadMachine();
            machine.PrintInitial();
            machine.Minimize();
            machine.PrintMinimized();
        }

        static private Machine LoadMachine()
        {
            StreamReader sr = new StreamReader(Path);
            string machineType = sr.ReadLine().Trim();
            switch (machineType)
            {
                case "ml":
                    return new MiliMachine(ref sr);

                case "mr":
                    return new MooreMachine(ref sr);

                default:
                    throw new Exception("Incorrect automat name in first line");
            }
        }
    }
}
