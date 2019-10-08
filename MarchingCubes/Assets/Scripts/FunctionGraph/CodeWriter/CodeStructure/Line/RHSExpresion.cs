using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace FuncGraph.CodeWriting
{
    public class RHSExpresion
    {
        public const string PlaceHolder = "§";

        //TODO Seems stupid think of something better
        StringBuilder expressionBuilder;

        public string Expresion { get { return expressionBuilder.ToString(); } }

        public RHSExpresion(string expresion)
        {
            expressionBuilder = new StringBuilder(expresion);
        }

        public void Append(string str)
        {
            expressionBuilder.Append(str.StartsWith(" ") ? str : " " + str);
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

        public void WriteAsAssignment(IndentedStreamWriter w, Variable v)
        {
            v.WriteAsReference(w);
            w.Write((Expresion.StartsWith("=") ? "" : "= ") + GetAsEOL());
        }

        public void WriteAfterKeyWord(IndentedStreamWriter w, string keyWord)
        {
            w.Write(keyWord + (keyWord.EndsWith(" ") ? "" : " "));
            w.Write(GetAsEOL()); 
        }

        public string GetAsEOL()
        {
            return Expresion + (Expresion.EndsWith(";") ? "" : ";");
        }

        public static implicit operator RHSExpresion(string str)
        {
            return new RHSExpresion(str);
        }

        public void Insert(string toInsert, int placeholderIndex = 0)
        {
            List<int> indices = new List<int>();
            var str = expressionBuilder.ToString();
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '§')
                    indices.Add(i);
            }

            if (placeholderIndex >= indices.Count)
                throw new Exception("Trying to insert at non existing place");

            expressionBuilder.Replace("§", toInsert, indices[placeholderIndex], 1);
        }
    }
}