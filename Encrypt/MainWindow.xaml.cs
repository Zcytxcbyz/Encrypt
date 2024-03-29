﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace Encrypt
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string SettingFilePath =
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\"
            + Path.GetFileNameWithoutExtension(System.Windows.Forms.Application.ExecutablePath)
            + ".dat";
        public bool storylock = false;
        public MainWindow()
        {
                InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BeginStoryboard(FormLoadStory);
            this.Title = Properties.Resources.TitleLabelText;
            TitleLabel.Content = Properties.Resources.TitleLabelText;
            KeyLabel.Content = Properties.Resources.KeyLabelText;
            EncryptButton.Content = Properties.Resources.EncryptButtonText;
            DecryptButton.Content = Properties.Resources.DecryptButtonText;
            CloseButton.ToolTip = Properties.Resources.CloseButtonToolTip;
            MinButton.ToolTip = Properties.Resources.MinButtonToolTip;
            if (File.Exists(SettingFilePath))
            {
                FileStream fileStream = new FileStream(SettingFilePath, FileMode.OpenOrCreate, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                this.Top = binaryReader.ReadDouble();
                this.Left = binaryReader.ReadDouble();
                binaryReader.Close();
                fileStream.Close();
            }
            ErrorInfo.Text = "";
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorInfo.Text = "";
            string str = GetText(MainTextBox);
            string key = KeyTextBox.Text;
            string result = AESCrypto.AES.AESEncrypt(str, key);
            if (result != null)
            {
                LoadText(MainTextBox, result);
            }
            else
            {
                //LoadText(MainTextBox, AESCrypto.AES.ErrInfo.Message, Brushes.Red);
                MainTextBox.Document.Blocks.Clear();
                ErrorInfo.Text = Properties.Resources.EncryptedFailed;
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorInfo.Text = "";
            string str = GetText(MainTextBox);
            string key = KeyTextBox.Text;
            string result = AESCrypto.AES.AESDecrypt(str, key);
            if (result != null)
            {
                LoadText(MainTextBox, result);
            }
            else
            {
                //System.Collections.ListDictionaryInternal
                //LoadText(MainTextBox, AESCrypto.AES.ErrInfo.Message, Brushes.Red);
                MainTextBox.Document.Blocks.Clear();
                ErrorInfo.Text = Properties.Resources.DecryptedFailed;
            }
        }
        private string GetText(RichTextBox richTextBox)
        {
            TextRange textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            return textRange.Text;
        }
        private void LoadText(RichTextBox richTextBox, string txtContent)
        {
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(new Paragraph(new Run(txtContent)));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            FileStream fileStream = new FileStream(SettingFilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            binaryWriter.Write(this.Top);
            binaryWriter.Write(this.Left);
            binaryWriter.Close();
            fileStream.Close();
            this.ShowInTaskbar = false;
            BeginStoryboard(FormCloseStory);      
        }

        private void FormCloseStory_Completed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (!storylock)
            {
                if (this.WindowState == WindowState.Normal)
                {
                    storylock = true;
                    this.WindowState = WindowState.Normal;
                    BeginStoryboard(FormNorStory);

                }
                else if (this.WindowState == WindowState.Minimized)
                {
                    storylock = true;
                    this.WindowState = WindowState.Normal;
                    BeginStoryboard(FormMinStory);
                }
            }
        }

        private void FormMinStory_Completed(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;
            storylock = false;
        }

        private void FormNorStory_Completed(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
            storylock = false;
        }

        private void MainTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ErrorInfo.Text = "";
        }
    }
}

