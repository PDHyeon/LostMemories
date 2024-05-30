using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{
    // 앞뒤로 움직이도록 설정하자.
    // Max 값을 기준으로 -max, +max 사이에서 이동하도록 만들자.
    public float moveMaxX;
    public float moveMaxY;
    public float moveMaxZ;

    public float moveSpeed;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector3.forward * moveSpeed;
    }
}
