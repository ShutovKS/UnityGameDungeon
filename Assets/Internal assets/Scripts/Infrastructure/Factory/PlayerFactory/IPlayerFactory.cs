﻿using UnityEngine;

namespace Infrastructure.Factory.PlayerFactory
{
    public interface IPlayerFactory : IPlayerFactoryInfo
    {
        GameObject CreatePlayer(GameObject prefab, Vector3 position);
        void DestroyPlayer();
    }
}