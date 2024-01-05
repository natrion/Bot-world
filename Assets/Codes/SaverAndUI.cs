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
        Menu.SetActive(false);
        Load();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.activeSelf);
        }
        UnityTransport unityTransport = NetworkManager.Singleton.GetComponent<UnityTransport>();
 
        unityTransport.SetConnectionData(IP, port);
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
    //multyplayer 
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
}
