using System;
using System.IO;

namespace Determinator
{
    class Program
    {
        const string Path = "R1.txt";
        static void Main(string[] args)
        {
            StreamReader sr = new StreamReader(Path);
            Machine machine = new Machine(ref sr);
            machine.PrintInitial();
            machine.Determinate();
            machine.PrintFinal();
            sr.Close();
        }
    }
}
