using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class ForLoopHeader : CodeStructure
    {
        Line initAssignment;
        Line condition;
        Line increment;
        
        public string LineToWrite
        {
            get
            {
                string incr = increment.LineToWrite.EndsWith(";") ? increment.LineToWrite.Remove(increment.LineToWrite.Length - 1) : increment.LineToWrite;
                string initAssign = initAssignment.LineToWrite.EndsWith(";") ? initAssignment.LineToWrite : initAssignment.LineToWrite + ";";
                string cond = condition.LineToWrite.EndsWith(";") ? condition.LineToWrite : condition.LineToWrite + ";";
                return $"for({initAssign} {cond} {incr})";
            }
        }

        public ForLoopHeader(Line initAssignment, Line condition, Line increment)
        {
            this.initAssignment = initAssignment ?? throw new ArgumentNullException(nameof(initAssignment));
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.increment = increment ?? throw new ArgumentNullException(nameof(increment));
        }

        public override void Write(IndentedStreamWriter w)
        {
            w.Write(LineToWrite);
        }
    }
}