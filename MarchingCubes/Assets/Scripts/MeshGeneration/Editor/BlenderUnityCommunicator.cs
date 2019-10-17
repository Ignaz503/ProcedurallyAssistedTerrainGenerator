using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;
using UnityEditor;
using UnityEditor.Compilation;

namespace BlenderUnityCommunication
{
    public class BlenderUnityCommunicator
    {
        public interface ICommunicationUnityInstruction
        {
            void Execute();
        }

        public struct ConnectionForcedClosedInstruction : ICommunicationUnityInstruction
        {
            public void Execute()
            {
                //TODO restart the entire blender communication Process
                throw new NotImplementedException();
            }
        }

        public struct FinishedMeshEditingInstruction : ICommunicationUnityInstruction
        {
            public void Execute()
            {
                //read in the fbx
                throw new NotImplementedException();
            }
        }

        static BlenderUnityCommunicator instance;
        public static BlenderUnityCommunicator Instance
        {
            get
            {
                if (instance == null)
                {
                    CreateInstance(TCPCommandServer.DefaultPort);
                }
                return instance;
            }
        }

        public static void CreateInstance(int port)
        {
            instance = new BlenderUnityCommunicator(port);
        }

        public string PathToBlender;

        int port;

        TCPCommandServer server;

        CancellationTokenSource cts;

        Process blenderProcess;

        Queue<ICommunicationUnityInstruction> instructionQueue;

        private BlenderUnityCommunicator(int port)
        {
            this.port = port;
            instructionQueue = new Queue<ICommunicationUnityInstruction>();
        }

        void EditorUpdate()
        {
            if (instructionQueue.Count != 0)
            {
                HandleInstruction();
            }
        }

        private void HandleInstruction()
        {
            ICommunicationUnityInstruction inst;
            for (int i = 0; i < instructionQueue.Count; i++)
            {
                lock (instructionQueue)
                {
                    inst = instructionQueue.Dequeue();
                }
                inst.Execute();
            }
        }

        private void MakeUseableInEditor()
        {
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;

            EditorApplication.quitting -= OnQuit;
            EditorApplication.quitting += OnQuit;

            CompilationPipeline.compilationStarted -= OnRecompile;
            CompilationPipeline.compilationStarted += OnRecompile;

        }

        void OnQuit()
        {
            EditorApplication.update -= EditorUpdate;
            //TODO inform tcp thread of quit?
            if (cts != null)
                cts.Cancel();
        }

        void OnRecompile(object o)
        {
            if (server != null)
            {
                if (cts != null)
                    cts.Cancel();
            }
        }

        public void ExportChunksForEdit(GameObject[] toEdit, string filePath)
        {
            for (int i = 0; i < toEdit.Length; i++)
            {
                ModelExporter.ExportObject(filePath + $"/chunk{i}.fbx", toEdit[i]);
            }
        }

        public void StartBlender()
        {
            blenderProcess = Process.Start(PathToBlender);
        }

        public void CloseBlender()
        {
            if (blenderProcess != null)
                blenderProcess.Kill();
        }

        public void StartTCPServer()
        {
            cts = new CancellationTokenSource();
            //TODO start server and so on
            var s  = new BlenderUnityTCPCommandServer();

            //add all the data to send via tag

            //and start the server
            server = s;
            server.SetCancelationToke(cts.Token);

            Thread t = new Thread(() => { server.OpenServer(); server.TCPLoop(); });
            t.IsBackground = true; //??? can i do that
            t.Start();


        }

        public void GiveInstruction(ICommunicationUnityInstruction instruction)
        {
            lock (instructionQueue)
            {
                instructionQueue.Enqueue(instruction);
            }
        }
    }
}