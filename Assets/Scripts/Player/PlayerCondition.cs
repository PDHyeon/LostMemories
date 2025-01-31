﻿using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakePhysicalDamage(int damage);
}

public class PlayerCondition : MonoBehaviour, IDamageable
{
    public UICondition uiCondition;

    Condition health { get { return uiCondition.health; } }
    Condition stamina { get { return uiCondition.stamina; } }

    public event Action onTakeDamage;
    private void Update()
    {
        health.Subtract(health.passiveValue * Time.deltaTime);
        stamina.Add(stamina.passiveValue * Time.deltaTime);

        if (health.curValue <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void AddStamina(float amount)
    {
        stamina.Add(amount);
    }

    private void Die()
    {
        Debug.Log("죽었다.");
    }

    public void TakePhysicalDamage(int damage)
    {
        health.Subtract(damage);
        onTakeDamage?.Invoke();
    }
}
