using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class CodeWriter
{
    public abstract void WriteToFile(string path);
}


public class RHSExpresion
{
    public string Expresion { get; protected set; }

    public RHSExpresion(string expresion)
    {
        this.Expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
    }

    public void Append(string str)
    {
        Expresion += (Expresion.EndsWith(" ") ? "":" ")  + str;
    }

    public void Append(RHSExpresion expr)
    {
        Append(expr.Expresion);
    }

    public void Write(StreamWriter writer, bool endOfLine = true)
    {
        if (endOfLine)
        {
            string expr= Expresion.EndsWith(";") ? Expresion : Expresion + ";";
            writer.WriteLine(expr);
        }
        else
        {
            string exp = Expresion.EndsWith(";") ? Expresion.Remove(Expresion.Length -1): Expresion;
            writer.Write(exp);
        }
    }
}

public abstract class Variable
{
    protected string type;
    protected string name;

    public Variable(string type, string name)
    {
        this.type = type ?? throw new ArgumentNullException(nameof(type));
        this.name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public abstract void Write(StreamWriter writer);

}

public class Member : Variable
{
    protected string accessor;

    public string Name
    {
        get { return name; }
    }

    public Member(string accessor, string type, string name) : base(type, name)
    {
        this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
    }

    public override void Write(StreamWriter writer)
    {
        writer.WriteLine($"{accessor} {type} {name};");
    }
}

public class Local : Variable
{
    string initializer;

    public string Name
    {
        get { return name; }
    }

    public Local( string type, string name, string initializer,) : base(type, name)
    {
        initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
    }

    public override void Write(StreamWriter writer)
    {
        string init = initializer.EndsWith(";") ? initializer : initializer + ";";
        writer.WriteLine($"{type} {name} = {init};");
    }
}

public class Parameter : Variable
{
    public Parameter(string type, string name) : base(type, name)
    {}

    public override void Write(StreamWriter writer)
    {
        writer.Write($"{type} {name}");
    }
}

public class Function
{
    protected string accessor;
    protected string returnType;
    protected string name;
    protected List<Parameter> parameters;
    protected List<Local> locals;
    protected List<Line> lines;

    public Function(string accessor, string returnType, string name)
    {
        this.accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        this.returnType = returnType ?? throw new ArgumentNullException(nameof(returnType));
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.parameters = new List<Parameter>();
        this.locals = new List<Local>();
        this.lines = new List<Line>();
    }

    public Function(string accessor, string returnType, string name, List<Parameter> parameters) : this(accessor, returnType, name)
    {
        this.parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
    }
}

public abstract class Line
{
    public abstract void Write(StreamWriter w);
}