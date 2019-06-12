using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Parameter : Variable
    {
        public Parameter(string type, string name) : base(type, name)
        { }

        public override bool Equals(object obj)
        {
            if (obj is Parameter)
            {
                var parObj = obj as Parameter;
                return type == parObj.type && name == parObj.name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void Write(StreamWriter writer)
        {
            writer.Write($"{type} {name}");
        }

        
    }
}