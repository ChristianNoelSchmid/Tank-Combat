using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    [SerializeField]
    private float rotOffset;

    private Transform trans;

    private Camera mainCamera;

    void Start()
    {
        trans = transform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        float angleRadians = Mathf.Atan2(mouseWorldPos.y - trans.position.y, mouseWorldPos.x - trans.position.x);
        float angleDegrees = 180.0f / Mathf.PI * angleRadians;

        trans.rotation = Quaternion.Euler(0.0f, 0.0f, angleDegrees + rotOffset);
    }
}
