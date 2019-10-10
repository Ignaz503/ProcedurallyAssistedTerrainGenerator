using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
    using UnityEditor;
    using UnityEditor.SceneManagement;
#endif


public class ThreadedDataRequester
{

    static ThreadedDataRequester instance;

    public static ThreadedDataRequester Instance {
        get {
            if (instance == null) {
                instance = CreateInstance();
            }
            return instance;
        }

    }

    Queue<ThreadInfo> dataQueue = new Queue<ThreadInfo>();

    public void RequestData(Func<object> generateData, Action<object> callback)
    {
        ThreadStart tStart = delegate {
            instance.DataThread(generateData, callback);
        };
        Thread t = new Thread(tStart) { IsBackground = true };
        t.Start();
    }

    void DataThread(Func<object> generateData, Action<object> callback)
    {

        object data = generateData();

        lock (dataQueue)
        {
            dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }

    private void Update()
    {
        HandleDataQueue();
    }

    void HandleDataQueue()
    {
        if (dataQueue.Count > 0)
        {
            for (int i = 0; i < dataQueue.Count; i++)
            {
                ThreadInfo threadInfo = dataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    struct ThreadInfo
    {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> callback, object parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }

    private ThreadedDataRequester()
    {}

    static ThreadedDataRequester CreateInstance() {

        if (instance != null)
            return instance;

        var inst =  new ThreadedDataRequester();
        //after recompile?
        #if UNITY_EDITOR
            inst.MakeUseableInEditor();
        #endif
        return inst;
 
    }

#if UNITY_EDITOR


    public void MakeUseableInEditor()
    {
        instance = this;
        EditorApplication.update -= Update;
        EditorApplication.update += Update;
        EditorApplication.quitting -= OnQuit;
        EditorApplication.quitting += OnQuit;
    }

    private void OnQuit()
    {
        EditorApplication.update -= Update;
    }

    public void EditorHello() {
        Debug.Log("Threaded Data Requester is now usable in editor mode");
    }

#endif

}