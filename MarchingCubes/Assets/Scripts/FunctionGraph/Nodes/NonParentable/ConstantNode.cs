using System;
using System.Collections;
using System.Globalization;
using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

[EditorNodeType(EditorNodeTypeAttribute.NodeType.FloatField)]
public class ConstantNode : BaseFuncGraphNode
{
    public override string ShortDescription { get { return "Const"; } }

    public float Constant;

    public ConstantNode(FunctionGraph g):base(g)
    {}

    public override float Evaluate()
    {
        return Constant;
    }

    public override int Validate()
    {
        return 0;
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write(Constant.ToString(new CultureInfo("en-US")) +"f");
        writer.Write(" ");
    }

    //public override void WriteToCSharp(CSharpCodeWriter writer)
    //{
    //    throw new System.NotImplementedException();
    //}
}
