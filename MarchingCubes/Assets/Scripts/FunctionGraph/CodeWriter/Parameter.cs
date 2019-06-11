using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Parameter : Variable
    {
        public Parameter(string type, string name) : base(type, name)
        { }

        public override void Write(StreamWriter writer)
        {
            writer.Write($"{type} {name}");
        }
    }
}