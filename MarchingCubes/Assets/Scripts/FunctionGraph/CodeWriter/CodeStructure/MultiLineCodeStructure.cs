using System.Collections.Generic;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public abstract class MultiLineCodeStructure : CodeStructure
    {
        protected List<Line> lines;

        public MultiLineCodeStructure()
        {
            lines = new List<Line>();
        }

        public override void Write(IndentedStreamWriter w)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Write(w);
            }
        }

        public void AddLine(Line l)
        {
            lines.Add(l);
        }

    }
}