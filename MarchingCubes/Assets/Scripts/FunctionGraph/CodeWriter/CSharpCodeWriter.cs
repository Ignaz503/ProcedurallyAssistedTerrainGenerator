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
        public Namespace CurrentNameSpace {
            get{ return currentNamespace; }
            set
            {
                if (currentNamespace != null)
                {
                    Debug.LogError("Only One Active Namsepace at a time.Call TempStore and restore after the fact.");
                    return;
                }
                currentNamespace = value;
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

        RHSExpresion currentRhsExpression;
        
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
        Stack<RHSExpresion> tempRHSExprStorage;
        Stack<Function> tempFunctionStorage;
        Stack<Class> tempClassStorage;

        public CSharpCodeWriter()
        {
            tempFunctionStorage = new Stack<Function>();
            tempCodeStructureStorage = new Stack<CodeStructure>();
            tempRHSExprStorage = new Stack<RHSExpresion>();
        }

        public void SwitchNameSpace(string name)
        {
            //TODO
        }

        public void CreateNameSpace(string name)
        {
            //TODO
        }

        public void StoreClassTemporarily()
        {
            tempCodeStructureStorage.Push(currentCodeStructure);
            tempRHSExprStorage.Push(currentRhsExpression);
            tempFunctionStorage.Push(currentFunction);
            tempClassStorage.Push(currentClass);
        }

        public void StoreClassTemporarilyAndClear()
        {
            StoreClassTemporarily();
            currentCodeStructure = null;
            currentRhsExpression = "";
            currentFunction = null;
            currentClass = null;
        }

        public void RestorePreviousClass()
        {
            currentCodeStructure = tempCodeStructureStorage.Pop();
            currentRhsExpression = tempRHSExprStorage.Pop();
            currentFunction = tempFunctionStorage.Pop();
            currentClass = tempClassStorage.Pop();
        }

        public void FinishCurrentClass()
        {
            //TODO
            //check if current namespace not null if so go to special namespace
            //Add current class to namespace
            //clear
        }

        public void StoreFunctionTemporarily()
        {
            tempCodeStructureStorage.Push(currentCodeStructure);
            tempRHSExprStorage.Push(currentRhsExpression);
            tempFunctionStorage.Push(currentFunction);
        }

        public void StoreFunctionTemporarilyAndClear()
        {
            StoreFunctionTemporarily();
            currentCodeStructure = null;
            currentRhsExpression = "";
            currentFunction = null;
        }

        public void RestorePreviousFunction()
        {
            currentCodeStructure = tempCodeStructureStorage.Pop();
            currentRhsExpression = tempRHSExprStorage.Pop();
            currentFunction = tempFunctionStorage.Pop();
        }

        public void FinishCurrentFunction()
        {
            CurrentClass.AddFunction(currentFunction);
            currentCodeStructure = null;
            currentRhsExpression = "";
            currentFunction = null;
        }

        public void StoreCurrentCodeStructureTemporarily()
        {
            tempCodeStructureStorage.Push(currentCodeStructure);
            tempRHSExprStorage.Push(currentRhsExpression);
        }

        public void StoreCurrentCodeStructureTemporarilyAndClear()
        {
            StoreCurrentCodeStructureTemporarily();
            currentRhsExpression = "";
            currentCodeStructure = null;
        }

        public void RestorePreviousCodeStructure()
        {
            currentCodeStructure = tempCodeStructureStorage.Pop();
            currentRhsExpression = tempRHSExprStorage.Pop();
        }

        public void FinishCodeStructure()
        {
            currentFunction.AddCodeStructure(currentCodeStructure);
            currentCodeStructure = null;
            currentRhsExpression = "";
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

        public void WriteToFile(string path)
        {
            throw new System.NotImplementedException();
        }

        public void AddToCurrentRHS(RHSExpresion toAdd)
        {
            currentRhsExpression.Append(toAdd);
        }

        public void AddToCurrentRHS(string toAdd)
        {
            currentRhsExpression.Append(toAdd);
        }

    }

}