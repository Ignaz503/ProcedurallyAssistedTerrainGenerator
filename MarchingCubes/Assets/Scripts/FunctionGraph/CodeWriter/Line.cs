using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public abstract class CodeStructure
    {
        //Maybe unneeded
        public abstract void Write(StreamWriter w);
    }

    public abstract class Line : CodeStructure
    {
        public abstract string LineToWrite { get; }
    }

    public class BasicLine : Line
    {
        string lineToWrite;

        public override string LineToWrite
        {
            get
            {
                return lineToWrite;
            }
        }

        public BasicLine(string lineToWrite)
        {
            this.lineToWrite = lineToWrite ?? throw new ArgumentNullException(nameof(lineToWrite));
        }

        public override void Write(StreamWriter w)
        {
            w.WriteLine(lineToWrite);
        }
    }

    public class EmptyLine : BasicLine
    {
        public EmptyLine() : base(" ")
        {

        }
    } 

    public abstract class AssignmentLine : Line
    {
        protected RHSExpresion expresion;

        public AssignmentLine( RHSExpresion expresion)
        {
            this.expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
        }

        protected void WriteAssignment(StreamWriter w)
        {
            w.Write($"= {expresion.GetAsEOL()}");
        }
    }

    public class LocalAssignmentLine : AssignmentLine
    {
        protected Local local;
        protected bool asInitializer;

        public override string LineToWrite
        {
            get
            {
                return asInitializer ? ( local.GetAsDefinition() + $"= {expresion.GetAsEOL()}") : ( local.GetAsReference() + $"= {expresion.GetAsEOL()}");
            }
        }

        public LocalAssignmentLine(Local local, RHSExpresion initializer, bool asInitializer = false) : base(initializer)
        {
            this.local = local ?? throw new ArgumentNullException(nameof(local));
            this.asInitializer = asInitializer;
        }

        public override void Write(StreamWriter w)
        {
            if (asInitializer)
            {
                local.WriteAsDefinitionWithInitializer(w,expresion);
            }
            else
            {
                local.WriteAsReference(w);
                WriteAssignment(w);
            }
        }
    }

    public class LocalInitializerLine : LocalAssignmentLine
    {
        public LocalInitializerLine(Local l, RHSExpresion expr) : base(l, expr, true)
        {}
    }

    public class MemberAssignmentLine : AssignmentLine
    {
        Member mem;

        public override string LineToWrite
        {
            get
            {
                return $"{mem.GetAsReference()} = {expresion.GetAsEOL()}";
            }
        }

        public MemberAssignmentLine(Member mem, RHSExpresion expresion) : base(expresion)
        {
            this.mem = mem ?? throw new ArgumentNullException(nameof(mem));
        }

        public override void Write(StreamWriter w)
        {
            mem.WriteWithAssignment(w, expresion);
        }
    }

    public class ReturnLine : Line
    {
        RHSExpresion expresion;

        public override string LineToWrite
        {
            get
            {
                return $"return {expresion.GetAsEOL()}";
            }
        }

        public ReturnLine(RHSExpresion expresion)
        {
            this.expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
        }

        public override void Write(StreamWriter w)
        {
            w.WriteLine(LineToWrite);
        }
    }

    public abstract class MultiLineCodeStructure : CodeStructure
    {
        protected List<Line> lines;

        public MultiLineCodeStructure()
        {
            lines = new List<Line>();
        }

        public override void Write(StreamWriter w)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].Write(w);
            }
        }

    }

    public class ForLoopHeader : Line
    {
        Line initAssignment;
        RHSExpresion condition;
        Line increment;

        public override string LineToWrite
        {
            get
            {
                string incr = increment.LineToWrite.EndsWith(";") ? increment.LineToWrite.Remove(increment.LineToWrite.Length - 1) : increment.LineToWrite;
                string initAssign = initAssignment.LineToWrite.EndsWith(";") ? initAssignment.LineToWrite : initAssignment.LineToWrite + ";";
                return $"for({initAssign} {condition.GetAsEOL()} {incr})";
            }
        }

        public ForLoopHeader(Line initAssignment, RHSExpresion condition, Line increment)
        {
            this.initAssignment = initAssignment ?? throw new ArgumentNullException(nameof(initAssignment));
            this.condition = condition ?? throw new ArgumentNullException(nameof(condition));
            this.increment = increment ?? throw new ArgumentNullException(nameof(increment));
        }

        public override void Write(StreamWriter w)
        {
            w.Write(LineToWrite);
        }
    }

    public class ForLoopStructure : MultiLineCodeStructure
    {
        public ForLoopStructure(Line incrementerInitAssign, RHSExpresion condition, Line incrementer, List<Line> linesInBody) : base()
        {
            
            lines.Add(new ForLoopHeader(incrementerInitAssign, condition, incrementer));
            lines.Add(new BasicLine("{"));
            if (linesInBody != null)
            {
                for (int i = 0; i < linesInBody.Count; i++)
                {
                    lines.Add(linesInBody[i]);
                }
            }
            lines.Add(new BasicLine("}"));
        }

        public ForLoopStructure(Line incrementerAssignment, RHSExpresion condition, Line incrementer, Line lineInBody):base()
        {


            lines.Add(new ForLoopHeader(incrementerAssignment, condition, incrementer));
            lines.Add(new BasicLine("{"));
            lines.Add(lineInBody);
            lines.Add(new BasicLine("}"));
        }
    }
}