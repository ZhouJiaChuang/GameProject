using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSGameState
{
    public static ERunPlatform RunPlatform
    {
        get
        {
#if UNITY_EDITOR
            return ERunPlatform.Editor;
#else
            return ERunPlatform.AndroidEditor;
#endif
        }
    }
}
