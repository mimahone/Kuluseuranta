/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 4.4.2016
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
  /// Class for Payments Entering business logic
  /// </summary>
  public class PaymentsEntering
  {
    #region PROPERTIES

    private static ObservableCollection<Payment> payments;

    public static ObservableCollection<Payment> PaymentList { get { return payments; } }

    /// <summary>
    /// Property for Logged User
    /// </summary>
    public static User LoggedUser { get; set; }

    /// <summary>
    /// Shows if collection has changes
    /// </summary>
    public static bool IsDirty
    {
      get { return payments.ToList().Exists(p => p.Status == Status.Deleted || p.Status == Status.Created || p.Status == Status.Modified); }
    }

    #endregion

    #region METHODS

    /// <summary>
    /// Refresh Payments List
    /// </summary>
    /// <param name="options">Search Options</param>
    public static void RefreshPayments(SearchOptions options)
    {
      try
      {
        payments = new ObservableCollection<Payment>();
        DataTable table = DBPayments.GetPayments(options);

        // ORM
        Payment payment;

        foreach (DataRow row in table.Rows)
        {
          payment = new Payment(row.Field<Guid>(0));
          payment.OwnerId = row.Field<Guid>(1);
          payment.Payor = row.Field<string>(2);
          payment.DueDate = row.IsNull(3) ? (DateTime?)null : row.Field<DateTime>(3);
          payment.PaidDate = row.IsNull(4) ? (DateTime?)null : row.Field<DateTime>(4);
          payment.Amount = row.IsNull(5) ? 0 : row.Field<double>(5);
          payment.CategoryId = row.Field<Guid>(6);
          payment.SubCategoryId = row.Field<Guid>(7);
          payment.Notes = row.Field<string>(8);
          payment.Created = row.Field<DateTime>(9);
          payment.CreatorId = row.Field<Guid>(10);
          payment.Modified = row.IsNull(11) ? (DateTime?)null : row.Field<DateTime>(11);
          payment.ModifierId = row.IsNull(12) ? Guid.Empty : row.Field<Guid>(12);
          payment.Archived = row.IsNull(13) ? (DateTime?)null : row.Field<DateTime>(13);
          payment.ArchiverId = row.IsNull(14) ? Guid.Empty : row.Field<Guid>(14);
          payments.Add(payment);
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Create Payment
    /// </summary>
    /// <param name="payment">Payment to Create</param>
    /// <returns>Count of affected rows</returns>
    public static int CreatePayment(Payment payment)
    {
      try
      {
        if (payment.Id == Guid.Empty) payment.Id = Guid.NewGuid();
        payment.Created = DateTime.Now;
        payment.CreatorId = LoggedUser.Id;
        return DBPayments.CreatePayment(payment);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Update Payment
    /// </summary>
    /// <param name="payment">Payment to Update</param>
    /// <returns>Count of affected rows</returns>
    public static int UpdatePayment(Payment payment)
    {
      try
      {
        payment.Modified = DateTime.Now;
        payment.ModifierId = LoggedUser.Id;
        return DBPayments.UpdatePayment(payment);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Delete Payment
    /// </summary>
    /// <param name="payment">Payment to Delete</param>
    /// <returns>Count of affected rows</returns>
    public static int DeletePayment(Payment payment)
    {
      try
      {
        return DBPayments.DeletePayment(payment.Id);
      }
      catch (Exception)
      {
        throw;
      }
    }

    /// <summary>
    /// Archive Payment
    /// </summary>
    /// <param name="payment">Payment to Archive</param>
    /// <returns>Count of affected rows</returns>
    public static int ArchivePayment(Payment payment)
    {
      try
      {
        payment.Archived = DateTime.Now;
        payment.ArchiverId = LoggedUser.Id;
        return DBPayments.ArchivePayment(payment.Id, payment.ArchiverId);
      }
      catch (Exception)
      {
        throw;
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
        List<Payment> deletedList = payments.ToList().FindAll(p => p.Status == Status.Deleted);

        foreach (Payment item in deletedList)
        {
          if (DeletePayment(item) > 0)
          {
            item.Status = Status.Unchanged;
            i++;
          }
        }

        // Save created items
        List<Payment> createdList = payments.ToList().FindAll(p => p.Status == Status.Created || p.Id == Guid.Empty);

        foreach (Payment item in createdList)
        {
          if (CreatePayment(item) > 0)
          {
            item.Status = Status.Unchanged;
            i++;
          }
        }

        // Save modified items
        List<Payment> modifiedList = payments.ToList().FindAll(p => p.Status == Status.Modified && p.Id != Guid.Empty);

        foreach (Payment item in modifiedList)
        {
          if (UpdatePayment(item) > 0)
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

    #endregion
  }
}
