using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System;
using System.Windows;

namespace Kuluseuranta.View
{


  /// <summary>
  /// Interaction logic for ChangePasswordWindow.xaml
  /// </summary>
  public partial class ChangePasswordWindow : Window
  {
    /// <summary>
    /// Property for Target User
    /// </summary>
    public User TargetUser { get; private set; }

    #region Constructor

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="targetUser">Target User</param>
    public ChangePasswordWindow(User targetUser)
    {
      InitializeComponent();
      TargetUser = targetUser;
      pwOldPassword.Focus();
    }

    #endregion

    private void btnChangePassword_Click(object sender, RoutedEventArgs e)
    {
      if (pwPassword.Password.Length < 8)
      {
        MessageBox.Show(
            string.Format(Localization.Language.PasswordLengthMustBeAtLeastXMarks, 8), Localization.Language.CannotChangePassword,
            MessageBoxButton.OK, MessageBoxImage.Warning);
        pwPassword.Focus();
        return;
      }

      if (!string.IsNullOrWhiteSpace(pwPassword.Password) && !pwPassword.Password.Equals(pwPassword2.Password))
      {
        MessageBox.Show(
          Localization.Language.PasswordsDoesNotMatch, Localization.Language.CannotChangePassword, MessageBoxButton.OK, MessageBoxImage.Warning);
        pwPassword.Focus();
        return;
      }

      if (!string.IsNullOrWhiteSpace(pwPassword.Password) && pwOldPassword.Password.Equals(pwPassword.Password))
      {
        MessageBox.Show(
          Localization.Language.NewPasswordCannotBeOldPassword, Localization.Language.CannotChangePassword, MessageBoxButton.OK, MessageBoxImage.Warning);
        pwPassword.Focus();
        return;
      }

      try
      {
        if (!string.IsNullOrWhiteSpace(pwPassword.Password) && pwPassword.Password.Equals(pwPassword2.Password))
        {
          int i = UserManagement.SetPassword(TargetUser, pwPassword.Password);

          if (i > 0)
          {
            MessageBox.Show(Localization.Language.PasswordChanged, Localization.Language.PasswordChanging, MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
          }
          else
          {
            throw new Exception();
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(Localization.Language.PasswordChangingFailed + " " + ex.Message, Localization.Language.PasswordChanging, MessageBoxButton.OK, MessageBoxImage.Error);
      }
    }
  }
}
