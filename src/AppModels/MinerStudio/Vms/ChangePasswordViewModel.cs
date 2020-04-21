﻿using NTMiner.Controllers;
using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class ChangePasswordViewModel : ViewModelBase {
        private Guid _actionCaptchaId = Guid.NewGuid();
        private string _actionCaptcha;
        private string _newPassword;
        private string _oldPassword;

        public Guid Id { get; private set; } = Guid.NewGuid();

        public ICommand RefreshCaptcha { get; private set; }
        public ICommand Ok { get; private set; }

        public ChangePasswordViewModel() {
            this.RefreshCaptcha = new DelegateCommand(() => {
                ActionCaptchaId = Guid.NewGuid();
            });
            this.Ok = new DelegateCommand(() => {
                // TODO:
            });
        }

        public string OldPassword {
            get => _oldPassword;
            set {
                _oldPassword = value;
                OnPropertyChanged(nameof(OldPassword));
            }
        }

        public string NewPassword {
            get => _newPassword;
            set {
                _newPassword = value;
                OnPropertyChanged(nameof(NewPassword));
            }
        }

        public Guid ActionCaptchaId {
            get => _actionCaptchaId;
            set {
                _actionCaptchaId = value;
                OnPropertyChanged(nameof(CaptchaUrl));
            }
        }
        /// <summary>
        /// <see cref="ISignUpInput.ActionCaptcha"/>
        /// </summary>
        public string ActionCaptcha {
            get => _actionCaptcha;
            set {
                _actionCaptcha = value;
                OnPropertyChanged(nameof(ActionCaptcha));
            }
        }

        public string CaptchaUrl {
            get {
                return $"http://{RpcRoot.OfficialServerAddress}/api/{RpcRoot.GetControllerName<ICaptchaController<string>>()}/{nameof(ICaptchaController<string>.Get)}?id={ActionCaptchaId.ToString()}";
            }
        }
    }
}