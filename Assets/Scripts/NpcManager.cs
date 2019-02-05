using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// singleton con referencias de todos los npc en escena... tiene la funcion de ordenar a cada uno de estos, 
/// el realizar una verificacion... esta es llamada cada frame por el update manager
/// </summary>
public class NpcManager : MonoBehaviour
{
    public static NpcManager instance;

    public GameObject[] npcs;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        if (instance != this)
            Destroy(gameObject);

        ContarNpcsEnEscena();

    }

    void ContarNpcsEnEscena()
    {
        npcs = GameObject.FindGameObjectsWithTag("NpcTag");
    }

  
    public void MoverEnemigos()
    {
        if (npcs.Length != 0)
            foreach (GameObject enemigo in npcs)
            {
                if(enemigo != null)
                enemigo.SendMessage("VerificarCampoDeVision", enemigo.transform.position);
            }
    }
}
