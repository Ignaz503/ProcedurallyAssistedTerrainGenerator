using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace FuncGraph.CodeWriting
{
    public class IndentedStreamWriter
    {
        const string tab = "\t";

        StreamWriter writer;
        int currentIndentLevel;
        string indentString;

        public IndentedStreamWriter(StreamWriter writer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            currentIndentLevel = 0;
            indentString = "";
        }

        public void IncreaseIndentLevel(int increase)
        {
            increase = Mathf.Abs(increase);

            for (int i = 0; i < increase; i++)
            {
                indentString += tab;
            }
            currentIndentLevel += increase; 
        }

        public void DecreaseIndentLevel(int decrease)
        {
            decrease = Mathf.Abs(decrease);
            if (currentIndentLevel - decrease <= 0)
            {
                currentIndentLevel = 0;
                indentString = "";
            }
            else
            {
                //for every decrease remove from string
                for (int i = 0; i < decrease; i++)
                {
                    //TODO
                }
                currentIndentLevel -= decrease;
            }
        }

        public void WriteLine(string line)
        {
            //TODO
            throw new NotImplementedException();
        }

        public void Write(string toWrite)
        {
            //TODO
            throw new NotImplementedException();
        }

    }
}
