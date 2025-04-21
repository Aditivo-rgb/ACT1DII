using UnityEngine;
using UnityEngine.AI;

public class Spawn : MonoBehaviour
{
    [SerializeField] GameObject zombiePrefab;
    [SerializeField] int number;
    [SerializeField] float spawnRadius;
    [SerializeField] private bool spawnOnStart = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (spawnOnStart)
            SpawnAll();
    }

    
    void SpawnAll()
    {
        for (int i = 0; i < number; i++)
        {
            //Saco un vector random alrededor de mi spawner con un radio de una esfera de 1
            Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * spawnRadius;

            NavMeshHit hit;
            //si no estoy tocando el Navmesh no crea zombies
            if (NavMesh.SamplePosition (randomPoint, out hit, 10f, NavMesh.AllAreas))
            {
                Instantiate(zombiePrefab, hit.position, Quaternion.identity);
            }
            else
            {
                i--;
            }

            
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (!spawnOnStart && collider.CompareTag("Player"))
            SpawnAll();
            
        
    }
}
