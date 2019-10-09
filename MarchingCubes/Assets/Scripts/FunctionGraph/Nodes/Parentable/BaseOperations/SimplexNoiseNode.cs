using System.Collections;
using System.Collections.Generic;
using System.IO;
using FuncGraph.CodeWriting;
using UnityEngine;

public class SimplexNoiseNode : FixedSizeMultiChildNode
{
    static string[] labels = new string[3] {"x","y","z"};

    public SimplexNoiseNode(FunctionGraph graph) : base(3,labels, graph)
    {
        
    }

    float x { get { return childNodes[0].Evaluate(); } }
    float y { get { return childNodes[1].Evaluate(); } }
    float z { get { return childNodes[2].Evaluate(); } }

    public override string ShortDescription { get { return "3D Simplex Noise"; } }

    public override float Evaluate()
    {
        return SimplexNoise.Evaulate(x,y,z);
    }

    public override int ValidateSelf()
    {
        return 0;
    }

    public override void Write(StreamWriter writer)
    {
        writer.Write("SimplexNoise.Evaulate( ");
        childNodes[0].Write(writer);
        writer.Write(", ");
        childNodes[1].Write(writer);
        writer.Write(", ");
        childNodes[2].Write(writer);
        writer.Write(") ");
    }

    //public override void WriteToCSharp(CSharpCodeWriter writer)
    //{
    //    throw new System.NotImplementedException();
    //}
}
