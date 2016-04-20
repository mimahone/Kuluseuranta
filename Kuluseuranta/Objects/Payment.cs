/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 20.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kuluseuranta.Objects
{
  /// <summary>
  /// Class for Payment
  /// </summary>
  [Table("Payments")]
  public class Payment : BaseObject
  {
    #region PROPERTIES

    /// <summary>
    /// OwnerId property
    /// </summary>
    public Guid OwnerId { get; set; }

    private string payorsName;

    /// <summary>
    /// Payor's Name property
    /// </summary>
    public string PayorsName
    {
      get { return payorsName; }
      set
      {
        payorsName = value;
        Notify("PayorsName");
        Notify("DisplayName");
      }
    }

    private string payorsAccount;

    /// <summary>
    /// Payor's Account property
    /// </summary>
    public string PayorsAccount
    {
      get { return payorsAccount; }
      set
      {
        payorsAccount = value;
        Notify("PayorsAccount");
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
    [Column("Paid")]
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
    [NotMapped]
    public string DisplayName {
      get {
        if (PaidDate.HasValue)
        {
          return string.Format("{0}, {1}: {2:n} {3}", PaidDate.Value.ToShortDateString(), PayorsName, Amount, Currency);
        }

        return string.Format("---, {0}: {1:n} {2}", PayorsName, Amount, Currency);
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

    private double amount;

    /// <summary>
    /// Amount property
    /// </summary>
    public double Amount
    {
      get { return amount; }
      set
      {
        amount = value;
        Notify("DisplayName");
      }
    }

    private string currency = "EUR";

    /// <summary>
    /// Currency property
    /// </summary>
    public string Currency
    {
      get { return currency; }
      set
      {
        currency = value;
        Notify("DisplayName");
      }
    }

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
