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
        private string GrammaType;

        public Machine(ref StreamReader sr)
        {
            InputSignals = new List<char>();
            List<State> states = new List<State>();
            string line;
            GrammaType = sr.ReadLine();
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
                    if (transition.Length == 0)
                    {
                        continue;
                    }
                    if (transition.Length == 1)
                    {
                        transitionInputSignal = transition[0];
                        transitionStateName.Add('H');
                    }
                    else
                    {
                        transitionInputSignal = GrammaType == "L" ? transition[1] : transition[0];
                        transitionStateName.Add(GrammaType == "L" ? transition[0] : transition[1]);
                    }

                    if (!InputSignals.Contains(transitionInputSignal))
                    {
                        InputSignals.Add(transitionInputSignal);
                    }

                    if (GrammaType == "R")
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
                Name = InitialStates.First(s => GrammaType == "L" ? s.Name.First() == 'H' : true).Name.ToHashSet(),
                Transitions = new List<Transition>()
            };

            firstState.Name = GetLockStates(firstState.Name);
            FinalStates.Add(firstState);

            Queue<State> queue = new Queue<State>();
            queue.Enqueue(firstState);

            while (queue.Count > 0)
            {
                State nextState = queue.Dequeue();
                nextState.Name = GetLockStates(nextState.Name);
                foreach (char signal in InputSignals)
                {
                    if (signal == 'E') continue;
                    HashSet<char> newStateName = GetLockStates(GetNewStateName(nextState, signal));
                    if (newStateName.Count > 0)
                    {
                        State state = FinalStates.FirstOrDefault(s => s.Name.SetEquals(newStateName));
                        if (state == null)
                        {
                            state = new State()
                            {
                                Name = newStateName.ToList().ToHashSet(),
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

        private HashSet<char> GetLockStates(HashSet<char> stateName)
        {
            Queue<State> lockingQueue = new Queue<State>();
            List<State> lockingStates = stateName.Select(n => InitialStates.First(s => s.Name.First() == n)).ToList();
            foreach (State lockState in lockingStates)
            {
                lockingQueue.Enqueue(lockState);
            }

            while (lockingQueue.Count > 0)
            {
                State lockState = lockingQueue.Dequeue();
                List<State> lockingStates2 = lockState.Transitions
                    .Where(t => t.InputSignal == 'E')
                    .Select(t => t.TargetState).ToList();
                foreach (var state in lockingStates2)
                {
                    if (!stateName.Contains(state.Name.First()))
                    {
                        lockingQueue.Enqueue(state);
                        stateName.Add(state.Name.First());
                    }
                }
            }
            return stateName;
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
