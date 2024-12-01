using EnemySpace;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Enemy;

public class ChaseState : StateMachineBehaviour
{
    private NavMeshAgent agent;

    private EnemyController enemyController;
    private EnemySoundManager sound;
    private GameObject player;

    private Vector3 direction, pos;
    private float distance, stopChasingDistance = 1.1f;

    private float bigBoySpeed = 0.5f, normalSpeed = 2f, explosionSpeed = 0.8f;
    private float walkValue = 1.3f, runValue = 2f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (enemyController == null)
        {
            enemyController = animator.gameObject.GetComponent<EnemyController>();
            agent = enemyController.agent;

            sound = enemyController.sound;
        }
        
        SetEnemyAnimSpeed(animator);

        float stateValue = 0f;
        if (enemyController.attackType == EnemyAttackType.Normal)
        {
            stateValue = walkValue;
        }
        else
        {
            stateValue = runValue;
        }
        animator.SetFloat("enemy_state", stateValue);

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        sound.PlayChaseSound();

        if (enemyController.HP > 0)
        {
            direction = (player.transform.position - enemyController.transform.position).normalized;
            pos = player.transform.position - direction * stopChasingDistance;

            agent.SetDestination(pos);
        }
    }

    private void SetEnemyAnimSpeed(Animator animator)
    {
        if (enemyController.attackType == EnemyAttackType.Smite)
        {
            animator.speed = bigBoySpeed;
        }
        else if (enemyController.attackType == EnemyAttackType.Normal)
        {
            animator.speed = normalSpeed;
        }
        else
        {
            animator.speed = explosionSpeed;
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.speed = 1f;
        agent.ResetPath();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
