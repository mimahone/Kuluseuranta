﻿using Kuluseuranta.Objects;
using System.Windows;

namespace Kuluseuranta.View
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private User LoggedUser = null;

    public MainWindow()
    {
      InitializeComponent();
      IniMyStaff();
    }

    private void IniMyStaff()
    {
      LoggedUser = BL.UserManagement.LoginUser("mika.mahonen@live.fi", "k6058");

      if (LoggedUser == null)
      {
        LoginWindow w = new LoginWindow();
        w.ShowDialog();
        LoggedUser = w.LoggedUser;

        bool isLogged = (LoggedUser != null);

        if (isLogged)
        {
          tbLoggedUser.Text = string.Format(Localization.Language.LoggedUserX, LoggedUser.FullName);
        }

        btnUsers.IsEnabled = isLogged;
        btnCategories.IsEnabled = isLogged;
        btnPayments.IsEnabled = isLogged;
        btnReports.IsEnabled = isLogged;
      }
    }

    private void btnUsers_Click(object sender, RoutedEventArgs e)
    {
      Visibility = Visibility.Hidden;
      UsersWindow w = new UsersWindow(LoggedUser);
      w.ShowDialog();
      Visibility = Visibility.Visible;
    }

    private void btnCategories_Click(object sender, RoutedEventArgs e)
    {
      Visibility = Visibility.Hidden;
      CategoriesWindow w = new CategoriesWindow(LoggedUser);
      w.ShowDialog();
      Visibility = Visibility.Visible;
    }

    private void btnPayments_Click(object sender, RoutedEventArgs e)
    {
      Visibility = Visibility.Hidden;
      PaymentsWindow w = new PaymentsWindow(LoggedUser);
      w.ShowDialog();
      Visibility = Visibility.Visible;
    }

    private void btnReports_Click(object sender, RoutedEventArgs e)
    {
      Visibility = Visibility.Hidden;
      ReportsWindow w = new ReportsWindow(LoggedUser);
      w.ShowDialog();
      Visibility = Visibility.Visible;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      Application.Current.Shutdown();
    }
  }
}