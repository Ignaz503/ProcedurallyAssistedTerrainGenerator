using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
