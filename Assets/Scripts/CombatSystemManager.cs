using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystemManager : MonoBehaviour
{

    [SerializeField] private GameObject morteroPrefab;
    [SerializeField] private GameObject fireBombPrefab;
    [SerializeField] private GameObject bazookaShotPrefab;
    [SerializeField] private GameObject enemyGrenadePrefab;
    [SerializeField] private GameObject playerGrenadePrefab;
    [SerializeField] private GameObject misilePrefab;
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private GameObject playerBulletPrefab;

    public static CombatSystemManager instance = null;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        if (instance != this)
            Destroy(gameObject);
    }


    public void FireMortero(GameObject go)
    {
        Instantiate(morteroPrefab, go.transform.position, go.transform.rotation);
    }

    public void FireFireBomb(GameObject go)
    {
        Instantiate(fireBombPrefab, go.transform.position, go.transform.rotation);
    }

    public void FireBazooka(GameObject go)
    {
        Instantiate(bazookaShotPrefab, go.transform.position, go.transform.rotation);
    }

    public void FireGrenade(GameObject go)
    {
        Instantiate(playerGrenadePrefab, go.transform.position, go.transform.rotation);
    }

    public void FireEnemyGrenade(GameObject go)
    {
        Instantiate(enemyGrenadePrefab, go.transform.position, go.transform.rotation);
    }

    public void FireRocket(GameObject go)
    {
        Instantiate(rocketPrefab, go.transform.position, go.transform.rotation);
    }

    public void FireBullet(GameObject go)
    {
        Instantiate(playerBulletPrefab, go.transform.position, go.transform.rotation);
    }

    public void FireMisil(GameObject go)
    {
        Instantiate(misilePrefab, go.transform.position, go.transform.rotation);
    }



}
