using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FuncGraph.CodeWriting
{
    public class Ctor : Function
    {
        Class classToCreate;
        public bool HasBaseConstructorCall { get { return classToCreate.HasBaseClass; } }
        List<Variable> variablePassedToBase;
        
        public Ctor(Class classToCreate,string accessor, string name) : base(accessor, "", name)
        {
            this.classToCreate = classToCreate;
            variablePassedToBase = new List<Variable>();
        }

        public Ctor(Class classToCreate, string accessor, string name, List<Parameter> parameters) : base(accessor, "", name, parameters)
        {
            this.classToCreate = classToCreate;
            variablePassedToBase = new List<Variable>();
        }

        public void AddVariablePassedToBaseCtor(Variable var)
        {
            variablePassedToBase.Add(var);
        }

        public void RemoveVariablePassedToBaseCtor(Variable var)
        {
            variablePassedToBase.Remove(var);
        }

        public override void Write(IndentedStreamWriter writer)
        {
            WriteCtorHeader(writer);
            WriteFunctionBody(writer);
        }

        void WriteCtorHeader(IndentedStreamWriter writer)
        {
            writer.WriteLine(BuildFunctionHead());
        }

        protected override string BuildFunctionHead()
        {
            StringBuilder builder = new StringBuilder(base.BuildFunctionHead());



            if (HasBaseConstructorCall)
            {
                //if has base call -> add : base(params)
                builder.Append(" : base(");

                builder.Append(parameters[0].GetAsReference());
                for (int i = 1; i < parameters.Count; i++)
                {
                    builder.Append(", ");
                    builder.Append(parameters[i].GetAsReference());
                }

                builder.Append(")");
            }


            return builder.ToString();
        }
    }
}