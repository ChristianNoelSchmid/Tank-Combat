using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform target;

    private Transform trans;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform; 
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = trans.position;

        //position = Vector3.Lerp(trans.position, target.position, Time.deltaTime * speed);

        position.x += Input.GetAxis("Horizontal") * speed;
        position.y += Input.GetAxis("Vertical") * speed;
        position.z = -10.0f;
        trans.position = position;
    }
}
