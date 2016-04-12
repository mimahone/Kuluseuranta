/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 11.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using Kuluseuranta.DB;
using Kuluseuranta.Objects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// <param name="userId">User Id</param>
    /// <param name="parentId">Parent Id (default Empty Guid)</param>
    /// <param name="includeArchived">If true archived users will be listed too (default false)</param>
    public static void RefreshCategories(Guid userId, Guid parentId = default(Guid), bool includeArchived = false)
    {
      try
      {
        categories = new ObservableCollection<Category>();

        DataTable dt = DBCategories.GetCategories(userId, parentId);

        // ORM

        if (includeArchived)
        {
          foreach (DataRow row in dt.Rows)
          {
            categories.Add(MakeCategory(row));
          }
        }
        else
        {
          foreach (DataRow row in dt.Select("Archived IS NULL"))
          {
            categories.Add(MakeCategory(row));
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Makes Category object of DataRow
    /// </summary>
    /// <param name="row">DataRow containing category data</param>
    /// <returns>Category object</returns>
    private static Category MakeCategory(DataRow row)
    {
      if (row == null) return null;

      Category category = new Category(row.Field<Guid>(0));
      category.ParentId = row.Field<Guid>(1);
      category.Level = row.Field<int>(2);
      category.OwnerId = row.Field<Guid>(3);
      category.Name = row.Field<string>(4);
      category.Description = row.Field<string>(5);
      category.Created = row.Field<DateTime>(6);
      category.CreatorId = row.Field<Guid>(7);
      category.Modified = row.IsNull(8) ? (DateTime?)null : row.Field<DateTime>(8);
      category.ModifierId = row.IsNull(9) ? Guid.Empty : row.Field<Guid>(9);
      category.Archived = row.IsNull(10) ? (DateTime?)null : row.Field<DateTime>(10);
      category.ArchiverId = row.IsNull(11) ? Guid.Empty : row.Field<Guid>(11);

      return category;
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

        int c = DBCategories.CreateCategory(category);
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

        int c = DBCategories.UpdateCategory(category);
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
        return DBCategories.DeleteCategory(category.Id);
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
        return DBCategories.ArchiveCategory(category.Id, category.ArchiverId);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Saves changes to database
    /// </summary>
    /// <returns>Count of affected rows</returns>
    public static int SaveChanges()
    {
      //TODO: Implement for EF
      int i = 0;

      try
      {
        // Remove deleted items
        List<Category> deletedList = categories.ToList().FindAll(p => p.Status == Status.Deleted);

        foreach (Category item in deletedList)
        {
          if (DeleteCategory(item) > 0)
          {
            item.Status = Status.Unchanged;
            i++;
          }
        }

        // Save created items
        List<Category> createdList = categories.ToList().FindAll(p => p.Status == Status.Created || p.Id == Guid.Empty);

        foreach (Category item in createdList)
        {
          if (CreateCategory(item) > 0)
          {
            item.Status = Status.Unchanged;
            i++;
          }
        }

        // Save modified items
        List<Category> modifiedList = categories.ToList().FindAll(p => p.Status == Status.Modified && p.Id != Guid.Empty);

        foreach (Category item in modifiedList)
        {
          if (UpdateCategory(item) > 0)
          {
            item.Status = Status.Unchanged;
            i++;
          }
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
