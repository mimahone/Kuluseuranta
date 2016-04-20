using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Kuluseuranta.View
{
  /// <summary>
  /// Interaction logic for ReportsWindow.xaml
  /// </summary>
  public partial class ReportsWindow : Window
  {
    #region FIELDS AND PROPERTIES

    ObservableCollection<Payment> localData;
    ICollectionView view; // For filttering data

    /// <summary>
    /// Property for Logged User
    /// </summary>
    private User LoggedUser { get; set; }

    #endregion FIELDS AND PROPERTIES

    #region CONSTRUCTORS

    /// <summary>
    /// Constructor with Logged User
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    public ReportsWindow(User loggedUser)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      IniMyStuff();
    }

    private void IniMyStuff()
    {
      dtpDate1.SelectedDate = DateTime.Today.AddMonths(-1);
      dtpDate2.SelectedDate = DateTime.Today;
      FillCategoryCombo();

      btnGet_Click(this, null);
    }

    #endregion CONSTRUCTORS

    #region METHODS

    private void FillCategoryCombo()
    {
      CategoryMaintenance.RefreshCategories(LoggedUser);
      var list = CategoryMaintenance.CategoryList;
      list.Insert(0, new Category(Guid.Empty) { Name = Localization.Language.AllCategories });
      cboCategory.ItemsSource = list;
      cboCategory.SelectedValue = Guid.Empty; // All Categories as default selection

      var subList = new ObservableCollection<Category>();
      subList.Insert(0, new Category(Guid.Empty) { Name = Localization.Language.AllSubCategories });
      cboSubCategory.ItemsSource = subList;
      cboSubCategory.SelectedValue = Guid.Empty; // All Sub Categories as default selection
    }

    private bool Filtering(object item)
    {
      Payment payment = (item as Payment);

      // Period filttering

      if (payment.DueDate.HasValue)
      {
        bool periodMatch = false;

        // Start Date Filttering
        periodMatch = payment.DueDate.Value >= dtpDate1.SelectedDate;
        if (!periodMatch) return false;

        // End Date Filttering
        periodMatch = payment.DueDate.Value <= dtpDate2.SelectedDate;
        if (!periodMatch) return false;
      }

      // Category (and SubCategory) filttering

      if (cboCategory.SelectedIndex == -1 || (Guid)cboCategory.SelectedValue == Guid.Empty)
      {
        return true;
      }
      else
      {
        bool categoryMatch = payment.CategoryId.Equals(cboCategory.SelectedValue);

        if (!categoryMatch) return false;

        if (cboSubCategory.SelectedIndex == -1 || (Guid)cboSubCategory.SelectedValue == Guid.Empty)
        {
          return categoryMatch;
        }
        else
        {
          return categoryMatch && payment.SubCategoryId.Equals(cboSubCategory.SelectedValue);
        }
      }
    }

    #endregion METHODS

    #region EVENT HANDLERS

    private void cboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cboCategory.SelectedItem != null)
      {
        Category category = (Category)cboCategory.SelectedItem;
        CategoryMaintenance.RefreshCategories(LoggedUser, category.Id, true);
        var list = CategoryMaintenance.CategoryList;
        list.Insert(0, new Category(Guid.Empty) { Name = Localization.Language.AllSubCategories });
        cboSubCategory.ItemsSource = list;
        cboSubCategory.SelectedValue = Guid.Empty; // All Sub Categories as default selection
      }
    }

    private void btnGet_Click(object sender, RoutedEventArgs e)
    {
      //var options = new SearchOptions
      //{
      //  StartDate = dtpDate1.SelectedDate,
      //  EndDate = dtpDate2.SelectedDate,
      //  CategoryId = (Guid?)cboCategory.SelectedValue,
      //  SubCategoryId = (Guid?)cboSubCategory.SelectedValue,
      //  UserId = LoggedUser.Id
      //};

      PaymentsEntering.RefreshPayments(LoggedUser);
      localData = PaymentsEntering.PaymentList;

      // View for filttering
      view = CollectionViewSource.GetDefaultView(localData);
      CollectionView view2 = new ListCollectionView(localData);

      // Filter payments according selected period, category and subcategory
      view.Filter = Filtering;
      view2.Filter = Filtering;

      dgReports.DataContext = localData;

      lbMessages.Content = string.Format(Localization.Language.DataRetrievedDetailsMessage, 
          ((DateTime)dtpDate1.SelectedDate).ToShortDateString(),
          ((DateTime)dtpDate2.SelectedDate).ToShortDateString(),
          cboCategory.Text,
          cboSubCategory.Text
        );

      double sum = 0;

      foreach (var item in view2)
      {
        sum += ((Payment)item).Amount;
      }

      lbRows.Content = string.Format(Localization.Language.RowsCountX, view2.Count);
      lbTotal.Content = string.Format(Localization.Language.RowsSumX, string.Format("{0:C}", sum));
    }

    private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
    {
      PaymentsWindow w = new PaymentsWindow(LoggedUser, (Payment)(sender as DataGridRow).Item);
      w.ShowDialog();
      btnGet_Click(this, null);
    }

    #endregion EVENT HANDLERS

  }
}
