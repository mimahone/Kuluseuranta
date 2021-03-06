﻿/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 11.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kuluseuranta.Objects
{
  /// <summary>
  /// Enum for user roles
  /// </summary>
  public enum UserRole
  {
    BasicUser = 0,
    AdminUser = 1
  }

  /// <summary>
  /// Class for User object
  /// </summary>
  [Table("Users")]
  public class User : BaseObject
  {
    #region PROPERTIES

    private string firstName;
    private string lastName;
    private string userName;
    private string email;
    private string notes;
    private UserRole userRole;

    /// <summary>
    /// First name property
    /// </summary>
    public string FirstName
    {
      get { return firstName; }
      set
      {
        firstName = value;
        Notify("FirstName");
        Notify("FullName");
      }
    }

    /// <summary>
    /// Last name property
    /// </summary>
    public string LastName
    {
      get { return lastName; }
      set
      {
        lastName = value;
        Notify("LastName");
        Notify("FullName");
      }
    }

    /// <summary>
    /// Last name property
    /// </summary>
    public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

    /// <summary>
    /// User name property
    /// </summary>
    public string UserName
    {
      get { return userName; }
      set
      {
        userName = value;
        Notify("UserName");
      }
    }

    /// <summary>
    /// Email property
    /// </summary>
    public string Email
    {
      get { return email; }
      set
      {
        email = value;
        Notify("Email");
      }
    }

    /// <summary>
    /// Password property
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Notes property
    /// </summary>
    public string Notes
    {
      get { return notes; }
      set
      {
        notes = value;
        Notify("Notes");
      }
    }

    /// <summary>
    /// Photo property
    /// </summary>
    [NotMapped]
    public byte[] Photo { get; set; }

    /// <summary>
    /// User role for user
    /// </summary>
    public UserRole UserRole
    {
      get { return userRole; }
      set
      {
        userRole = value;
        Notify("UserRole");
      }
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    /// <summary>
    /// Basic constructor
    /// </summary>
    public User() { }

    /// <summary>
    /// Constructor with id
    /// </summary>
    /// <param name="id">User id</param>
    public User(Guid id)
    {
      Id = id;
    }

    #endregion CONSTRUCTORS

  }
}
