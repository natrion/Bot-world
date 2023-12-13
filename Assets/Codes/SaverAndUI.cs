using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] GameObject Player;
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
}
