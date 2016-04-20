/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 20.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using Kuluseuranta.DB;
using Kuluseuranta.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Kuluseuranta.BL
{
  /// <summary>
  /// Class for Category Maintenance business logic
  /// </summary>
  public class CategoryMaintenance
  {
    #region PROPERTIES

    private static ObservableCollection<Category> categories;

    public static ObservableCollection<Category> CategoryList { get { return categories; } }

    /// <summary>
    /// Property for Logged User
    /// </summary>
    public static User LoggedUser { get; set; }

    /// <summary>
    /// Shows if collection has changes
    /// </summary>
    public static bool IsDirty
    {
      get { return categories.ToList().Exists(p => p.Status == Status.Deleted || p.Status == Status.Created || p.Status == Status.Modified); }
    }

    #endregion PROPERTIES

    #region METHODS

    /// <summary>
    /// Refresh Categories List
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    /// <param name="parentId">Parent Id (default Empty Guid)</param>
    /// <param name="includeArchived">If true archived users will be listed too (default false)</param>
    public static void RefreshCategories(User loggedUser, Guid parentId = default(Guid), bool includeArchived = false)
    {
      try
      {
        categories = new ObservableCollection<Category>();
        var list = DBCategories.GetList(loggedUser, parentId, includeArchived);

        foreach (var item in list)
        {
          categories.Add(item);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Create Category
    /// </summary>
    /// <param name="category">Category to Create</param>
    /// <returns>Count of affected rows</returns>
    public static int CreateCategory(Category category)
    {
      try
      {
        if (category.Id == Guid.Empty) category.Id = Guid.NewGuid();
        category.Created = DateTime.Now;
        category.CreatorId = LoggedUser.Id;

        int c = DBCategories.Create(category);
        if (c > 0)
        {
          category.Status = Status.Unchanged;
        }
        return c;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Update Category
    /// </summary>
    /// <param name="category">Category to Update</param>
    /// <returns>Count of affected rows</returns>
    public static int UpdateCategory(Category category)
    {
      try
      {
        category.Modified = DateTime.Now;
        category.ModifierId = LoggedUser.Id;

        int c = DBCategories.Update(category);
        if (c > 0)
        {
          category.Status = Status.Unchanged;
        }
        return c;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Delete Category
    /// </summary>
    /// <param name="category">Category to Delete</param>
    /// <returns>Count of affected rows</returns>
    public static int DeleteCategory(Category category)
    {
      try
      {
        int c = DBCategories.Delete(category);
        if (c > 0)
        {
          category.Status = Status.Unchanged;
        }
        return c;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Archive Category
    /// </summary>
    /// <param name="category">Category to Archive</param>
    /// <returns>Count of affected rows</returns>
    public static int ArchiveCategory(Category category)
    {
      try
      {
        category.Archived = DateTime.Now;
        category.ArchiverId = LoggedUser.Id;

        var subCategories = DBCategories.GetList(LoggedUser, category.Id);

        int c = 0;

        foreach (var subCategory in subCategories)
        {
          subCategory.Archived = category.Archived;
          subCategory.ArchiverId = LoggedUser.Id;
          c += DBCategories.Update(subCategory);
        }

        c += DBCategories.Update(category);
        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Saves changes to database
    /// </summary>
    /// <returns>Count of affected rows</returns>
    public static int SaveChanges()
    {
      int i = 0;

      try
      {
        // Remove deleted items
        List<Category> deletedList = categories.ToList().FindAll(p => p.Status == Status.Deleted);

        foreach (Category item in deletedList)
        {
          if (DeleteCategory(item) > 0) i++;
        }

        // Save created items
        List<Category> createdList = categories.ToList().FindAll(p => p.Status == Status.Created || p.Id == Guid.Empty);

        foreach (Category item in createdList)
        {
          if (CreateCategory(item) > 0) i++;
        }

        // Save modified items
        List<Category> modifiedList = categories.ToList().FindAll(p => p.Status == Status.Modified && p.Id != Guid.Empty);

        foreach (Category item in modifiedList)
        {
          if (UpdateCategory(item) > 0) i++;
        }

        return i;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    #endregion METHODS
  }
}
