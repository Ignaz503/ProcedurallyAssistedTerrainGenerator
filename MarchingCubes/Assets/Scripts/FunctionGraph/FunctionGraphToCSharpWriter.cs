#pragma warning disable 649 // disable unassigned variable warning
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionGraphToCSharpCompiler 
{
    string className;
    List<string> usingLines;
    List<string> locals;
    List<string> members;
    List<string> ctorLines;
    List<string> methodLines;

    string currentLine;

    public FunctionGraphToCSharpCompiler()
    {
        usingLines = new List<string>();
        locals = new List<string>();
        members = new List<string>();
        ctorLines = new List<string>();
        methodLines = new List<string>();
        currentLine = " ";
    }


    /// <summary>
    /// sets class name
    /// </summary>
    /// <param name="className"></param>
    public void SetClassName(string className)
    {
        this.className = className;
    }

    /// <summary>
    /// append to current line
    /// </summary>
    /// <param name="appendum">what to append</param>
    public void AppendToCurrentLine(string appendum)
    {
        if (!appendum.EndsWith(" "))
            appendum += " ";
        //TODO
        currentLine += appendum;
    }
    /// <summary>
    /// ends current line and returns the reference to this line
    /// eg:
    /// temp2 = a + b;
    /// funtion returns temp2
    /// </summary>
    public string EndCurrentLineAddToLinesToWrite()
    {
        methodLines.Add(currentLine + " ;");
        currentLine = " ";
        return $"temp{methodLines.Count - 1}";
    }

    /// <summary>
    /// returns the current line for temp storage
    /// </summary>
    public string TemporarilyEndCurrentLine()
    {
        string st = currentLine;
        currentLine = " ";
        return st;
    }


    /// <summary>
    /// Adds a local var to the function
    /// </summary>
    /// <param name="localCtorLine">something like new System.Random()</param>
    /// <returns>the name of the local to reference</returns>
    public string AddLocal(string localCtorLine)
    {
        locals.Add(localCtorLine);
        return $"local{locals.Count - 1}";
    }

    /// <summary>
    /// adds a member, and the correct line for the cotr of the object
    /// </summary>
    /// <param name="memerbType">A type string eg: int</param>
    /// <param name="memberCtorLine">the ctor line eg new int[12]</param>
    /// <returns>the string with which to reference this member</returns>
    public string AddMember(string memerbType, string memberCtorLine)
    {
        string memb = $"member{members.Count}";

        members.Add($"{memerbType} {memb}");
        ctorLines.Add($"{memb} = {memberCtorLine}");

        return memb;
    }

    public void AddUsingDirective(string nameSpace)
    {
        if (!usingLines.Contains(nameSpace))
        {
            //new namespace
            usingLines.Add(nameSpace);
        }
    }

    public void WriteMethodStart()
    {
        //TOO
    }

    public void WriteMethodEnd()
    {

    }


    void WriteEnd()
    {
    }

    public void WriteTo(string path)
    {
        //TODO
    }
}
