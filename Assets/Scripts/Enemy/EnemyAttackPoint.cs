using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemySpace
{
    public class EnemyAttackPoint : MonoBehaviour
    {
        [SerializeField] private EnemyController enemyController;
        //public delegate void EnemyAttackHandler(float damage);
        //public static event EnemyAttackHandler OnEnemyAttack;
        private bool delay = false;

        //private void OnEnable()
        //{
        //    Invoke("WaitForDelay", 0.5f);
        //}

        //private void OnDisable()
        //{
        //    delay = true;
        //}

        //private void WaitForDelay()
        //{
        //    delay = false;
        //}

        private async void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            if (player != null && !delay)
            {
                enemyController.InflictDamageToPlayer();
                delay = true;
                await UniTask.WaitForSeconds(0.4f);
                delay = false;
            }
        }
    }
}