/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 1.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using Kuluseuranta.Objects;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Kuluseuranta.DB
{
  /// <summary>
  /// Class for Payment Enetering Dao
  /// </summary>
  public class DBPayments
  {
    /// <summary>
    /// Connection String
    /// </summary>
    private static string ConnectionString
    {
      get { return Properties.Settings.Default.ConnectionString; }
    }

    /// <summary>
    /// Get List of Payments
    /// </summary>
    /// <param name="options">Search Options</param>
    /// <returns>DataTable containing Payments</returns>
    public static DataTable GetPayments(SearchOptions options)
    {
      const string sql = @"
SELECT 
  PaymentID, OwnerID, PayorName, DueDate, Paid, Amount, CategoryID, SubCategoryID, Notes,
  Created, CreatorId, Modified, ModifierId, Archived, ArchiverId
FROM 
  Payments
WHERE
  OwnerID = @OwnerId
ORDER BY
  DueDate DESC
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(sql, connection);
          command.Parameters.AddWithValue("@OwnerId", options.UserId);

          //if (options.StartDate.HasValue)
          //{
          //  command.Parameters.AddWithValue("@StartDate", options.StartDate.Value);
          //}

          //if (options.EndDate.HasValue)
          //{
          //  command.Parameters.AddWithValue("@EndDate", options.EndDate.Value);
          //}

          //if (options.CategoryId.HasValue)
          //{
          //  command.Parameters.AddWithValue("@CategoryId", options.CategoryId.Value);
          //}

          //if (options.SubCategoryId.HasValue)
          //{
          //  command.Parameters.AddWithValue("@SubCategoryId", options.SubCategoryId.Value);
          //}

          SqlDataAdapter adapter = new SqlDataAdapter(command);
          DataTable table = new DataTable("Payments");
          adapter.Fill(table);
          return table;
        }
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
    public static int CreatePayment(Payment payment)
    {
      const string sql = @"
INSERT INTO Payments (
  PaymentID,
  OwnerID,
  PayorName,
  DueDate,
  Paid,
  Amount,
  Notes,
  CategoryID,
  SubCategoryID,  
  Created,
  CreatorId
)
VALUES (
  @Id,
  @OwnerId,
  @Payor,
  @DueDate,
  @PaidDate,
  @Amount,
  @Notes,
  @CategoryId,
  @SubCategoryId,
  @Created,
  @CreatorId
)
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(sql, connection);
          command.Parameters.AddWithValue("@Id", payment.Id);
          command.Parameters.AddWithValue("@OwnerId", payment.OwnerId);
          command.Parameters.AddWithValue("@Payor", payment.Payor);
          command.Parameters.AddWithValue("@DueDate", payment.DueDate);
          command.Parameters.AddWithValue("@PaidDate", payment.PaidDate);
          command.Parameters.AddWithValue("@Amount", payment.Amount);
          command.Parameters.AddWithValue("@Notes", (payment.Notes == null ? "" : payment.Notes));
          command.Parameters.AddWithValue("@CategoryId", payment.CategoryId);
          command.Parameters.AddWithValue("@SubCategoryId", payment.SubCategoryId);
          command.Parameters.AddWithValue("@Created", payment.Created);
          command.Parameters.AddWithValue("@CreatorId", payment.CreatorId);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("CreatePayment() failed to insert new payment!");
          }

          return c;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Updates existing Payment to Database
    /// </summary>
    /// <param name="payment">Payment to Update</param>
    /// <returns>Count of affected rows in database</returns>
    public static int UpdatePayment(Payment payment)
    {
      const string cmdText = @"
UPDATE Payments SET
  OwnerId = @OwnerId,
  PayorName = @Payor,
  DueDate = @DueDate,
  Paid = @PaidDate,
  Amount = @Amount,
  Notes = @Notes,
  CategoryId = @CategoryId,
  SubCategoryId = @SubCategoryId,
  Modified = @Modified,
  ModifierId = @ModifierId
WHERE
  PaymentID = @Id
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(cmdText, connection);
          command.Parameters.AddWithValue("@Id", payment.Id);
          command.Parameters.AddWithValue("@OwnerId", payment.OwnerId);
          command.Parameters.AddWithValue("@Payor", payment.Payor);
          command.Parameters.AddWithValue("@DueDate", payment.DueDate);
          command.Parameters.AddWithValue("@PaidDate", payment.PaidDate);
          command.Parameters.AddWithValue("@Amount", payment.Amount);
          command.Parameters.AddWithValue("@Notes", (payment.Notes == null ? "" : payment.Notes));
          command.Parameters.AddWithValue("@CategoryId", payment.CategoryId);
          command.Parameters.AddWithValue("@SubCategoryId", payment.SubCategoryId);
          command.Parameters.AddWithValue("@Modified", payment.Modified);
          command.Parameters.AddWithValue("@ModifierId", payment.ModifierId);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("UpdatePayment() failed to update payment's details!");
          }

          return c;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Delete Payment from Database
    /// </summary>
    /// <param name="id">Id of Payment to Delete</param>
    /// <returns>Count of affected rows in database</returns>
    public static int DeletePayment(Guid id)
    {
      const string cmdText = @"DELETE FROM Payments WHERE PaymentID = @Id";

      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(cmdText, connection);
          command.Parameters.AddWithValue("@Id", id);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("DeletePayment() failed to delete payment!");
          }

          return c;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Archives existing Payment to Database
    /// </summary>
    /// <param name="id">Id of Payment to Archive</param>
    /// <param name="archiverId">Archiver (User) Id</param>
    /// <returns>Count of affected rows in database</returns>
    public static int ArchivePayment(Guid id, Guid archiverId)
    {
      const string cmdText = @"UPDATE Payments SET Archived = GETDATE(), ArchiverId = @ArchiverId WHERE PaymentID = @Id";

      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(cmdText, connection);
          command.Parameters.AddWithValue("@Id", id);
          command.Parameters.AddWithValue("@ArchiverId", archiverId);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("ArchivePayment() failed to archive payment's details!");
          }

          return c;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

  }
}
