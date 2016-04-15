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
            categories.Add(makeCategory(row));
          }
        }
        else
        {
          foreach (DataRow row in dt.Select("Archived IS NULL"))
          {
            categories.Add(makeCategory(row));
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
    private static Category makeCategory(DataRow row)
    {
      if (row == null) return null;

      Category category = new Category(row.Field<Guid>("CategoryId"));
      category.ParentId = row.Field<Guid>("ParentId");
      category.Level = row.Field<int>("CategoryLevel");
      category.OwnerId = row.Field<Guid>("OwnerId");
      category.Name = row.Field<string>("Name");
      category.Description = row.Field<string>("Description");
      category.Created = row.Field<DateTime>("Created");
      category.CreatorId = row.Field<Guid>("CreatorId");
      category.Modified = row.Field<DateTime?>("Modified");
      category.ModifierId = row.IsNull("ModifierId") ? Guid.Empty : row.Field<Guid>("ModifierId");
      category.Archived = row.Field<DateTime?>("Archived");
      category.ArchiverId = row.IsNull("ArchiverId") ? Guid.Empty : row.Field<Guid>("ArchiverId");

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
