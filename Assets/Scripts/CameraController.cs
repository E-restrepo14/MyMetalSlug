using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private float myX = 2;
    [SerializeField] private float myY = 2;
    [SerializeField] private float myZ = 10;


    void LateUpdate()
    {
        if (transform.position.x <= 71)
        {

            if (UpdateManager.instance.myPlayerController.miraALaDerecha)
            {
                if (transform.position.x < player.transform.position.x + myX)
                    transform.position = new Vector3(player.transform.position.x + myX, myY, -myZ);
            }
        }
        else
            UiManager.instance.StartCoroutine("LevelComplete");
    }
}
