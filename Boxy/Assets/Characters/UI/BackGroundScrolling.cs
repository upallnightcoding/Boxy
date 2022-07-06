using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundScrolling : MonoBehaviour
{
    private float speed = 0.75f;
    private float rePosition = 9.97f;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.y < -rePosition)
        {
            transform.position = new Vector3(transform.position.x, rePosition, 0.0f);
        }
    }
}
