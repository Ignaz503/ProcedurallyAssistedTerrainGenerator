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

        public void Write(StreamWriter write)
        {
            //TODO
            //WRITE HEAD:

            //WRITE BODY
            //WRITE MEMBERS
            //WRITE FUNCTIONS
            //DONE;
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

    }
}