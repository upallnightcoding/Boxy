using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundScrolling : MonoBehaviour
{
    [SerializeField] Camera aCamera;

    private float speed = 0.75f;
    private float rePosition = 9.97f;

    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * speed * Time.deltaTime);

        if (transform.position.y < -rePosition + aCamera.transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, rePosition + aCamera.transform.position.y, 0.0f);
        }
    }
}
