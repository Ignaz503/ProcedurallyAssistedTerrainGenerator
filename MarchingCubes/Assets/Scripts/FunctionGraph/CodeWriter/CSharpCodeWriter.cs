using System.Collections.Generic;
using UnityEngine;

namespace FuncGraph.CodeWriting
{
    public class CSharpCodeWriter : ICodeWriter
    {
        Class _currentClass;
        public Class CurrentClass
        {
            get { return _currentClass; }
            set
            {
                if (_currentClass != null)
                {
                    Debug.LogError("Can Only one class at a time");
                    return;
                }
                _currentClass = value;
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

        public CSharpCodeWriter()
        {
            tempFunctionStorage = new Stack<Function>();
            tempCodeStructureStorage = new Stack<CodeStructure>();
            tempRHSExprStorage = new Stack<RHSExpresion>();
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