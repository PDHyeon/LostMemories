using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{
    // �յڷ� �����̵��� ��������.
    // Max ���� �������� -max, +max ���̿��� �̵��ϵ��� ������.
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
