﻿using System;
using Units.Enemy.State_Machines;
using Units.Enemy.State_Machines.State;
using UnityEngine;
using UnityEngine.AI;

namespace Units.Enemy
{
    public class Enemy : MonoBehaviour
    {
        private StateMachine _stateMachine;
        public Action<string> AnimationTriggerName;
        public bool playerInRange;

        public void SetUp(float healthPoints, float effectiveDistance, float cleavage, float attackCooldown,
            float speedMove, float speedRotate)
        {
            var thisTransform = transform;
            var playerTransform = GameObject.FindWithTag("Player").transform;

            var animator = TryGetComponent<Animator>(out var animatorComponent)
                ? animatorComponent
                : gameObject.AddComponent<Animator>();

            var navMeshAgent = TryGetComponent<NavMeshAgent>(out var agent)
                ? agent
                : gameObject.AddComponent<NavMeshAgent>();

            _stateMachine = new StateMachine();

            var combatReadiness = new CombatReadiness(animator, thisTransform, playerTransform, speedRotate);
            var searchPositionForPatrol = new SearchPositionForPatrol(thisTransform, out var targetTransform);
            var patrol = new Patrol(animator, navMeshAgent, thisTransform, targetTransform, speedMove / 2);
            var attack = new Attack(this, animator);
            var dead = new Dead(animator);
            var moveToPlayer = new MoveToPlayer(
                animator,
                navMeshAgent,
                thisTransform,
                playerTransform,
                speedMove,
                effectiveDistance * 0.85f);

            At(moveToPlayer, patrol, PlayerInRangeVisible());
            At(moveToPlayer, combatReadiness, PlayerNonInReachOfAttack());
            At(attack, combatReadiness, CanAttack());
            At(combatReadiness, moveToPlayer, PlayerInReachOfAttack());
            At(combatReadiness, attack, AttackOver());
            At(patrol, searchPositionForPatrol, HasTargetForPatrol());
            At(searchPositionForPatrol, moveToPlayer, PlayerNonInRangeVisible());
            At(searchPositionForPatrol, patrol, StuckForOverATwoSecondInPatrol());
            At(searchPositionForPatrol, patrol, ReachedPatrolPoint());
            AtAny(dead, IsDead());

            _stateMachine.SetState(searchPositionForPatrol);

            void At(IState to, IState from, Func<bool> condition) => _stateMachine.AddTransition(to, from, condition);
            void AtAny(IState to, Func<bool> condition) => _stateMachine.AddAnyTransition(to, condition);

            Func<bool> StuckForOverATwoSecondInPatrol() => () => patrol.TimeStuck >= 2f;
            Func<bool> PlayerInRangeVisible() => () => playerTransform != null && playerInRange;
            Func<bool> PlayerNonInRangeVisible() => () => !playerInRange;
            Func<bool> CanAttack() => () => combatReadiness.TimePassed >= attackCooldown;
            Func<bool> AttackOver() => () => attack.attackEnd;
            Func<bool> IsDead() => () => healthPoints <= 0;

            Func<bool> PlayerInReachOfAttack() => () =>
                Vector3.Distance(thisTransform.position, playerTransform.position) <= effectiveDistance * 0.9;

            Func<bool> PlayerNonInReachOfAttack() => () =>
                Vector3.Distance(thisTransform.position, playerTransform.position) > effectiveDistance * 1.1;

            Func<bool> HasTargetForPatrol() => () =>
                Vector3.Distance(thisTransform.position, targetTransform.position) > 5f;

            Func<bool> ReachedPatrolPoint() => () =>
                Vector3.Distance(thisTransform.position, targetTransform.position) < 1f;
        }

        private void Update() => _stateMachine?.Tick();
        public void AnimationTrigger(string triggerName) => AnimationTriggerName?.Invoke(triggerName);
    }
}