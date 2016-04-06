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
  /// Interaction logic for CategoriesWindow.xaml
  /// </summary>
  public partial class CategoriesWindow : Window
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
    public CategoriesWindow(User loggedUser)
    {
      InitializeComponent();
      LoggedUser = loggedUser;
      IniMyStuff();
    }

    private void IniMyStuff()
    {
      FillTypeCombo();
      btnRefresh_Click(this, null);
      txtName.Focus();
    }

    #endregion Constructor

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
        CategoryMaintenance.RefreshCategories(LoggedUser.Id);
        lstCategories.DataContext = CategoryMaintenance.CategoryList;
        message = string.Format(Localization.Language.CategoryListUpdatedAtX, DateTime.Now);
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
      Category newCategory = new Category();
      newCategory.Status = Status.Created;
      newCategory.Level = 1;
      spCategory.DataContext = newCategory;
      cboTypes.SelectedIndex = 0;
      txtName.Focus();
      lbMessages.Content = Localization.Language.AddNewCategoryMessage;
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Category category = (Category)lstCategories.SelectedItem;

        MessageBoxResult result = MessageBox.Show(
          string.Format(Localization.Language.ConfirmDeletetingCategoryX, category.Name), Localization.Language.ConfirmDeleteting,
          MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
          if (category.Id != Guid.Empty)
          {
            if (CategoryMaintenance.DeleteCategory(category) > 0)
            {
              category.Status = Status.Deleted;
              ((ObservableCollection<Category>)lstCategories.DataContext).Remove(category);
            }
          }
          else // When not yet saved will be deleted ie. Id = Guid.Empty
          {
            ((ObservableCollection<Category>)lstCategories.DataContext).Remove(category);
          }

          lbMessages.Content = string.Format(Localization.Language.CategoryXIsDeleted, category.Name);
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = ex.Message;
      }
    }

    private void btnSave_Click(object sender, RoutedEventArgs e)
    {
      if (spCategory.DataContext == null)
      {
        lbMessages.Content = Localization.Language.CannotSaveCategoryNotSelectedMessage;
        return;
      }

      Category category = (Category)spCategory.DataContext;

      if (category == null) return;

      try
      {
        if (HasDetailsErrors(category)) return; // Check for field values

        // Check that category with the same name is not in the collection already
        Category found = ((ObservableCollection<Category>)lstCategories.DataContext).FirstOrDefault(p => p.Name == category.Name && p.Id != category.Id);

        if (found != null)
        {
          MessageBox.Show(Localization.Language.SameNameCategoryListedAlready);
          throw new Exception(Localization.Language.SameNameCategoryListedAlready);
        }

        category.OwnerId = (Guid)cboTypes.SelectedValue;

        if (category.Id != Guid.Empty)
        {
          category.Status = Status.Modified;
          CategoryMaintenance.UpdateCategory(category);
          lbMessages.Content = string.Format(Localization.Language.CategorysXDetailsAreUpdated, category.Name);
        }
        else
        {
          CategoryMaintenance.CreateCategory(category);
          ((ObservableCollection<Category>)lstCategories.DataContext).Add(category);
          lstCategories.SelectedIndex = lstCategories.Items.IndexOf(category);
          lstCategories.ScrollIntoView(lstCategories.SelectedItem);
          lbMessages.Content = string.Format(Localization.Language.NewCategoryXIsSaved, category.Name);
        }
      }
      catch (Exception ex)
      {
        lbMessages.Content = string.Format(Localization.Language.CannotSaveBecauseX, ex.Message);
      }
    }

    private bool HasDetailsErrors(Category category)
    {
      bool errors = false;
      string message = "";
      TextBox txt = null;

      if (string.IsNullOrWhiteSpace(category.Name))
      {
        txt = txtName;
        message = Localization.Language.CategoryIsMissing;
        errors = true;
      }

      if (errors)
      {
        MessageBox.Show(string.Format(Localization.Language.CannotSaveBecauseX, message));
        txt.Focus();
      }

      return errors;
    }

    private void btnSubCategories_Click(object sender, RoutedEventArgs e)
    {
      if (spCategory.DataContext == null || lstCategories.Items.Count == 0 || Guid.Parse(ID.Text) == Guid.Empty)
      {
        MessageBox.Show(
          Localization.Language.SelectCategoryFirstMessage,
          Localization.Language.CategorySelection,
          MessageBoxButton.OK, MessageBoxImage.Information);
        return;
      }

      SubCategoriesWindow w = new SubCategoriesWindow(LoggedUser, Guid.Parse(ID.Text));
      w.ShowDialog();
    }

    private void lstCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (lstCategories.SelectedItem != null)
      {
        Category category = (Category)lstCategories.SelectedItem;
        spCategory.DataContext = category;
        cboTypes.SelectedValue = category.OwnerId;
        lbMessages.Content = string.Format(Localization.Language.SelectedCategoryX, category.Name);
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

  }
}
