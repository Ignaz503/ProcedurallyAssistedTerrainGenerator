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

        public abstract void WriteAsDefinition(IndentedStreamWriter writer);

        public string GetAsReference()
        {
            return name + (name.EndsWith(" ") ? "" : " ");
        }

        public virtual string GetAsDefinition()
        {
            return $"{type} {name} ";
        }

        public void WriteAsReference(IndentedStreamWriter writer)
        {
            writer.Write(name + (name.EndsWith(" ") ? "" : " "));
        }

        public bool IsOfType(string type)
        {
            return type == this.type;
        }
    }
}