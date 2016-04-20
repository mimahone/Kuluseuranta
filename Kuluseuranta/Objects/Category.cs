/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 20.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kuluseuranta.Objects
{
  /// <summary>
  /// Class for Category object
  /// </summary>
  [Table("Categories")]
  public class Category : BaseObject
  {
    #region PROPERTIES

    private string name;

    /// <summary>
    /// Name property
    /// </summary>
    public string Name
    {
      get { return name; }
      set
      {
        name = value;
        Notify("Name");
      }
    }

    /// <summary>
    /// Description property
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Level of Category
    /// (Will be 1 for main level Category)
    /// </summary>
    [Column("CategoryLevel")]
    public int Level { get; set; }

    /// <summary>
    /// Parent id for Category
    /// (Will be empty guid for main level Category)
    /// </summary>
    public Guid ParentId { get; set; }

    /// <summary>
    /// Owner id for Category
    /// (Will be empty guid if common Category)
    /// </summary>
    public Guid OwnerId { get; set; }

    private ObservableCollection<Category> subCategories = new ObservableCollection<Category>();

    /// <summary>
    /// List of sub categories for the category
    /// (Only next level sub categories list)
    /// </summary>
    [NotMapped]
    public ObservableCollection<Category> SubCategories
    {
      get
      {
        return subCategories;
      }
      set
      {
        subCategories = value;
      }
    }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    /// <summary>
    /// Basic constructor
    /// </summary>
    public Category() { }

    /// <summary>
    /// Constructor with id
    /// </summary>
    /// <param name="id">Category id</param>
    public Category(Guid id)
    {
      Id = id;
    }

    #endregion CONSTRUCTORS
  }
}
