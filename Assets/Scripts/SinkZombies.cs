using UnityEngine;

public class SinkZombies : MonoBehaviour
{
    float destroyHeight;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void StartSink()
    {
        destroyHeight = Terrain.activeTerrain.SampleHeight(this.transform.position) - 5;//altura a la que destruye

        //Destruyo todos los colliders
        Collider[] colList = this.transform.GetComponentsInChildren<Collider>();
        foreach (Collider c in colList)
        {
            Destroy(c);
        }
        //llamo al método después de 3 segundos y hago que corra cada 0.1 segundos
        InvokeRepeating("SinkIntoGround", 3f, 0.1f);
    }

    void SinkIntoGround()
    {
        this.transform.Translate(0, -0.001f, 0);//lo hundo 
        if(this.transform.position.y < destroyHeight - 0.01f)//cuando paso de altura lo destruyo
        {
            Destroy(this.gameObject);
        }
    }
   
}
