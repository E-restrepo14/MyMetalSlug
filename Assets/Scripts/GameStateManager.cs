using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EstadosDeJuego
{
    noJugando,
    jugando,
    terminado
}


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        if (instance != this)
            Destroy(gameObject);
    }

    public EstadosDeJuego estadoActual;

   

    public void CambiarAEstadoJugando()
    {
        estadoActual = EstadosDeJuego.jugando;
    }

    public void CambiarAEstadoNoJugando()
    {
        estadoActual = EstadosDeJuego.noJugando;
    }

    public void CambiarAEstadoTerminado()
    {
        estadoActual = EstadosDeJuego.terminado;
    }

}
