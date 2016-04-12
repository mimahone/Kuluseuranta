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
  /// Class for User Management business logic
  /// </summary>
  public class UserManagement
  {
    #region PROPERTIES

    private static ObservableCollection<User> users;

    public static ObservableCollection<User> UserList { get { return users; } }

    /// <summary>
    /// Property for Logged User
    /// </summary>
    public static User LoggedUser { get; set; }

    /// <summary>
    /// Shows if collection has changes
    /// </summary>
    public static bool IsDirty
    {
      get { return users.ToList().Exists(p => p.Status == Status.Deleted || p.Status == Status.Created || p.Status == Status.Modified); }
    }

    #endregion PROPERTIES

    #region METHODS

    /// <summary>
    /// Refresh Users List
    /// </summary>
    public static void RefreshUsers()
    {
      try
      {
        users = new ObservableCollection<User>();
        DataTable dt = DBUsers.GetUsers();

        // ORM

        if (LoggedUser.UserRole != UserRole.AdminUser)
        {
          DataRow[] row = dt.Select(string.Format("UserId='{0}'", LoggedUser.Id));
          users.Add(MakeUser(row[0]));
        }
        else
        {
          foreach (DataRow row in dt.Rows)
          {
            users.Add(MakeUser(row));
          }
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Makes User object of DataRow
    /// </summary>
    /// <param name="row">DataRow containing user data</param>
    /// <returns>User object</returns>
    private static User MakeUser(DataRow row)
    {
      if (row == null) return null;

      User user = new User(row.Field<Guid>(0));
      user.FirstName = row.Field<string>(1);
      user.LastName = row.Field<string>(2);
      user.Email = row.Field<string>(3);
      user.Notes = row.Field<string>(4);
      user.UserRole = (row.Field<bool>(5) ? UserRole.AdminUser : UserRole.BasicUser);
      user.Created = row.Field<DateTime>(6);
      user.CreatorId = row.Field<Guid>(7);
      user.Modified = row.IsNull(8) ? (DateTime?)null : row.Field<DateTime>(8);
      user.ModifierId = row.IsNull(9) ? Guid.Empty : row.Field<Guid>(9);
      user.Archived = row.IsNull(10) ? (DateTime?)null : row.Field<DateTime>(10);
      user.ArchiverId = row.IsNull(11) ? Guid.Empty : row.Field<Guid>(11);

      return user;
    }

    /// <summary>
    /// Create User
    /// </summary>
    /// <param name="user">User to Create</param>
    /// <returns>Count of affected rows</returns>
    public static int CreateUser(User user)
    {
      try
      {
        if (user.Id == Guid.Empty) user.Id = Guid.NewGuid();
        user.Created = DateTime.Now;
        user.CreatorId = LoggedUser.Id;

        int c = DBUsers.CreateUser(user);
        if (c > 0)
        {
          user.Status = Status.Unchanged;
        }
        return c;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Update User
    /// </summary>
    /// <param name="user">User to Update</param>
    /// <returns>Count of affected rows</returns>
    public static int UpdateUser(User user)
    {
      try
      {
        user.Modified = DateTime.Now;
        user.ModifierId = LoggedUser.Id;

        int c = DBUsers.UpdateUser(user);
        if (c > 0)
        {
          user.Status = Status.Unchanged;
        }
        return c;
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Delete User
    /// </summary>
    /// <param name="user">User to Delete</param>
    /// <returns>Count of affected rows</returns>
    public static int DeleteUser(User user)
    {
      try
      {
        return DBUsers.DeleteUser(user.Id);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Archive User
    /// </summary>
    /// <param name="user">User to Archive</param>
    /// <returns>Count of affected rows</returns>
    public static int ArchiveUser(User user)
    {
      try
      {
        user.Archived = DateTime.Now;
        user.ArchiverId = LoggedUser.Id;
        return DBUsers.ArchiveUser(user.Id, user.ArchiverId);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Login User
    /// </summary>
    /// <param name="userName">User Name</param>
    /// <param name="password">Password</param>
    /// <returns>User matching with User Name and Password</returns>
    public static User LoginUser(string userName, string password)
    {
      try
      {
        return MakeUser(DBUsers.LoginUser(userName, password));
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Set password for the user
    /// </summary>
    /// <param name="userId">Target User id</param>
    /// <param name="password">Password to set</param>
    /// <returns>Count of affected rows</returns>
    public static int SetPassword(Guid userId, string password)
    {
      try
      {
        return DBUsers.SetPassword(userId, password);
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
        List<User> deletedList = users.ToList().FindAll(p => p.Status == Status.Deleted);

        foreach (User item in deletedList)
        {
          if (DeleteUser(item) > 0)
          {
            item.Status = Status.Unchanged;
            i++;
          }
        }

        // Save created items
        List<User> createdList = users.ToList().FindAll(p => p.Status == Status.Created || p.Id == Guid.Empty);

        foreach (User item in createdList)
        {
          if (CreateUser(item) > 0)
          {
            item.Status = Status.Unchanged;
            i++;
          }
        }

        // Save modified items
        List<User> modifiedList = users.ToList().FindAll(p => p.Status == Status.Modified && p.Id != Guid.Empty);

        foreach (User item in modifiedList)
        {
          if (UpdateUser(item) > 0)
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
