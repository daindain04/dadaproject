using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleScroll : MonoBehaviour
{
    private float minPosY = -10f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameOver == false) {
            transform.position += Vector3.down * GameManager.instance.moveSpeed * Time.deltaTime;
            if (transform.position.y <= minPosY) {
                Destroy(gameObject);
                GameManager.instance.AddObstacleCount();
            }
        }
    }
}
