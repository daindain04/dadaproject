using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float duration = 0.5f;

    [SerializeField]
    private float magnitude = 0.2f;

    private Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    public void Play() {
        StartCoroutine("Shake");
    }

    IEnumerator Shake() {
        float elapsedTime = 0f;
        while (elapsedTime < duration) {
            transform.position = initialPosition + (Vector3) Random.insideUnitCircle * magnitude;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = initialPosition;
    }
}
