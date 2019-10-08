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

        public override void WriteAsDefinition(IndentedStreamWriter writer)
        {
            writer.WriteLine($"{accessor} {type} {name};");
        }

        public override string GetAsDefinition()
        {
            return $"{accessor} {type} {name};";
        }

        public void WriteWithAssignment(IndentedStreamWriter w, RHSExpresion initializer)
        {
            w.WriteLine($"{name} = {initializer.GetAsEOL()}");
        }

        public bool PartialEquals(string name)
        {
            return name == this.name;
        }

        public bool PartialEqualsSubString(string subString)
        {
            return name.Contains(subString);
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