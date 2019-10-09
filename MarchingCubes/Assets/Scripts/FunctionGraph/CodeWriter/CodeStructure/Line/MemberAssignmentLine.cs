using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class MemberAssignmentLine : AssignmentLine
    {
        Member mem;

        public override string LineToWrite
        {
            get
            {
                return $"{mem.GetAsReference()} = {expresion.GetAsEOL()}";
            }
        }

        public MemberAssignmentLine(Member mem, string expresion) : base(expresion)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
        }

        public override void Write(IndentedStreamWriter w)
        {
            w.Write($"{mem.Name}");
            WriteAssignment(w);
        }
    }
}