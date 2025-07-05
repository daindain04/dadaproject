using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapybaraGroundScroll : MonoBehaviour
{
    [SerializeField]
    private float offsetX = 0.1f;

    private Material material;

    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (CapybaraGameManager.instance.isGameOver == false) {
            Vector2 offset = new Vector2(offsetX * Time.deltaTime, 0);
            material.mainTextureOffset += offset;
        }
    }
}
