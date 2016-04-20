/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 19.4.2016
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
  /// Class for User Management Dao
  /// </summary>
  public class DBUsers
  {
    #region PROPERTIES

    /// <summary>
    /// Password salt
    /// </summary>
    private const string _salt = "hti5I1&D1761";

    #endregion PROPERTIES

    #region METHODS

    /// <summary>
    /// Password hashing
    /// </summary>
    /// <param name="password">Password to Hash</param>
    /// <returns>Hashed password</returns>
    public static string CalculateHashedPassword(string password)
    {
      using (var sha = System.Security.Cryptography.SHA256.Create())
      {
        var computedHash = sha.ComputeHash(System.Text.Encoding.Unicode.GetBytes(password + _salt));
        return Convert.ToBase64String(computedHash);
      }
    }

    /// <summary>
    /// Get Users from Database
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    /// <param name="includeArchived">If true archived users will be listed too (default false)</param>
    /// <returns>List of Users</returns>
    public static List<User> GetList(User loggedUser, bool includeArchived = false)
    {
      List<User> list;

      try
      {
        using (var db = new PaymentsContext())
        {
          if (loggedUser.UserRole == UserRole.AdminUser)
          {
            if (includeArchived)
            {
              list = db.Users
              .OrderBy(p => p.FirstName)
              .ThenBy(p => p.LastName)
              .ToList();
            }
            else
            {
              list = db.Users
                .Where(p => p.Archived == null)
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToList();
            }
          }
          else
          {
            list = db.Users.Where(p => p.Id == loggedUser.Id).ToList();
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
    /// Creates new User to Database
    /// </summary>
    /// <param name="user">User to Create</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Create(User user)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Users.Add(user);
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Create<User>() failed to create new user!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Updates existing User in Database
    /// </summary>
    /// <param name="user">User to Update</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Update(User user)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Users.Attach(user);
          db.Entry(user).State = EntityState.Modified;
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Update<User>() failed to update user's details!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Delete User from Database
    /// </summary>
    /// <param name="user">User to Delete</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Delete(User user)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Users.Attach(user);
          db.Users.Remove(user);
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Delete<User>() failed to delete user!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Login User
    /// </summary>
    /// <param name="userName">User's user name or email</param>
    /// <param name="password">Password</param>
    /// <returns>User matching with User Name and Password</returns>
    public static User LoginUser(string userName, string password)
    {
      User user;

      try
      {
        string hashedPassword = CalculateHashedPassword(password);

        using (var db = new PaymentsContext())
        {
          user = db.Users.FirstOrDefault(p => (p.UserName == userName || p.Email == userName) && p.Password == hashedPassword);
        }

        return user;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Get User by User Name
    /// </summary>
    /// <param name="userName">User Name</param>
    /// <returns>Matching User by User Name</returns>
    public static User GetUserByUserName(string userName)
    {
      User user;

      try
      {
        using (var db = new PaymentsContext())
        {
          user = db.Users.FirstOrDefault(p => p.UserName == userName);
        }

        return user;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    #endregion METHODS

  }
}

//http://www.entityframeworktutorial.net/code-first/entity-framework-code-first.aspx