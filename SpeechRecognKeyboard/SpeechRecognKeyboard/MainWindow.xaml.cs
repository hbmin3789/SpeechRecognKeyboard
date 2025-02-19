﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SpeechRecognKeyboard
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSetKeySetting_Click(object sender, RoutedEventArgs e)
        {
            diaKeySetting.IsOpen = !diaKeySetting.IsOpen;
        }

        private void cbLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string stringformat = "Theme/Localization_{0}.xaml";
            string lang = (cbLanguage.SelectedItem as ComboBoxItem).Tag.ToString();
            var mergedDict = Application.Current.Resources.MergedDictionaries;

            mergedDict[mergedDict.Count - 1].Source = new Uri(string.Format(stringformat, lang), UriKind.Relative);
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            diaSetting.IsOpen = !diaSetting.IsOpen;
        }
    }
}
