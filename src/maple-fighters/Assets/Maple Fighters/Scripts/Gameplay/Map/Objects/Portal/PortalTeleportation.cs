﻿using System.Threading.Tasks;
using CommonTools.Coroutines;
using Game.Common;
using Scripts.Gameplay.Entity;
using Scripts.Services.Game;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Gameplay.Map.Objects
{
    [RequireComponent(typeof(EntityIdentifier))]
    public class PortalTeleportation : MonoBehaviour
    {
        private int entityId;
        private ExternalCoroutinesExecutor coroutinesExecutor;

        private void Start()
        {
            entityId = GetComponent<IEntity>().Id;
            coroutinesExecutor = new ExternalCoroutinesExecutor();
        }

        private void Update()
        {
            coroutinesExecutor?.Update();
        }

        private void OnDestroy()
        {
            coroutinesExecutor?.Dispose();
        }

        public void Teleport()
        {
            coroutinesExecutor?.StartTask(
                method: ChangeScene,
                onException: (e) => 
                    Debug.LogError("Failed to send change scene operation."));
        }

        private async Task ChangeScene(IYield yield)
        {
            var gameService = FindObjectOfType<GameService>();
            if (gameService != null)
            {
                var parameters = 
                    await gameService.GameSceneApi?.ChangeSceneAsync(
                        yield,
                        new ChangeSceneRequestParameters(entityId));

                var map = parameters.Map;
                if (map != 0)
                {
                    var mapName = map.ToString();
                    SceneManager.LoadScene(sceneName: mapName);
                }
                else
                {
                    Debug.Log(
                        $"Unable to teleport to the desired map index: {map}.");
                }
            }
        }
    }
}