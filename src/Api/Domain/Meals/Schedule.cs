namespace Api.Domain.Meals;

/// <summary>
/// Enum which defines when a meal is scheduled within the current week.
/// </summary>
public enum Schedule
{
    /// <summary>
    /// Meal is not scheduled for this week.
    /// </summary>
    None,
    /// <summary>
    /// Meal is scheduled for Monday.
    /// </summary>
    Monday,
    /// <summary>
    /// Meal is scheduled for Tuesday.
    /// </summary>
    Tuesday,
    /// <summary>
    /// Meal is scheduled for Wednesday.
    /// </summary>
    Wednesday,
    /// <summary>
    /// Meal is scheduled for Thursday.
    /// </summary>
    Thursday,
    /// <summary>
    /// Meal is scheduled for Friday.
    /// </summary>
    Friday,
    /// <summary>
    /// Meal is scheduled for Saturday.
    /// </summary>
    Saturday,
    /// <summary>
    /// Meal is scheduled for Sunday.
    /// </summary>
    Sunday
}