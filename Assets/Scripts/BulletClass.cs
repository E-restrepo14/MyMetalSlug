using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo lo que lleve este script para que funcione correctamente debe de tener colliders tipo triggers
public class BulletClass : MonoBehaviour
{
    // si se puede elimina con otro proyectil... no se puede eliminar con el piso.
    public bool isDestructible;
    public bool onlyDestroyedByTime;
    public bool afectedByGravity;
    public bool isInteligent;

    public float intelligentMisileSpeed;
    public float distanciaDeLanzamiento;
    public float alturaDeLanzamiento;
    private bool reboto = false;

    public string allyCharacterTag;

    // aqui le debo poner la condicional que no reaccione con otras balas de npcs





    void  OnEnable()
    {

        if (GetComponent<Collider>().attachedRigidbody && afectedByGravity == true)
            GetComponent<Collider>().attachedRigidbody.useGravity = true;
        
            // con esta linea puedo destruir los proyectiles utilizando el tiempo
            Destroy(gameObject, 30);

        if (isInteligent == true)
            StartCoroutine("FollowTarget(UpdateManager.instance.player.transform, 0, intelligentMisileSpeed)");

        AddForce();
    }

    private void AddForce()
    {
        GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, alturaDeLanzamiento,distanciaDeLanzamiento));
    }

    void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.tag == "EnemyBulletTag")
        {
            Destroy(this.gameObject);
        }
        else
        // en caso de que sea una granada del player esta debe rebotar una vez antes de destruirse
        if (gameObject.tag == "BulletTag" && afectedByGravity)
        {
            if (reboto == false)
                reboto = true;
            else
                Destroy(this.gameObject);
        }
        else
        if (allyCharacterTag != null && collision.transform.parent != null)
        {
            if (collision.transform.parent.gameObject.tag != allyCharacterTag)
            {
                Destroy(this.gameObject);
            }
        }
        
    }

    void OnTriggerEnter(Collider other)
    {
        // este if es para que ignore los otros colliders con su mismo tag 
        if (other.gameObject.tag != gameObject.tag)
        {
            // este if es para verificar si el proyectil se destruye con una bala
            if (other.gameObject.CompareTag("BulletTag"))
            {
                if (isDestructible == true)
                    Destroy(this.gameObject);
            }
            else
            {
                // este if es para verificar si el proyectil se destruye con el suelo
                if (other.gameObject.CompareTag("GroundTag"))
                {
                    if(isDestructible == false && onlyDestroyedByTime == false)
                    {
                        Destroy(this.gameObject);
                    }
                }
                else
                {
                    if (other.gameObject.CompareTag("EnemyBulletTag"))
                    {
                        if (isDestructible == true)
                            Destroy(this.gameObject);
                    }
                    else
                    {
                        // este if es para verificar si el proyectil se destruye con un personaje enemigo de quien lo disparó
                        if (allyCharacterTag != null && other.transform.parent != null)
                        {
                            if (other.transform.parent.gameObject.tag != allyCharacterTag)
                            {
                                Destroy(this.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }

    IEnumerator FollowTarget(Transform target, float distanceToStop, float speed)
    {
        while (gameObject.activeSelf && Vector3.Distance(transform.position, target.position) > distanceToStop)
        {
            transform.LookAt(target);
            GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * speed, ForceMode.Force);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
