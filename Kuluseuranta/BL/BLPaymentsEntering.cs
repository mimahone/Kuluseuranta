/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 20.4.2016
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

    #endregion PROPERTIES

    #region METHODS

    /// <summary>
    /// Refresh Payments List
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    public static void RefreshPayments(User loggedUser)
    {
      try
      {
        payments = new ObservableCollection<Payment>();
        var list = DBPayments.GetList(loggedUser);

        foreach (var item in list)
        {
          payments.Add(item);
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

        int c = DBPayments.Create(payment);
        if (c > 0)
        {
          payment.Status = Status.Unchanged;
        }
        return c;
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

        int c = DBPayments.Update(payment);
        if (c > 0)
        {
          payment.Status = Status.Unchanged;
        }
        return c;
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
        int c = DBPayments.Delete(payment);
        if (c > 0)
        {
          payment.Status = Status.Unchanged;
        }
        return c;
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
        return DBPayments.Update(payment);
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
          if (DeletePayment(item) > 0) i++;
        }

        // Save created items
        List<Payment> createdList = payments.ToList().FindAll(p => p.Status == Status.Created || p.Id == Guid.Empty);

        foreach (Payment item in createdList)
        {
          if (CreatePayment(item) > 0) i++;
        }

        // Save modified items
        List<Payment> modifiedList = payments.ToList().FindAll(p => p.Status == Status.Modified && p.Id != Guid.Empty);

        foreach (Payment item in modifiedList)
        {
          if (UpdatePayment(item) > 0) i++;
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
