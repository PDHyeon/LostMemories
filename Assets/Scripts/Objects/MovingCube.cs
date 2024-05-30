using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCube : MonoBehaviour
{
    // �յڷ� �����̵��� ��������.
    // Max ���� �������� -max, +max ���̿��� �̵��ϵ��� ������.
    public float moveMaxZ;

    Vector3 originVector;

    public float moveSpeed;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originVector = this.transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(transform.position.z) > moveMaxZ)
        {
            moveSpeed *= -1;
        }

        rb.velocity = Vector3.forward * moveSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            CharacterManager.Instance.Player.controller.AddAccSpeed(rb.velocity);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            CharacterManager.Instance.Player.controller.ResetAccSpeed();
            Debug.Log("������");
        }
    }
}
