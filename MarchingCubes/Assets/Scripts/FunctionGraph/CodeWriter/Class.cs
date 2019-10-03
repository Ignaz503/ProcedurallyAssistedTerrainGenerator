using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

//TODO WRAP STREAMWRITER with writer that tracks indentation and so on for  nicer fomratting

namespace FuncGraph.CodeWriting
{
    public class Class
    {
        public string Name { get; protected set; }

        public bool HasBaseClass { get { return BaseClass != "" ; } }
        public string BaseClass { get; protected set; }

        public bool HasInterfacesImplemented { get { return implentedInterfaces.Count > 0; } }
        List<string> implentedInterfaces;
        public IEnumerable<string> ImplentedInterfaces { get { return implentedInterfaces; } }

        protected List<Member> members;
        public List<Function> functions;

        public Class(string name, string baseClass = "")
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            BaseClass = baseClass;
            members = new List<Member>();
            functions = new List<Function>();
            implentedInterfaces = new List<string>();
        }

        public void AddMember(Member mem)
        {
            members.Add(mem);
        }

        public void AddFunction(Function func)
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

        public string GetNameForFunction(string furtherIdentifier)
        {
            return $"func{functions.Count}_{furtherIdentifier}";
        }

        public string GetNameForMember(string furtherIdentifier)
        {
            return $"mem{members.Count}_{furtherIdentifier}";
        }

        public bool HasFunctionWithPartialSignature(string returnType, List<Parameter> parameter)
        {
            //TODO
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].PartialEquals(returnType, parameter))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasFunction(string returnType, string name, List<Parameter> parameter)
        {
            //TODO
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].Equals(returnType,name, parameter))
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasFunctionSubString(string returnType, string subString, List<Parameter> parameters)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].EqualsSubstring(returnType, subString, parameters))
                    return true;
            }
            return false;
        }

        public bool HasCtor(List<Parameter> parameter)
        {
            //TODO
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].PartialEquals("", parameter))
                {
                    return true;
                }
            }
            return false;
        }

        public Function GetFunctionWithPartialSignature(string returnType, List<Parameter> parameters)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].PartialEquals(returnType, parameters))
                {
                    return functions[i];
                }
            }
            return null;
        }

        public Function GetFunction(string returnType, string name, List<Parameter> parameters)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].Equals(returnType, name, parameters))
                    return functions[i];
            }
            return null;
        }

        public Function GetFunctionSubstring(string returnType, string subStr, List<Parameter> parameters)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].EqualsSubstring(returnType, subStr, parameters))
                {
                    return functions[i];
                }
            }
            return null;
        }

        public Ctor GetCtor(List<Parameter> parameter)
        {
            for (int i = 0; i < functions.Count; i++)
            {
                if (functions[i].PartialEquals("", parameter))
                    return functions[i] as Ctor;
            }
            return null;
        }

        public bool HasMember(string name)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].PartialEquals(name))
                    return true;
            }
            return false;
        }

        public bool HasMember(string type, string name)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].PartialEquals(type, name))
                    return true;
            }
            return false;
        }

        public bool HasMemberSubstring(string substr)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].PartialEqualsSubString(substr))
                    return true;
            }
            return false;
        }

        public Member GetMember(string name)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].PartialEquals(name))
                    return members[i];
            }
            return null;
        }

        public Member GetMember(string type, string name)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].PartialEquals(type,name))
                    return members[i];
            }
            return null;
        }

        public Member GetMemberSubstring(string substring)
        {
            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].PartialEqualsSubString(substring))
                    return members[i];
            }
            return null;
        }

        public List<Member> GetMembersOfType(string type)
        {
            List<Member> mems = new List<Member>();

            for (int i = 0; i < members.Count; i++)
            {
                if (members[i].IsOfType(type))
                {
                    mems.Add(members[i]);
                }
            }
            return mems;
        }

        public bool ImplementsInterface(string interfaceName)
        {
            for (int i = 0; i < implentedInterfaces.Count; i++)
            {
                if (implentedInterfaces[i].Equals(interfaceName))
                    return true;
            }
            return false;
        }

        public void AddInterface(string interfaceName)
        {
            implentedInterfaces.Add(interfaceName);
        }

        public bool RemoveInterface(string interfaceName)
        {
            return implentedInterfaces.Remove(interfaceName);
        }

        public void ChangeBaseClass(string newBaseClass)
        {
            BaseClass = newBaseClass;
        }

        public void Write(StreamWriter write)
        {
            writeHeader(write);
            writeBody(write);
            //DONE;
        }

        void writeHeader(StreamWriter writer)
        {
            writer.WriteLine(BuildHeader());
        }

        string BuildHeader()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("class ");

            builder.Append(Name);

            if (HasBaseClass || HasInterfacesImplemented)
                builder.Append(" : ");

            if (HasBaseClass)
                builder.Append(BaseClass);

            if (HasInterfacesImplemented)
            {
                for (int i = 0; i < implentedInterfaces.Count; i++)
                {
                    if (i == 0 && !HasBaseClass)
                    {
                        //case of class Ass : IInterFace1, IInterface2..
                        // instead of class Ass : BaseClass, IInterface1, Interface2...
                        builder.Append(implentedInterfaces[i]);
                        continue;
                    }

                    builder.Append(", ");
                    builder.Append(implentedInterfaces[i]);
                }
            }
            return builder.ToString();
        }

        void writeBody(StreamWriter writer)
        {
            writer.WriteLine("{");
            //TODO
            //Members
            //functions
            writer.WriteLine("}");
        }
    }
}