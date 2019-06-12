using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public abstract class Variable
    {
        protected string type;
        protected string name;

        public Variable(string type, string name)
        {
            this.type = type ?? throw new ArgumentNullException(nameof(type));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public abstract void Write(StreamWriter writer);

        public void WriteAsReference(StreamWriter writer)
        {
            writer.Write(name + (name.EndsWith(" ") ? "" : " "));
        }
    }
}