using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class BasicLine : Line
    {
        string lineToWrite;

        public override string LineToWrite
        {
            get
            {
                return lineToWrite;
            }
        }

        public BasicLine(string lineToWrite)
        {
            this.lineToWrite = lineToWrite ?? throw new ArgumentNullException(nameof(lineToWrite));
        }

        public override void Write(StreamWriter w)
        {
            w.WriteLine(lineToWrite);
        }
    }
}