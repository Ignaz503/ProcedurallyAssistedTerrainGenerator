using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
    public abstract class Line
    {
        //Maybe unneeded
        public abstract void Write(StreamWriter w);
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

        public ReturnLine(RHSExpresion expresion)
        {
            this.expresion = expresion ?? throw new ArgumentNullException(nameof(expresion));
        }

        public override void Write(StreamWriter w)
        {
            w.Write($"return {expresion.GetAsEOL()}");
        }
    }

}