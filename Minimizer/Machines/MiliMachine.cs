using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Minimizer.Machines
{
    class MiliMachine : Machine
    {
        private struct MiliTransition
        {
            public string State;

            public string OutputSignal;

            public string Class;
        }
        private struct MiliClass
        {
            public string[] OutSсheme;

            public List<string> States;
        }

        private int StatesNum;
        private int InputSignalsNum;
        private int OutputSignalsNum;

        private string[] States;
        private string[] StateClasses;
        private MiliTransition[][] Transitions;

        private MiliClass[] FinalClasses;
        private string[] FinalStates;
        private MiliTransition[][] FinalTransitions;

        public MiliMachine(ref StreamReader sr)
        {
            StatesNum = int.Parse(sr.ReadLine());
            InputSignalsNum = int.Parse(sr.ReadLine());
            OutputSignalsNum = int.Parse(sr.ReadLine());

            States = new Regex(" +").Split(sr.ReadLine());
            Transitions = new MiliTransition[InputSignalsNum][];
            for (int i = 0; i < InputSignalsNum; i++)
            {
                Transitions[i] = new Regex(" +").Split(sr.ReadLine()).Select(t =>
                {
                    string[] values = t.Split("/");
                    return new MiliTransition()
                    {
                        State = values[0],
                        OutputSignal = values[1]
                    };
                }).ToArray();
            }
            StateClasses = new string[StatesNum];
        }

        public void Minimize()
        {
            List<MiliClass> classes = GetInitialClasses();
            int oldClassesCount = 0;
            while (oldClassesCount != classes.Count)
            {
                oldClassesCount = classes.Count();
                Update(classes);
                classes = GetSecondaryClasses();
            }

            FinalStates = classes.Select((c, i) => "s" + i).ToArray();
            FinalTransitions = new MiliTransition[InputSignalsNum][];
            FinalClasses = classes.ToArray();

            for (int i = 0; i < FinalTransitions.Length; i++)
            {
                FinalTransitions[i] = new MiliTransition[classes.Count()];

                for (int j = 0; j < classes.Count(); j++)
                {
                    string originalState = classes[j].States[0];
                    var originalStateIndex = Array.IndexOf(States, originalState);
                    FinalTransitions[i][j] = new MiliTransition()
                    {
                        State = "s" + GetClassIndexByState(Transitions[i][originalStateIndex].State, classes.ToArray()),
                        OutputSignal = Transitions[i][originalStateIndex].OutputSignal
                    };
                }
            }
        }

        private int GetClassIndexByState(string state, MiliClass[] classes)
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

        private List<MiliClass> GetInitialClasses()
        {
            //сформировать классы эквивалентности
            List<MiliClass> classes = new List<MiliClass>();
            //по каждому столбцу
            for (int i = 0; i < States.Length; i++)
            {
                bool stateAddedToClass = false; //столбец добален в сущетсвующий класс

                //поискать существующий класс
                foreach (var eqClass in classes)
                {
                    bool eqScheme = true; //класс подошел
                    for (int j = 0; j < InputSignalsNum; j++)
                    {
                        if (eqClass.OutSсheme[j] != Transitions[j][i].OutputSignal)
                        {
                            eqScheme = false;
                        }
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
                    string[] scheme = new string[InputSignalsNum];
                    for (int j = 0; j < InputSignalsNum; j++)
                    {
                        scheme[j] = Transitions[j][i].OutputSignal;

                    }
                    classes.Add(new MiliClass
                    {
                        States = new List<string>() { States[i] },
                        OutSсheme = scheme
                    });
                }
            }

            return classes;
        }

        private List<MiliClass> GetSecondaryClasses()
        {
            List<MiliClass> classes = new List<MiliClass>();

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

                    classes.Add(new MiliClass
                    {
                        States = new List<string>() { States[i] },
                        OutSсheme = scheme
                    });
                }
            }

            return classes;
        }

        private void Update(List<MiliClass> classes)
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
            Console.WriteLine("Mili");
            Console.WriteLine("Initial:");
            Print(States, Transitions);
        }

        public void PrintMinimized()
        {
            Console.WriteLine("Minimized:");
            Print(FinalStates, FinalTransitions, FinalClasses);
        }

        private void Print(string[] states, MiliTransition[][] transitions, MiliClass[] classes = null)
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
                Console.Write(string.Format("{0, -7}", state));
            }
            Console.WriteLine();


            for (int i = 0; i < transitions.Length; i++)
            {
                for (int j = 0; j < transitions[0].Length; j++)
                {
                    Console.Write(string.Format("{0, -3}/{1, -3}", transitions[i][j].State, transitions[i][j].OutputSignal));
                }
                Console.WriteLine();
            }
        }
    }
}
