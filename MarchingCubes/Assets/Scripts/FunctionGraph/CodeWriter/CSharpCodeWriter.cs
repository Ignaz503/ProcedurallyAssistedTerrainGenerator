using System.Collections.Generic;
using UnityEngine;

//TODO:
// maybe don't distinguish between ctor and function that hard
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

        Ctor currentCtor;
        public Ctor CurrentCtor
        {
            get { return currentCtor; }
            set
            {
                if (currentCtor != null)
                    throw new System.Exception("Only one ctor workable at the time, temp store current and retry");
                currentCtor = value;
            }
        }

        Line currentLine;
        public Line CurrentLine
        {
            get { return currentLine; }
            set
            {
                if (currentLine != null)
                    throw new System.Exception("only one line at a time, temp store before switching");
                currentLine = value;
            }
        }

        Stack<Ctor> constructorTemporaryStorageStack;
        Stack<Function> functionTemporaryStorageStack;
        Stack<Line> lineTemporaryStorageStack;


        public CSharpCodeWriter()
        {
            constructorTemporaryStorageStack = new Stack<Ctor>();
            functionTemporaryStorageStack = new Stack<Function>();
            lineTemporaryStorageStack = new Stack<Line>();

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

        public void WriteToDirectory(string path)
        {
            //for every namespace call write
            for (int i = 0; i < namespaces.Count; i++)
            {
                namespaces[i].Write(path);
            }
        }

        public void StoreCurrentLineTemporarily()
        {
            lineTemporaryStorageStack.Push(currentLine);
        }

        public void ClearCurrentLine()
        {
            currentLine = null;
        }

        public void StoreAndClearCurrentLine()
        {
            StoreCurrentLineTemporarily();
            ClearCurrentLine();
        }

        public void RestorePreviousLine()
        {
            currentLine = lineTemporaryStorageStack.Pop();
        }

        public void FinishCurrentLine(bool isCtorLine=false)
        {
            if (isCtorLine)
            {
                currentCtor.AddCodeStructure(currentLine);
            }
            else
            {
                currentFunction.AddCodeStructure(currentLine);
            }
            //clear
            ClearCurrentLine();
        }

        public void StoreCurrentFunctionTemporarily(bool cascade = true)
        {
            functionTemporaryStorageStack.Push(currentFunction);
            if (cascade)
                StoreCurrentLineTemporarily();
        }

        public void ClearCurrentFunction(bool cascade = true)
        {
            currentFunction = null;
            if (cascade)
                ClearCurrentLine();
        }

        public void StoreAndClearCurrentFunction(bool cascade = true)
        {
            StoreCurrentFunctionTemporarily(cascade);
            ClearCurrentFunction(cascade);
        }

        public void FinishCurrentFunction(bool cascade = true)
        {
            if (cascade)
                FinishCurrentLine();
            CurrentClass.AddFunction(currentFunction);
            ClearCurrentFunction(cascade);
        }

        public void StoreConstructorTemporarily(bool cascade = true)
        {
            constructorTemporaryStorageStack.Push(currentCtor);
            if (cascade)
                StoreCurrentLineTemporarily();
        }

        public void ClearCurrentConstructor(bool cascade = true)
        {
            currentCtor = null;
            if (cascade)
                ClearCurrentLine();
        }

        public void StoreAndClearCurrentCtor(bool cascade = true)
        {
            StoreConstructorTemporarily(cascade);
            ClearCurrentConstructor(cascade);
        }

        public void FinishCurrentConstructor(bool cascade = true)
        {
            if (cascade)
                FinishCurrentLine(isCtorLine: true);
            CurrentClass.AddCtor(currentCtor);
            ClearCurrentConstructor(cascade);
        }

    }

}