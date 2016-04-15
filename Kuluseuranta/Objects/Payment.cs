/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 11.4.2016
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

    private string payor;

    /// <summary>
    /// Payor's name property
    /// </summary>
    public string Payor
    {
      get { return payor; }
      set
      {
        payor = value;
        Notify("Payor");
        Notify("DisplayName");
      }
    }

    /// <summary>
    /// Due Date property
    /// </summary>
    public DateTime? DueDate { get; set; }

    private DateTime? paidDate;

    /// <summary>
    /// Paid Date property
    /// </summary>
    public DateTime? PaidDate
    {
      get { return paidDate; }
      set
      {
        paidDate = value;
        Notify("PaidDate");
        Notify("DisplayName");
      }
    }

    /// <summary>
    /// Display Name property
    /// </summary>
    public string DisplayName {
      get {
        if (PaidDate.HasValue)
        {
          return string.Format("{0}, {1}", Payor, PaidDate.Value.ToShortDateString());
        }

        return string.Format("{0}, ---", Payor);
      }
    }

    private string referenceNumber;

    /// <summary>
    /// Reference Number property
    /// </summary>
    public string ReferenceNumber
    {
      get { return referenceNumber; }
      set
      {
        referenceNumber = value;
        Notify("ReferenceNumber");
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
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// SubCategoryId property
    /// </summary>
    public Guid? SubCategoryId { get; set; }

    #endregion PROPERTIES

    #region CONSTRUCTORS

    /// <summary>
    /// Basic constructor
    /// </summary>
    public Payment() { }

    /// <summary>
    /// Constructor with id
    /// </summary>
    /// <param name="id">Payment id</param>
    public Payment(Guid id)
    {
      Id = id;
    }

    #endregion CONSTRUCTORS
  }
}
