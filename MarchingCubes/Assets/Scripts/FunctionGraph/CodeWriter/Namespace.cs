﻿using System;
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
            {
            }

            protected override void WriteBodyEnd(StreamWriter writer)
            {

            }
            protected override void WriteBodyStart(StreamWriter writer)
            { }
            protected override void WriteHeader(StreamWriter writer)
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

        public void Write(StreamWriter writer)
        {
            //TODO: Write every class in namepace
            //add namsepace at start
        }

        protected virtual void WriteHeader(StreamWriter writer)
        {
            writer.WriteLine($"namespace {Name}");
        }

        protected virtual void WriteBodyStart(StreamWriter writer)
        {
            writer.WriteLine("{");
        }

        protected virtual void WriteBodyEnd(StreamWriter writer)
        {
            writer.WriteLine("}");
        }

        public void RemoveClass(Class classToRem)
        {
            classesInNameSpace.Remove(classToRem);
        }
    }
}
