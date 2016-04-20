/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 20.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using Kuluseuranta.Objects;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;

namespace Kuluseuranta.DB
{
  /// <summary>
  /// Class for Category Maintenance Dao
  /// </summary>
  public class DBCategories : BaseDB
  {
    #region METHODS

    /// <summary>
    /// Get Categories from Database
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    /// <param name="parentId">Parent Id (default Empty Guid)</param>
    /// <param name="includeArchived">If true archived users will be listed too (default false)</param>
    /// <returns>List of Categories</returns>
    public static List<Category> GetList(User loggedUser, Guid parentId = default(Guid), bool includeArchived = false)
    {
      List<Category> list;

      try
      {
        using (var db = new PaymentsContext())
        {
          if (includeArchived)
          {
            list = db.Categories
              .Where(p => p.ParentId == parentId && (p.OwnerId == Guid.Empty || p.OwnerId == loggedUser.Id))
              .OrderBy(p => p.Name)
              .ToList();
          }
          else
          {
            list = db.Categories
              .Where(p => p.Archived == null && p.ParentId == parentId && (p.OwnerId == Guid.Empty || p.OwnerId == loggedUser.Id))
              .OrderBy(p => p.Name)
              .ToList();
          }
        }

        return list;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Creates new Category to Database
    /// </summary>
    /// <param name="category">Category to Create</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Create(Category category)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Categories.Add(category);
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Create<Category>() failed to create new category!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Updates existing Category in Database
    /// </summary>
    /// <param name="category">Category to Update</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Update(Category category)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Categories.Attach(category);
          db.Entry(category).State = EntityState.Modified;
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Update<Category>() failed to update category's details!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Delete Category with sub categories from Database
    /// </summary>
    /// <param name="category">Category to Delete</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Delete(Category category)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          var subCategories = db.Categories.Where(p => p.ParentId == category.Id);

          foreach (var subCategory in subCategories)
          {
            db.Categories.Attach(subCategory);
            db.Categories.Remove(subCategory);
          }

          db.Categories.Attach(category);
          db.Categories.Remove(category);
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Delete<Category>() failed to delete category!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    #endregion METHODS

  }
}
