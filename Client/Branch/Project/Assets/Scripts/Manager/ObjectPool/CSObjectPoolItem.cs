using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSObjectPoolItem
{
    public CSObjectPoolBase owner;
    public bool isUse = true;
    public GameObject go;
    public object objParam;
    public float RemoveTime;
}
