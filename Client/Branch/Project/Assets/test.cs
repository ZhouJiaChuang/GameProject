using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    CSScene csscene = null;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (csscene == null) csscene = new CSScene();
            CSNetwork.SendSocketEvent(ESocketEvent.ResUpdateViewPlayerMessage);
        }
    }
}
