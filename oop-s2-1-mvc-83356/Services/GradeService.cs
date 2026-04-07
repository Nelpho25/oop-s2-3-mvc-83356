using oop_s2_1_mvc_83356.Services;

namespace oop_s2_1_mvc_83356.Services;

public class GradeService : IGradeService
{
    private readonly ILogger<GradeService> _logger;

    public GradeService(ILogger<GradeService> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Calculate letter grade from percentage
    /// A: 90-100%, B: 75-89%, C: 60-74%, F: 0-59%
    /// </summary>
    public string CalculateGrade(decimal score, decimal maxScore)
    {
        if (!IsValidScore(score, maxScore))
        {
            _logger.LogWarning("Invalid score {Score} for max {MaxScore}", score, maxScore);
            return "F";
        }

        var percentage = GetPercentage(score, maxScore);
        
        return percentage switch
        {
            >= 90 => "A",
            >= 75 => "B",
            >= 60 => "C",
            _ => "F"
        };
    }

    /// <summary>
    /// Get percentage from score and max score
    /// </summary>
    public decimal GetPercentage(decimal score, decimal maxScore)
    {
        if (maxScore <= 0)
        {
            _logger.LogWarning("Invalid maxScore {MaxScore} for percentage calculation", maxScore);
            return 0;
        }

        var percentage = (score / maxScore) * 100;
        return Math.Min(percentage, 100); // Cap at 100%
    }

    /// <summary>
    /// Validate that score doesn't exceed max score
    /// </summary>
    public bool IsValidScore(decimal score, decimal maxScore)
    {
        return score >= 0 && score <= maxScore && maxScore > 0;
    }
}
