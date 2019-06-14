using System.Collections;

namespace FuncGraph.CodeWriting
{
    public abstract class Line : CodeStructure
    {
        public abstract string LineToWrite { get; }
    }
}