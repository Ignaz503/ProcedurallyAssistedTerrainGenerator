using System.Collections;

namespace FuncGraph.CodeWriting
{
    public  interface ICodeWriter
    {
        void WriteToFile(string path);
    }
}