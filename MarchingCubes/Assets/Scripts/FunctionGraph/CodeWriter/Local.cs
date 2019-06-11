using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Local : Variable
    {
        protected string initializer;

        public string Name
        {
            get { return name; }
        }

        public Local(string type, string name, string initializer) : base(type, name)
        {
            this.initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        }

        public override void Write(StreamWriter writer)
        {
            string init = initializer.EndsWith(";") ? initializer : initializer + ";";
            writer.WriteLine($"{type} {name} = {init};");
        }
    }
}