using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] obstacles; // 0 ~ 4 (max 5)

    [SerializeField]
    private float obstacleInterval = 2f;

    // Start is called before the first frame update
    void Start()
    {
        StartSpawning();
    }
    
    private void StartSpawning() {
        StartCoroutine("SpawnObstacleRoutine");
    }

    public void StopSpawning() {
        StopCoroutine("SpawnObstacleRoutine");
    }

    IEnumerator SpawnObstacleRoutine() {
        yield return new WaitForSeconds(0.5f);
        while (true) {
            SpawnObstacle();
            yield return new WaitForSeconds(obstacleInterval);
        }
    }

    private void SpawnObstacle() {
        int index = Random.Range(0, obstacles.Length);
        Vector3 position = new Vector3(obstacles[index].transform.position.x, transform.position.y, 0);
        Instantiate(obstacles[index], position, Quaternion.identity);
    }

    public void DecreaseObstacleInterval() {
        obstacleInterval -= 0.2f;
        if (obstacleInterval < 0.5f) {
            obstacleInterval = 0.5f;
        }
    }
}
