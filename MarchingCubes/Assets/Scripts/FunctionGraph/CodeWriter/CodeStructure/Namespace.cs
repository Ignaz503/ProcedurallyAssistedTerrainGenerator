using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FuncGraph.CodeWriting
{
    public class Namespace
    {
        protected class NoNamespace : Namespace
        {
            public NoNamespace() : base("")
            { }

            protected override void WriteBodyEnd(IndentedStreamWriter writer)
            { }
            protected override void WriteBodyStart(IndentedStreamWriter writer)
            { }
            protected override void WriteHeader(IndentedStreamWriter writer)
            { }
        }

        protected static NoNamespace globalNamespace;

        public static Namespace GlobalNamespace
        {
            get
            {
                if (globalNamespace == null)
                {
                    globalNamespace = new NoNamespace();
                }
                return globalNamespace;
            }
        }

        public string Name { get; protected set; }
        List<Class> classesInNameSpace;
        public IEnumerable<Class> ClassesInNameSpace { get { return classesInNameSpace; } }

        public bool HasClasses { get { return classesInNameSpace.Count > 0; } }

        public Namespace(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            classesInNameSpace = new List<Class>();
        }

        public void CreateNewClassInNameSpace(string name, string baseClass = "")
        {
            classesInNameSpace.Add(new Class(name, baseClass));
        }

        public void AddClass(Class c)
        {
            classesInNameSpace.Add(c);
        }

        public void Write(string pathDirectory)
        {
            //Write every class in namepace
            for (int i = 0; i < classesInNameSpace.Count; i++)
            {
                //open streamwriter 
                using (var sw = new StreamWriter(pathDirectory + $"/{classesInNameSpace[i].Name}.cs"))
                {
                    var writer = new IndentedStreamWriter(sw);
                    //write namspace header and start body
                    WriteHeader(writer);
                    WriteBodyStart(writer);
                    //write class
                    writer.IncreaseIndentLevel();
                    classesInNameSpace[0].Write(writer);
                    writer.DecreaseIndentLevel();
                    //end namespace body
                    WriteBodyEnd(writer);
                }
            }
            //done
        }

        protected virtual void WriteHeader(IndentedStreamWriter writer)
        {
            writer.WriteLine($"namespace {Name}");
        }

        protected virtual void WriteBodyStart(IndentedStreamWriter writer)
        {
            writer.WriteLine("{");
        }

        protected virtual void WriteBodyEnd(IndentedStreamWriter writer)
        {
            writer.WriteLine("}");
        }

        public void RemoveClass(Class classToRem)
        {
            classesInNameSpace.Remove(classToRem);
        }

        public override bool Equals(object obj)
        {
            if (obj is Namespace)
            {
                return (obj as Namespace).Name == Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 1645094557;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<List<Class>>.Default.GetHashCode(classesInNameSpace);
            hashCode = hashCode * -1521134295 + EqualityComparer<IEnumerable<Class>>.Default.GetHashCode(ClassesInNameSpace);
            hashCode = hashCode * -1521134295 + HasClasses.GetHashCode();
            return hashCode;
        }

    }
}
