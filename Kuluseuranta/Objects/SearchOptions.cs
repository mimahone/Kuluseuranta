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
  /// Class for Search Options
  /// </summary>
  public class SearchOptions
  {
    /// <summary>
    /// Property for UserId option
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Property for Start Date option
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Property for End Date option
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Property for CategoryId option
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Property for SubCategoryId option
    /// </summary>
    public Guid? SubCategoryId { get; set; }
  }
}
