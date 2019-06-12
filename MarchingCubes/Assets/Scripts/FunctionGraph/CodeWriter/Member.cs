using System;
using System.Collections.Generic;
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

        public bool PartialEquals(string name)
        {
            return name == this.name;
        }

        public bool SameType(string type)
        {
            return type == this.type;
        }

        public bool PartialEquals(string type, string name)
        {
            return this.name == name && this.type == type;
        }

        public override bool Equals(object obj)
        {
            if (obj is Member)
            {
                var obMem = obj as Member;

                return obMem.name == name && obMem.type == type && obMem.accessor == accessor;

            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}