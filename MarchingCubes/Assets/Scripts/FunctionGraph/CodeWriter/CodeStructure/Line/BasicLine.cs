﻿using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class BasicLine : Line
    {
        protected string lineToWrite;

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

        protected BasicLine(){ lineToWrite = ""; }

        public override void Write(IndentedStreamWriter w)
        {
            w.WriteLine(lineToWrite);
        }
    }
}