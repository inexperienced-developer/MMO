using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace InexperiencedDeveloper.Utils.Log
{
    public class IDLogger : MonoBehaviour
    {
        [SerializeField] private Transform debugArea;
        [SerializeField] private GameObject errorObj;
        private static GameObject errorText;
        private static Transform debug;
        private static ScrollRect scroll;

        private static List<GameObject> messages = new List<GameObject>();

        public static string Timestamp
        {
            get
            {
                var dateTime = DateTime.Now.ToString("HH:mm");
                return $"[{dateTime}]";
            }

        }

        public static void Log(string log)
        {
            Debug.Log($"{Timestamp} {log}");
        }

        public static void LogWarning(string log)
        {
            Debug.LogWarning($"{Timestamp} {log}");
        }

        public static void LogError(string log)
        {
            Debug.LogError($"{Timestamp} {log}");
        }

        private void Awake()
        {
            //debug = debugArea;
            //errorText = errorObj;
            //scroll = GetComponent<ScrollRect>();
        }

        //public static void Log(string log)
        //{
        //    var dateTime = DateTime.Now.ToString("HH:mm");
        //    var msg = $"[{dateTime}] {log}";
        //    var textObj = CreateMessage(msg, Color.green);
        //    AddMessage(textObj);
        //}

        //public static void LogWarning(string log)
        //{
        //    var dateTime = DateTime.Now.ToString("HH:mm");
        //    var msg = $"[{dateTime}] {log}";
        //    var textObj = CreateMessage(msg, Color.yellow);
        //    AddMessage(textObj);
        //    Debug.Log(msg);
        //}

        //public static void LogError(string log, bool pausePlayback)
        //{
        //    var dateTime = DateTime.Now.ToString("HH:mm");
        //    var msg = $"[{dateTime}] {log}";
        //    var textObj = CreateMessage(msg, Color.red);
        //    AddMessage(textObj);
        //    if (pausePlayback)
        //        Debug.LogError(msg);
        //    else
        //        Debug.LogWarning(msg);
        //}

        //private static GameObject CreateMessage(string msg, Color color)
        //{
        //    var textObj = Instantiate(errorText, debug);
        //    var text = textObj.GetComponent<TMP_Text>();
        //    text.SetText(msg);
        //    text.color = color;
        //    return textObj;
        //}

        //private static void AddMessage(GameObject msg)
        //{
        //    if (messages.Count > 25)
        //    {
        //        Destroy(messages[0]);
        //        messages.RemoveAt(0);
        //    }
        //    messages.Add(msg);
        //}
    }
}

