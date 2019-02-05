using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
///  calse encargada de modificar todo lo que es la interfaz de usuario
/// </summary>
public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject pantallaPlayingGame;
    [SerializeField] private GameObject pantallaStart;
    [SerializeField] private GameObject pantallaGameOver;
    [SerializeField] private GameObject pantallaPrisioneros;
    [SerializeField] private GameObject pantallaLevelComplete;
    [SerializeField] private GameObject pantallaMenuOrPause;

    public bool ispaused = false;
    private bool prohibidoContinuar = false;
    private bool completoElNivel = false;


    [SerializeField]
    private Text countdownText;
    [SerializeField]
    private Text continueText;
    [SerializeField]
    private Text pauseOrMenuText;


   


    public static UiManager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        if (instance != this)
            Destroy(gameObject);
    }



    private void Start()
    {
        StartCoroutine("Iniciar");
    }




    public IEnumerator ActualizarPlayingScreen()
    {
        // solo se llama si el estado de juego es "jugando" verifica cuantas municiones, bombas y tiempo limite tiene el personaje y los actualiza en la pantallaPlayingGame
        yield return new WaitForSeconds(0);
    }

    public IEnumerator Iniciar()
    {
        //activa y desactivar la pantalla start unas tres veces y cambiar el estado de juego a "jugando"
        for (int i = 0; i < 6; i++)
        {
            pantallaStart.SetActive(!pantallaStart.activeSelf);
            yield return new WaitForSeconds(0.5f);
        }
        GameStateManager.instance.CambiarAEstadoJugando();
        GameManager.instance.StartCoroutine("GetInvensible");

    }

    public IEnumerator Perder()
    {
        if(prohibidoContinuar == false)
        {
            prohibidoContinuar = true;
            // cambia el estado de juego a no jugando
            GameStateManager.instance.CambiarAEstadoNoJugando();        
            int secondsToEnd = 9;
            countdownText.text = secondsToEnd.ToString();

            // setea la cuenta regresiva a 9 y hace parpadear el texto de continue
            while (GameStateManager.instance.estadoActual == EstadosDeJuego.noJugando)
            {
                pantallaGameOver.SetActive(true);
                continueText.enabled = (!continueText.enabled);
                yield return new WaitForSeconds(1.5f);
                secondsToEnd--;

                // si la cuenta regresiva llega a 0... se pasa a estado de juego "terminado", desactiva la pantalla game over e invoca la coroutine MenuOrPause()
                if ( secondsToEnd < 0 && GameStateManager.instance.estadoActual == EstadosDeJuego.noJugando)
                {
                    GameStateManager.instance.CambiarAEstadoTerminado();
                    pantallaGameOver.SetActive(false);
                    StartCoroutine("MenuOrPause");
                }

                countdownText.text = secondsToEnd.ToString();
            }
        }   
        else
        yield return new WaitForSeconds (0);     
    }

    // este se llama desde un boton de la pantalla game over
    public void Continue()
    {
        prohibidoContinuar = false;
        GameStateManager.instance.CambiarAEstadoJugando();
        pantallaGameOver.SetActive(false);
        GameManager.instance.StartCoroutine("GetInvensible");
    }



    public IEnumerator LevelComplete()
    {
        if (completoElNivel == false)
        {
            completoElNivel = true;

            // todo esto se ejecuta si el estado de juego es  "no jugando"
            // activar la pantalla prisioneros unos tres segundos, luego desactivarla
            pantallaPrisioneros.SetActive(true);
            yield return new WaitForSeconds(3);
            pantallaPrisioneros.SetActive(false);

            // activar y desactivar la pantalla level complete unas tres veces, termina desactivada
            for (int i = 0; i < 6; i++)
            {
                pantallaLevelComplete.SetActive(!pantallaLevelComplete.activeSelf);
                yield return new WaitForSeconds(0.5f);
            }

            // luego pasar al estado de juego "terminado" e invocar el menu or pause
            GameStateManager.instance.CambiarAEstadoTerminado();
            StartCoroutine("MenuOrPause");
        }
        else
        yield return new WaitForSeconds(0);
    }

    public IEnumerator MenuOrPause()
    {
        ispaused = !ispaused;

        if (ispaused)
        {
            // cuando se llame verificar el estado de juego, si está "jugando" el texto dirá pause... y se cambia a estado "no jugando" 
            if (GameStateManager.instance.estadoActual == EstadosDeJuego.jugando)
            {
                pauseOrMenuText.text = "PAUSE";
                GameStateManager.instance.CambiarAEstadoNoJugando();
            }
            //si está "terminado" el texto dirá menu... y seguirá en estado "terminado"
            if (GameStateManager.instance.estadoActual == EstadosDeJuego.terminado)
                pauseOrMenuText.text = "MENU";

            pantallaMenuOrPause.SetActive(true);
            GameManager.instance.ChangeTimeScale();
            //activa la pantalla menu y cada que esto pase, setea el timescale en 0 ... 
            yield return new WaitForSeconds(0);
        }
        // si detecta de nuevo la tecla escape y está en "no jugando" desactiva la pantalla menu y pone el timescale en lo que estaba antes.
        else
        {
            if (GameStateManager.instance.estadoActual == EstadosDeJuego.noJugando)
            {
                pantallaMenuOrPause.SetActive(false);
                GameManager.instance.ChangeTimeScale();
            }
        }
    }



}
