namespace LL1
{
    public class Rule
    {
        public int IdRule { get; }
        public string GuideSymbols { get; }
        public bool Shift { get; }
        public bool Error { get; }
        public int Pointer { get; }
        public bool InStack { get; }
        public bool End { get; }

        public Rule(int idRule, string guideSymbols, bool shift, bool error, int pointer, bool inStack, bool end)
        {
            IdRule = idRule;
            GuideSymbols = guideSymbols;
            Shift = shift;
            Error = error;
            Pointer = pointer;
            InStack = inStack;
            End = end;
        }
    }
}