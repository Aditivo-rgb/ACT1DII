using UnityEngine;

public class CompassController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject pointer;
    [SerializeField] private RectTransform compassLine;
    private RectTransform rect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rect = pointer.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        /* 
        Calculo dónde poner el objeto en el compás según cómo está mirando el jugador y dónde está el objetivo. 
        1. Cojo las esquinas del "compassLine" y mido el tamaño con `Vector3.Distance`.
        2. Consigo la dirección entre el jugador y el objetivo.
        3. Calculo el ángulo entre los dos con `Vector3.SignedAngle`.
        4. Ajusto ese ángulo entre -90 y 90 grados, lo normalizo y lo adapto al tamaño del compás.
        5. muevo el objeto en el eje `x` según el cálculo.
        */
        Vector3[] corners = new Vector3[4];
        compassLine.GetLocalCorners(corners);
        float pointScale = Vector3.Distance(corners[1], corners[2]);
        Vector3 direction = target.transform.position - player.transform.position;
        float angleToTarget = Vector3.SignedAngle(player.transform.forward, direction, player.transform.up);
        angleToTarget = Mathf.Clamp(angleToTarget, -90, 90) / 180.0f * pointScale;
        rect.localPosition = new Vector3(angleToTarget, rect.localPosition.y, rect.localPosition.z);
    }
}
