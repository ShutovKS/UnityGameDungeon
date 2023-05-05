﻿using Data.Dynamic.PlayerData;
using Infrastructure.GlobalStateMachine.StateMachine;
using Item.Weapon;
using Services.PersistentProgress;
using Services.SaveLoad;
using Services.Watchers.SaveLoadWatcher;
using UnityEngine;

namespace Infrastructure.GlobalStateMachine.States
{
    public class ProgressLoadingForMainState : StateOneParam<GameInstance, GameObject>
    {
        public ProgressLoadingForMainState(GameInstance context, ISaveLoadService saveLoadService,
            ISaveLoadInstancesWatcher saveLoadInstancesWatcher,
            IPersistentProgressService persistentProgressService) : base(context)
        {
            _saveLoadService = saveLoadService;
            _saveLoadInstancesWatcher = saveLoadInstancesWatcher;
            _persistentProgressService = persistentProgressService;
        }

        private readonly ISaveLoadService _saveLoadService;
        private readonly ISaveLoadInstancesWatcher _saveLoadInstancesWatcher;
        private readonly IPersistentProgressService _persistentProgressService;

        public override void Enter(GameObject portal)
        {
            LoadProgressOrInitNew();

            InformProgressReaders();

            Context.StateMachine.SwitchState<MainLocationState, GameObject>(portal);
        }

        private void LoadProgressOrInitNew()
        {
            _persistentProgressService.SetProgress(_saveLoadService.LoadProgress() ?? InitNewProgress());
        }

        private PlayerProgress InitNewProgress()
        {
            return new PlayerProgress
            {
                selectedWeapon = new SelectedWeapon { weaponType = WeaponType.Sword }
            };
        }

        private void InformProgressReaders()
        {
            Debug.Log($"Debug information"); 
            foreach (var progressLoadable in _saveLoadInstancesWatcher.ProgressLoadable)
            {
                progressLoadable.LoadProgress(_persistentProgressService.PlayerProgress);
            }
        }
    }
}