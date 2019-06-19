using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class ForLoopHeader : Line
    {
        Line initAssignment;
        RHSExpresion condition;
        Line increment;
        
        public override string LineToWrite
        {
            get
            {
                string incr = increment.LineToWrite.EndsWith(";") ? increment.LineToWrite.Remove(increment.LineToWrite.Length - 1) : increment.LineToWrite;
                string initAssign = initAssignment.LineToWrite.EndsWith(";") ? initAssignment.LineToWrite : initAssignment.LineToWrite + ";";
                return $"for({initAssign} {condition.GetAsEOL()} {incr})";
            }
        }

        public ForLoopHeader(Line initAssignment, RHSExpresion condition, Line increment)
        {
            this.initAssignment = initAssignment ?? throw new ArgumentNullException(nameof(initAssignment));
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.increment = increment ?? throw new ArgumentNullException(nameof(increment));
        }

        public override void Write(StreamWriter w)
        {
            w.Write(LineToWrite);
        }
    }
}