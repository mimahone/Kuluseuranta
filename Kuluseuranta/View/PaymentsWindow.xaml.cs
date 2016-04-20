using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kuluseuranta.View
{
  /// <summary>
  /// Interaction logic for PaymentsWindow.xaml
  /// </summary>
  public partial class PaymentsWindow : Window
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
    /// <param name="payment">Default payment to select</param>
    public PaymentsWindow(User loggedUser, Payment payment = null)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      IniMyStuff();

      if (payment != null)
      {
        //TODO:...
        //lstPayments.SelectedItem = payment;
        //lstPayments.ScrollIntoView(lstPayments.SelectedItem);
      }
    }

    private void IniMyStuff()
    {
      FillCategoryCombo();
      btnRefresh_Click(this, null);
    }

    #endregion CONSTRUCTORS

    #region METHODS

    private void FillCategoryCombo()
    {
      CategoryMaintenance.RefreshCategories(LoggedUser);
      cboCategory.ItemsSource = CategoryMaintenance.CategoryList;
    }

    private bool HasDetailsErrors(Payment payment)
    {
      bool errors = false;
      string message = "";
      //TextBox txt = null;

      if (string.IsNullOrWhiteSpace(payment.PayorsName))
      {
        //txt = txtPayor;
        message = Localization.Language.PayorIsMissing;
        errors = true;
      }

      if (payment.PaidDate == null)
      {
        //txt = txtPayor;
        message = Localization.Language.PaidDateIsMissing;
        errors = true;
      }

      if (errors)
      {
        MessageBox.Show(string.Format(Localization.Language.CannotSaveBecauseX, message));
        //txt.Focus();
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
        PaymentsEntering.RefreshPayments(LoggedUser);
        lstPayments.DataContext = PaymentsEntering.PaymentList;
        message = string.Format(Localization.Language.PaymentListUpdatedAtX, DateTime.Now);
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
      Payment newPayment = new Payment();
      newPayment.Status = Status.Created;
      newPayment.OwnerId = LoggedUser.Id;
      newPayment.CreatorId = LoggedUser.Id;
      cboCategory.SelectedIndex = -1;
      cboSubCategory.SelectedIndex = -1;
      cboSubCategory.ItemsSource = null;

      spPayment.DataContext = newPayment;
      spPayment.Visibility = Visibility.Visible;
      txtPayorsName.Focus();
      lbMessages.Content = Localization.Language.AddNewPaymentMessage;
    }

    private void btnCopyNew_Click(object sender, RoutedEventArgs e)
    {
      Payment source = (Payment)spPayment.DataContext;

      Payment target = new Payment();
      target.Status = Status.Created;
      target.OwnerId = LoggedUser.Id;
      target.PayorsName = source.PayorsName;
      target.PayorsAccount = source.PayorsAccount;
      target.DueDate = null;
      target.PaidDate = null;
      target.ReferenceNumber = source.ReferenceNumber;
      target.Amount = source.Amount;
      target.Currency = source.Currency;
      target.CategoryId = source.CategoryId;
      target.SubCategoryId = source.SubCategoryId;
      target.Notes = source.Notes;
      target.CreatorId = LoggedUser.Id;

      spPayment.DataContext = target;
      spPayment.Visibility = Visibility.Visible;
      txtPayorsName.Focus();
      lbMessages.Content = Localization.Language.AddNewPaymentMessage;
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Payment payment = (Payment)lstPayments.SelectedItem;

        MessageBoxResult result = MessageBox.Show(
          string.Format(Localization.Language.ConfirmDeletetingPaymentX, payment.DisplayName), Localization.Language.ConfirmDeleteting,
          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
          if (payment.Id != Guid.Empty)
          {
            if (PaymentsEntering.DeletePayment(payment) > 0)
            {
              payment.Status = Status.Deleted;
              ((ObservableCollection<Payment>)lstPayments.DataContext).Remove(payment);
            }
          }
          else // When not yet saved will be deleted ie. Id = Guid.Empty
          {
            ((ObservableCollection<Payment>)lstPayments.DataContext).Remove(payment);
          }

          lbMessages.Content = Localization.Language.PaymentIsDeleted;
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = ex.Message;
      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (spPayment.DataContext == null)
      {
        lbMessages.Content = Localization.Language.CannotSavePaymentNotSelectedMessage;
        return;
      }

      Payment payment = (Payment)spPayment.DataContext;

      if (payment == null) return;

      try
      {
        if (HasDetailsErrors(payment)) return; // Check for field values

        // Check that Payment with the same name is not in the collection already
        Payment found = ((ObservableCollection<Payment>)lstPayments.DataContext).FirstOrDefault(p => p.DisplayName == payment.DisplayName && p.Id != payment.Id);

        if (found != null)
        {
          MessageBox.Show(Localization.Language.SameNamePaymentListedAlready);
          throw new Exception(Localization.Language.SameNamePaymentListedAlready);
        }

        if (cboCategory.SelectedValue != null) payment.CategoryId = (Guid)cboCategory.SelectedValue;
        if (cboSubCategory.SelectedValue != null) payment.SubCategoryId = (Guid)cboSubCategory.SelectedValue;

        PaymentsEntering.LoggedUser = LoggedUser;

        if (payment.Id != Guid.Empty)
        {
          PaymentsEntering.UpdatePayment(payment);
          lbMessages.Content = string.Format(Localization.Language.PaymentsXDetailsAreUpdated, payment.DisplayName);
        }
        else
        {
          PaymentsEntering.CreatePayment(payment);
          ((ObservableCollection<Payment>)lstPayments.DataContext).Add(payment);
          lstPayments.SelectedIndex = lstPayments.Items.IndexOf(payment);
          lstPayments.ScrollIntoView(lstPayments.SelectedItem);
          lbMessages.Content = string.Format(Localization.Language.NewPaymentXIsSaved, payment.DisplayName);
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = string.Format(Localization.Language.CannotSaveBecauseX, ex.Message);
      }
    }

    private void lstPayments_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (PaymentsEntering.IsDirty)
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
          Payment current = (Payment)spPayment.DataContext;
          current.Status = Status.Unchanged;
        }
      }

      if (lstPayments.SelectedItem != null)
      {
        Payment payment = (Payment)lstPayments.SelectedItem;
        spPayment.DataContext = payment;
        cboCategory.SelectedValue = payment.CategoryId;
        cboSubCategory.SelectedValue = payment.SubCategoryId;
        btnDelete.Visibility = Visibility.Visible;
        btnCopyNew.Visibility = Visibility.Visible;
        spPayment.Visibility = Visibility.Visible;
        lbMessages.Content = string.Format(Localization.Language.SelectedPaymentX, payment.DisplayName);
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (PaymentsEntering.IsDirty)
      {
        MessageBoxResult result = MessageBox.Show(
          Localization.Language.ChangesAreNotSavedYetSaveNowMessage, Localization.Language.UnsavedChanges,
          MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
          btnSave_Click(this, null);
        }
      }
    }

    private void cboCategory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (cboCategory.SelectedItem != null)
      {
        Category category = (Category)cboCategory.SelectedItem;
        CategoryMaintenance.RefreshCategories(LoggedUser, category.Id);
        cboSubCategory.ItemsSource = CategoryMaintenance.CategoryList;
      }
    }

    #endregion EVENT HANDLERS

  }
}
