using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionGraphValidationMessageLogger
{
    public enum LoggerMode
    {
        NotifcationAndCollect,
        NotificationNoCollect,
        Collect
    }

    static FunctionGraphValidationMessageLogger instance;

    event System.Action<Message> OnMessageRecieved;

    LoggerMode mode;

    List<Message> messages;

    private FunctionGraphValidationMessageLogger()
    {
        mode = LoggerMode.Collect;
        messages = new List<Message>();
    }

    static FunctionGraphValidationMessageLogger GetInstance() {
        if (instance == null)
            instance = new FunctionGraphValidationMessageLogger();
        return instance;
    }

    public static void LogWarning(string warning) {
        GetInstance().messages.Add(new Message() { Type = Message.MessageType.Warning, Msg = warning });
    }

    public static void LogError(string error) {
        GetInstance().messages.Add(new Message() { Type = Message.MessageType.Error, Msg = error });
    }

    public static LoggerMode Mode
    {
        get {
            return GetInstance().mode;
        }
        set {
            ChangeMode(value);
        }
    }

    void NotifyAboutAlreadyCollectedMessages()
    {
        for (int i = 0; i < messages.Count; i++)
        {
            OnMessageRecieved?.Invoke(GetInstance().messages[i]);
        }
    }

    static void ChangeMode(LoggerMode newMode)
    {
        GetInstance().mode = newMode;
        //try notifiy about prev recieved msgs
        GetInstance().NotifyAboutAlreadyCollectedMessages();
    }

    public static void NotifyAlreadyRecievedMessages()
    {
        GetInstance().NotifyAboutAlreadyCollectedMessages();
    }

    void AddMessage(Message m)
    {
        switch (mode)
        {
            case LoggerMode.Collect:
                messages.Add(m);
                break;
            case LoggerMode.NotifcationAndCollect:
                messages.Add(m);
                OnMessageRecieved?.Invoke(m);
                break;
            case LoggerMode.NotificationNoCollect:
                OnMessageRecieved?.Invoke(m);
                break;
        }

    }

    public static void Print(ILogger l) {
        GetInstance().PrintMessages(l);
    }

    public static void RegisterForMessageNotifications(System.Action<Message> callback)
    {
        GetInstance().OnMessageRecieved += callback;
    }

    public static void UnregisterFromMessageNotifications(System.Action<Message> callback)
    {
        GetInstance().OnMessageRecieved -= callback;
    }

    void PrintMessages(ILogger l) {
        for (int i = 0; i < messages.Count; i++)
        {
            messages[i].Log(l);
        }
    }

    public struct Message {
        public enum MessageType {
            Warning,
            Error
        }

        public MessageType Type { get; set; }
        public string Msg { get; set; }

        public void Log(ILogger l) {
            switch (Type) {
                case MessageType.Warning:
                    LogWarning(l);
                    break;
                case MessageType.Error:
                    LogError(l);
                    break;
            }
        }

        void LogWarning(ILogger l) {
            l.LogWarning(Type.ToString(), Msg);
        }

        void LogError(ILogger l) {
            l.LogError(Type.ToString(), Msg);
        }
    }
}
