using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System;
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
using System.Windows.Shapes;

namespace Kuluseuranta.View
{
  /// <summary>
  /// Interaction logic for UserNameWindow.xaml
  /// </summary>
  public partial class UserNameWindow : Window
  {
    #region PROPERTIES

    /// <summary>
    /// Property for Logged User
    /// </summary>
    private User LoggedUser { get; set; }

    /// <summary>
    /// Property for Target User
    /// </summary>
    private User TargetUser { get; set; }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    /// <summary>
    /// Constructor with Logged and Target User
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    /// <param name="targetUser">Target User</param>
    public UserNameWindow(User loggedUser, User targetUser)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      TargetUser = targetUser;
      tbUserName.Text = targetUser.UserName;
      tbUserName.SelectAll();
      tbUserName.Focus();
    }

    #endregion CONSTRUCTORS

    #region EVENT HANDLERS

    private void btnSetUserName_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        if (UserManagement.SetUserName(TargetUser, tbUserName.Text) > 0)
        {
          Close();
        }
        else
        {
          throw new Exception(Localization.Language.UnexpectedErrorOccurredWhileSaving);
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(
            string.Format(Localization.Language.SettingUserNameFailedBecauseOfX, ex.Message),
            Localization.Language.SettingUserName,
            MessageBoxButton.OK, MessageBoxImage.Warning);
        tbUserName.Focus();
      }
    }

    #endregion EVENT HANDLERS
  }
}
