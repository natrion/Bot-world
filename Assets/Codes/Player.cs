using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float ScrolSpeed;
    [SerializeField] private Vector2 MinMaxCameraSize;
    [SerializeField] private float playermidle;
    void FixedUpdate()
    {
        player.transform.position  += new Vector3 ( Input.GetAxisRaw("Horizontal"), 0 ,Input.GetAxisRaw("Vertical"))* movementSpeed;
        transform.position =  new Vector3(player.transform.position.x, 0, player.transform.position.z+ playermidle);

        float scroolWheelState = Input.GetAxis("Mouse ScrollWheel")* ScrolSpeed;
        transform.GetChild(0).GetComponent<Camera>().orthographicSize += scroolWheelState * transform.GetChild(0).GetComponent<Camera>().orthographicSize;
        Vector3 PlaneSize = transform.GetChild(1).transform.localScale;
        transform.GetChild(1).transform.localScale += new Vector3(scroolWheelState * PlaneSize.x, 0, scroolWheelState * PlaneSize.x);

        transform.GetChild(0).GetComponent<Camera>().orthographicSize = Mathf.Clamp(transform.GetChild(0).GetComponent<Camera>().orthographicSize , MinMaxCameraSize.x, MinMaxCameraSize.y);
        transform.GetChild(1).transform.localScale = new Vector3(Mathf.Clamp(transform.GetChild(1).transform.localScale.x, MinMaxCameraSize.x, MinMaxCameraSize.y) , 50 , Mathf.Clamp(transform.GetChild(1).transform.localScale.z, MinMaxCameraSize.x, MinMaxCameraSize.y));
    }
}
