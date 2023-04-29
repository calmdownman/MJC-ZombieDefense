using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoints;
    public EnemyFSM[] zombiePrefabs;
    List <EnemyFSM> zombies = new List<EnemyFSM>();

    int level;
    float timer;
    private void Awake()
    {
        spawnPoints = GetComponentsInChildren<Transform>();
    }

    public void Spawn(int pointNum) 
    {
        Transform spawnPoint = spawnPoints[pointNum+1];

        EnemyFSM zombie = Instantiate(zombiePrefabs[Random.Range(0, zombiePrefabs.Length)],spawnPoint.position,spawnPoint.rotation);

        zombies.Add(zombie);

        zombie.onDeath += () => zombies.Remove(zombie);
    }

}
