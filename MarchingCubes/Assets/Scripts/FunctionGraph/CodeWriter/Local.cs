using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Local : Variable
    {
        protected RHSExpresion initializer;

        public string Name
        {
            get { return name; }
        }

        public Local(string type, string name, string initializer) : base(type, name)
        {
            this.initializer = new RHSExpresion(initializer);
        }

        public Local(string type, string name, RHSExpresion initializer) : base(type,name)
        {
            this.initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        }

        public override void Write(StreamWriter writer)
        {
            writer.WriteLine($"{type} {name} = {initializer.GetAsEOL()}");
        }
    }
}