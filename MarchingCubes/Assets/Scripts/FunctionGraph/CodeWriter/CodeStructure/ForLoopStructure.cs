using System.Collections.Generic;

namespace FuncGraph.CodeWriting
{
    public class ForLoopStructure : MultiLineCodeStructure
    {
        public ForLoopStructure(Line incrementerInitAssign, Line condition, Line incrementer, List<Line> linesInBody) : base()
        {
            
            lines.Add(new ForLoopHeader(incrementerInitAssign, condition, incrementer));
            lines.Add(new BasicLine("{"));
            if (linesInBody != null)
            {
                for (int i = 0; i < linesInBody.Count; i++)
                {
                    lines.Add(linesInBody[i]);
                }
            }
            lines.Add(new BasicLine("}"));
        }

        public ForLoopStructure(Line incrementerAssignment, Line condition, Line incrementer, Line lineInBody):base()
        {
            lines.Add(new ForLoopHeader(incrementerAssignment, condition, incrementer));
            lines.Add(new BasicLine("{"));
            lines.Add(lineInBody);
            lines.Add(new BasicLine("}"));
        }
    }
}