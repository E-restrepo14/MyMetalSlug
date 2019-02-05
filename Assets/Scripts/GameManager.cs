using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject feets;

    public GameObject inmunityShield;

    public bool vaAMorir = false;



    public static GameManager instance = null;
    void Awake()
    {
        Time.timeScale = 1.5f;

        if (instance == null)
            instance = this;
        else
        if (instance != this)
            Destroy(gameObject);
    }
    private void Start()
    {
        head = UpdateManager.instance.player.transform.GetChild(0).gameObject;
        feets = UpdateManager.instance.player.transform.GetChild(1).gameObject;
        inmunityShield = UpdateManager.instance.player.transform.GetChild(2).gameObject;
    }


    // esta coroutina se encarga de hacer parpadear al personaje por un tiempo pero luego llama la coroutina perder() (almacenada en uimanager)
    public IEnumerator Die()
    {
            
        if(vaAMorir == false)
        {
            vaAMorir = true;
            for (int i = 0; i < 10; i++)
            {
                feets.GetComponent<Renderer>().enabled = !feets.GetComponent<Renderer>().enabled;
                head.GetComponent<Renderer>().enabled = !head.GetComponent<Renderer>().enabled;

                yield return new WaitForSeconds(0.1f);
            }

            UiManager.instance.StartCoroutine("Perder");
        }
        else
        yield return new WaitForSeconds (0);
    }

    public IEnumerator GetInvensible()
    {
        vaAMorir = false;
        UpdateManager.instance.myPlayerController.Spawn();

        if (inmunityShield != null)
            inmunityShield.SetActive(true);

        for (int i = 0; i < 60; i++)
        {
            feets.GetComponent<Renderer>().enabled = !feets.GetComponent<Renderer>().enabled;
            head.GetComponent<Renderer>().enabled = !head.GetComponent<Renderer>().enabled;
            inmunityShield.transform.Rotate(0, 20, 0);
            yield return new WaitForSeconds(0.1f);
        }

        inmunityShield.SetActive(false);
    }

    
    public void Restart()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ChangeTimeScale()
    {
        if (Time.timeScale == 1.5f)
            Time.timeScale = 0;
        else
            Time.timeScale = 1.5f;
    }
}
