using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Win32MsgBox;

namespace Win32MsgBoxDemo.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();

            ErrorBtn.Click += Error;
            WarningBtn.Click += Warning;
            InfoBtn.Click += Info;
            YesNoBtn.Click += YesNo;
            YesNoCancelBtn.Click += YesNoCancel;
            RetryBtn.Click += Retry;
            RetryContinueBtn.Click += RetryContinue;
            AbortRetryBtn.Click += AbortRetry;
        }

        #region Button handlers

        private void Error(object? sender, RoutedEventArgs e)
        {
            Win32MsgBox.ShowError(this, "An error has occurred.");
        }

        private void Warning(object? sender, RoutedEventArgs e)
        {
            Win32MsgBox.ShowWarning(this, "Something bad might happen soon.");
        }

        private void Info(object? sender, RoutedEventArgs e)
        {
            Win32MsgBox.ShowInfo(this, "The operation has completed.");
        }

        private void YesNo(object? sender, RoutedEventArgs e)
        {
            MsgBoxRetVal ret = Win32MsgBox.ShowYesNo(this, "Exit program?");
            Win32MsgBox.ShowInfo(this, "User selected " + ret.ToString());
        }

        private void YesNoCancel(object? sender, RoutedEventArgs e)
        {
            MsgBoxRetVal ret = Win32MsgBox.ShowYesNoCancel(this, "Exit program?");
            Win32MsgBox.ShowInfo(this, "User selected " + ret.ToString());
        }

        private void Retry(object? sender, RoutedEventArgs e)
        {
            MsgBoxRetVal ret = Win32MsgBox.ShowRetryCancel(this, "Try again?");
            Win32MsgBox.ShowInfo(this, "User selected " + ret.ToString());
        }

        private void RetryContinue(object? sender, RoutedEventArgs e)
        {
            MsgBoxRetVal ret = Win32MsgBox.ShowCancelRetryContinue(this, "Try again or keep going?");
            Win32MsgBox.ShowInfo(this, "User selected " + ret.ToString());
        }

        private void AbortRetry(object? sender, RoutedEventArgs e)
        {
            MsgBoxRetVal ret = Win32MsgBox.ShowAbortRetryIgnore(this, "Not ready reading drive A");
            Win32MsgBox.ShowInfo(this, "User selected " + ret.ToString());
        }

        #endregion
    }
}