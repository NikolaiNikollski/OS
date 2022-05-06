using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Minimizer.Machines
{
    class MooreMachine : Machine
    {
        private struct Transition
        {
            public string State;

            public string Class;
        }
        private struct Class
        {
            public string[] OutSсheme;

            public List<string> States;
        }

        private int StatesNum;
        private int InputSignalsNum;
        private int OutputSignalsNum;

        private string[] States;
        private string[] OutputSignals;
        private string[] StateClasses;
        private Transition[][] Transitions;

        private Class[] FinalClasses;
        private string[] FinalStates;
        private string[] FinalOutputSignals;
        private Transition[][] FinalTransitions;
        public MooreMachine(ref StreamReader sr)
        {
            StatesNum = int.Parse(sr.ReadLine());
            InputSignalsNum = int.Parse(sr.ReadLine());
            OutputSignalsNum = int.Parse(sr.ReadLine());

            States = new Regex(" +").Split(sr.ReadLine());
            OutputSignals = new Regex(" +").Split(sr.ReadLine());
            Transitions = new Transition[InputSignalsNum][];
            for (int i = 0; i < InputSignalsNum; i++)
            {
                Transitions[i] = new Regex(" +").Split(sr.ReadLine()).Select(t =>
                {
                    return new Transition()
                    {
                        State = t
                    };
                }).ToArray();
            }
            StateClasses = new string[StatesNum];
        }

        public void Minimize()
        {
            List<Class> classes = GetInitialClasses();
            int oldClassesCount = 0;
            while (oldClassesCount != classes.Count)
            {
                oldClassesCount = classes.Count();
                Update(classes);
                classes = GetSecondaryClasses();
            }

            FinalStates = classes.Select((c, i) => "s" + i).ToArray();
            FinalTransitions = new Transition[InputSignalsNum][];
            FinalClasses = classes.ToArray();

            for (int i = 0; i < FinalTransitions.Length; i++)
            {
                FinalTransitions[i] = new Transition[classes.Count()];

                for (int j = 0; j < classes.Count(); j++)
                {
                    string originalState = classes[j].States[0];
                    var originalStateIndex = Array.IndexOf(States, originalState);
                    FinalTransitions[i][j] = new Transition()
                    {
                        State = "s" + GetClassIndexByState(Transitions[i][originalStateIndex].State, classes.ToArray()),
                    };
                }
            }

            FinalOutputSignals = new string[classes.Count()];
            for (int i = 0; i < classes.Count(); i++)
            {
                FinalOutputSignals[i] = OutputSignals[GetClassIndexByState(classes[i].States[0], classes.ToArray())];
            }
        }
        private int GetClassIndexByState(string state, Class[] classes)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                if (classes[i].States.Contains(state))
                {
                    return i;
                }
            }

            throw new Exception();
        }

        private List<Class> GetInitialClasses()
        {
            //сформировать классы эквивалентности
            List<Class> classes = new List<Class>();
            //по каждому столбцу
            for (int i = 0; i < States.Length; i++)
            {
                bool stateAddedToClass = false; //столбец добален в сущетсвующий класс

                //поискать существующий класс
                foreach (var eqClass in classes)
                {
                    bool eqScheme = true; //класс подошел
                    if (eqClass.OutSсheme[0] != OutputSignals[i])
                    {
                        eqScheme = false;
                    }

                    if (eqScheme)
                    {
                        eqClass.States.Add(States[i]);
                        stateAddedToClass = true;
                    }
                }

                //если подходящий класс не найден, создаем новый
                if (!stateAddedToClass)
                {
                    string[] scheme = new string[2];
                    scheme[0] = OutputSignals[i];

                    classes.Add(new Class
                    {
                        States = new List<string>() { States[i] },
                        OutSсheme = scheme
                    });
                }
            }

            return classes;
        }
        private List<Class> GetSecondaryClasses()
        {
            List<Class> classes = new List<Class>();

            for (int i = 0; i < States.Length; i++)
            {
                bool stateAddedToClass = false; //столбец добален в сущетсвующий класс

                //поискать существующий класс
                foreach (var eqClass in classes)
                {
                    bool eqScheme = true; //класс подошел
                    for (int j = 0; j < InputSignalsNum; j++)
                    {
                        if (eqClass.OutSсheme[j] != Transitions[j][i].Class)
                        {
                            eqScheme = false;
                        }
                    }

                    if (eqClass.OutSсheme[InputSignalsNum] != StateClasses[i])
                    {
                        eqScheme = false;
                    }

                    if (eqScheme)
                    {
                        eqClass.States.Add(States[i]);
                        stateAddedToClass = true;
                    }
                }

                //если подходящий класс не найден, создаем новый
                if (!stateAddedToClass)
                {
                    string[] scheme = new string[InputSignalsNum + 1];
                    for (int j = 0; j < InputSignalsNum; j++)
                    {
                        scheme[j] = Transitions[j][i].Class;
                    }
                    scheme[InputSignalsNum] = StateClasses[i];

                    classes.Add(new Class
                    {
                        States = new List<string>() { States[i] },
                        OutSсheme = scheme
                    });
                }
            }

            return classes;
        }


        private void Update(List<Class> classes)
        {
            //каждая cтрока
            for (int i = 0; i < InputSignalsNum; i++)
            {
                //каждый столбец
                for (int j = 0; j < StatesNum; j++)
                {
                    for (int k = 0; k < classes.Count; k++)
                    {
                        //найти, к какому классу принажлежит переход
                        if (classes[k].States.Contains(Transitions[i][j].State))
                        {
                            Transitions[i][j].Class = k.ToString();
                        }

                        //найти, к какому классу принажлежит сотояние
                        if (classes[k].States.Contains(States[j]))
                        {
                            StateClasses[j] = k.ToString();
                        }
                    }
                }
            }
        }

        public void PrintInitial()
        {
            Console.WriteLine("Moore");
            Console.WriteLine("Initial:");
            Print(States, OutputSignals, Transitions);
        }

        public void PrintMinimized()
        {
            Console.WriteLine("Minimized:");
            Print(FinalStates, FinalOutputSignals, FinalTransitions, FinalClasses);
        }

        private void Print(string[] states, string[] outputSignals, Transition[][] transitions, Class[] classes = null)
        {
            if (classes != null)
            {
                for (int i = 0; i < classes.Length; i++)
                {
                    Console.WriteLine($"({classes[i].States.Aggregate((x, y) => x.ToString() + ", " + y.ToString())}) => s{i}");
                }
            }

            foreach (var state in states)
            {
                Console.Write(string.Format("{0, -4}", state));
            }
            Console.WriteLine();

            foreach (var simbol in outputSignals)
            {
                Console.Write(string.Format("{0, -4}", simbol));
            }
            Console.WriteLine();

            for (int i = 0; i < transitions.Length; i++)
            {
                for (int j = 0; j < transitions[0].Length; j++)
                {
                    Console.Write(string.Format("{0, -4}", transitions[i][j].State));
                }
                Console.WriteLine();
            }
        }
    }
}
