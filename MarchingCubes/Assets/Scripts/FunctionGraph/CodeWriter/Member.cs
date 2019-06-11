using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Member : Variable
    {
        protected string accessor;

        public string Name
        {
            get { return name; }
        }

        public Member(string accessor, string type, string name) : base(type, name)
        {
            this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        }

        public override void Write(StreamWriter writer)
        {
            writer.WriteLine($"{accessor} {type} {name};");
        }
    }
}