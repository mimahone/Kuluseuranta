/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 4.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using Kuluseuranta.Objects;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Kuluseuranta.DB
{
  /// <summary>
  /// Class for User Management Dao
  /// </summary>
  public class DBUsers
  {
    /// <summary>
    /// Connection String
    /// </summary>
    private static string ConnectionString
    {
      get { return Properties.Settings.Default.ConnectionString; }
    }

    /// <summary>
    /// Password salt
    /// </summary>
    private const string _salt = "hti5I1&D1761";

    /// <summary>
    /// Password hashing
    /// </summary>
    /// <param name="password">Password to Hash</param>
    /// <returns>Hashed password</returns>
    private static string CalculateHashedPassword(string password)
    {
      using (var sha = System.Security.Cryptography.SHA256.Create())
      {
        var computedHash = sha.ComputeHash(System.Text.Encoding.Unicode.GetBytes(password + _salt));
        return Convert.ToBase64String(computedHash);
      }
    }

    /// <summary>
    /// Get Users as DataTable from Database
    /// </summary>
    /// <returns>DataTable containing Users</returns>
    public static DataTable GetUsers()
    {
      const string sql = @"
SELECT 
  UserID, FirstName, LastName, Email, Notes, IsAdmin,
  Created, CreatorId, Modified, ModifierId, Archived, ArchiverId
FROM 
  Users
ORDER BY
  FirstName, LastName
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlDataAdapter adapter = new SqlDataAdapter(sql, connection);
          DataTable table = new DataTable("Users");
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
    /// Creates new User to Database
    /// </summary>
    /// <param name="user">User to Create</param>
    /// <returns>Count of affected rows in database</returns>
    public static int CreateUser(User user)
    {
      const string sql = @"
INSERT INTO Users (
  UserId,
  FirstName,
  LastName,
  Email,
  Notes,
  IsAdmin,
  Created,
  CreatorId
)
VALUES (
  @Id,
  @FirstName,
  @LastName,
  @Email,
  @Notes,
  @IsAdmin,
  @Created,
  @CreatorId
)
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(sql, connection);
          command.Parameters.AddWithValue("@Id", user.Id);
          command.Parameters.AddWithValue("@FirstName", user.FirstName);
          command.Parameters.AddWithValue("@LastName", user.LastName);
          command.Parameters.AddWithValue("@Email", (user.Email == null ? "" : user.Email));
          command.Parameters.AddWithValue("@Notes", (user.Notes == null ? "" : user.Notes));
          command.Parameters.AddWithValue("@IsAdmin", (int)user.UserRole);
          command.Parameters.AddWithValue("@Created", user.Created);
          command.Parameters.AddWithValue("@CreatorId", user.CreatorId);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("CreateUser() failed to create new user!");
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
    /// Updates existing User to Database
    /// </summary>
    /// <param name="user">User to Update</param>
    /// <returns>Count of affected rows in database</returns>
    public static int UpdateUser(User user)
    {
      const string cmdText = @"
UPDATE Users SET
  FirstName = @FirstName,
  LastName = @LastName,
  Email = @Email,
  Notes = @Notes,
  IsAdmin = @IsAdmin,
  Modified = @Modified,
  ModifierId = @ModifierId
WHERE
  UserID = @Id
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(cmdText, connection);
          command.Parameters.AddWithValue("@Id", user.Id);
          command.Parameters.AddWithValue("@FirstName", user.FirstName);
          command.Parameters.AddWithValue("@LastName", user.LastName);
          command.Parameters.AddWithValue("@Email", (user.Email == null ? "" : user.Email));
          command.Parameters.AddWithValue("@Notes", (user.Notes == null ? "" : user.Notes));
          command.Parameters.AddWithValue("@IsAdmin", (int)user.UserRole);
          command.Parameters.AddWithValue("@Modified", user.Modified);
          command.Parameters.AddWithValue("@ModifierId", user.ModifierId);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("UpdateUser() failed to update user's details!");
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
    /// Delete User from Database
    /// </summary>
    /// <param name="id">Id of User to Delete</param>
    /// <returns>Count of affected rows in database</returns>
    public static int DeleteUser(Guid id)
    {
      const string cmdText = @"DELETE FROM Users WHERE UserID = @Id";

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
            throw new Exception("DeleteUser() failed to delete user!");
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
    /// Archives existing User to Database
    /// </summary>
    /// <param name="id">Id of Item (User) to Archive</param>
    /// <param name="archiverId">Archiver (User) Id</param>
    /// <returns>Count of affected rows in database</returns>
    public static int ArchiveUser(Guid id, Guid archiverId)
    {
      const string cmdText = @"UPDATE Users SET Archived = GETDATE(), ArchiverId = @ArchiverId WHERE UserID = @Id";

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
            throw new Exception("ArchiveUser() failed to archive user's details!");
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
    /// Login User
    /// </summary>
    /// <param name="email">User's email</param>
    /// <param name="password">Password</param>
    /// <returns>User matching with Email and Password</returns>
    public static DataRow LoginUser(string email, string password)
    {
      const string sql = @"
SELECT 
  UserID, FirstName, LastName, Email, Notes, IsAdmin,
  Created, CreatorId, Modified, ModifierId, Archived, ArchiverId
FROM 
  Users
WHERE
  Email = @Email AND Password = @Password
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(sql, connection);
          command.Parameters.AddWithValue("@Email", email);
          command.Parameters.AddWithValue("@Password", CalculateHashedPassword(password));
          SqlDataAdapter adapter = new SqlDataAdapter(command);
          DataTable table = new DataTable("User");
          adapter.Fill(table);

          if (table.Rows.Count == 1)
            return table.Rows[0];
          else
            return null;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    /// <summary>
    /// Set password for the user
    /// </summary>
    /// <param name="userId">Target User id</param>
    /// <param name="password">Password to set</param>
    /// <returns>Count of affected rows</returns>
    public static int SetPassword(Guid userId, string password)
    {
      const string cmdText = @"UPDATE Users SET Password = @Password WHERE UserID = @Id";

      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(cmdText, connection);
          command.Parameters.AddWithValue("@Id", userId);
          command.Parameters.AddWithValue("@Password", CalculateHashedPassword(password));

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("SetPassword() failed to set user's password!");
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
