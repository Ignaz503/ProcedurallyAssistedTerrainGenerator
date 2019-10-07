using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class FunctionGraphCSharpCodeWriter : CSharpCodeWriter
    {
        public Namespace FunctionGraphNameSpace { get; protected set; }

        public FunctionGraphCSharpCodeWriter(Namespace functionGraphNameSpace)
        {
            FunctionGraphNameSpace = functionGraphNameSpace ?? throw new ArgumentNullException(nameof(functionGraphNameSpace));
        }

        public new void WriteToFile(string path)
        {
            using (var writer = new StreamWriter(path))
            {
                CurrentClass.Write(new IndentedStreamWriter(writer));
            }
        }
    }

}