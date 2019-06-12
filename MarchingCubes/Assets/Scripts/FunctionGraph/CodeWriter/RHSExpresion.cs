using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public class RHSExpresion
    {
        public string Expresion { get; protected set; }

        public RHSExpresion(string expresion)
        {
            this.Expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
        }

        public void Append(string str)
        {
            Expresion += (Expresion.EndsWith(" ") ? "" : " ") + str;
        }

        public void Append(RHSExpresion expr)
        {
            Append(expr.Expresion);
        }

        public void Write(StreamWriter writer, bool endOfLine = true)
        {
            if (endOfLine)
            {
                writer.WriteLine(GetAsEOL());
            }
            else
            {
                string exp = Expresion.EndsWith(";") ? Expresion.Remove(Expresion.Length - 1) : Expresion;
                writer.Write(exp);
            }
        }

        public void WriteAsAssignment(StreamWriter w, Variable v)
        {
            v.WriteAsReference(w);
            w.Write((Expresion.StartsWith("=") ? "" : "= ") + GetAsEOL());
        }

        public void WriteAfterKeyWord(StreamWriter w, string keyWord)
        {
            w.Write(keyWord + (keyWord.EndsWith(" ") ? "" : " "));
            w.Write(GetAsEOL()); 
        }

        public string GetAsEOL()
        {
            return Expresion + (Expresion.EndsWith(";") ? "" : ";");
        }

    }
}