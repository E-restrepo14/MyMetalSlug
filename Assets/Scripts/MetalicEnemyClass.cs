using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetalicEnemyClass : MonoBehaviour
{

    public bool isCar;
    public bool isTank;
    public bool isMorteroTank;
    public bool isMortero;
    public bool isTurret1;

    // si este booleano es verdadero debe tener dos hijos gameobject o imprimira un error
    public bool isTurret2;

    [SerializeField] private GameObject Canon1;
    [SerializeField] private GameObject Canon2;

    [SerializeField] private GameObject thirdChild;



    float enemyNextFire;
    [SerializeField] private float enemyFireRate = 2.5f;

    public int metalicEnemyLife = 12;



    private void Awake()
    {
        Canon1 = transform.GetChild(0).gameObject;
    }

    // este void nos permite detectar al jugador y llamar un disparo
    private void OnTriggerStay(Collider other)
    {
        if (other.transform.parent != null)
        {
            if (other.transform.parent.gameObject.tag == "Player")
            {
                if (Time.time > enemyNextFire && enemyFireRate > 1)
                {
                    enemyNextFire = Time.time + enemyFireRate;
                    VerifyOwnTypeOfEnemy();
                }
            }
        }
    }


    public void VerifyOwnTypeOfEnemy()
    {
        if (isMortero == true)
        {
            if (Canon1 != null)
                CombatSystemManager.instance.FireMortero(Canon1);
        }
        else
        {
            if (isTurret1 == true)
            {
                if (Canon1 != null)
                    CombatSystemManager.instance.FireFireBomb(Canon1);
            }
            else
            {
                if (isTurret2 == true)
                {
                    Canon2 = transform.GetChild(1).gameObject;

                    if (Canon1 != null)
                        CombatSystemManager.instance.FireFireBomb(Canon1);

                    if (Canon2 != null)
                        CombatSystemManager.instance.FireFireBomb(Canon2);
                }
                else
                {
                    if (isCar == true)
                    {
                        if (Canon1 != null)
                            CombatSystemManager.instance.FireMisil(Canon1);
                    }
                    else
                    {
                        if (isTank == true)
                        {
                            if (Canon1 != null)
                                CombatSystemManager.instance.FireFireBomb(Canon1);
                        }
                        else
                        {
                            if (isMorteroTank == true)
                            {
                                if (Canon1 != null)
                                    CombatSystemManager.instance.FireMortero(Canon1);
                            }
                        }
                    }
                }
            }
        }
    }



    // esta corrutina nos permite eliminar este gameobject
    public IEnumerator MetalicEnemyDie()
    {
        if (isMorteroTank == true)
        {
            Canon2 = transform.GetChild(1).gameObject;
            thirdChild = transform.GetChild(2).gameObject;

            GetComponent<MeshCollider>().enabled = false;
            GetComponent<Rigidbody>().useGravity = true;
            GetComponent<Rigidbody>().isKinematic = false;

            for (int i = 0; i < 10; i++)
            {
                Canon2.GetComponent<MeshRenderer>().enabled = !Canon2.GetComponent<MeshRenderer>().enabled;
                thirdChild.GetComponent<MeshRenderer>().enabled = !thirdChild.GetComponent<MeshRenderer>().enabled;
                yield return new WaitForSeconds(0.1f);
            }

            Canon1.SetActive(false);
            Canon2.SetActive(false);
            thirdChild.SetActive(false);

            

        }
        else
        {
            for (int i = 0; i < 10; i++)
            {
                GetComponent<Renderer>().enabled = !GetComponent<Renderer>().enabled;
                yield return new WaitForSeconds(0.1f);
            }
            Destroy(gameObject);
        }
    }

    // este void nos permite recibir daño y hacerlo mediante colisiones (necesitan ser dos rigidbodys para que funcione correctamente)
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.parent != null)
        {
            if (collision.transform.parent.gameObject.tag == "Player")
            {
                // el mortero no hace daño si es tocado por el jugador
                if (isMortero == false)
                UpdateManager.instance.myPlayerController.GetDamage();
                print("hizo daño al jugador");
            }
        }

        if (collision.gameObject.CompareTag("BulletTag"))
        {
            TakeDamage(1);
        }

        if (collision.gameObject.CompareTag("PlayerWeaponTag"))
        {
            TakeDamage(8);
        }

    }

    public void TakeDamage(int damage)
    {
        print("taked " + damage + " of damage");
        metalicEnemyLife -= damage;
        if (metalicEnemyLife <= 0)
            StartCoroutine("MetalicEnemyDie");

    }
}