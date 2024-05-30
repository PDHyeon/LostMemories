using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float jumpPower;
    private Vector2 curMovementInput;
    public LayerMask groundLayerMask;
    private float originSpeed;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    public bool canLook = true;

    public Action quickSlot;
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        originSpeed = moveSpeed;
    }

    private void Start()
    {
        // 마우스 커서 보이지 않도록 설정하기.
        Cursor.lockState = CursorLockMode.Locked;
        Debug.Log(originSpeed);
    }

    // 물리 연산관련은 FixedUpdate를 주로 이용
    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    private void Move()
    {
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= moveSpeed;
        dir.y = _rigidbody.velocity.y;

        _rigidbody.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed && IsGrounded())
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started && IsGrounded())
        {
            Debug.Log(((KeyControl)context.control).keyCode);
            _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
        }
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.1f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    public void Jump()
    {
        _rigidbody.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
    }

    public void BoostSpeed(float speedRate)
    {
        moveSpeed *= speedRate;
    }

    public void ResetSpeed()
    {
        moveSpeed = originSpeed;
    }

    public IEnumerator SpeedUp(float speedRate)
    {
        BoostSpeed(speedRate);

        yield return new WaitForSeconds(3f);

        ResetSpeed();
    }
}
