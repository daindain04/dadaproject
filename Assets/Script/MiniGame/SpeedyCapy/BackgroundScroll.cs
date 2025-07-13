using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
    private float height = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.isGameOver == false) {
            transform.position += Vector3.down * GameManager.instance.moveSpeed * Time.deltaTime;
            if (transform.position.y <= -7.5f) {
                transform.position += new Vector3(0, height * 3f, 0);
            }
        }
    }
}
