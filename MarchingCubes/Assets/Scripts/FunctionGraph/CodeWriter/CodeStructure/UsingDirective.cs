using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FuncGraph.CodeWriting
{
    public class UsingDirective : BasicLine
    {
        public Namespace nameSpaceUsed { get; protected set; }

        public UsingDirective(Namespace nameSpaceUsed)
        {
            this.nameSpaceUsed = nameSpaceUsed ?? throw new ArgumentNullException(nameof(nameSpaceUsed));
            lineToWrite = $"using {nameSpaceUsed.Name};";
        }

    }
}
