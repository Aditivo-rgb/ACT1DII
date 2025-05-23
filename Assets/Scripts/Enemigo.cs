using UnityEngine;
using UnityEngine.AI;

public class Enemigo : MonoBehaviour, Danhable
{
    private NavMeshAgent agent;
    private Player target;//mi target tiene el script player
    private Animator anim;

    [Header("Recogibles")]
    [SerializeField] GameObject medkit;
    [SerializeField] GameObject ammo;

    [Header("Sistema de movimiento")]
    [SerializeField] private float walkingSpeed;
    [SerializeField] private float runningSpeed;

    [Header("Sistema de combate")]
    [SerializeField] private Transform puntoAtaque;
    [SerializeField] private float radioAtaque;
    [SerializeField] private float danhoAtaque = 20;
    [SerializeField] GameObject ragdoll;
    [SerializeField] private float vidas = 20;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] splatClips;



    enum STATE { IDLE, WANDER, ATTACK, CHASE, DEAD}
    STATE state = STATE.IDLE;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        target = FindObjectOfType<Player>();//le pido que cuando inicie localice cual es el objeto con player
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        //state machine que va desde el idle hasta la muerte
        switch (state)
        {
            case STATE.IDLE:
                if (CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0,5000) < 5)
                state = STATE.WANDER;
                break;
            case STATE.WANDER:
                Wander();
                if (CanSeePlayer()) state = STATE.CHASE;
                else if (Random.Range(0, 5000) < 5)
                {
                    state = STATE.IDLE;
                    TurnOffTriggers();
                    agent.ResetPath();
                }
                    break;
            case STATE.CHASE:
                Chase();

                if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                {
                    state = STATE.ATTACK;
                }

                if(ForgetPlayer())
                {
                    state = STATE.WANDER;
                    agent.ResetPath();
                }
                break;
            case STATE.ATTACK:
                TurnOffTriggers();
                LanzarAtaque();
                EnfocarObjetivo();

                if (DistanceToPlayer() > agent.stoppingDistance + 2)
                {
                    agent.isStopped = false;
                    state = STATE.CHASE;
                }
                    
                break;
            case STATE.DEAD:
                Destroy(agent);
                this.GetComponent<SinkZombies>().StartSink();
                break;
        }

    }
    //m�todos del State machine
    private void Chase()
    {
        agent.SetDestination(target.transform.position);
        agent.stoppingDistance = 2;
        TurnOffTriggers();
        agent.speed = runningSpeed;
        anim.SetBool("isRunning", true);
    }
    private void Wander()
    {
        if (!agent.hasPath)
        {
            float newX = this.transform.position.x + Random.Range(-5, 5);
            float newZ = this.transform.position.z + Random.Range(-5, 5);
            Vector3 terrainPos = Terrain.activeTerrain.GetPosition();
            float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ)) + terrainPos.y;
            Vector3 dest = new Vector3(newX, newY, newZ);
            agent.SetDestination(dest);
            agent.stoppingDistance = 0;
            TurnOffTriggers();
            agent.speed = walkingSpeed;
            anim.SetBool("isWalking", true);

        }
    }
    float DistanceToPlayer()
    {
        return Vector3.Distance(target.transform.position, this.transform.position);
    }
    bool CanSeePlayer()
    {
        if(DistanceToPlayer() < 10)
            return true;
        return false;
    }
    bool ForgetPlayer() 
    {
        if (DistanceToPlayer() > 20)
            return true;
        return false;
    }
    void TurnOffTriggers()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isDead", false);
    }

    //Combate
    private void LanzarAtaque()
    {
        agent.isStopped = true;

        anim.SetBool("isAttacking", true);

    }
    private void Atacar()
    {
        HacerSonarGolpes();
        //Lanzo el overlap desde un punto de ataque y  bajo un radio de ataque y recojo todos los collider tocados
        Collider[] coliderTocados = Physics.OverlapSphere(puntoAtaque.position, radioAtaque);
        foreach (Collider coll in  coliderTocados)
        {
            if(coll.TryGetComponent(out Danhable danhable))
            {
                danhable.RecibirDahno(danhoAtaque);
            }
        }
    }
    private void EnfocarObjetivo()
    {
        //Si tienes un objetivo B y quieres sacar la direcci�n desde el punto A siempre restas destino - origen y normalizas
        Vector3 direccionAObjetivo = (target.transform.position - transform.position).normalized;
        //para que la direccion a la que gira no le haga tumbarse pongo a 0 el vector en y
        direccionAObjetivo.y = 0;
        //Con esa direccion saco el �ngulo al que el personaje tiene que girarse
        Quaternion rotacionAObjetivo = Quaternion.LookRotation(direccionAObjetivo);
        transform.rotation = rotacionAObjetivo;
    }
    private void FinDeAtaque()
    {
        agent.isStopped = false;
        anim.SetBool("isWalking", true);
        Debug.Log("Me muevo otra vez");
        anim.SetBool("isAttacking", false);
    }
    public void RecibirDahno(float danho)
    {
        vidas -= danho;
        if (vidas <= 0)
        {
            InstanciarRecogibles();

            if (Random.Range(0, 10) < 5)
            {
                GameObject rd = Instantiate(ragdoll, this.transform.position, this.transform.rotation);
                rd.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 10000);
                Destroy(this.gameObject);
            }
            else
            {
                TurnOffTriggers();
                anim.SetBool("isDead", true);
                
                state = STATE.DEAD;
            }
            
        }
    }
    void HacerSonarGolpes()
    {
        // No hacer nada si ya est� sonando un sonido
        if (audioSource.isPlaying) return;

        // Elegir un sonido aleatorio
        int n = Random.Range(0, splatClips.Length);
        audioSource.clip = splatClips[n];
        audioSource.Play();
    }

    void InstanciarRecogibles()
    {
        if (Random.value < 0.5f)
        {
            GameObject objetoASpawnear = Random.value < 0.5f ? medkit : ammo;

            // Obtener la posici�n actual del zombi
            Vector3 posicionSpawn = transform.position;

            // Aumentar la posici�n Y para elevar el objeto
            posicionSpawn.y += 0.5f; // Ajusta este valor seg�n lo que necesites

            // Spawnea el objeto en la nueva posici�n elevada
            Instantiate(objetoASpawnear, posicionSpawn, Quaternion.identity);
        }
    
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(puntoAtaque.position, radioAtaque);
    }
}
