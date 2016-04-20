/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 20.4.2016
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
  /// Class for Payment Enetering Dao
  /// </summary>
  public class DBPayments : BaseDB
  {
    #region METHODS

    /// <summary>
    /// Get List of Payments
    /// </summary>
    /// <param name="loggedUser">Logged User</param>
    /// <returns>List of Payments</returns>
    public static List<Payment> GetList(User loggedUser)
    {
      List<Payment> list;

      try
      {
        using (var db = new PaymentsContext())
        {
          list = db.Payments
            .Where(p => p.OwnerId == loggedUser.Id)
            .OrderByDescending(p => p.PaidDate)
            .ToList();
        }

        return list;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Creates new Payment to Database
    /// </summary>
    /// <param name="payment">Payment to Create</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Create(Payment payment)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Payments.Add(payment);
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Create<Payment>() failed to create new payment!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Updates existing Payment in Database
    /// </summary>
    /// <param name="payment">Payment to Update</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Update(Payment payment)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Payments.Attach(payment);
          db.Entry(payment).State = EntityState.Modified;
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Update<Payment>() failed to update payment's details!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Delete Payment from Database
    /// </summary>
    /// <param name="payment">Payment to Delete</param>
    /// <returns>Count of affected rows in database</returns>
    public static int Delete(Payment payment)
    {
      int c = 0;

      try
      {
        using (var db = new PaymentsContext())
        {
          db.Payments.Attach(payment);
          db.Payments.Remove(payment);
          c = db.SaveChanges();
        }

        if (c < 1)
        {
          throw new Exception("Delete<Payment>() failed to delete payment!");
        }

        return c;
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    #endregion METHODS

  }
}
