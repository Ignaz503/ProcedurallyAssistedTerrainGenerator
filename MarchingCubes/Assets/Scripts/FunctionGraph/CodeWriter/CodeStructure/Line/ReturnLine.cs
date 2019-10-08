using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class ReturnLine : Line
    {
        RHSExpresion expresion;

        public override string LineToWrite
        {
            get
            {
                return $"return {expresion.GetAsEOL()}";
            }
        }

        public ReturnLine(RHSExpresion expresion)
        {
            this.expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
        }

        public ReturnLine()
        {}

        public void SetRHSExpression(RHSExpresion exp)
        {
            expresion = exp;
        }

        public override void Write(IndentedStreamWriter w)
        {
            w.WriteLine(LineToWrite);
        }
    }
}