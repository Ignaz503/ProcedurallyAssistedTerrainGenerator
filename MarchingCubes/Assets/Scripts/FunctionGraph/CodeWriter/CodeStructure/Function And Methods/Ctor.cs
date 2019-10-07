using System.Collections.Generic;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Ctor : Function
    {
        public Ctor(string accessor, string name) : base(accessor, "", name)
        {

        }

        public Ctor(string accessor, string name, List<Parameter> parameters) : base(accessor, "", name, parameters)
        {

        }

        public override void Write(StreamWriter writer)
        {
            //TODO
        }
    }
}