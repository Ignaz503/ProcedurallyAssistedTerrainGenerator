using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FuncGraph.CodeWriting
{
    public class UsingDirective : BasicLine
    {
        public Namespace NamespaceUsed { get; protected set; }

        public UsingDirective(Namespace nameSpaceUsed)
        {
            this.NamespaceUsed = nameSpaceUsed ?? throw new ArgumentNullException(nameof(nameSpaceUsed));
            lineToWrite = $"using {nameSpaceUsed.Name};";
        }

        public override bool Equals(object obj)
        {
            var directive = obj as UsingDirective;
            return directive != null && directive.NamespaceUsed == NamespaceUsed; 
        }

        public override int GetHashCode()
        {
            return -1769782419 + EqualityComparer<Namespace>.Default.GetHashCode(NamespaceUsed);
        }
    }
}
