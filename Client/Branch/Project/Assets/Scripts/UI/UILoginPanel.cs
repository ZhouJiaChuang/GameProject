using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginPanel : UIBase
{
    private Button _Btn_EditorLogin;
    public Button Btn_EditorLogin
    {
        get
        {
            return _Btn_EditorLogin ? _Btn_EditorLogin : (_Btn_EditorLogin = transform.Find("Widget/Editor/Btn_EditorLogin").GetComponent<Button>());
        }
    }
    public void Start()
    {
        if (Btn_EditorLogin != null)
        {
            Btn_EditorLogin.onClick.AddListener(OnClickBtn);
        }
    }

    public void OnClickBtn()
    {
        UnityEngine.Debug.LogError("1");
    }
}
