using System.Collections.Generic;
using UnityEngine;

//TODO:
// Cascading of clear functions, as well as save and restore functions
namespace FuncGraph.CodeWriting
{
    public class CSharpCodeWriter : ICodeWriter
    {
        List<Namespace> namespaces;
        Namespace currentNamespace;
        public Namespace CurrentNamespace {
            get{ return currentNamespace; }
            set
            {
                currentNamespace = value ?? Namespace.GlobalNamespace;
            }
        }

        Class currentClass;
        public Class CurrentClass
        {
            get { return currentClass; }
            set
            {
                if (currentClass != null)
                {
                    Debug.LogError("Can only work on one class at a time");
                    return;
                }
                currentClass = value;
            }
        }
    
        Function currentFunction;
        public Function CurrentFunction
        {
            get { return currentFunction; }
            set
            {
                if (currentFunction != null)
                {
                    throw new System.Exception("Object not null. Did you forget to temporarily store the previous function? Or maybe you forgot to finish up the preivous function.");
                }
                currentFunction = value;
            }
        }

        Line currentLine;
        public Line CurrentRHSExpression { get { return currentLine; } }
        
        CodeStructure currentCodeStructure;
        public CodeStructure CurrentCodeStructure
        {
            get { return currentCodeStructure; }
            set
            {
                if (currentCodeStructure != null)
                {
                    throw new System.Exception("Code Structure not null. Did you forget to temporarily store the previous structure? Or maybe you forgot to finish it up.");
                }
                currentCodeStructure = value;
            }
        }

        Stack<CodeStructure> tempCodeStructureStorage;
        Stack<Line> tempLineStorage;
        Stack<Function> tempFunctionStorage;
        Stack<Class> tempClassStorage;

        public CSharpCodeWriter()
        {
            tempFunctionStorage = new Stack<Function>();
            tempCodeStructureStorage = new Stack<CodeStructure>();
            tempLineStorage = new Stack<Line>();
            tempClassStorage = new Stack<Class>();

            //Add global namespace to namespaces
            namespaces = new List<Namespace>();
            namespaces.Add(Namespace.GlobalNamespace);

        }

        public Namespace CreateNameSpace(string name, bool setAsCurrent = false)
        {
            for (int i = 0; i < namespaces.Count; i++)
            {
                if (namespaces[i].Name == name)
                {
                    if (setAsCurrent)
                        CurrentNamespace = namespaces[i];
                    return namespaces[i];
                }
            }
            namespaces.Add(new Namespace(name));
            if (setAsCurrent)
                CurrentNamespace = namespaces[namespaces.Count - 1];
            return namespaces[namespaces.Count - 1];
        }

        public void StoreClassTemporarily()
        {
            tempCodeStructureStorage.Push(currentCodeStructure);
            tempLineStorage.Push(currentLine);
            tempFunctionStorage.Push(currentFunction);
            tempClassStorage.Push(currentClass);
        }

        public void StoreClassTemporarilyAndClear()
        {
            StoreClassTemporarily();
            currentCodeStructure = null;
            currentLine  = null;
            currentFunction = null;
            currentClass = null;
        }

        public void RestorePreviousClass()
        {
            currentCodeStructure = tempCodeStructureStorage.Pop();
            currentLine = tempLineStorage.Pop();
            currentFunction = tempFunctionStorage.Pop();
            currentClass = tempClassStorage.Pop();
        }

        public void FinishCurrentClass()
        {
            //classes are added to namespace on creation global if not defined, can we changed at will
            //all we need to do is to clear everything

            currentClass = null;
            currentFunction = null;
            currentLine = null;
            currentCodeStructure = null;

            //clear
        }

        public void StoreFunctionTemporarily()
        {
            tempCodeStructureStorage.Push(currentCodeStructure);
            tempLineStorage.Push(currentLine);
            tempFunctionStorage.Push(currentFunction);
        }

        public void StoreFunctionTemporarilyAndClear()
        {
            StoreFunctionTemporarily();
            currentCodeStructure = null;
            currentLine = null;
            currentFunction = null;
        }

        public void RestorePreviousFunction()
        {
            currentCodeStructure = tempCodeStructureStorage.Pop();
            currentLine = tempLineStorage.Pop();
            currentFunction = tempFunctionStorage.Pop();
        }

        public void FinishCurrentFunction()
        {
            CurrentClass.AddFunction(currentFunction);
            currentCodeStructure = null;
            currentLine = null;
            currentFunction = null;
        }

        public void StoreCurrentCodeStructureTemporarily()
        {
            tempCodeStructureStorage.Push(currentCodeStructure);
            tempLineStorage.Push(currentLine);
        }

        public void StoreCurrentCodeStructureTemporarilyAndClear()
        {
            StoreCurrentCodeStructureTemporarily();
            currentLine = null;
            currentCodeStructure = null;
        }

        public void RestorePreviousCodeStructure()
        {
            currentCodeStructure = tempCodeStructureStorage.Pop();
            currentLine = tempLineStorage.Pop();
        }

        public void FinishCodeStructure()
        {
            currentFunction.AddCodeStructure(currentCodeStructure);
            currentCodeStructure = null;
            currentLine = null;
        }

        public void AddLineForCurrentCodeStructure(Line l)
        {
            if (currentCodeStructure != null && currentCodeStructure is MultiLineCodeStructure)
            {
                (currentCodeStructure as MultiLineCodeStructure).AddLine(l);
            }
            else
            {
                //just add the line to the function
                currentFunction.AddCodeStructure(l);
            }
        }

        public void WriteToDirectory(string path)
        {
            //for every namespace call write
            for (int i = 0; i < namespaces.Count; i++)
            {
                namespaces[i].Write(path);
            }
        }

        public void AddToCurrentLine(string toAdd)
        {
            currentLine.Append(toAdd);
        }    

    }

}