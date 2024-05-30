using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPoint : MonoBehaviour
{
    public float jumpPower = 20.0f;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent<Player>(out Player player))
        {
            CharacterManager.Instance.Player.controller.Jump();
        }
    }
}
