using UnityEngine;
using UnityEngine.UI;

public class MakeRadarObject : MonoBehaviour
{
    [SerializeField] private Image image;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Agrega este GameObject al radar, usando esta imagen como su icono.
        Radar.RegisterRadarObject(this.gameObject, image);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        Radar.RemoveRadarObject(this.gameObject);
    }
}
