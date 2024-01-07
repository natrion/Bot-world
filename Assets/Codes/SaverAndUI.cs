using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using TMPro;
using System;
public class SaverAndUI : MonoBehaviour
{
    [SerializeField] GameObject Menu;
    private void Start()
    {
        UiPanelsRef = new List<GameObject>();
        resetMenu();   
        Menu.SetActive(false);
        inventory.SetActive(false);
        Load();
    }
    private void Update()
    {
        controlEscMenu();
        controlInventoryMenu();
        //UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        //unityTransport.SetConnectionData(IP, port);
    }
    void controlEscMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.activeSelf);
        }
    }
    public void Resume()
    {
        Menu.SetActive(false);
    }
    public void Quit()
    {
        Save();
        Application.Quit();
    }

    //saving and loding
    /*
    private class GameInformation
    {
        public Vector3 PlayerPosition;
    }
    */
    //saving loading
    public  GameObject Player;
    public void Save()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetFloat("PlayerX", Player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerZ", Player.transform.position.z);
    }
    public void Load()
    {
        if (PlayerPrefs.HasKey("PlayerX") == true)
        {
            Player.transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX"), Player.transform.position.y, PlayerPrefs.GetFloat("PlayerZ"));         
        }       
    }
    //Inventoy
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject InventoryContent;
    [SerializeField] ObjInf objInf;
    private List<GameObject>UiPanelsRef;
    void resetMenu()
    {
        for (int i = 0; i < objInf.itemInf.Count; i++)
        {
            if (UiPanelsRef.Count-1<i)
            {
                GameObject newPanel = Instantiate(objInf.itemInf[i].gameObject);
                newPanel.transform.parent = InventoryContent.transform;
                UiPanelsRef.Add( newPanel);
            }
        }
    }
    void changeinventoryParameters()
    {
        for (int i = 0; i < UiPanelsRef.Count; i++)
        {
            UiPanelsRef[i].transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = objInf.itemInf[i].amount.ToString();
            if (objInf.itemInf[i].amount == 0)
            {
                UiPanelsRef[i].SetActive(false);
            }
            else
            {
                UiPanelsRef[i].SetActive(true);
            } 
        }
    }
    void controlInventoryMenu()
    {
        changeinventoryParameters();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            inventory.SetActive(!inventory.active);
        }
    }
    //multyplayer 
    /*
    public netComunicator NetComunicator;
    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        NetComunicator.StartNet();
    }
    public void StartHost()
    {
        NetworkManager.Singleton.StartServer();
        NetComunicator.StartNet();
    }
    [SerializeField] private TMP_InputField AdressChange;
    [SerializeField] private TMP_InputField PortChange;
    [SerializeField] private string IP;
    [SerializeField] private ushort port;
    public void OnChangeAdress()
    {
        IP = AdressChange.text;
    }
    public void OnChangePort()
    {
        port = Convert.ToUInt16(PortChange.text);
    }
    */
}
