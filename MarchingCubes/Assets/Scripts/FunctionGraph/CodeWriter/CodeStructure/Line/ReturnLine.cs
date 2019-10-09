using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class ReturnLine : Line
    {

        public override string LineToWrite
        {
            get
            {
                return $"return {expresion.GetAsEOL()}";
            }
        }

        public ReturnLine(string expresion)
        {
            this.expresion = new Expresion(expresion);
        }

        public ReturnLine()
        {}

        public void SetRHSExpression(string exp)
        {
            expresion = new Expresion(exp);
        }

        public override void Write(IndentedStreamWriter w)
        {
            w.WriteLine(LineToWrite);
        }

    }
}