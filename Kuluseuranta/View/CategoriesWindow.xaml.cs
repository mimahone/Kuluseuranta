using Kuluseuranta.BL;
using Kuluseuranta.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public static ObservableCollection<Category> Categories = new ObservableCollection<Category>();

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
        Categories = new ObservableCollection<Category>();
        Category root = new Category() { Name = Localization.Language.Categories };
        Categories.Add(root);

        foreach (Category category in CategoryMaintenance.CategoryList)
        {
          root.SubCategories.Add(category);
        }

        foreach (Category category in root.SubCategories)
        {
          CategoryMaintenance.RefreshCategories(LoggedUser.Id, category.Id);

          foreach (Category subCategory in CategoryMaintenance.CategoryList)
          {
            category.SubCategories.Add(subCategory);
          }
        }

        trvCategories.ItemsSource = Categories;

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
      try
      {
        spCategory.Visibility = Visibility.Visible;
        Category category = (Category)trvCategories.SelectedItem;

        Category newCategory = new Category();
        newCategory.CreatorId = LoggedUser.Id;

        if (category.Id == Guid.Empty) // Adding 1st level category
        {
          newCategory.ParentId = Guid.Empty;
          newCategory.Level = 1;
          lbMessages.Content = Localization.Language.AddNewCategoryMessage;
        }
        else // Adding 2nd level category
        {
          newCategory.ParentId = category.Id;
          newCategory.Level = 2;
          lbMessages.Content = Localization.Language.AddNewSubCategoryMessage;
        }

        spCategory.DataContext = newCategory;
        cboTypes.SelectedIndex = 0;
        txtName.Focus();
      }
      catch (Exception ex)
      {
        lbMessages.Content = ex.Message;
      }
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
      try
      {
        Category category = (Category)trvCategories.SelectedItem;

        if (category.Id == Guid.Empty)
        {
          MessageBox.Show(Localization.Language.RootCategoryCannotBeDeleted);
          return;
        }

        string message = "";

        if (category.ParentId == Guid.Empty)
        {
          message = string.Format(Localization.Language.ConfirmDeletetingCategoryX, category.Name);
        }
        else
        {
          message = string.Format(Localization.Language.ConfirmDeletetingSubCategoryX, category.Name);
        }

        MessageBoxResult result = MessageBox.Show(message, Localization.Language.ConfirmDeleteting, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
          if (category.Id != Guid.Empty)
          {
            if (CategoryMaintenance.DeleteCategory(category) > 0)
            {
              btnRefresh_Click(this, null);
            }
          }
          else // When not yet saved will be deleted ie. Id = Guid.Empty
          {
            btnRefresh_Click(this, null);
          }

          
          if (category.ParentId == Guid.Empty)
          {
            lbMessages.Content = string.Format(Localization.Language.CategoryXIsDeleted, category.Name);
          }
          else
          {
            lbMessages.Content = string.Format(Localization.Language.SubCategoryXIsDeleted, category.Name);
          }
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

        CategoryMaintenance.LoggedUser = LoggedUser;
        category.OwnerId = (Guid)cboTypes.SelectedValue;

        if (category.Id != Guid.Empty)
        {
          CategoryMaintenance.UpdateCategory(category);
          lbMessages.Content = string.Format(Localization.Language.CategorysXDetailsAreUpdated, category.Name);
        }
        else
        {
          CategoryMaintenance.CreateCategory(category);
          btnRefresh_Click(this, null);
          

          if (category.ParentId == Guid.Empty)
          {
            lbMessages.Content = string.Format(Localization.Language.NewCategoryXIsSaved, category.Name);
          }
          else
          {
            lbMessages.Content = string.Format(Localization.Language.NewSubCategoryXIsSaved, category.Name);
          }
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

    private void trvCategories_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      Category category = (Category)trvCategories.SelectedItem;

      if (category.Id == Guid.Empty)
      {
        spCategory.Visibility = Visibility.Hidden;
        btnDelete.Visibility = Visibility.Hidden;
        lbMessages.Content = Localization.Language.RootCategorySelected;
      }
      else
      {
        spCategory.Visibility = Visibility.Visible;
        btnDelete.Visibility = Visibility.Visible;

        spCategory.DataContext = category;
        cboTypes.SelectedValue = category.OwnerId;

        if (category.ParentId == Guid.Empty)
        {
          lbMessages.Content = string.Format(Localization.Language.SelectedCategoryX, category.Name);
        }
        else
        {
          lbMessages.Content = string.Format(Localization.Language.SelectedSubCategoryX, category.Name);
        }
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
