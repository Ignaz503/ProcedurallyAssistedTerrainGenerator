namespace FuncGraph.CodeWriting
{
    public class LocalInitializerLine : LocalAssignmentLine
    {
        public LocalInitializerLine(Local l, string expr) : base(l, expr, true)
        {}
    }
}