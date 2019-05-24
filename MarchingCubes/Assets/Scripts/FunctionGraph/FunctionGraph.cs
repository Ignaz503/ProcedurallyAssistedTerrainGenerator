using System;
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

    Dictionary<VariableNames, Func<float>> variables;

    public BaseFuncGraphNode RootNode { get;  set; }

    public FunctionGraph()
    {
        variables = new Dictionary<VariableNames, System.Func<float>>()
        {
            {VariableNames.X, null },
            {VariableNames.Y, null },
            {VariableNames.Z, null },
            {VariableNames.XLocal, null },
            {VariableNames.YLocal, null },
            {VariableNames.ZLocal, null }
        };
    }

    public float GetVariableValue(VariableNames name)
    {
        if (variables.ContainsKey(name)) {
            return variables[name]();
        }
        return 0;//problem
    }

    public void SetVariables(SamplePointCoordValueFunctionPair x, SamplePointCoordValueFunctionPair y, SamplePointCoordValueFunctionPair z)
    {
        variables[VariableNames.X] = x.ValueWorld;
        variables[VariableNames.Y] = y.ValueWorld;
        variables[VariableNames.Z] = z.ValueWorld;
        variables[VariableNames.XLocal] = x.ValueLocal;
        variables[VariableNames.YLocal] = y.ValueLocal;
        variables[VariableNames.ZLocal] = z.ValueLocal;
    }

    public float Evaluate()
    {
        return RootNode.Evaluate();
    }

    public struct SamplePointCoordValueFunctionPair
    {
        public Func<float> ValueWorld;
        public Func<float> ValueLocal;
    }

    public int ValidateGraph(ILogger l)
    {
        FunctionGraphValidationMessageLogger.RegisterForMessageNotifications((m) => m.Log(l));
        FunctionGraphValidationMessageLogger.Mode = FunctionGraphValidationMessageLogger.LoggerMode.NotificationNoCollect;
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
}
