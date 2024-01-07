using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using System;
using System.IO.Compression;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Networking;
public class netComunicator : NetworkBehaviour
{
    public GameObject[] ObjectTypeStor;
    private GameInf gameInf;
    private SimpGameInf SimpgameInf;
    [SerializeField] public TextMeshProUGUI TestText;
    [SerializeField] private SaverAndUI GameInformation;
    private bool networkStarted;
    public static bool CanDoThingsOnNetwork;


    private void Start()
    {
        gameInf = new GameInf();
        gameInf.objectInfSize = -1;
        gameInf.objectInf = new ObjectInf[10000];

        SimpgameInf.objectInfSize = -1;
        SimpgameInf.objectInf = new SimpObjectInf[10000];
    }
    public struct GameInf 
    {
        public int objectInfSize;
        public ObjectInf[] objectInf;
    }
    [System.Serializable]
    public struct SimpGameInf
    {
        public int objectInfSize;
        public SimpObjectInf[] objectInf;
    }
    public struct ObjectInf
    {
        public bool exists;
        public GameObject ObjectReference;
        public int ObjectType;
    }
    [System.Serializable]
    public struct SimpObjectInf
    {
        public bool exists;
        public float[] position;
        public int ObjectType;
    }
    //functions
    private IEnumerator Wait()
    {    
        yield return new WaitForSecondsRealtime(2f);
        networkStarted = true;
        if (IsServer == false) CallServerToUpdateServerRpc();
        yield return new WaitForSecondsRealtime(0.5f);
        InvokeRepeating("UpdateNet", 0, 2);
        CanDoThingsOnNetwork = true;
        
        TestText.text = "netStarted";
    }
    private bool buffer;
    public void StartNet()
    {
        if (buffer == true) return;
        buffer = true;
        StartCoroutine(Wait());
    }
    void UpdateNet()
    {
        UpdateAll(SimpgameInf);
    }            
    [ServerRpc(RequireOwnership = false)]
    private void CallServerToUpdateServerRpc()
    {
        print("calledServer");
        worldClientRpc(Packamedata(SimpgameInf));
    }
    [ServerRpc(RequireOwnership = false)]
    private void worldServerRpc(string packedworldInf)
    {
        ChangeToSet(UnpackGamedata(packedworldInf));
        //worldClientRpc(packedworldInf);
    }
    [ClientRpc]
    private void worldClientRpc(string packedworldInf)
    {
        print("GotToClient");
        ChangeToSet(UnpackGamedata(packedworldInf));
    }
    private void UpdateAll(SimpGameInf worldInf)
    {
        string packedData = Packamedata(worldInf);
        if (IsClient == true)  worldServerRpc(packedData); 
        if (IsServer == true) worldClientRpc(packedData);
    }
    public int ChangeSomethinOnServer(bool Change, int WhatThing, ObjectInf Object,bool UpdateSrver)
    {

        if (networkStarted == false) return 0;
        int whatChanged = 0;

        float[] Position = new float[3] { Object.ObjectReference.transform.position.x, Object.ObjectReference.transform.position.y, Object.ObjectReference.transform.position.z };
        SimpObjectInf simpObjectInf = new SimpObjectInf { ObjectType = Object.ObjectType , exists = Object.exists , position = Position };

        if (Change == false)
        {
            SimpgameInf.objectInfSize += 1;
            SimpgameInf.objectInf[SimpgameInf.objectInfSize] = simpObjectInf;
            gameInf.objectInfSize += 1;
            gameInf.objectInf[gameInf.objectInfSize] = Object;        

            whatChanged = gameInf.objectInfSize;
        }
        else
        {
            SimpgameInf.objectInf[WhatThing] = simpObjectInf;
            gameInf.objectInf[WhatThing] = Object;            
            whatChanged = WhatThing;
        }
        if(UpdateSrver == true) UpdateAll(SimpgameInf);
        return whatChanged;
    }

    private void ChangeToSet(SimpGameInf SentgameInf)
    {
        for (int i = 0; i < SentgameInf.objectInfSize+1; i++)
        {
            if (gameInf.objectInf[i].exists != SentgameInf.objectInf[i].exists)
            {
                if (gameInf.objectInf[i].exists == true & SentgameInf.objectInf[i].exists == false)
                {
                    //delete
                    gameInf.objectInf[i].exists = false;
                    Destroy(gameInf.objectInf[i].ObjectReference);
                }
                else
                {
                    //create   
                    gameInf.objectInf[i].exists = true;
                    gameInf.objectInf[i].ObjectType = SentgameInf.objectInf[i].ObjectType;
                    GameObject newGameObject = Instantiate(ObjectTypeStor[SentgameInf.objectInf[i].ObjectType]);
                    gameInf.objectInf[i].ObjectReference = newGameObject;
                }
            }
            
            if (gameInf.objectInf[i].ObjectType != SentgameInf.objectInf[i].ObjectType)
            {
                Destroy(gameInf.objectInf[i].ObjectReference);
                GameObject newGameObject = Instantiate(ObjectTypeStor[SentgameInf.objectInf[i].ObjectType]);
                gameInf.objectInf[i].ObjectReference = newGameObject;
                gameInf.objectInf[i].ObjectType = SentgameInf.objectInf[i].ObjectType;
            }
            
            if (gameInf.objectInf[i].ObjectReference.transform.position != null)
            {
                Vector3 Position = new Vector3(SentgameInf.objectInf[i].position[0], SentgameInf.objectInf[i].position[1], SentgameInf.objectInf[i].position[2]);
                if (Position != gameInf.objectInf[i].ObjectReference.transform.position) gameInf.objectInf[i].ObjectReference.transform.position = Position;
            }

        }
        SimpgameInf.objectInfSize = SentgameInf.objectInfSize;
    }

    //private int playerNumber;
    //WorldInf.ON
    public string Packamedata(SimpGameInf _message)
    {
        using (MemoryStream input = new MemoryStream())
        {
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(input, _message);
            input.Seek(0, SeekOrigin.Begin);

            using (MemoryStream output = new MemoryStream())
            using (DeflateStream deflateStream = new DeflateStream(output, CompressionMode.Compress))
            {
                input.CopyTo(deflateStream);
                deflateStream.Close();

                return Convert.ToBase64String(output.ToArray());
            }
        }
    }
    
    public SimpGameInf UnpackGamedata(string _packed)
    {
        using (MemoryStream input = new MemoryStream(Convert.FromBase64String(_packed)))
        using (DeflateStream deflateStream = new DeflateStream(input, CompressionMode.Decompress))
        using (MemoryStream output = new MemoryStream())
        {
            deflateStream.CopyTo(output);
            deflateStream.Close();
            output.Seek(0, SeekOrigin.Begin);

            BinaryFormatter bformatter = new BinaryFormatter();
            SimpGameInf message = (SimpGameInf)bformatter.Deserialize(output);
            return message;
        }
    }
}
