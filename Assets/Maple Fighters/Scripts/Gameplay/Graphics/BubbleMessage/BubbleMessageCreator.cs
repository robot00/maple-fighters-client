﻿using UnityEngine;

namespace Assets.Scripts.Graphics
{
    public static class BubbleMessageCreator
    {
        private const string RESOURCE_PATH = "Game/Graphics/BubbleMessage";

        public static void Create(Transform owner, string message)
        {
            var bubbleMessageObject = Resources.Load<GameObject>(RESOURCE_PATH);
            var bubbleMessageGameObject = Object.Instantiate(bubbleMessageObject, owner.position, Quaternion.identity, owner);
            var bubbleMessage = bubbleMessageGameObject.GetComponent<BubbleMessage>();
            bubbleMessage.Initialize(message);
        }
    }
}