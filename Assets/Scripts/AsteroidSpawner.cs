using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AsteroidSpawner : MonoBehaviour
{
    public int AsteroidCount;
    public int RewardCount;
    public List<GameObject> AsteroidPrefabs;
    public List<GameObject> SpaceDebris;
    public List<GameObject> RewardPrefabs;
    public bool SpawnerEnabled = true;
    // Start is called before the first frame update
    void Start()
    {
        if(enabled)
        {
            for (int i = 0; i < AsteroidCount; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-3200, 3200), Random.Range(-3200, 3200), Random.Range(-3200, 3200));
                while (Vector3.Distance(randomPos, Vector3.zero) < 300)
                {
                    Debug.Log("Finding new position");
                    randomPos = new Vector3(Random.Range(-3200, 3200), Random.Range(-3200, 3200), Random.Range(-3200, 3200));
                }
                GameObject Asteroid = Instantiate(AsteroidPrefabs[Random.Range(0, AsteroidPrefabs.Count - 1)], randomPos, Quaternion.identity);
                Rigidbody asteroidRB = Asteroid.AddComponent<Rigidbody>();
                asteroidRB.useGravity = false;
                Vector3 randomForce = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
                asteroidRB.AddForce(randomForce);
                Asteroid.AddComponent<BulletManager>();
                Asteroid.GetComponent<MeshCollider>().convex = true;
                Asteroid.GetComponent<MeshCollider>().isTrigger = true;
                Asteroid.transform.DOScale(Random.Range(1, 3), 0.1f);
            }
            for (int i = 0; i < AsteroidCount / 2; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-3200, 3200), Random.Range(-3200, 3200), Random.Range(-3200, 3200));
                while (Vector3.Distance(randomPos, Vector3.zero) < 300)
                {
                    randomPos = new Vector3(Random.Range(-3200, 3200), Random.Range(-3200, 3200), Random.Range(-3200, 3200));
                }
                GameObject Asteroid = Instantiate(SpaceDebris[Random.Range(0, SpaceDebris.Count - 1)], randomPos, Quaternion.identity);
                Rigidbody asteroidRB = Asteroid.AddComponent<Rigidbody>();
                asteroidRB.useGravity = false;
                Vector3 randomForce = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
                asteroidRB.AddForce(randomForce);
                Asteroid.GetComponent<MeshCollider>().convex = true;
                Asteroid.transform.DOScale(Random.Range(1, 3), 0.1f);
            }
            for (int i = 0; i < RewardCount / 2; i++)
            {
                Vector3 randomPos = new Vector3(Random.Range(-3200, 3200), Random.Range(-3200, 3200), Random.Range(-3200, 3200));
                while (Vector3.Distance(randomPos, Vector3.zero) < 300)
                {
                    randomPos = new Vector3(Random.Range(-3200, 3200), Random.Range(-3200, 3200), Random.Range(-3200, 3200));
                }
                GameObject Asteroid = Instantiate(RewardPrefabs[Random.Range(0, RewardPrefabs.Count - 1)], randomPos, Quaternion.identity);
                Rigidbody asteroidRB = Asteroid.AddComponent<Rigidbody>();
                asteroidRB.useGravity = false;
                Vector3 randomForce = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), Random.Range(-3, 3));
                asteroidRB.AddForce(randomForce);
                Asteroid.GetComponent<MeshCollider>().convex = true;
                Asteroid.GetComponent<MeshCollider>().isTrigger = true;
                Asteroid.transform.DOScale(Random.Range(10, 20), 0.1f);
            }
        }
        
    }

}
