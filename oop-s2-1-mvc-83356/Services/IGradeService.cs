using oop_s2_1_mvc_83356.Models;

namespace oop_s2_1_mvc_83356.Services;

/// <summary>
/// Service for grade and assessment calculations
/// </summary>
public interface IGradeService
{
    /// <summary>
    /// Calculate letter grade from numeric score
    /// </summary>
    string CalculateGrade(decimal score, decimal maxScore);

    /// <summary>
    /// Get percentage from score
    /// </summary>
    decimal GetPercentage(decimal score, decimal maxScore);

    /// <summary>
    /// Check if assignment result score is valid
    /// </summary>
    bool IsValidScore(decimal score, decimal maxScore);
}
