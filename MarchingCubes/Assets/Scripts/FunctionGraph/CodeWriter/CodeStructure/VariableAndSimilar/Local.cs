using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Local : Variable
    {
        public string Name
        {
            get { return name; }
        }

        public Local(string type, string name) : base(type, name)
        {}

        public override void WriteAsDefinition(StreamWriter writer)
        {
            writer.WriteLine($"{type} {name};");
        }

        public string GetAsDefinition()
        {
            return $"{type} {name}";
        }

        public void WriteAsDefinitionWithInitializer(StreamWriter writer, RHSExpresion initializer)
        {
            writer.WriteLine($"{type} {name} = {initializer.GetAsEOL()}");
        }

        public bool PartialEquals(string name)
        {
            return this.name == name;
        }

        public bool PartialEqualsSubstring(string substring)
        {
            return this.name.Contains(substring);
        }

        public bool Equals(string type, string name)
        {
            return this.type == type && this.name == name;
        }

        public override bool Equals(object obj)
        {
            if (obj is Local)
            {
                var locObj = obj as Local;
                return Equals(locObj.type, locObj.type);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}