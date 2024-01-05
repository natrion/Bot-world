using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float ScrolSpeed;
    [SerializeField] private float Scrolwiew;
    [SerializeField] private Vector2 MinMaxCameraSize;
    [SerializeField] private float playermidle;
    [SerializeField] private netComunicator comunicator;
    private int playerNumber;
    private bool WasAdded;

    void FixedUpdate()
    {
        Vector3 Waspos = player.transform.position;
        player.transform.position  += new Vector3 ( Input.GetAxisRaw("Horizontal"), 0 ,Input.GetAxisRaw("Vertical"))* movementSpeed;
        transform.position =  new Vector3(player.transform.position.x, 0, player.transform.position.z+ playermidle);

        if (netComunicator.CanDoThingsOnNetwork == true )
        {
            if (WasAdded == false)
            {
                WasAdded = true;
                playerNumber = comunicator.ChangeSomethinOnServer(false, 0, new netComunicator.ObjectInf { ObjectType = 0, exists = true, ObjectReference = player },true);
            }
            if (Waspos != player.transform.position)
            {
                comunicator.ChangeSomethinOnServer(true, playerNumber, new netComunicator.ObjectInf { ObjectType = 0, exists = true, ObjectReference = player },false);
            }
        }

        //camerascroling
        Scrolwiew += Input.GetAxis("Mouse ScrollWheel") * ScrolSpeed * Scrolwiew;
        Scrolwiew = Mathf.Clamp(Scrolwiew, MinMaxCameraSize.x, MinMaxCameraSize.y);

        float size = Scrolwiew * MinMaxCameraSize.x;
        transform.GetChild(0).GetComponent<Camera>().orthographicSize = size;
        transform.GetChild(1).transform.localScale = new Vector3(size, 1, size);

        //transform.GetChild(0).GetComponent<Camera>().orthographicSize = Mathf.Clamp(transform.GetChild(0).GetComponent<Camera>().orthographicSize , MinMaxCameraSize.x, MinMaxCameraSize.y);
        //transform.GetChild(1).transform.localScale = new Vector3(Mathf.Clamp(transform.GetChild(1).transform.localScale.x, MinMaxCameraSize.x, MinMaxCameraSize.y) , 50 , Mathf.Clamp(transform.GetChild(1).transform.localScale.z, MinMaxCameraSize.x, MinMaxCameraSize.y));
    }
}
