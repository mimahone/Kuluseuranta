using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kuluseuranta.View
{
  /// <summary>
  /// Interaction logic for SubCategoriesWindow.xaml
  /// </summary>
  public partial class SubCategoriesWindow : Window
  {
    /// <summary>
    /// Property for Logged User
    /// </summary>
    private User LoggedUser { get; set; }

    /// <summary>
    /// Property for Parent Category Id
    /// </summary>
    private Guid ParentId { get; set; }

    #region Constructor

    /// <summary>
    /// Constructor with Logged User and Parent category
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    /// <param name="categoryId">Parent Category id</param>
    public SubCategoriesWindow(User loggedUser, Guid categoryId)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      ParentId = categoryId;
      IniMyStuff();
    }

    private void IniMyStuff()
    {
      FillTypeCombo();
      FillCategoryCombo();
      btnRefresh_Click(this, null);

      // If no sub categories -> Add new
      if (lstSubCategories.DataContext == null || lstSubCategories.Items.Count == 0)
      {
        btnNew_Click(this, null);
      }

      txtName.Focus();
    }

    #endregion Constructor

    private void FillCategoryCombo()
    {
      CategoryMaintenance.RefreshCategories(LoggedUser.Id);
      cboCategory.ItemsSource = CategoryMaintenance.CategoryList;
    }

    private void FillTypeCombo()
    {
      Dictionary<Guid, string> types = new Dictionary<Guid, string>();
      types.Add(LoggedUser.Id, Localization.Language.PersonalOnly);
      types.Add(Guid.Empty, Localization.Language.CommonForAll);
      cboTypes.ItemsSource = types;
    }

    private void btnRefresh_Click(object sender, RoutedEventArgs e)
    {
      string message = "";

      try
      {
        CategoryMaintenance.RefreshCategories(LoggedUser.Id, ParentId);
        lstSubCategories.DataContext = CategoryMaintenance.CategoryList;

        if (lstSubCategories.DataContext == null || lstSubCategories.Items.Count == 0)
        {
          message = Localization.Language.SelectedCategoryHasNoSubCategories;
        }
        else
        {
          message = string.Format(Localization.Language.SubCategoryListUpdatedAtX, DateTime.Now);
        }
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
      Category newSubCategory = new Category();
      newSubCategory.Status = Status.Created;
      newSubCategory.ParentId = ParentId;
      newSubCategory.Level = 2;
      spSubCategory.DataContext = newSubCategory;
      cboTypes.SelectedIndex = 0;
      txtName.Focus();
      lbMessages.Content = Localization.Language.AddNewSubCategoryMessage;
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Category subCategory = (Category)lstSubCategories.SelectedItem;

        MessageBoxResult result = MessageBox.Show(
          string.Format(Localization.Language.ConfirmDeletetingSubCategoryX, subCategory.Name), Localization.Language.ConfirmDeleteting,
          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
          if (subCategory.Id != Guid.Empty)
          {
            if (CategoryMaintenance.DeleteCategory(subCategory) > 0)
            {
              subCategory.Status = Status.Deleted;
              ((ObservableCollection<Category>)lstSubCategories.DataContext).Remove(subCategory);
            }
          }
          else // When not yet saved will be deleted ie. Id = Guid.Empty
          {
            ((ObservableCollection<Category>)lstSubCategories.DataContext).Remove(subCategory);
          }

          lbMessages.Content = Localization.Language.SubCategoryIsDeleted;
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = ex.Message;
      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (spSubCategory.DataContext == null)
      {
        lbMessages.Content = Localization.Language.CannotSaveSubCategoryNotSelectedMessage;
        return;
      }

      Category subCategory = (Category)spSubCategory.DataContext;

      if (subCategory == null) return;

      try
      {
        if (HasDetailsErrors(subCategory)) return; // Check for field values

        // Check that sub category with the same name is not in the collection already
        Category found = ((ObservableCollection<Category>)lstSubCategories.DataContext).FirstOrDefault(p => p.Name == subCategory.Name && p.Id != subCategory.Id);

        if (found != null)
        {
          MessageBox.Show(Localization.Language.SameNameSubCategoryListedAlready);
          throw new Exception(Localization.Language.SameNameSubCategoryListedAlready);
        }

        subCategory.OwnerId = (Guid)cboTypes.SelectedValue;

        if (subCategory.Id != Guid.Empty)
        {
          subCategory.Status = Status.Modified;
          CategoryMaintenance.UpdateCategory(subCategory);
          lbMessages.Content = string.Format(Localization.Language.SubCategorysXDetailsAreUpdated, subCategory.Name);
        }
        else
        {
          CategoryMaintenance.CreateCategory(subCategory);
          ((ObservableCollection<Category>)lstSubCategories.DataContext).Add(subCategory);
          lstSubCategories.SelectedIndex = lstSubCategories.Items.IndexOf(subCategory);
          lstSubCategories.ScrollIntoView(lstSubCategories.SelectedItem);
          lbMessages.Content = string.Format(Localization.Language.NewSubCategoryXIsSaved, subCategory.Name);
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = string.Format(Localization.Language.CannotSaveBecauseX, ex.Message);
      }
    }

    private bool HasDetailsErrors(Category subCategory)
    {
      bool errors = false;
      string message = "";
      TextBox txt = null;

      if (string.IsNullOrWhiteSpace(subCategory.Name))
      {
        txt = txtName;
        message = Localization.Language.SubCategoryIsMissing;
        errors = true;
      }

      if (errors)
      {
        MessageBox.Show(string.Format(Localization.Language.CannotSaveBecauseX, message));
        txt.Focus();
      }

      return errors;
    }

    private void btnReturn_Click(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void lstSubCategories_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      if (lstSubCategories.SelectedItem != null)
      {
        Category subCategory = (Category)lstSubCategories.SelectedItem;
        spSubCategory.DataContext = subCategory;
        cboTypes.SelectedValue = subCategory.OwnerId;
        lbMessages.Content = string.Format(Localization.Language.SelectedSubCategoryX, subCategory.Name);
      }
      else
      {
        Category subCategory = new Category();
        subCategory.ParentId = ((Category)cboCategory.SelectedItem).Id;
        spSubCategory.DataContext = subCategory;
        cboTypes.SelectedValue = LoggedUser.Id;
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      if (CategoryMaintenance.IsDirty)
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
        ParentId = category.Id;
        btnRefresh_Click(this, null);
      }
    }
  }
}
