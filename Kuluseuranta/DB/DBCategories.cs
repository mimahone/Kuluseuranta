/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 24.3.2016 Modified: 11.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
using Kuluseuranta.Objects;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Kuluseuranta.DB
{
  /// <summary>
  /// Class for Category Maintenance Dao
  /// </summary>
  public class DBCategories
  {
    #region PROPERTIES

    /// <summary>
    /// Connection String
    /// </summary>
    private static string ConnectionString
    {
      get { return Properties.Settings.Default.ConnectionString; }
    }

    #endregion PROPERTIES

    #region METHODS

    /// <summary>
    /// Get Categories as DataTable from Database
    /// </summary>
    /// <param name="userId">User Id</param>
    /// <param name="parentId">Parent Id (default Empty Guid)</param>
    /// <returns>DataTable containing Categories</returns>
    public static DataTable GetCategories(Guid userId, Guid parentId = default(Guid))
    {
      const string sql = @"
SELECT 
  CategoryId, ParentId, CategoryLevel, OwnerId, Name, Description,
  Created, CreatorId, Modified, ModifierId, Archived, ArchiverId
FROM 
  Categories
WHERE
  ParentId = @ParentId AND
  (OwnerId IN ('00000000-0000-0000-0000-000000000000', @OwnerId))
ORDER BY
  Name
";
      try
      {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(sql, connection);
          command.Parameters.AddWithValue("@ParentId", parentId);
          command.Parameters.AddWithValue("@OwnerId", userId);
 
          SqlDataAdapter adapter = new SqlDataAdapter(command);
          DataTable table = new DataTable("Categories");
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
    /// Creates new Category to Database
    /// </summary>
    /// <param name="category">Category to Create</param>
    /// <returns>Count of affected rows in database</returns>
    public static int CreateCategory(Category category)
    {
      try
      {
        const string sql = @"
INSERT INTO Categories (
  CategoryId,
  ParentId,
  CategoryLevel,
  OwnerId,
  Name,
  Description,
  Created,
  CreatorId
)
VALUES (
  @Id,
  @ParentId,
  @Level,
  @OwnerId,
  @Name,
  @Description,
  @Created,
  @CreatorId
)
";
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(sql, connection);
          command.Parameters.AddWithValue("@Id", category.Id);
          command.Parameters.AddWithValue("@ParentId", category.ParentId);
          command.Parameters.AddWithValue("@Level", category.Level);
          command.Parameters.AddWithValue("@OwnerId", category.OwnerId);
          command.Parameters.AddWithValue("@Name", category.Name);
          command.Parameters.AddWithValue("@Description", (category.Description == null ? "" : category.Description));
          command.Parameters.AddWithValue("@Created", category.Created);
          command.Parameters.AddWithValue("@CreatorId", category.CreatorId);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("CreateCategory() failed to create new category!");
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
    /// Updates existing Category to Database
    /// </summary>
    /// <param name="category">Category to Update</param>
    /// <returns>Count of affected rows in database</returns>
    public static int UpdateCategory(Category category)
    {
      try
      {
        const string cmdText = @"
UPDATE Categories SET
  ParentId = @ParentId,
  CategoryLevel = @Level,
  OwnerId = @OwnerId,
  Name = @Name,
  Description = @Description,
  Modified = @Modified,
  ModifierId = @ModifierId
WHERE
  CategoryId = @Id
";
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(cmdText, connection);
          command.Parameters.AddWithValue("@Id", category.Id);
          command.Parameters.AddWithValue("@ParentId", category.ParentId);
          command.Parameters.AddWithValue("@Level", category.Level);
          command.Parameters.AddWithValue("@OwnerId", category.OwnerId);
          command.Parameters.AddWithValue("@Name", category.Name);
          command.Parameters.AddWithValue("@Description", (category.Description == null ? "" : category.Description));
          command.Parameters.AddWithValue("@Modified", category.Modified);
          command.Parameters.AddWithValue("@ModifierId", category.ModifierId);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("UpdateCategory() failed to update category's details!");
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
    /// Delete Category from Database
    /// </summary>
    /// <param name="id">Id of Category to Delete</param>
    public static int DeleteCategory(Guid id)
    {
      try
      {
        const string cmdText = @"DELETE FROM Categories WHERE CategoryId = @Id OR ParentId = @Id";

        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
          SqlCommand command = new SqlCommand(cmdText, connection);
          command.Parameters.AddWithValue("@Id", id);

          connection.Open();
          int c = command.ExecuteNonQuery();
          connection.Close();

          if (c < 1)
          {
            throw new Exception("DeleteCategory() failed to delete category!");
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
    /// Archives existing Category to Database
    /// </summary>
    /// <param name="id">Id of Category to Archive</param>
    /// <param name="archiverId">Archiver (User) Id</param>
    /// <returns>Count of affected rows in database</returns>
    public static int ArchiveCategory(Guid id, Guid archiverId)
    {
      try
      {
        const string cmdText = @"
UPDATE Categories SET
  Archived = GETDATE(),
  ArchiverId = @ArchiverId
WHERE
  Archived IS NULL AND
  (CategoryId = @Id OR ParentId = @Id)
";
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
            throw new Exception("ArchiveCategory() failed to archive category's details!");
          }

          return c;
        }
      }
      catch (Exception ex)
      {
        throw ex;
      }
    }

    #endregion METHODS

  }
}
