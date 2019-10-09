using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public abstract class AssignmentLine : Line
    {

        public AssignmentLine( string expresion)
        {
            this.expresion = new Expresion(expresion);
        }

        protected void WriteAssignment(IndentedStreamWriter w)
        {
            w.Write($" = {expresion.GetAsEOL()}");
        }
    }
}