using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjInf : MonoBehaviour
{
    public List<ItemInf> itemInf;
    [System.Serializable]
    public class ItemInf
    {
        public string Name;
        public GameObject gameObject;
        public float amount;
    }
    public List<GameObject> TypeInf ;

    public int worldSize;

    public float tileSize;

    public GameObject[,] PositionInf;
    private void Start()
    {
        PositionInf = new GameObject[worldSize, worldSize];
    }

}
