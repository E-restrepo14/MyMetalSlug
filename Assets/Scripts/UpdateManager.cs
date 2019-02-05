using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public bool jump = false;
    public bool sIsPressed = false;
    public bool pointUp = false;
    public Vector3 direction;
    public GameObject player;
    public PlayerController myPlayerController;

    public static UpdateManager instance = null;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        if (instance != this)
            Destroy(gameObject);

        myPlayerController = player.GetComponent<PlayerController>();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UiManager.instance.StartCoroutine("MenuOrPause");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
            myPlayerController.Jump();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jump = false;
            myPlayerController.StopJump();
        }
        //====================================================
        if (Input.GetKey(KeyCode.W))
        {
            pointUp = true;
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            pointUp = false;
        }
        //====================================================
        if (Input.GetKeyDown(KeyCode.E))
        {
            myPlayerController.ThrowGrenade();
        }
        //====================================================
        if (Input.GetKey(KeyCode.S))
        {
            sIsPressed = true;
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            sIsPressed = false;
        }
        //====================================================
        if (Input.GetKey(KeyCode.A))
        {
            direction = Vector3.left;
            myPlayerController.Walk();
        }

        if (Input.GetKey(KeyCode.D))
        {
            direction = Vector3.right;
            myPlayerController.Walk();
        }
        //====================================================




        //====================================================
        if (Input.GetMouseButton(0))
        {
            myPlayerController.Shoot();
        }

        if (Input.GetMouseButton(1))
        {
            myPlayerController.StartCoroutine("PlayerStab");
        }

        myPlayerController.VerifyImputs();
        NpcManager.instance.MoverEnemigos();

    }

    private void FixedUpdate()
    {
        jump = false;
    }

    //public void Morir()
    //{
    //    myPlayerController.StartCoroutine("Die");
    //}
}
