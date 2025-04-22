using UnityEngine;

public class ObjetoFlotante : MonoBehaviour
{
   [SerializeField] private float velocidadRotacion = 50f;     // Velocidad de rotación
   [SerializeField] private float amplitudFlotacion = 0.25f;   // Qué tanto sube y baja
    [SerializeField] private float frecuenciaFlotacion = 1f;    // Qué tan rápido sube y baja

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Rotación continua en el eje Y
        transform.Rotate(Vector3.up * velocidadRotacion * Time.deltaTime, Space.World);

        // Movimiento vertical suave (flotación tipo seno)
        float offsetY = Mathf.Sin(Time.time * frecuenciaFlotacion) * amplitudFlotacion;
        transform.position = new Vector3(posicionInicial.x, posicionInicial.y + offsetY, posicionInicial.z);
    }
}
