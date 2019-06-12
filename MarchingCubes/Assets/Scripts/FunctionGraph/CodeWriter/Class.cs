using System;
using System.Collections.Generic;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Class
    {
        public string Name { get; protected set; }
        protected List<Member> members;
        public List<Function> functions;

        public Class(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            members = new List<Member>();
            functions = new List<Function>();
        }

        public void AddMember(Member mem)
        {
            members.Add(mem);
        }

        public void AddFunctions(Function func)
        {
            functions.Add(func);
        }

        public Ctor GetACtor()
        {
            return new Ctor("public", Name);
        }

        public void AddCtor(Ctor ctor)
        {
            functions.Add(ctor);
        }

        public void Write(StreamWriter write)
        {
            //TODO
        }

        public string GetNameForFunction(string furtherIdentifier)
        {
            return $"func{functions.Count}_{furtherIdentifier}";
        }

        public string GetNameForMember(string furtherIdentifier)
        {
            return $"mem{members.Count}_{furtherIdentifier}";
        }

        public bool HasFunction(string returnType, List<Parameter> parameter)
        {
            //TODO
            return false;
        }

        public bool HasFunction(string returnType, string name, List<Parameter> parameter)
        {
            //TODO
            return false;
        }

        public bool HasCtor(List<Parameter> parameter)
        {
            //TODO
            return false;
        }
    }
}