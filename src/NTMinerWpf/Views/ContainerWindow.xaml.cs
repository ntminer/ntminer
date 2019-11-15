﻿using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class ContainerWindow : BlankWindow {
        #region static
        private static readonly Dictionary<Type, ContainerWindow> s_windowDicByType = new Dictionary<Type, ContainerWindow>();
        private static readonly Dictionary<Type, double> s_windowLeftDic = new Dictionary<Type, double>();
        private static readonly Dictionary<Type, double> s_windowTopDic = new Dictionary<Type, double>();
        private static readonly Dictionary<ContainerWindowViewModel, ContainerWindow> s_windowDic = new Dictionary<ContainerWindowViewModel, ContainerWindow>();

        static ContainerWindow() {
        }

        public static ContainerWindow GetWindow(ContainerWindowViewModel vm) {
            if (!s_windowDic.ContainsKey(vm)) {
                return null;
            }
            return s_windowDic[vm];
        }

        private bool _isNew = true;
        public bool IsNew {
            get {
                return _isNew;
            }
        }

        public static ContainerWindow ShowWindow<TUc>(
            ContainerWindowViewModel vm,
            Func<ContainerWindow, TUc> ucFactory,
            Action<UserControl> beforeShow = null,
            Action afterClose = null,
            bool fixedSize = false) where TUc : UserControl {
            if (vm == null) {
                throw new ArgumentNullException(nameof(vm));
            }
            if (ucFactory == null) {
                throw new ArgumentNullException(nameof(ucFactory));
            }
            ContainerWindow window = new ContainerWindow(vm, ucFactory, fixedSize);
            if (vm.IsDialogWindow) {
                window.ShowWindow(beforeShow);
                return window;
            }
            Type ucType = typeof(TUc);
            if (s_windowDicByType.ContainsKey(ucType)) {
                window = s_windowDicByType[ucType];
                window._isNew = false;
            }
            else {
                s_windowDic.Add(vm, window);
                window.Closed += (object sender, EventArgs e) => {
                    s_windowDic.Remove(vm);
                    afterClose?.Invoke();
                };
                s_windowDicByType.Add(ucType, window);
                if (s_windowLeftDic.ContainsKey(ucType)) {
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                    window.Left = s_windowLeftDic[ucType];
                    window.Top = s_windowTopDic[ucType];
                }
            }
            window.ShowWindow(beforeShow);
            return window;
        }
        #endregion

        private readonly UserControl _uc;
        private readonly ContainerWindowViewModel _vm;

        public UserControl Uc {
            get { return _uc; }
        }

        public ContainerWindowViewModel Vm {
            get {
                return _vm;
            }
        }

        public ContainerWindow(
            ContainerWindowViewModel vm,
            Func<ContainerWindow, UserControl> ucFactory,
            bool fixedSize = false,
            bool dragMove = true) {
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Save_Executed, Save_Enabled));
            _uc = ucFactory(this);
            vm.UcResourceDic = _uc.Resources;
            _vm = vm;
            this.DataContext = _vm;
            if (vm.Height == 0 && vm.Width == 0) {
                this.SizeToContent = SizeToContent.WidthAndHeight;
            }
            else {
                if (vm.Height == 0) {
                    this.SizeToContent = SizeToContent.Height;
                }
                else {
                    this.Height = vm.Height;
                    if (vm.MinHeight == 0) {
                        this.MinHeight = vm.Height / 2;
                    }
                }
                if (vm.Width == 0) {
                    this.SizeToContent = SizeToContent.Width;
                }
                else {
                    this.Width = vm.Width;
                    if (vm.MinWidth == 0) {
                        this.MinWidth = vm.Width / 2;
                    }
                }
            }
            if (vm.MinHeight != 0) {
                this.MinHeight = vm.MinHeight;
            }
            if (vm.MinWidth != 0) {
                this.MinWidth = vm.MinWidth;
            }

            InitializeComponent();

            if (fixedSize) {
                if (vm.IsDialogWindow) {
                    this.ResizeMode = ResizeMode.NoResize;
                }
                else {
                    // 如果不是对话窗口则不能改变窗口尺寸但可以最小化
                    this.ResizeMode = ResizeMode.CanMinimize;
                }
                vm.MaxVisible = Visibility.Collapsed;
            }
            if (dragMove) {
                this.MouseDown += (object sender, MouseButtonEventArgs e) => {
                    if (e.LeftButton == MouseButtonState.Pressed) {
                        this.DragMove();
                    }
                };
            }
            this.Container.Children.Add(_uc);
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e) {
            if (_vm.OnOk == null) {
                return;
            }
            if (_vm.OnOk.Invoke(_uc)) {
                this.Close();
            }
        }

        private void Save_Enabled(object sender, CanExecuteRoutedEventArgs e) {
            e.CanExecute = true;
        }

        public void ShowWindow(Action<UserControl> beforeShow = null) {
            beforeShow?.Invoke(_uc);
            if (Vm.IsDialogWindow || Vm.HasOwner) {
                var owner = WpfUtil.GetTopWindow();
                if (this != owner) {
                    this.Owner = owner;
                }
            }
            if (Vm.IsDialogWindow || Vm.HasOwner || Vm.HeaderVisible == Visibility.Collapsed) {
                this.ShowInTaskbar = false;
            }
            if (Vm.IsDialogWindow) {
                this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                this.ShowDialogEx();
            }
            else {
                this.ShowActivated = true;
                this.Show();
                if (this.WindowState == WindowState.Minimized) {
                    this.WindowState = WindowState.Normal;
                }
                this.Activate();
            }
        }

        protected override void OnClosed(EventArgs e) {
            Vm.OnClose?.Invoke(_uc);
            Type ucType = _uc.GetType();
            if (s_windowDicByType.ContainsKey(ucType)) {
                if (s_windowLeftDic.ContainsKey(ucType)) {
                    s_windowLeftDic[ucType] = this.Left;
                }
                else {
                    s_windowLeftDic.Add(ucType, this.Left);
                }
                if (s_windowTopDic.ContainsKey(ucType)) {
                    s_windowTopDic[ucType] = this.Top;
                }
                else {
                    s_windowTopDic.Add(ucType, this.Top);
                }
                s_windowDicByType.Remove(ucType);
            }
            base.OnClosed(e);
        }

        private void WindowIcon_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount == 2) {
                this.Close();
            }
        }
    }
}
