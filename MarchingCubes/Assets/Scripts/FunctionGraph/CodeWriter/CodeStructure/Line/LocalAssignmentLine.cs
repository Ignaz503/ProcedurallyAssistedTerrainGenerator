using System;
using System.IO;

namespace FuncGraph.CodeWriting
{
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

        public LocalAssignmentLine(Local local, string initializer, bool asInitializer = false) : base(initializer)
        {
            this.local = local ?? throw new ArgumentNullException(nameof(local));
            this.asInitializer = asInitializer;
        }

        public override void Write(IndentedStreamWriter w)
        {
            if (asInitializer)
            {
                string def = local.GetAsDefinition();
                w.Write(def);
                WriteAssignment(w);
            }
            else
            {
                local.WriteAsReference(w);
                WriteAssignment(w);
            }
        }

    }
}