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
            return Get<Button>("Widget/Editor/Btn_EditorLogin");
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
        CSSceneManager.Instance.Load(ESceneType.GameScene, null, null, OnLoadedGameScene);
    }

    private void OnLoadedGameScene()
    {
        UIManager.Instance.ClosePanel<UILoginPanel>();
        UIManager.Instance.CreatePanel<UIMainMenuPanel>();
    }
}
