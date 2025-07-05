using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraObjectSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject coin;

    [SerializeField]
    private GameObject[] obstacles;

    private float[] arrCoinPosY = { -2.5f, -0.75f, 1f }; 
    private float[] arrObstaclePosY = { -2.5f, -1.3f };   
    private float objectInterval = 0.2f;
    private int coinCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartObjectSpawning();
    }

    private void StartObjectSpawning() {
        StartCoroutine("SpawnObjectRoutine");
    }

    public void StopObjectSpawning() {
        StopCoroutine("SpawnObjectRoutine");
    }

    IEnumerator SpawnObjectRoutine() {
        yield return new WaitForSeconds(1f);
        while (true) {
            SpawnObject();
            yield return new WaitForSeconds(objectInterval);
        }
    }

    private void SpawnObject() {
        // SpawnCoin
        int coinPosYIndex = GetCoinPositionYIndex();
        SpawnCoin(coinPosYIndex);

        // SpawnObstacle
        if (coinPosYIndex == arrCoinPosY.Length - 1) {
            SpawnObstacle();
        }
    }

    private int GetCoinPositionYIndex() {
        if (coinCount < 15) {
            return 0;
        }

        if (coinCount % 10 == 9) { // 9, 19, 29, 39, ...
            // middle
            return 1;
        } else if (coinCount % 10 == 0) { // 0, 10, 20, 30, ...
            // high
            return 2;
        } else if (coinCount % 10 == 1) { // 1, 11, 21, 31, ...
            // middle
            return 1;
        } else {
            // low
            return 0;
        }
    }

    private void SpawnCoin(int index) {
        // float posY = Random.Range(-2.5f, 0.5f);
        float posY = arrCoinPosY[index];
        Vector3 position = new Vector3(transform.position.x, posY, transform.position.z);
        Instantiate(coin, position, Quaternion.identity);
        coinCount++;
    }

    private void SpawnObstacle() {
        int obstaclePosYIndex = Random.Range(0, arrObstaclePosY.Length);
        float posY = arrObstaclePosY[obstaclePosYIndex];
        Vector3 position = new Vector3(transform.position.x, posY, transform.position.z);

        int obstacleIndex = Random.Range(0, obstacles.Length);
        Instantiate(obstacles[obstacleIndex], position, Quaternion.identity);
    }
}
