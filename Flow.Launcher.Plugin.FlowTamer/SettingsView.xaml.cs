using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Flow.Launcher.Plugin.FlowTamer
{
    public partial class SettingsView : UserControl
    {
        private readonly Settings settings;
        private string BTPath { get => settings.BTPath; set => settings.BTPath = value; }

        public SettingsView(Settings settings_)
        {
            settings = settings_;
            InitializeComponent();
        }

        private void SV_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            IconComboBox.SelectedIndex = (int) settings.Icon;
        }

        private void openBTPath(object sender, RoutedEventArgs eventArgs)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Browser Tamer|bt.exe";
            dlg.DefaultExt = ".exe";

            if ((bool) dlg.ShowDialog() && !(string.IsNullOrWhiteSpace(dlg.FileName))) {
                settings.BTPath = dlg.FileName;
            }
        }
        
        private void IconPathChanged(object sender, SelectionChangedEventArgs args) {
            settings.Icon = (IconTypes)IconComboBox.SelectedIndex;
        }
    }
}