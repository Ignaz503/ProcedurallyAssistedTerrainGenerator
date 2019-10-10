using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace FuncGraph.CodeWriting
{
    public abstract class Line : CodeStructure
    {
        protected class Expresion
        {
            public const string PlaceHolder = "§";

            //TODO Seems stupid think of something better
            StringBuilder expressionBuilder;

            public string Content { get { return expressionBuilder.ToString(); } }

            public Expresion(string expresion)
            {
                expressionBuilder = new StringBuilder(expresion);
            }

            public void Append(string str)
            {
                expressionBuilder.Append(expressionBuilder.ToString().EndsWith(" ") || str.StartsWith(" ") ? str : " " + str);
            }

            public void Append(Expresion expr)
            {
                Append(expr.ToString());
            }

            public void WriteAsAssignment(IndentedStreamWriter w, Variable v)
            {
                v.WriteAsReference(w);
                w.Write((Content.ToString().StartsWith("=") ? "" : "= ") + GetAsEOL());
            }

            public void WriteAfterKeyWord(IndentedStreamWriter w, string keyWord)
            {
                w.Write(keyWord + (keyWord.EndsWith(" ") ? "" : " "));
                w.Write(GetAsEOL());
            }

            public string GetAsEOL()
            {
                return Content.ToString() + (Content.ToString().EndsWith(";") ? "" : ";");
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

        protected Expresion expresion;

        public Line()
        {
            expresion = new Expresion("");
        }

        public abstract string LineToWrite { get; }

        public void Append(string str)
        {
            expresion.Append(str);
        }

    }
}