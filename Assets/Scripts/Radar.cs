using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadarObject
{
    public Image icon { get; set; }
    public GameObject owner { get; set; }

}
    public class Radar : MonoBehaviour
    {
        public Transform playerPos;
        // Escala del mapa/radar. Ajusta cu�n lejos aparecen los objetos en el radar en relaci�n a su distancia real.
        public float mapScale = 2.0f;
        // Lista est�tica que guarda todos los objetos registrados en el radar junto con sus iconos.
        public static List<RadarObject> radObjects = new List<RadarObject>();
        // M�todo est�tico para registrar un nuevo objeto en el radar.
        public static void RegisterRadarObject(GameObject o, Image i)
        {
            // Crea una copia del icono que se usar� en el radar (normalmente se instancia como hijo de un panel de radar).
            Image image = Instantiate(i);
            // A�ade a la lista un nuevo RadarObject, que contiene el objeto original y su icono asociado en el radar.
            radObjects.Add(new RadarObject() { owner = o, icon = image });
        }

        public static void RemoveRadarObject(GameObject o)
        {
            // Se crea una nueva lista temporal para guardar los objetos que no se eliminar�n del radar.
            List<RadarObject> newList = new List<RadarObject>();
            // Se recorre cada objeto registrado actualmente en el radar.
            for (int i = 0; i < radObjects.Count; i++)
            {
                // Si el objeto actual del radar coincide con el objeto 'o' que queremos eliminar...
                if (radObjects[i].owner == o)
                {
                    // Se destruye el icono visual del radar asociado a ese objeto.
                    Destroy(radObjects[i].icon);
                    // Se salta al siguiente elemento sin a�adir este a la nueva lista.
                    continue;
                }
                else
                {
                    // Si no es el objeto que queremos eliminar, lo a�adimos a la nueva lista.
                    newList.Add(radObjects[i]);
                }
            }
            // Elimina todos los elementos actuales de la lista radObjects.
            radObjects.RemoveRange(0, radObjects.Count);
            // A�ade todos los objetos filtrados de la lista newList a radObjects.
            radObjects.AddRange(newList);
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created

    
        void Update()
        {
            if (playerPos == null) return;
            // Recorremos todos los objetos registrados en el radar.
            foreach (RadarObject ro in radObjects)
            {
                    // Calculamos la posici�n relativa del objeto respecto al jugador en el mundo 3D.
                    Vector3 radPos = ro.owner.transform.position - playerPos.position;
                    // Calculamos la distancia entre el jugador y el objeto, y la escalamos seg�n el tama�o del minimapa.
                    float disToObject = Vector3.Distance(playerPos.position, ro.owner.transform.position) * mapScale;

                    /* 
                    Calcula el �ngulo entre el jugador y el objeto representado en el radar. 
                    Utiliza la funci�n Mathf.Atan2 para obtener el �ngulo en radianes entre dos puntos en el plano X-Z. 
                    Luego se convierte a grados con Mathf.Rad2Deg. 
                    Se resta 270 grados para alinear el radar correctamente y tambi�n se resta la rotaci�n Y del jugador 
                    para que el radar gire con la direcci�n del jugador.
                    */
                    float deltay = Mathf.Atan2(radPos.x, radPos.z) + Mathf.Rad2Deg - 270 - playerPos.eulerAngles.y;
                    /* 
                    Calcula la nueva posici�n en el eje X del icono en el radar. 
                    Se usa el coseno del �ngulo calculado (convertido a radianes) para determinar la direcci�n horizontal. 
                    Se multiplica por -1 para invertir la direcci�n si es necesario. 
                    Luego se suma la distancia al objeto desde el jugador para ubicar el icono a una distancia proporcional en el radar.
                    */
                    radPos.x = disToObject + Mathf.Cos(deltay + Mathf.Deg2Rad) * -1;
                    /* 
                    Calcula la nueva posici�n en el eje Z del icono en el radar. 
                    Se utiliza el seno del mismo �ngulo para obtener la direcci�n vertical en el radar. 
                    Esto posiciona el icono en su lugar correcto respecto al centro del radar y seg�n la orientaci�n del jugador.
                    */
                    radPos.z = disToObject * Mathf.Sin(deltay * Mathf.Deg2Rad);

                    // Establece este objeto (el radar) como el padre del icono en la jerarqu�a de la UI.
                    ro.icon.transform.SetParent(this.transform);
                    // Obtiene el componente RectTransform del radar (este script debe estar en un UI element tipo Image o Panel).
                    RectTransform rt = this.GetComponent<RectTransform>();
                    // Pongo la posici�n del icono en pantalla, dentro del radar.
                    // Uso la posici�n relativa del objeto , ajustada por el pivot del radar para colocarlo correctamente.
                    // Uso radPos.x y radPos.z porque en un radar 2D el eje Y del mundo no es necesario (altura).
                    ro.icon.transform.position = new Vector3(radPos.x + rt.pivot.x, radPos.z + rt.pivot.y, 0) + this.transform.position;
            }
        }

    }