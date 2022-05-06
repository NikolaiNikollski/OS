using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Determinator
{
    public class State
    {
        public HashSet<char> Name;

        public List<Transition> Transitions;
    }

    public struct Transition
    {
        public char InputSignal;

        public State TargetState;
    }

    public class Machine
    {
        private List<State> InitialStates;
        private List<State> FinalStates;
        private List<char> InputSignals;
        private char GrammaType;
        const char LeftGramma = 'L';
        const char RightGramma = 'R';
        const char EndState = 'H';

        public Machine(ref StreamReader sr)
        {
            InputSignals = new List<char>();
            List<State> states = new List<State>();
            string line;
            GrammaType = sr.ReadLine()[0];
            while ((line = sr.ReadLine()) != null)
            {
                string[] values = line.Split(":");

                HashSet<char> stateName = new HashSet<char>() { values[0][0] };
                State state = FindOrCreateState(stateName, states);

                string transitionString = values[1];
                string[] transitions = transitionString.Split("|");
                foreach (string transition in transitions)
                {
                    char transitionInputSignal;
                    HashSet<char> transitionStateName = new HashSet<char>();
                    if (transition.Length == 1)
                    {
                        transitionInputSignal = transition[0];
                        transitionStateName.Add('H');
                    }
                    else
                    {
                        transitionInputSignal = GrammaType == LeftGramma ? transition[1] : transition[0];
                        transitionStateName.Add(GrammaType == LeftGramma ? transition[0] : transition[1]);
                    }

                    if (!InputSignals.Contains(transitionInputSignal))
                    {
                        InputSignals.Add(transitionInputSignal);
                    }

                    if (GrammaType == RightGramma)
                    {
                        state.Transitions.Add(new Transition()
                        {
                            TargetState = FindOrCreateState(transitionStateName, states),
                            InputSignal = transitionInputSignal
                        });
                    }
                    else
                    {
                        FindOrCreateState(transitionStateName, states).Transitions.Add(new Transition()
                        {
                            TargetState = state,
                            InputSignal = transitionInputSignal
                        });
                    }
                }
            }

            InitialStates = states;
        }

        private State FindOrCreateState(HashSet<char> stateName, List<State> states)
        {
            State state = states.FirstOrDefault(s => s.Name.SetEquals(stateName));
            if (state == null)
            {
                state = new State()
                {
                    Name = stateName,
                    Transitions = new List<Transition>()
                };
                states.Add(state);
            }

            return state;
        }

        public void Determinate()
        {
            FinalStates = new List<State>();
            
            State firstState = new State()
            {
                Name = InitialStates.First(s => GrammaType == LeftGramma ? s.Name.First() == EndState: true).Name.ToHashSet(),
                Transitions = new List<Transition>()
            };
            FinalStates.Add(firstState);

            Queue<State> queue = new Queue<State>();
            queue.Enqueue(firstState);

            while (queue.Count > 0)
            {
                State nextState = queue.Dequeue();
                foreach (char signal in InputSignals)
                {
                    HashSet<char> newStateName = GetNewStateName(nextState, signal);
                    if (newStateName.Count > 0)
                    {
                        State state = FinalStates.FirstOrDefault(s => s.Name.SetEquals(newStateName));
                        if (state == null)
                        {
                            state = new State()
                            {
                                Name = newStateName,
                                Transitions = new List<Transition>()
                            };
                            FinalStates.Add(state);
                            queue.Enqueue(state);
                        }

                        nextState.Transitions.Add(new Transition()
                        {
                            InputSignal = signal,
                            TargetState = state
                        });
                    }
                }
            }
        }

        private HashSet<char> GetNewStateName(State inState, char inputSignal)
        {
            HashSet<char> newStateName = new HashSet<char>();
            foreach (char stateName in inState.Name)
            {
                State state = InitialStates.First(s => s.Name.First() == stateName);
                foreach (Transition transition in state.Transitions)
                {
                    if (transition.InputSignal == inputSignal)
                    {
                        newStateName.Add(transition.TargetState.Name.First());
                    }
                }
            }

            return newStateName;
        }

        public void PrintInitial()
        {
            Console.WriteLine("Ininital:");
            Print(InitialStates);
        }

        public void PrintFinal()
        {
            Console.WriteLine("Determinded:");
            Print(FinalStates);
        }

        public void Print(List<State> states)
        {
            foreach (State state in states)
            {
                Console.Write($"{HashSetToString(state.Name)}:");
                foreach (Transition transition in state.Transitions)
                {
                    Console.Write($"{transition.InputSignal}{HashSetToString(transition.TargetState.Name)}|");
                }
                Console.WriteLine();
            }
        }

        private string HashSetToString(HashSet<char> inHash)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in inHash)
            {
                sb.Append(c);
            }

            return sb.ToString();
        }
    }
}
