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
  /// Class for Payment
  /// </summary>
  public class Payment : BaseObject
  {
    #region PROPERTIES

    /// <summary>
    /// OwnerId property
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Payor's name property
    /// </summary>
    public string Payor { get; set; }

    /// <summary>
    /// Due Date property
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Paid Date property
    /// </summary>
    public DateTime? PaidDate { get; set; }

    /// <summary>
    /// Display Name property
    /// </summary>
    public string DisplayName {
      get {
        if (DueDate.HasValue)
        {
          return string.Format("{0}, {1}", Payor, DueDate.Value.ToShortDateString());
        }

        return string.Format("{0}, ---", Payor);
      }
    }

    /// <summary>
    /// Amount property
    /// </summary>
    public double Amount { get; set; }

    /// <summary>
    /// Notes property
    /// </summary>
    public string Notes { get; set; }

    /// <summary>
    /// CategoryId property
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// SubCategoryId property
    /// </summary>
    public Guid SubCategoryId { get; set; }

    #endregion

    #region CONSTRUCTORS

    /// <summary>
    /// Basic constructor
    /// </summary>
    public Payment() { }

    /// <summary>
    /// Constructor with id
    /// </summary>
    /// <param name="id">User id</param>
    public Payment(Guid id)
    {
      Id = id;
    }

    #endregion
  }
}
