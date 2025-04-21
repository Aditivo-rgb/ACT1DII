using UnityEngine;
using UnityEngine.Rendering;


public class Player : MonoBehaviour, Danhable
{
    [SerializeField] private float velocidadMovimiento;
    [SerializeField] private float factorGravedad;
    [SerializeField] private float alturaDeSalto;
    [SerializeField] private Transform camara;
    [SerializeField] private InputManagerSO inputManager;
    [SerializeField] private Animator anim;
    [SerializeField] private ParticleSystem particles;

    [Header("Detección suelo")]
    [SerializeField] private Transform pies;
    [SerializeField] private float radioDeteccion;
    [SerializeField] private LayerMask queEsSuelo;
    private Enemigo enemigo;
    
    private CharacterController controller;

    private Vector3 direccionMovimiento;
    private Vector3 direccionInput;
    private Vector3 velocidadVertical;

   [Header ("Sistema de combate")]
   [SerializeField] private float vidas = 0f;
   [SerializeField] private float vidasMax = 100f;
   [SerializeField] private float distanciaDisparo = 500;
   [SerializeField] private float danhoDisparo = 20;

    [Header("Sistema de sonido")]
    [SerializeField] private AudioSource audiodisparo;
    [SerializeField] private AudioSource audiopasos;
    [SerializeField] private AudioSource audiogatillo;
    [SerializeField] private AudioSource[] audioPasos;
    [SerializeField] private float intervaloPasos = 0.5f;
    [SerializeField] private AudioSource audioReload;
    [SerializeField] private AudioSource audioHit;
    private float timerPasos = 0f;
    private bool meMuevo = false;
    

    private AudioSource audioSource;
   
    //Inventario
   private int ammo = 0;
   private int maxAmmo = 50;
   private int ammoClip = 0;
   private int ammoClipMax = 10;
   

    bool cursorIsLocked = true;
    bool lockCursor = true;


    private void OnEnable() //el player se suscribe a la llamada que ha hecho el input manager y crea su evento
    {
        inputManager.OnSaltar += Saltar;
        inputManager.OnMover += Mover;
        inputManager.OnDisparar += Disparar;
        inputManager.OnRecargar += Recargar;
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();

        // Poner vidas a tope

        vidas = vidasMax; 
    }

    // Update is called once per frame
    void Update()
    {
        AplicarMovimiento();

        ActualizarMovimiento();

        ManejarVelocidadVertical();
    }

    void FixedUpdate() 
    { 
        UpdateCursorLock();
    }  


    //Solo cuando se actualice el input de movimiento
    private void Mover(Vector2 ctx)
    {
        direccionInput = new Vector3(ctx.x, 0, ctx.y);
        
    }

    private void Saltar()
    {
        if(EstoyEnSuelo())
        {
        //fórmula cinemática, tu valor en y es igual a la raiz cuadrada de -2 * la gravedad * la altura que alcances
        velocidadVertical.y = Mathf.Sqrt(-2 * factorGravedad * alturaDeSalto);
            //lo animo
            Debug.Log("Saltando");
        }
        
    } 
  
    private void AplicarMovimiento()
    {
        //Me muevo hacia el delante de la cámara según mi z y hacia el lado de la cámara según mi x (que puede ser + ó - según el imput)
        direccionMovimiento = camara.forward * direccionInput.z + camara.right * direccionInput.x;
        direccionMovimiento.y = 0;//porque esto es la gravedad
        controller.Move(direccionMovimiento * velocidadMovimiento * Time.deltaTime);//hacia donde * cuán rápido * el tiempo que dure el movimiento
        //solo suenan los pasos si hay movimiento
        meMuevo = direccionInput.magnitude > 0.1f;
        if (meMuevo) 
        {
            timerPasos -= Time.deltaTime;//¿ya he pasado el tiempo desde el último paso?
            if (timerPasos <= 0f)//si ha pasado
            {
                HacerSonarPasos();//suenan mis pasos
                timerPasos = intervaloPasos;//reinicio el timer cada tiempo que le diga
            }
        }
        else
        {
            timerPasos = 0f;//si no me muevo reinicio
        }
        
    }

    private void ManejarVelocidadVertical()
    {
        //Si hemos aterrizado...
        if (EstoyEnSuelo() && velocidadVertical.y < 0)
        {
            //Entonces reseteo mi velocidad vertical
            velocidadVertical.y = 0;
        }
        AplicarGravedad();
    }

    private void ActualizarMovimiento()
    {
        if (direccionMovimiento.sqrMagnitude > 0)
        {
            //el bool va a depender de direccionMovimiento. si el vector del movimiento es mayor que cero me muevo, y por eso paso al estado true de moverme
            anim.SetBool("walking", true);
            RotarHaciaDestino();
        }
        else
        {
            anim.SetBool("walking", false);
        }
    }

    private void AplicarGravedad()
    {
        //A la velocidad vertical le sumo la gravedad multiplicada por el tiempo y eso se lo añado al controlador vertical del personaje por el tiempo también
        velocidadVertical.y += factorGravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }
    private bool EstoyEnSuelo()
    {
        return Physics.CheckSphere(pies.position, radioDeteccion, queEsSuelo);
    }
    
    private void RotarHaciaDestino()
    {
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);
        transform.rotation = rotacionObjetivo;
    }
    
    //Combate
    public void RecibirDahno(float danho)
    {
        vidas -= danho;
        if (vidas <= 0)
        {
            audioHit.Play();
            Debug.Log("Morí");
        }
    }

    private void Disparar()
    {
        if (ammoClip > 0)
        { 
            anim.SetTrigger("shoot");
            //meto el disparo para que pase a la vez que el sonido
            ammoClip--;
            Debug.Log("Ammo left" + ammo);
            audiodisparo.Play();
            particles.Play();
        

            //lo encapsulo dentro de un if porque me devuelve un bool
            //mira a ver si impactas en algo...
            if(Physics.Raycast(camara.position, camara.forward, out RaycastHit hitInfo, distanciaDisparo))//origen, dirección, distancia y qué hemos tocado (acepción 12)
            {
                    //Saber si el gameObject con el que he colisionado tiene la interfaz buscada
                    //mira a ver si ese algo es dañable
                    if (hitInfo.transform.TryGetComponent (out Danhable sistemaDanho))
                    {
                        if(!hitInfo.transform.CompareTag("Player"))
                        {
                            sistemaDanho.RecibirDahno(danhoDisparo);
                            Debug.Log("Estás dando");
                        }
                     
                   
                    }

            }
                
        }         
            
        else
        {
            anim.SetTrigger("noBullets");
            audiogatillo.Play();
        }
    }
    private void Recargar()
    {
        anim.SetTrigger("reload");//animacion de recarga
        audioReload.Play();
        int amountNeeded = ammoClipMax - ammoClip;//cuantas balas necesitamos para llenar el cargador
        int ammoAvaliable = amountNeeded < ammo ? amountNeeded : ammo;//cuantas balas podemos usar para recargar
        ammo -= ammoAvaliable;//resto las balas que uso de la reserva
        ammoClip += ammoAvaliable;//sumo estas balas al cargador
    }
    
    //Recoger
    void OnTriggerEnter(Collider collider)
    {
        //Munición
       if (collider.gameObject.CompareTag("Ammo") && ammo < maxAmmo)
       {
            ammo += Mathf.Clamp(ammo + 10, 0, maxAmmo);
            Debug.Log("Ammo" + ammo);
            //Sonido
            AudioSource audioAmmo = collider.gameObject.GetComponent<AudioSource>();
            if (audioAmmo != null && audioAmmo.clip != null)
            {
                AudioSource.PlayClipAtPoint(audioAmmo.clip, collider.transform.position);
            }

            Destroy(collider.gameObject);
               
       }
        
        //Botiquines
        else if (collider.gameObject.CompareTag("Medkit") && vidas < vidasMax)
        {
            vidas = Mathf.Clamp(vidas + 20, 0, vidasMax);
            Debug.Log("Medkit:" + vidas);
        //Sonido
            AudioSource audioMedkit = collider.gameObject.GetComponent<AudioSource>();
            if (audioMedkit != null && audioMedkit.clip != null)
            {
                    AudioSource.PlayClipAtPoint(audioMedkit.clip, collider.transform.position);
            }

            Destroy(collider.gameObject);
        }
        
    }

    //Ratón
    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        if (lockCursor)
        {
            InternalLockUpdate();
        }
    }

    public void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            cursorIsLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            cursorIsLocked = true;
        }

        if(cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!cursorIsLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void HacerSonarPasos()
    {
        if (audioPasos.Length == 0) return;//tiene sonidos mi array? si no, no ejecutes

        int n = Random.Range(1, audioPasos.Length);//cojo un número aleatorio entre 1 y mi último índice
        audioPasos[n].Play();//suenan los pasos

        // Mezclamos el orden para evitar que se repita el mismo clip
        AudioSource temp = audioPasos[n];//el sonido se almacena en n
        audioPasos[n] = audioPasos[0]; // muevo el último sonido al 0 
        audioPasos[0] = temp; // para que no suene otra vez de inmediato
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pies.position, radioDeteccion); 
    }

}
