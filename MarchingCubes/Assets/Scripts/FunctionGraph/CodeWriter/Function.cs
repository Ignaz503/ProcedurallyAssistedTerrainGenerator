using System;
using System.Collections.Generic;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class Function
    {
        protected string accessor;
        protected string returnType;
        protected string name;
        protected List<Parameter> parameters;
        protected List<Local> locals;
        protected List<CodeStructure> codeStructures;

        public Function(string accessor, string returnType, string name)
        {
            this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
            this.returnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.parameters = new List<Parameter>();
            this.locals = new List<Local>();
            this.codeStructures = new List<CodeStructure>();
        }

        public Function(string accessor, string returnType, string name, List<Parameter> parameters) : this(accessor, returnType, name)
        {
            this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }

        public void AddParameter(Parameter param)
        {
            parameters.Add(param);
        }

        public void AddLocal(Local l)
        {
            locals.Add(l);
        }

        public void AddLine(CodeStructure l)
        {
            codeStructures.Add(l);
        }

        public string GetLocalName(string furtherIdentifier)
        {
            return $"local{locals.Count}_{furtherIdentifier}";
        }

        public void InsertLine(CodeStructure l, int idx)
        {
            codeStructures.Insert(idx, l);
        }

        public virtual void Write(StreamWriter writer)
        {
            //TODO
            throw new NotImplementedException();
        }

        public bool PartialEquals(string returnType, List<Parameter> parameters)
        {
            if (returnType != this.returnType)
                return false;
            if (parameters == null && this.parameters != null && this.parameters.Count > 0)
                return false;
            if (parameters != null && this.parameters == null && parameters.Count > 0)
                return false;
            if (parameters != null && this.parameters != null && parameters.Count != this.parameters.Count)
                return false;
            bool same = true;
            for (int i = 0; i < this.parameters.Count; i++)
            {
                if (!parameters[i].Equals(this.parameters[i]))
                {
                    same = false;
                    break;
                }
            }
            return same;
        }

        public bool PartialEquals(string name)
        {
            return this.name.Contains(name);
        }

        public bool EqualsSubstring(string retutnrType, string substring, List<Parameter> parameters)
        {
            if (!this.name.Contains(substring))
                return false;
            return PartialEquals(returnType, parameters);
        }

        public bool Equals(string returnType, string name, List<Parameter> parameters)
        {
            if (name != this.name)
                return false;
            return PartialEquals(returnType, parameters);
        }

        public bool HasLocal(string name)
        {
            for (int i = 0; i < locals.Count; i++)
            {
                if (locals[i].PartialEquals(name))
                    return true;
            }
            return false;
        }

        public bool HasLocalSubstring(string substring)
        {
            for (int i = 0; i < locals.Count; i++)
            {
                if (locals[i].PartialEqualsSubstring(substring))
                    return true;
            }
            return false;
        }

        public Local GetLocal(string name)
        {
            for (int i = 0; i < locals.Count; i++)
            {
                if (locals[i].PartialEquals(name))
                    return locals[i];
            }
            return null;
        }

        public Local GetLocalSubstring(string subString)
        {
            for (int i = 0; i < locals.Count; i++)
            {
                if (locals[i].PartialEqualsSubstring(subString))
                    return locals[i];
            }
            return null;
        }

    }
}