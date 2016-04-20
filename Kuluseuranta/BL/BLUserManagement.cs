/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 19.4.2016
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
    /// <param name="includeArchived">If true archived users will be listed too (default false)</param>
    public static void RefreshUsers(bool includeArchived = false)
    {
      try
      {
        users = new ObservableCollection<User>();
        var list = DBUsers.GetList(LoggedUser, includeArchived);

        foreach (var item in list)
        {
          users.Add(item);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
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

        int c = DBUsers.Create(user);
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

        int c = DBUsers.Update(user);
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
        int c = DBUsers.Delete(user);
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
        return DBUsers.Update(user);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Login User
    /// </summary>
    /// <param name="userName">User Name or email</param>
    /// <param name="password">Password</param>
    /// <returns>User matching with User Name and Password</returns>
    public static User LoginUser(string userName, string password)
    {
      try
      {
        return DBUsers.LoginUser(userName, password);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Set User Name for the user
    /// </summary>
    /// <param name="user">Target User</param>
    /// <param name="userName">User Name to set</param>
    /// <returns>Count of affected rows</returns>
    public static int SetUserName(User user, string userName)
    {
      try
      {
        if (!string.IsNullOrEmpty(userName))
        {
          //Check that user name is unique!
          User found = DBUsers.GetUserByUserName(userName);

          if (found != null && found.Id != user.Id)
          {
            throw new Exception(Localization.Language.UserNameAlreadyInUseMessage);
          }
        }

        user.UserName = userName;

        return DBUsers.Update(user);
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Set password for the user
    /// </summary>
    /// <param name="user">Target User</param>
    /// <param name="password">Password to set</param>
    /// <returns>Count of affected rows</returns>
    public static int SetPassword(User user, string password)
    {
      try
      {
        user.Password = DBUsers.CalculateHashedPassword(password);
        return DBUsers.Update(user);
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
          if (DeleteUser(item) > 0) i++;
        }

        // Save created items
        List<User> createdList = users.ToList().FindAll(p => p.Status == Status.Created || p.Id == Guid.Empty);

        foreach (User item in createdList)
        {
          if (CreateUser(item) > 0) i++;
        }

        // Save modified items
        List<User> modifiedList = users.ToList().FindAll(p => p.Status == Status.Modified && p.Id != Guid.Empty);

        foreach (User item in modifiedList)
        {
          if (UpdateUser(item) > 0) i++;
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
