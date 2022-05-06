using System.Collections.Generic;

namespace LL1
{
    public class RuleStorage
    {
        public List<Rule> Rules { get; }
        private static int Null = -1;

        public RuleStorage()
        {
            Rules = new List<Rule>() 
            {
                new Rule(0, "1a-(", false, true, 1, false, false),
                new Rule(1, "1a-(", false, true, 3, true, false),
                new Rule(2, "\0", true, true, Null, false, true),
                new Rule(3, "1a-(", false, true, 4, false, false),
                new Rule(4, "1a-(", false, true, 14, true, false),
                new Rule(5, "+)\0", false, true, 6, false, false),
                new Rule(6, "+", false, false, 11, false, false),
                new Rule(7, ")", false, false, 10, false, false),
                new Rule(8, "\0", false, true, 9, false, false),
                new Rule(9, "\0", false, true, Null, false, false),
                new Rule(10, ")", false, true, Null, false, false),
                new Rule(11, "+", true, true, 12, false, false),
                new Rule(12, "1a-(", false, true, 14, true, false),
                new Rule(13, "+)\0", false, true, 6, false, false),
                new Rule(14, "1a-(", false, true, 15, false, false),
                new Rule(15, "1a-(", false, true, 27, true, false),
                new Rule(16, "*+)\0", false, true, 17, false, false),
                new Rule(17, "*", false, false, 24, false, false),
                new Rule(18, "+", false, false, 23, false, false),
                new Rule(19, ")", false, false, 22, false, false),
                new Rule(20, "\0", false, true, 21, false, false),
                new Rule(21, "\0", false, true, Null, false, false),
                new Rule(22, ")", false, true, Null, false, false),
                new Rule(23, "+", false, true, Null, false, false),
                new Rule(24, "*", true, true, 25, false, false),
                new Rule(25, "1a-(", false, true, 27, true, false),
                new Rule(26, "*+)\0", false, true, 17, false, false),
                new Rule(27, "1", false, false, 31, false, false),
                new Rule(28, "a", false, false, 32, false, false),
                new Rule(29, "-", false, false, 33, false, false),
                new Rule(30, "(", false, true, 35, false, false),
                new Rule(31, "1", true, true, Null, false, false),
                new Rule(32, "a", true, true, Null, false, false),
                new Rule(33, "-", true, true, 34, false, false),
                new Rule(34, "1a-(", false, true, 27, false, false),
                new Rule(35, "(", true, true, 36, false, false),
                new Rule(36, "1a-(", false, true, 3, true, false),
                new Rule(37, ")", true, true, Null, false, false)
            };
        }
    }
}