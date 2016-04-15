/*
* Copyright (C) JAMK/IT/Mika Mähönen
* This file is part of the IIO11300 course's final project.
* Created: 14.4.2016 Modified: 14.4.2016
* Authors: Mika Mähönen (K6058), Esa Salmikangas
*/
namespace Kuluseuranta.DB
{
  public abstract class BaseDB
  {
    #region PROPERTIES

    /// <summary>
    /// Connection String
    /// </summary>
    protected internal static string ConnectionString
    {
      get { return Properties.Settings.Default.ConnectionString; }
    }

    #endregion PROPERTIES
  }
}
