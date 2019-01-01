﻿using Chat.Common;
using CommonTools.Log;
using Scripts.Containers;
using Scripts.Services;
using Scripts.UI.Core;
using Scripts.UI.Windows;
using Scripts.Utils;

namespace Scripts.UI.Controllers
{
    public class ChatController : MonoSingleton<ChatController>
    {
        private bool isChatWindowExists;

        private void Start()
        {
            CreateChatWindow();
            Connect();
        }

        private void Connect()
        {
            ChatConnectionProvider.GetInstance().Connect();
        }

        private void CreateChatWindow()
        {
            var chatWindow = UserInterfaceContainer.GetInstance().Add<ChatWindow>();
            chatWindow.MessageAdded += OnMessageAdded;

            isChatWindowExists = true;
        }

        private void RemoveChatWindow()
        {
            var chatWindow = UserInterfaceContainer.GetInstance()?.Get<ChatWindow>().AssertNotNull();
            if (chatWindow != null)
            {
                chatWindow.MessageAdded -= OnMessageAdded;
            }

            UserInterfaceContainer.GetInstance()?.Remove(chatWindow);
        }

        public void OnNonAuthorized()
        {
            var chatWindow = UserInterfaceContainer.GetInstance().Get<ChatWindow>().AssertNotNull();
            chatWindow.AddMessage("Authorization with chat server failed.", ChatMessageColor.Red);
        }

        public void OnConnectionClosed()
        {
            var chatWindow = UserInterfaceContainer.GetInstance()?.Get<ChatWindow>().AssertNotNull();
            chatWindow?.AddMessage("A connection with chat server has been closed.", ChatMessageColor.Red);
        }

        public void OnAuthorized()
        {
            var chatWindow = UserInterfaceContainer.GetInstance().Get<ChatWindow>().AssertNotNull();
            var chatPeerLogic = ServiceContainer.ChatService.GetPeerLogic<IChatPeerLogicAPI>().AssertNotNull();
            chatPeerLogic.ChatMessageReceived.AddListener(parameters => chatWindow.AddMessage(parameters.Message));
        }

        protected override void OnDestroying()
        {
            base.OnDestroying();

            if (isChatWindowExists)
            {
                RemoveChatWindow();
            }
        }

        private void OnMessageAdded(string message)
        {
            var chatPeerLogic = ServiceContainer.ChatService.GetPeerLogic<IChatPeerLogicAPI>();
            chatPeerLogic?.SendChatMessage(new ChatMessageRequestParameters(message));
        }
    }
}