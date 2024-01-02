using UnityEngine;
using UnityEngine.VFX; 

public class SpawnPrefabOnKeyPress : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public float sphereRadius = 5f;
    //public Texture2D colorMap;
    //public Texture2D positionMap;

    void Update()
    {
        // Check if the space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Spawn the prefab at a random position within the sphere
            Vector3 randomPosition = Random.insideUnitSphere * sphereRadius;
            GameObject spawnedPrefab = Instantiate(prefabToSpawn, randomPosition, Quaternion.identity);

            
            VisualEffect pointcloudvfx = spawnedPrefab.GetComponent<VisualEffect>();
            //pointcloudvfx.SetTexture("ColorMap",colorMap);
            //pointcloudvfx.SetTexture("PositionMap",positionMap);
            // Optionally, you can do additional setup for the spawned object here
            // For example, you might want to attach the spawned object to a specific parent object.
            // spawnedPrefab.transform.parent = transform;
            AutoOrbitCamera autoOrbitCamera = FindObjectOfType<AutoOrbitCamera>();
            if (autoOrbitCamera != null)
            {
                autoOrbitCamera.SetNewRandomPosition(randomPosition);
            }
        }
    }
}
