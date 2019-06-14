using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public abstract class AssignmentLine : Line
    {
        protected RHSExpresion expresion;

        public AssignmentLine( RHSExpresion expresion)
        {
            this.expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
        }

        protected void WriteAssignment(StreamWriter w)
        {
            w.Write($"= {expresion.GetAsEOL()}");
        }
    }
}