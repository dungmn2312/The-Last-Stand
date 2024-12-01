using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GrenadeController : Grenade
{
    public delegate void GrenadeExplodeHandler(GrenadeController grenade);
    public static event GrenadeExplodeHandler OnGrenadeExplode;

    private Rigidbody rb;
    private Vector3 direction;
    [SerializeField] private float force;


    internal Collider[] enemyCollider;

    private GameObject player;
    private Collider grenadeCollider;

    private void Awake()
    {
        //player = GameObject.FindWithTag("Player");
        rb = GetComponent<Rigidbody>();
        damage = 50;

        //Collider playerCollider = player.GetComponent<CharacterController>();
        //grenadeCollider = GetComponent<CapsuleCollider>();

        //Physics.IgnoreCollision(grenadeCollider, playerCollider);
    }

    
    public void ThrowGrenade(Transform startThrowPos)
    {

        rb.velocity = Vector3.zero;

        direction = startThrowPos.forward + new Vector3(0f, 0.2f, 0f);
        transform.position = startThrowPos.position;

        rb.AddForce(direction * force, ForceMode.Impulse);

        GrenadeExplode();
    }

    private async void GrenadeExplode()
    {
        await UniTask.WaitForSeconds(countDownTime);
        GrenadeSoundManager.Instance.PlayGrenadeExplosionSound();
        enemyCollider = GetEnemyCollider();
        OnGrenadeExplode?.Invoke(this);

        GrenadePooling.Instance.ReturnGrenade(gameObject);
    }

    private Collider[] GetEnemyCollider()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + new Vector3(0f, 1f, 0f), damageRadius);
        return colliders;
    }
}
