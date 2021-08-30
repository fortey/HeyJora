using Jint;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class JSCompiler : MonoBehaviour
{
    public Text text;
    public Text codeText;
    public InputField inputField;
    //private string code;

    private Engine engine;

	private void Awake()
	{
        engine = new Engine();
        engine.SetValue("log", new Action<object>(msg => {
            string textMsg;
            if (msg == null)
                textMsg = "null";
            else
                textMsg = msg.ToString();
            text.text += textMsg + "\n"; Debug.Log(textMsg); }));
    }
	void Start()
    {
        //var input = gameObject.GetComponent<InputField>();
        //var se = new InputField.SubmitEvent();
        //se.AddListener(ExecuteFunc);
        //input.onEndEdit = se;
    }

    public void OnCodeChanged(string newCode)
	{
        //code = newCode;
	}
   public void ExecuteFunc(string func)
	{        
        engine.Execute(func);
    }

    public void Submit()
	{
        ExecuteFunc(codeText.text);

    }

    public void SetValue<T>(string _name, T value)
	{
        engine.SetValue(_name, value);
	}

    public void AddLog(string m)
	{
        text.text += m + "\n";
    }
}
