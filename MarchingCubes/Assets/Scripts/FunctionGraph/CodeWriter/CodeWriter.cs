using System.Collections;
using UnityEngine;

namespace FuncGraph.CodeWriting
{

    public abstract class CodeWriter
    {
        public abstract void WriteToFile(string path);
    }
}