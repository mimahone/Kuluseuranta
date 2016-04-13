using Kuluseuranta.Objects;
using System.Windows;

namespace Kuluseuranta.View
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private User LoggedUser { get; set; }

    public MainWindow(User loggedUser)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      IniMyStaff();
    }

    private void IniMyStaff()
    {
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
      else
      {
        tbLoggedUser.Text = string.Format(Localization.Language.LoggedUserX, LoggedUser.FullName);
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
