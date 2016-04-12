using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Kuluseuranta.View
{
  /// <summary>
  /// Interaction logic for Users.xaml
  /// </summary>
  public partial class UsersWindow : Window
  {
    #region PROPERTIES

    /// <summary>
    /// Property for Logged User
    /// </summary>
    private User LoggedUser { get; set; }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    /// <summary>
    /// Constructor with Logged User
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    public UsersWindow(User loggedUser)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      UserManagement.LoggedUser = loggedUser;
      IniMyStuff();
    }

    private void IniMyStuff()
    {
      FillUserRoleCombo();
      btnRefresh_Click(this, null);

      if (LoggedUser.UserRole != UserRole.AdminUser)
      {
        btnNew.Visibility = Visibility.Collapsed;
        btnDelete.Visibility = Visibility.Collapsed;
        cboUserRole.IsEnabled = false;
      }

      txtFirstName.Focus();
    }

    #endregion CONSTRUCTORS

    #region METHODS

    private void FillUserRoleCombo()
    {
      Dictionary<int, string> roles = new Dictionary<int, string>();
      roles.Add(0, Localization.Language.NormalUser);
      roles.Add(1, Localization.Language.AdminUser);
      cboUserRole.ItemsSource = roles;
    }

    private bool HasDetailsErrors(User user)
    {
      bool errors = false;
      string message = "";
      TextBox txt = null;

      if (string.IsNullOrWhiteSpace(user.FirstName))
      {
        txt = txtFirstName;
        message = Localization.Language.FirstNameIsMissing;
        errors = true;
      }

      if (!string.IsNullOrEmpty(user.Email) && !Regex.IsMatch(user.Email.Trim(), @"^([a-zA-Z_])([a-zA-Z0-9_\-\.]*)@(\[((25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\.){3}|((([a-zA-Z0-9\-]+)\.)+))([a-zA-Z]{2,}|(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\])$"))
      {
        txt = txtFirstName;
        message = Localization.Language.EmailIsIncorrect;
        errors = true;
      }

      if (string.IsNullOrWhiteSpace(user.LastName))
      {
        txt = txtLastName;
        message = Localization.Language.LastNameIsMissing;
        errors = true;
      }

      if (errors)
      {
        MessageBox.Show(string.Format("{0} {1}!", Localization.Language.CannotSaveBecauseX, message));
        txt.Focus();
      }

      return errors;
    }

    #endregion METHODS

    #region EVENT HANDLERS

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
      string message = "";

      try
      {
        UserManagement.RefreshUsers();
        lstUsers.DataContext = UserManagement.UserList;
        message = string.Format(Localization.Language.UserListUpdatedAtX, DateTime.Now);
      }
      catch (Exception ex)
      {
        message = ex.Message;
        MessageBox.Show(ex.Message);
      }
      finally
      {
        lbMessages.Content = message;
      }
    }

    private void btnNew_Click(object sender, RoutedEventArgs e)
    {
      User newUser = new User();
      newUser.Status = Status.Created;
      spUser.DataContext = newUser;
      cboUserRole.SelectedIndex = 0;
      txtFirstName.Focus();
      lbMessages.Content = Localization.Language.AddNewUserMessage;
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        User user = (User)lstUsers.SelectedItem;

        MessageBoxResult result = MessageBox.Show(
          string.Format(Localization.Language.ConfirmDeletetingUserX, user.FullName), Localization.Language.ConfirmDeleteting,
          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
          if (user.Id != Guid.Empty)
          {
            if (UserManagement.ArchiveUser(user) > 0)
            {
              ((ObservableCollection<User>)lstUsers.DataContext).Remove(user);
            }
          }
          else // When not yet saved will be deleted ie. Id = Guid.Empty
          {
            ((ObservableCollection<User>)lstUsers.DataContext).Remove(user);
          }

          lbMessages.Content = string.Format(Localization.Language.UserXIsDeleted, user.FullName);
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = ex.Message;
      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (spUser.DataContext == null)
      {
        lbMessages.Content = Localization.Language.CannotSaveUserNotSelectedMessage;
        return;
      }

      User user = (User)spUser.DataContext;

      if (user == null) return;

      try
      {
        if (HasDetailsErrors(user)) return; // Check for field values

        // Check that same name user is not in the collection already
        User found = ((ObservableCollection<User>)lstUsers.DataContext).FirstOrDefault(p => p.FullName == user.FullName && p.Id != user.Id);

        if (found != null)
        {
          MessageBox.Show(Localization.Language.SameNameUserListedAlready);
          throw new Exception(Localization.Language.SameNameUserListedAlready);
        }

        user.UserRole = (UserRole)cboUserRole.SelectedValue;

        if (user.Id != Guid.Empty)
        {
          UserManagement.UpdateUser(user);
          lbMessages.Content = string.Format(Localization.Language.UsersXDetailsAreUpdated, user.FullName); 
        }
        else
        {
          UserManagement.CreateUser(user);
          ((ObservableCollection<User>)lstUsers.DataContext).Add(user);
          lstUsers.SelectedIndex = lstUsers.Items.IndexOf(user);
          lstUsers.ScrollIntoView(lstUsers.SelectedItem);
          lbMessages.Content = string.Format(Localization.Language.NewUserXIsSaved, user.FullName); 
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = string.Format(Localization.Language.CannotSaveBecauseX, ex.Message);
      }
    }

    private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (UserManagement.IsDirty)
      {
        MessageBoxResult result = MessageBox.Show(
          Localization.Language.ChangesAreNotSavedYetSaveNowMessage, Localization.Language.UnsavedChanges,
          MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
          btnSave_Click(this, null);
        }
        else
        {
          User current = (User)spUser.DataContext;
          current.Status = Status.Unchanged;
        }
      }

      if (lstUsers.SelectedItem != null)
      {
        User user = (User)lstUsers.SelectedItem;
        spUser.DataContext = user;
        cboUserRole.SelectedValue = (int)user.UserRole;
        lbMessages.Content = string.Format(Localization.Language.SelectedUserX, user.FullName);
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (UserManagement.IsDirty)
      {
        MessageBoxResult result = MessageBox.Show(
          Localization.Language.ChangesAreNotSavedYetSaveNowMessage, Localization.Language.UnsavedChanges,
          MessageBoxButton.YesNo, MessageBoxImage.Question);

        if(result == MessageBoxResult.Yes)
        {
          btnSave_Click(this, null);
        }
      }
    }

    private void btnChangePassword_Click(object sender, RoutedEventArgs e)
    {
      if (lstUsers.SelectedItem == null) return;
      ChangePasswordWindow w = new ChangePasswordWindow((User)lstUsers.SelectedItem);
      w.ShowDialog();
    }

    #endregion EVENT HANDLERS
  }
}
