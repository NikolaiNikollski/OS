using System;
using System.Collections.Generic;
using System.Linq;

namespace LL1
{
    class Program
    {
        static Stack<Rule> ReserveRules = new Stack<Rule>();
        static RuleStorage Table = new RuleStorage();
        static Queue<char> Expression;
        static char CurrCh;

        private const char EndSymbol = '\0';
        private static int Null = -1;

        static void Main(string[] args)
        {
            List<string> tests = new List<string>()
            {
                "((1+1))", "*", "-a", "-1", "-a*", "-()", "*()", "(*)", "(-)", "(a)", "(1)", "-11)1", "a-11a1a1*-11a",
                "(-a1", "(1+1)", "((1+1))", "(((1+1)))",
                "(((1+1)))+1", "(((1+1))+1)", "(((1+1)+1))",
                "(1*1)", "((1*1))", "(((1*1)))", "(((1*1)))*1", "(((1*1))*1)", "(((1*1)*1))", "-((-((--a))))"
            };

            foreach (string test in tests)
            {
                Expression = new Queue<char>(test);
                GC();
                bool isValid = ValidExpression();
                Console.WriteLine($"{test}: {isValid}");
            }
        }

        private static bool ValidExpression()
        {
            Rule currRule = Table.Rules.First();
            Rule newRule;

            for (; ; )
            {
                if (currRule.GuideSymbols.Contains(CurrCh))
                {
                    if (currRule.End) return true;
                    newRule = TakeRuleByIndex(currRule.Pointer);
                }
                else if (currRule.Error)
                {
                    return false;
                }
                else
                {
                    newRule = TakeRuleByIndex(currRule.IdRule + 1);
                }

                if (currRule.Shift) GC();

                if (currRule.InStack) ReserveRules.Push(Table.Rules[currRule.IdRule + 1]);

                currRule = newRule;
            }
        }

        private static Rule TakeRuleByIndex(int index)
        {
            return index == Null ? ReserveRules.Pop() : Table.Rules[index];
        }

        private static void GC()
        {
            if (Expression.Count == 0)
            {
                CurrCh = EndSymbol;
            }
            else
            {
                CurrCh = Expression.Dequeue();
            }
        }
    }
} 