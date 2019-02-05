using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundTag"))
        {
            isGrounded = true;
            canPointDown = false; 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("EnemyBulletTag"))
        {
            if(other.GetComponent<BulletClass>() != null)
            GetDamage();
        }
    }

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

    // este metodo se llamará desde el UpdateManager y se ejecutará cada frame
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

    public void VerifyAim()
    {
        //if (UpdateManager.instance.sIsPressed == true)
        //{
          //  return;
        //}
      //  else // este else se ejecutará mientras la tecla S no este presionada
      //  {
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
        //}
    }

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


    // metodo consistente en ubicar al personaje en su spawnpoint
    public void Spawn()
    {
        transform.Translate(new Vector3(0, 8, 0));
        gameObject.tag = "Player";
        playerLife = 1;
    }

    // metodo consistente en rotar al personaje 180°
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

    // metodo consistente en darle un gira al arma
    public void PointGun(Vector3 rotation)
    {
        head.transform.Rotate(rotation);
    }

    // metodo consistente en verificar si puede saltar el personaje y ordenar que lo haga
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

    public void StopJump()
    {
        rb.AddForce(new Vector2(0f, -powerOfJumpStop));
    }

    // metodo que consiste en bajar la cabeza y el arma del personaje
    public void Crouch()
    {
        head.transform.position += new Vector3(0, -0.6f, 0);
    }

    // metodo que consiste en subir la cabeza y el arma del personaje
    public void StandUp()
    {
        head.transform.position += new Vector3(0, 0.6f, 0);
    }

    // metodo consistente en instanciar un proyectil en una dirección
    public void Shoot()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            CombatSystemManager.instance.FireBullet(head);
        }
    }

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

    public void ThrowGrenade()
    {
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            CombatSystemManager.instance.FireGrenade(head);
        }
    }
}
