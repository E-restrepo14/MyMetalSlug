using System.Collections;
using UnityEngine;

/// <summary>
/// clase encargada de llamar ciertas acciones y dar un comportamiento caracteristico a cada una del as variables de enemigo que existen en el juego
/// </summary>
public class Enemy : MonoBehaviour
{

    [SerializeField]
    private float enemySpeed = 0.2f;
    [SerializeField]
    Collider[] CollidersDetectados;
    public GameObject target;
    [SerializeField]
    GameObject tempTarget;
    [SerializeField] private int enemyLife = 1;
    public float radius = 5.72f;
    public float enemyDirection;

    private bool isFacingright = false;
    private bool isCovered = false;
    private bool isjumping = false;
    [SerializeField] private float enemyPowerJump = 400;
    private float secondsJumping;
    private GameObject enemyWeapon;

    [SerializeField] private GameObject bazooka;
    [SerializeField] private GameObject knife;
    [SerializeField] private GameObject shield;



    public bool hasShield;
    public bool hasKnife;
    public bool canAim;
    public bool hasGrenades;

    float enemyNextFire;
    public float enemyFireRate;

    /// <summary>
    /// este metodo toma todos los colliders en una locacion y en un radio especifico 
    /// y los almacena en un array, (esto se debe llamar por un tercero y ejecutar cada frame) 
    /// </summary>
    /// <param name="center"> otorga una posicion a la cual hacer un barrido y recoleccion de todos los colliders cercanos </param>
    public void VerificarCampoDeVision(Vector3 center)
    {
        CollidersDetectados = Physics.OverlapSphere(center, radius);
        target = null;
        if (enemyLife > 0)
            ElegirTarget();
    }

    private void Awake()
    {
        enemyWeapon = transform.GetChild(0).GetChild(0).gameObject;

        if (hasShield)
        {
            shield.SetActive(true);
            enemyLife = 8;
        }
        if (hasKnife)
        {
            knife.SetActive(true);
            radius = 19;
        }
        if (hasGrenades)
        {
            radius = 12;
            enemyFireRate = 2.5f;
        }
        if (canAim)
        {
            bazooka.SetActive(true);
            radius = 10;
        }
    }

    /// <summary>
    /// verifica que entre los collider detectados, el target sea quien tenga el tag Playe
    /// </summary>
    void ElegirTarget()
    {
        foreach (Collider coll in CollidersDetectados)
        {
            if (coll.transform.parent != null)
            {
                tempTarget = coll.transform.parent.gameObject;

                if (tempTarget.CompareTag("Player"))
                {
                    target = tempTarget;

                    if (canAim == true)
                    {
                        enemyDirection = (target.transform.position.x - this.transform.position.x);
                        LookAtTarget();
                        if (Time.time > enemyNextFire && enemyFireRate > 1)
                        {
                            enemyNextFire = Time.time + enemyFireRate;
                            CombatSystemManager.instance.FireBazooka(enemyWeapon);
                        }
                        PointBazooka();
                    }
                    else
                    {
                        if (hasKnife == true)
                        {
                            Cubrirse();
                            StartCoroutine ("Jump");
                        }
                        else
                        {
                            if (hasGrenades == true)
                            {
                                Cubrirse();
                                enemyDirection = (target.transform.position.x - this.transform.position.x);
                                LookAtTarget();
                                if (Time.time > enemyNextFire && enemyFireRate > 1)
                                {
                                    enemyNextFire = Time.time + enemyFireRate;
                                    CombatSystemManager.instance.FireEnemyGrenade(enemyWeapon);
                                }
                            }
                            else
                            {
                                if (hasShield == true)
                                {
                                    Caminar();
                                }
                                else
                                {
                                    enemyDirection = (target.transform.position.x - this.transform.position.x);
                                    LookAtTarget();
                                    if (Time.time > enemyNextFire && enemyFireRate > 1)
                                    {
                                        enemyNextFire = Time.time + enemyFireRate;
                                        CombatSystemManager.instance.FireRocket(enemyWeapon);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// metodo consistente en dirigirse hacia el jugador
    /// </summary>
    public void Caminar()
    {
        enemyDirection = (target.transform.position.x - this.transform.position.x);
        LookAtTarget();
        enemyDirection *= (enemySpeed * Time.deltaTime);
        transform.Translate(enemyDirection, 0,0,Space.World);
    }

    /// <summary>
    /// metodo consistente en hacer agachar al soldado enemigo
    /// </summary>
    public void Cubrirse()
    {
        if (isCovered == false)
        {
            isCovered = true;
            transform.GetChild(0).gameObject.transform.position += new Vector3(0, -0.6f, 0);
        }
    }

    /// <summary>
    /// metodo consistente en hacer girar al soldado enemigo media vuelta
    /// </summary>
    public void Flip()
    {
        transform.Rotate(0, 180f, 0);
    }


    /// <summary>
    /// metodo consistente en hacer saltar al soldado y posteriormente apuñalar
    /// </summary>
    /// <returns> retorna un tiempo de espera de 0.1 segundos </returns>
    public IEnumerator Jump()
    {

        if (isjumping == false)
        {
            isjumping = true;
            enemyDirection = (target.transform.position.x - this.transform.position.x);
            secondsJumping = 3;
        }

        LookAtTarget();

        enemyDirection *= (enemySpeed * Time.deltaTime);
        transform.Translate(enemyDirection, 0, 0, Space.World);

        if (secondsJumping > 0)
        {
            GetComponent<Rigidbody>().AddForce(new Vector2(0f, enemyPowerJump));
            secondsJumping--;
            yield return new WaitForSeconds(0.1f);
            enemyWeapon.transform.Rotate(Vector3.right * 10);


        }

    }

    /// <summary>
    /// metodo consistente en verificar hacia que lado está el jugador y girarse hacia el
    /// </summary>
    public void LookAtTarget()
    {
        if (enemyDirection >= 0)
        {
            enemyDirection = 1f;
            if (isFacingright == true)
            {
                isFacingright = false;
                Flip();
            }
        }
        else
        {
            enemyDirection = -1f;
            if (isFacingright == false)
            {
                isFacingright = true;
                Flip();
            }
        }
    }

    /// <summary>
    /// metodo consistente en hacer daño al player si este se detecta dentro de su rango de daño cuando hizo el llamado a apuñalar
    /// </summary>
    public void Stab()
    {
        if (hasKnife == true && target != null)
        {
            Vector3 diff = target.transform.position - transform.position;
            float distance = diff.sqrMagnitude;
            if (distance < 1.5f)
            {
                UpdateManager.instance.myPlayerController.GetDamage();
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("GroundTag"))
        {
            isjumping = false;
            Stab();
        }
    }

    /// <summary>
    /// metodo para verificar cuanto daño recibir dependiendo del tag del collider que lo ingrese
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("BulletTag"))
        {
            enemyLife--;
            if (enemyLife <= 0)
                StartCoroutine("EnemyDie");
        }

        if (other.gameObject.CompareTag("PlayerWeaponTag"))
        {
            enemyLife-= 8;
            if (enemyLife <= 0)
                StartCoroutine("EnemyDie");
        }
    }

    /// <summary>
    /// metodo consistente en dan un efecto de muerte al personaje enemigo
    /// </summary>
    /// <returns></returns>
    public IEnumerator EnemyDie()
    {
        GameObject enemyFeets = transform.GetChild(0).gameObject;
        GameObject enemyHead = transform.GetChild(1).gameObject;

        for (int i = 0; i < 10; i++)
        {
            enemyFeets.GetComponent<Renderer>().enabled = !enemyFeets.GetComponent<Renderer>().enabled;
            enemyHead.GetComponent<Renderer>().enabled = !enemyHead.GetComponent<Renderer>().enabled;

            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// metodo consistente en hacer apuntar el arma del soldado enemigo hacia el personaje
    /// </summary>
    public void PointBazooka()
    {
        if(enemyWeapon != null)
        {
            enemyDirection = (target.transform.position.x - this.transform.position.x);
            LookAtTarget();
            enemyWeapon.transform.LookAt(target.transform);
        }
    }

}
