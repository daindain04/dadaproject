using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraBackgroundScroll : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    private float width;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        width = renderer.sprite.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (CapybaraGameManager.instance.isGameOver == false) {
            transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            if (transform.position.x <= -width) {
                transform.position += new Vector3(width * 2f, 0, 0);
            }
        }
    }
}
