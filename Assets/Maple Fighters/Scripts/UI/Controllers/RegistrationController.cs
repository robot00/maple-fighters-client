﻿using System.Threading.Tasks;
using CommonTools.Coroutines;
using CommonTools.Log;
using Registration.Common;
using Scripts.Containers;
using Scripts.ScriptableObjects;
using Scripts.Services;
using Scripts.UI.Core;
using Scripts.UI.Windows;

namespace Scripts.UI.Controllers
{
    public class RegistrationConnector : ServiceConnector<RegistrationConnector>
    {
        public void Connect()
        {
            var connectionInformation = ServicesConfiguration.GetInstance().GetConnectionInformation(ServersType.Registration);
            CoroutinesExecutor.StartTask((yield) => Connect(yield, ServiceContainer.RegistrationService, connectionInformation));
        }

        protected override void OnPreConnection()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnConnectionFailed()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnConnectionEstablished()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnPreAuthorization()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnNonAuthorized()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnAuthorized()
        {
            throw new System.NotImplementedException();
        }
    }

    public class RegistrationController : MonoSingleton<RegistrationController>
    {
        private RegistrationWindow registrationWindow;
        private LoginWindow loginWindow;

        private void Start()
        {
            registrationWindow = UserInterfaceContainer.Instance.Add<RegistrationWindow>();

            SubscribeToRegistrationWindowEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromRegistrationWindowEvents();

            UserInterfaceContainer.Instance.Remove(registrationWindow);
        }

        private void SubscribeToRegistrationWindowEvents()
        {
            registrationWindow.RegisterButtonClicked += OnRegisterButtonClicked;
            registrationWindow.BackButtonClicked += OnBackButtonClicked;
            registrationWindow.ShowNotice += (message) => Utils.ShowNotice(message, () => registrationWindow.Show());
        }

        private void UnsubscribeFromRegistrationWindowEvents()
        {
            registrationWindow.RegisterButtonClicked -= OnRegisterButtonClicked;
            registrationWindow.BackButtonClicked -= OnBackButtonClicked;
        }

        private void OnRegisterButtonClicked(string email, string password, string firstName, string lastName)
        {
            var parameters = new RegisterRequestParameters(email, password.CreateSha512(), firstName, lastName);
            CoroutinesExecutor.StartTask((y) => Connect(y, parameters));
        }

        private async Task Connect(IYield yield, RegisterRequestParameters parameters)
        {
            var noticeWindow = Utils.ShowNotice("Registration is in a process.. Please wait.", () => registrationWindow.Show());
            noticeWindow.OkButton.interactable = false;

            if (!IsConnected())
            {
                var connectionInformation = ServicesConfiguration.GetInstance().GetConnectionInformation(ServersType.Registration);
                var connectionStatus = await Connect(yield, ServiceContainer.RegistrationService, connectionInformation);
                if (connectionStatus == ConnectionStatus.Failed)
                {
                    noticeWindow.Message.text = "Could not connect to a registration server.";
                    noticeWindow.OkButton.interactable = true;
                    return;
                }
            }

            CoroutinesExecutor.StartTask((y) => Register(y, parameters));
        }

        private async Task Register(IYield yield, RegisterRequestParameters paramters)
        {
            var noticeWindow = UserInterfaceContainer.Instance.Get<NoticeWindow>().AssertNotNull();
            var responseParameters = await ServiceContainer.RegistrationService.Register(yield, paramters);

            switch (responseParameters.Status)
            {
                case RegisterStatus.Succeed:
                {
                    noticeWindow.Message.text = "Registration is completed successfully.";
                    noticeWindow.OkButtonClickedAction = OnBackButtonClicked;
                    noticeWindow.OkButton.interactable = true;
                    break;
                }
                case RegisterStatus.EmailExists:
                {
                    noticeWindow.Message.text = "Email address already exists.";
                    noticeWindow.OkButton.interactable = true;
                    break;
                }
                default:
                {
                    noticeWindow.Message.text = "Something went wrong, please try again.";
                    noticeWindow.OkButton.interactable = true;
                    break;
                }
            }

            if (responseParameters.Status != RegisterStatus.Succeed)
            {
                DisconnectAutomatically();
            }
            else
            {
                Disconnect();
            }
        }

        private void OnBackButtonClicked()
        {
            registrationWindow.ResetInputFields.Invoke();

            // TODO: It should access a login controller (?)

            if (loginWindow == null)
            {
                loginWindow = UserInterfaceContainer.Instance.Get<LoginWindow>().AssertNotNull();
            }

            loginWindow.Show();
        }
    }
}