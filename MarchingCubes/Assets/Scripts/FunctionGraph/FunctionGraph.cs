using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FuncGraph.CodeWriting;

public class FunctionGraph 
{
    public enum VariableNames
    {
        X,
        Y,
        Z,
        XLocal,
        YLocal,
        ZLocal
    }

    Dictionary<VariableNames, float> variables;

    public BaseFuncGraphNode RootNode { get;  set; }

    public string GraphName;

    public FunctionGraph()
    {
        variables = new Dictionary<VariableNames, float>()
        {
            {VariableNames.X, 0 },
            {VariableNames.Y, 0 },
            {VariableNames.Z, 0 },
            {VariableNames.XLocal, 0 },
            {VariableNames.YLocal, 0 },
            {VariableNames.ZLocal, 0 }
        };
    }

    public float GetVariableValue(VariableNames name)
    {
        return variables[name];
    }

    protected void SetVariables(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
    {
        variables[VariableNames.X] = x.ValueWorld;
        variables[VariableNames.Y] = y.ValueWorld;
        variables[VariableNames.Z] = z.ValueWorld;
        variables[VariableNames.XLocal] = x.ValueLocal;
        variables[VariableNames.YLocal] = y.ValueLocal;
        variables[VariableNames.ZLocal] = z.ValueLocal;
    }

    public float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)
    {
        SetVariables(x, y, z);
        return RootNode.Evaluate();
    }


    public int ValidateGraph(ILogger l)
    {
        FunctionGraphValidationMessageLogger.Mode = FunctionGraphValidationMessageLogger.LoggerMode.NotificationNoCollect;
        FunctionGraphValidationMessageLogger.RegisterForMessageNotifications((m) => m.Log(l));
        if (RootNode == null)
        {
            FunctionGraphValidationMessageLogger.LogError("No Root Node defined!");
            return 1;
        }
        return RootNode.Validate();
    }
#if UNITY_EDITOR
    public void ValidateGraphEditor()
    {
        ValidateGraph(Debug.unityLogger);
    }
#endif

    public void Write(StreamWriter writer)
    {
        writer.Write("using System;\n" +
        "using System.Collections;\n" +
        "using System.Collections.Generic;\n" +
        "using UnityEngine;\n\n");
        writer.WriteLine($"public class {GraphName} : IDensityFunc");
        writer.WriteLine("{");


        writer.WriteLine("\tpublic float Evaluate(SamplePointVariables x, SamplePointVariables y, SamplePointVariables z)");
        writer.WriteLine("\t{");
        writer.Write("\t\treturn ");
        RootNode.Write(writer);
        writer.Write(";\n");

        writer.WriteLine("\t}");

        writer.WriteLine("}");
        
    }

    public void WriteToCSharp(string directoryPath)
    {
        CSharpCodeWriter codeWriter = new CSharpCodeWriter();


        codeWriter.CreateNameSpace($"Compiled._{GraphName}");
        codeWriter.CurrentClass = new Class(GraphName, nameSpace:codeWriter.CurrentNamespace);
        codeWriter.CurrentClass.AddInterface(typeof(IDensityFunc).Name);

        codeWriter.CurrentClass.AddUsingDirective(new UsingDirective("System"));
        codeWriter.CurrentClass.AddUsingDirective(new UsingDirective("System.Collections"));
        codeWriter.CurrentClass.AddUsingDirective(new UsingDirective("System.Collections.Generic"));
        codeWriter.CurrentClass.AddUsingDirective(new UsingDirective("UnityEngine"));

        var ctor = codeWriter.CurrentClass.CreateACtor();
        codeWriter.CurrentClass.AddCtor(ctor);
        //TODO make things that have public as to string, maybe base types as well, get way to easier get the density func interface function
        codeWriter.CurrentFunction = new Function(
                                                    "public",
                                                    "float",
                                                    "Evaluate",
                                                    new List<Parameter>()
                                                    {
                                                        new Parameter(typeof(SamplePointVariables).Name,"x") ,
                                                        new Parameter(typeof(SamplePointVariables).Name,"y") ,
                                                        new Parameter(typeof(SamplePointVariables).Name,"z")
                                                    });
        codeWriter.CurrentCodeStructure = new ReturnLine();
        RootNode.WriteToCSharp(codeWriter);

        (codeWriter.CurrentCodeStructure as ReturnLine).SetRHSExpression(codeWriter.CurrentRHSExpression);
        codeWriter.FinishCodeStructure();
        codeWriter.FinishCurrentFunction();
        codeWriter.FinishCurrentClass();

        codeWriter.WriteToDirectory(directoryPath);

    }

}
