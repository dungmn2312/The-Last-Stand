using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IEnemy
{
    public float HP;
    public float damage;
    public float speed;

    public enum EnemyState
    {
        Idle,
        Chase,
        Attack
    }

    public enum EnemyAttackType
    {
        Normal,
        Smite,
        Explode
    }

    public virtual void TakeDamage(bool isGrenadeDamage, bool isCrit)
    {

    }

    public virtual void Attack()
    {

    }

    public virtual void StopAttack()
    {

    }
}
