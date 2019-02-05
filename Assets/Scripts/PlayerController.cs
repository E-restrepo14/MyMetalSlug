using System.Collections;
using UnityEngine;

/// <summary>
/// clase encargada de guardar y proveer y ejecutar metodos y variables, segun las verificaciones del update manager  
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private bool isGrounded = false;
    [SerializeField] private float powerJump = 400f;
    [SerializeField] private float powerOfJumpStop = 200f;
    [SerializeField] private float crouchSpeed = .36f;
    [SerializeField] private float runSpeed = .4f;
    [SerializeField] private int playerLife = 1;

    [SerializeField] private GameObject head;


    private Rigidbody rb;
    public bool miraALaDerecha = true;
    private float speed;

    public bool isCrouching = false;
    public bool isPointingUp = false;
    public bool isPointingDown = false;
    public bool canPointDown = false;
    public bool isStabing = false;
    float nextFire;
    public float fireRate;
    public GameObject bulletPrefab;


    private void Awake()
    {
        head = transform.GetChild(0).gameObject;
        rb = this.GetComponent<Rigidbody>();
        StartCoroutine("PlayerStab");
    }

    /// <summary>
    /// este void es utilizado para detectar cuando el personaje principal se encuentra 
    /// en el aire o ha aterrizado despues de saltar o caer
    /// </summary>
    /// <param name="collision"> collider que ha colisionado con Playercontroller </param>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundTag"))
        {
            isGrounded = true;
            canPointDown = false; 
        }
    }

    /// <summary>
    /// metodo utilizado para detectar cuando entra en contacto con un proyectil con collider tipo trigger
    /// </summary>
    /// <param name="other">collider tipo trigger que ha colisionado con Playercontroller </param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyBulletTag"))
        {
            if(other.GetComponent<BulletClass>() != null)
            GetDamage();
        }
    }

    /// <summary>
    /// metodo que es llamado cada vez que el player controller entra ne contacto con un proyectil enemigo con collider tipo trigger
    /// </summary>
    public void GetDamage()
    {    
        if (!GameManager.instance.inmunityShield.activeSelf && playerLife > 0)
        {
            playerLife--;
            if (playerLife <= 0)
            {
                GameManager.instance.StartCoroutine("Die");
                gameObject.tag = "Untagged";
            }
        }
    }


    /// <summary>
    /// metodo consistente en verificar hacia que lado del personaje se le está ordenando moverse.
    /// este metodo se llama desde el UpdateManager y se ejecutará cada frame
    /// </summary>
    public void VerifyImputs()
    {
        if (GameManager.instance.vaAMorir == false)
        {
            if (UpdateManager.instance.direction.x < 0 && miraALaDerecha)
            {
                Flip();
            }
            if (UpdateManager.instance.direction.x > 0 && !miraALaDerecha)
            {
                Flip();
            }
            VerifySComand();
            VerifyAim();
        }
    }

    /// <summary>
    /// proceso para verificar si segun el update manager, se debe ordenar al player que apunte el arma hacia arriba
    /// </summary>
    public void VerifyAim()
    {
            if (UpdateManager.instance.pointUp)
            {
                if (isPointingUp == false)
                {
                    PointGun(new Vector3(-90, 0, 0));
                    isPointingUp = true;
                }
            }
            else
            {
                if (isPointingUp)
                {
                    PointGun(new Vector3(90, 0, 0));
                    isPointingUp = false;
                }
            }

            if (canPointDown)
            {
                if (isPointingDown == false)
                {
                    PointGun(new Vector3(90, 0, 0));
                    isPointingDown = true;
                }
            }
            else
            {
                if (isPointingDown)
                {
                    PointGun(new Vector3(-90, 0, 0));
                    isPointingDown = false;
                }
            }
    }

    /// <summary>
    /// proceso para verificar si segun el update manager, se ha presionado la tecla S
    /// </summary>
    public void VerifySComand()
    {
        if (UpdateManager.instance.pointUp == true)
        {
            return;
        }
        else
        {
            if (UpdateManager.instance.sIsPressed) // si este if se cumple es porque detecta la S presionada
            {
                if (isGrounded) // si se cumple es porque elpersonaje está en la tierra (y detecta la S presionada)
                {
                    speed = crouchSpeed;
                    if (isCrouching == false)// y se cumple porque se detecta en posicion levantado
                    {
                        Crouch();
                        isCrouching = true;
                        canPointDown = false;
                    }
                }
                else // si se cumple es porque el personaje está en el aire (y detecta la S presionada)
                {
                    canPointDown = true;
                    speed = runSpeed;
                    if (isCrouching == true)// y se cumple porque se detecta en posicion agachado
                    {
                        StandUp();
                        isCrouching = false;
                    }
                }
            }
            // este else se ejecuta si no esta presionada la S... 
            else
            {

                speed = runSpeed;
                if (isCrouching == true) // si se cumple es porque está en posicion agachado (y liberaron la S)
                {
                    StandUp();
                    isCrouching = false;
                    canPointDown = false;
                }
                else // si se cumple es porque el personaje está en posicion de pie (y liberaron la S)
                {
                    canPointDown = false;
                }
            }
        }
    }

    /// <summary>
    ///  metodo consistente en ubicar al personaje en su spawnpoint
    /// </summary>
    public void Spawn()
    {
        transform.Translate(new Vector3(0, 8, 0));
        gameObject.tag = "Player";
        playerLife = 1;
    }

    /// <summary>
    /// metodo consistente en rotar al personaje 180°
    /// </summary>
    private void Flip()
    {
        miraALaDerecha = !miraALaDerecha;
        transform.Rotate(0, 180f, 0);
    }

    /// <summary>
    /// metodo consistente en mover al personaje
    /// </summary>
    public void Walk()
    {
        if (GameManager.instance.vaAMorir == false)
        {
            transform.Translate((UpdateManager.instance.direction * speed) * Time.deltaTime, Space.World);
        }
    }

    /// <summary>
    /// metodo consistente en darle un giro al arma
    /// </summary>
    /// <param name="rotation"> direccion hacia la cual se desea rotar el arma del player</param>
    public void PointGun(Vector3 rotation)
    {
        head.transform.Rotate(rotation);
    }

    /// <summary>
    /// metodo consistente en verificar si puede saltar el personaje y ordenar que lo haga
    /// </summary>
    public void Jump()
    {
        if (GameManager.instance.vaAMorir == false)
        {
            if (isGrounded)
            {
                isGrounded = false;
                rb.AddForce(new Vector2(0f, powerJump));
            }
        }
    }

    /// <summary>
    /// metodo encargado de interrumpir la altura del salto al liberar la tecla space en el UpdateManager
    /// </summary>
    public void StopJump()
    {
        rb.AddForce(new Vector2(0f, -powerOfJumpStop));
    }

    /// <summary>
    /// metodo que consiste en bajar la cabeza y el arma del personaje, haciendo que se agache
    /// </summary>
    public void Crouch()
    {
        head.transform.position += new Vector3(0, -0.6f, 0);
    }

    /// <summary>
    /// metodo que consiste en subir la cabeza y el arma del personaje, haciendo que se levante
    /// </summary>
    public void StandUp()
    {
        head.transform.position += new Vector3(0, 0.6f, 0);
    }


    /// <summary>
    /// metodo consistente en activar un gameobject, darle una rotacion durante un tiempo y desactivarlo
    /// </summary>
    /// <returns> retorna una espera de 0 segundos </returns>
    public IEnumerator PlayerStab()
    {
        if (isStabing == false)
        {
            head.transform.GetChild(0).gameObject.SetActive(true);
            isStabing = true;
            for (int i = 0; i < 20; i++)
            {
                head.transform.GetChild(0).Rotate(Vector3.left *18);
                yield return new WaitForSeconds(0);
            }
            isStabing = false;
            head.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// metodo consistente en instanciar llamar cada cierto tiempo la instanciacion de un prefab de la clase combatsystemmanager
    /// </summary>
    public void Shoot()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            CombatSystemManager.instance.FireBullet(head);
        }
    }

    /// <summary>
    /// metodo consistente en instanciar llamar cada cierto tiempo la instanciacion de otro prefab distinto de la clase combatsystemmanager.
    /// </summary>
    public void ThrowGrenade()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            CombatSystemManager.instance.FireGrenade(head);
        }
    }
}
