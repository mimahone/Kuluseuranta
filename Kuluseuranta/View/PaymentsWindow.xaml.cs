﻿using Kuluseuranta.BL;
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
    /// <summary>
    /// Property for Logged User
    /// </summary>
    private User LoggedUser { get; set; }

    #region Constructor

    /// <summary>
    /// Constructor with Logged User
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    public PaymentsWindow(User loggedUser)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      IniMyStuff();
    }

    private void IniMyStuff()
    {
      FillCategoryCombo();
      btnRefresh_Click(this, null);
    }

    #endregion Constructor

    private void FillCategoryCombo()
    {
      CategoryMaintenance.RefreshCategories(LoggedUser.Id);
      cboCategory.ItemsSource = CategoryMaintenance.CategoryList;
    }

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
      string message = "";

      SearchOptions options = new SearchOptions {
        UserId = LoggedUser.Id
      };

      try
      {
        PaymentsEntering.RefreshPayments(options);
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
      cboCategory.SelectedIndex = -1;
      cboSubCategory.SelectedIndex = -1;
      cboSubCategory.ItemsSource = null;
      spPayment.DataContext = newPayment;
      txtPayor.Focus();
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

        payment.CategoryId = (cboCategory.SelectedValue == null ? Guid.Empty : (Guid)cboCategory.SelectedValue);
        payment.SubCategoryId = (cboSubCategory.SelectedValue == null ? Guid.Empty : (Guid)cboSubCategory.SelectedValue);

        if (payment.Id != Guid.Empty)
        {
          payment.Status = Status.Modified;
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

    private bool HasDetailsErrors(Payment payment)
    {
      bool errors = false;
      string message = "";
      TextBox txt = null;

      if (string.IsNullOrWhiteSpace(payment.Payor))
      {
        txt = txtPayor;
        message = Localization.Language.PayorIsMissing;
        errors = true;
      }

      if (errors)
      {
        MessageBox.Show(string.Format(Localization.Language.CannotSaveBecauseX, message));
        txt.Focus();
      }

      return errors;
    }

    private void lstPayments_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (lstPayments.SelectedItem != null)
      {
        Payment payment = (Payment)lstPayments.SelectedItem;
        spPayment.DataContext = payment;
        cboCategory.SelectedValue = payment.CategoryId;
        cboSubCategory.SelectedValue = payment.SubCategoryId;
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
        CategoryMaintenance.RefreshCategories(LoggedUser.Id, category.Id);
        cboSubCategory.ItemsSource = CategoryMaintenance.CategoryList;
      }
    }
  }
}
