namespace FuncGraph.CodeWriting
{
    public class LocalInitializerLine : LocalAssignmentLine
    {
        public LocalInitializerLine(Local l, RHSExpresion expr) : base(l, expr, true)
        {}
    }
}