/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 1.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using System;

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
  public class User : BaseObject
  {
    #region PROPERTIES

    private string firstName;
    private string lastName;
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

    #endregion

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

    #endregion

  }
}
