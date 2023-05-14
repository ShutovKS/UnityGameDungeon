﻿using Data.Dynamic;
using Data.Dynamic.Player;
using Data.Static;
using Services.PersistentProgress;
using Skill;
using UnityEngine;
using UnityEngine.Events;

namespace Units.Player
{
    public class Player : MonoBehaviour, IProgressLoadable
    {
        private PlayerStaticDefaultData _defaultData;
        private UnityAction _playerDead;
        private float _healthPoint;
        private float _protection;
        private bool _isDead;

        public void SetUp(UnityAction playerDead, PlayerStaticDefaultData playerStaticDefaultData)
        {
            _playerDead = playerDead;
            _defaultData = playerStaticDefaultData;

            var triggerGetHit = new GameObject("TriggerGetHit")
            {
                transform =
                {
                    parent = transform,
                    localPosition = Vector3.zero
                }
            };

            triggerGetHit.AddComponent<GetHit>().SetUp(TakeDamage);
        }

        private void TakeDamage(float healthLoss)
        {
            if (healthLoss - _protection < 0) return;

            _healthPoint -= (healthLoss - _protection);
            if (_healthPoint <= 0 && !_isDead)
            {
                _playerDead?.Invoke();
                _isDead = true;
            }

            Debug.Log(_healthPoint);
        }

        public void LoadProgress(Progress progress)
        {
            _healthPoint =
                (_defaultData.MaxHealthPoints + progress.skillsLevel.Skills[SkillsType.HEALTH_Count]) *
                (1 + progress.skillsLevel.Skills[SkillsType.HEALTH_Percent] / 100f);

            _protection =
                (_defaultData.ProtectionPoints + progress.skillsLevel.Skills[SkillsType.PROTECTION_Count]) *
                (1 + progress.skillsLevel.Skills[SkillsType.PROTECTION_Percent] / 100f);
        }
    }
}