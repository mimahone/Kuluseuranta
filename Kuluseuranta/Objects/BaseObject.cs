/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 1.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using System;
using System.ComponentModel;

namespace Kuluseuranta.Objects
{
  /// <summary>
  /// Enum to contain object's modification status
  /// </summary>
  public enum Status
  {
    Unchanged = 0,
    Created = 1,
    Modified = 2,
    Deleted = 3
  }

  /// <summary>
  /// Base class with basic properties to inherit for other classes
  /// </summary>
  public abstract class BaseObject : INotifyPropertyChanged
  {
    #region PROPERTIES

    protected Guid id;
    protected DateTime created;
    protected DateTime? modified;

    /// <summary>
    /// Primary key (datarow identifier) for objects
    /// </summary>
    public Guid Id
    {
      get { return id; }
      set
      {
        id = value;
        Notify("Id");
      }
    }

    /// <summary>
    /// TimeStamp when object was Created
    /// </summary>
    public DateTime Created
    {
      get { return created; }
      set
      {
        created = value;
        Notify("Created");
      }
    }

    /// <summary>
    /// Creator Id (UserId) of user who created object
    /// </summary>
    public Guid CreatorId { get; set; }

    /// <summary>
    /// TimeStamp when object was last time Modified
    /// </summary>
    public DateTime? Modified
    {
      get { return modified; }
      set
      {
        modified = value;
        Notify("Modified");
      }
    }

    /// <summary>
    /// Modifier Id (UserId) of user who last time modified object
    /// </summary>
    public Guid ModifierId { get; set; }

    /// <summary>
    /// TimeStamp when object was last time Archived
    /// </summary>
    public DateTime? Archived { get; set; }

    /// <summary>
    /// Archiver Id (UserId) of user who archived object
    /// </summary>
    public Guid ArchiverId { get; set; }

    /// <summary>
    /// Object's modification status
    /// </summary>
    public Status Status { get; set; }

    #endregion

    #region INotifyPropertyChanged Members

    public event PropertyChangedEventHandler PropertyChanged;

    protected void Notify(string propName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
        Status = Status.Modified;
      }
    }

    #endregion INotifyPropertyChanged Members
  }
}
