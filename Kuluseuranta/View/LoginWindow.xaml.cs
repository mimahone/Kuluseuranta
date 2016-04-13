using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System.Windows;

namespace Kuluseuranta.View
{
  

  /// <summary>
  /// Interaction logic for LoginWindow.xaml
  /// </summary>
  public partial class LoginWindow : Window
  {
    /// <summary>
    /// Property for Logged User
    /// </summary>
    public User LoggedUser { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public LoginWindow()
    {
      InitializeComponent();
      tbUserName.Focus();
    }

    private void Login_Click(object sender, RoutedEventArgs e)
    {
      if (!string.IsNullOrWhiteSpace(tbUserName.Text))
      {
        LoggedUser = UserManagement.LoginUser(tbUserName.Text, pwPassword.Password);
        if (LoggedUser == null)
        {
          MessageBox.Show(
            Localization.Language.LoginFailedCheckUsernameAndPassword, Localization.Language.LoginFailed,
            MessageBoxButton.OK, MessageBoxImage.Warning);
          tbUserName.Focus();
        }
        else
        {
          MainWindow w = new MainWindow(LoggedUser);
          w.Show();
          Close();
        }
      }
      else
      {
        MessageBox.Show(
            Localization.Language.LoginFailedCheckUsernameAndPassword, Localization.Language.LoginFailed,
            MessageBoxButton.OK, MessageBoxImage.Warning);
        tbUserName.Focus();
      }
    }
  }
}
