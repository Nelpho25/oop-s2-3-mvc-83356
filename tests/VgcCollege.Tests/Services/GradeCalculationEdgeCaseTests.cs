using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using oop_s2_1_mvc_83356.Services;
using Xunit;

namespace VgcCollege.Tests.Services;

public class GradeCalculationEdgeCaseTests
{
    private readonly GradeService _gradeService;

    public GradeCalculationEdgeCaseTests()
    {
        var loggerMock = new Mock<ILogger<GradeService>>();
        _gradeService = new GradeService(loggerMock.Object);
    }

    /// <summary>
    /// Tests grade calculation with various scores and grading boundaries.
    /// </summary>
    [Theory]
    [InlineData(0, 100, "F")]      // 0% → F
    [InlineData(39, 100, "F")]     // 39% → F (boundary)
    [InlineData(40, 100, "F")]     // 40% → F (service logic: no D grade)
    [InlineData(54, 100, "F")]     // 54% → F (service logic: no D grade)
    [InlineData(55, 100, "F")]     // 55% → F (below 60%)
    [InlineData(59, 100, "F")]     // 59% → F (below 60%)
    [InlineData(60, 100, "C")]     // 60% → C
    [InlineData(74, 100, "C")]     // 74% → C (boundary)
    [InlineData(75, 100, "B")]     // 75% → B
    [InlineData(89, 100, "B")]     // 89% → B (boundary)
    [InlineData(90, 100, "A")]     // 90% → A
    [InlineData(100, 100, "A")]    // 100% → A
    public void CalculateGrade_WithBoundaryScores_ReturnsCorrectGrade(decimal score, decimal maxScore, string expectedGrade)
    {
        var grade = _gradeService.CalculateGrade(score, maxScore);
        grade.Should().Be(expectedGrade);
    }

    /// <summary>
    /// Tests grade calculation with different max scores.
    /// </summary>
    [Theory]
    [InlineData(9, 10, "A")]       // 90% → A
    [InlineData(7, 10, "C")]       // 70% → C (between 60-75%)
    [InlineData(3, 10, "F")]       // 30% → F
    [InlineData(45, 50, "A")]      // 90% → A
    [InlineData(20, 50, "F")]      // 40% → F
    [InlineData(72, 100, "C")]     // 72% → C
    [InlineData(105, 100, "F")]    // Invalid: score > max → F
    [InlineData(0, 0, "F")]        // Edge: zero division → F
    public void CalculateGrade_WithVariousMaxScores_ReturnsCorrectGrade(decimal score, decimal maxScore, string expectedGrade)
    {
        var grade = _gradeService.CalculateGrade(score, maxScore);
        grade.Should().Be(expectedGrade);
    }

    /// <summary>
    /// Tests percentage calculation with edge cases.
    /// </summary>
    [Theory]
    [InlineData(0, 100, 0)]       // 0/100 = 0%
    [InlineData(50, 100, 50)]     // 50/100 = 50%
    [InlineData(100, 100, 100)]   // 100/100 = 100%
    [InlineData(150, 100, 100)]   // 150/100 = 150% → capped at 100%
    [InlineData(0, 50, 0)]        // 0/50 = 0%
    [InlineData(25, 50, 50)]      // 25/50 = 50%
    [InlineData(50, 0, 0)]        // 50/0 → handled safely → 0%
    public void GetPercentage_WithEdgeCases_ReturnsCorrectPercentage(decimal score, decimal maxScore, decimal expectedPercentage)
    {
        var percentage = _gradeService.GetPercentage(score, maxScore);
        percentage.Should().Be(expectedPercentage);
    }

    /// <summary>
    /// Tests score validation with edge cases.
    /// </summary>
    [Theory]
    [InlineData(0, 100, true)]       // 0 is valid (minimum)
    [InlineData(50, 100, true)]      // 50 is valid (middle)
    [InlineData(100, 100, true)]     // 100 is valid (maximum)
    [InlineData(-1, 100, false)]     // Negative score is invalid
    [InlineData(101, 100, false)]    // Score exceeds max → invalid
    [InlineData(0, 0, false)]        // MaxScore = 0 is invalid
    [InlineData(-50, -100, false)]   // Both negative is invalid
    public void IsValidScore_WithEdgeCases_ReturnsCorrectValidity(decimal score, decimal maxScore, bool expected)
    {
        var isValid = _gradeService.IsValidScore(score, maxScore);
        isValid.Should().Be(expected);
    }

    /// <summary>
    /// Tests grade calculation consistency across multiple calls.
    /// </summary>
    [Fact]
    public void CalculateGrade_ConsistentAcrossMultipleCalls()
    {
        var score = 85m;
        var maxScore = 100m;

        var grade1 = _gradeService.CalculateGrade(score, maxScore);
        var grade2 = _gradeService.CalculateGrade(score, maxScore);
        var grade3 = _gradeService.CalculateGrade(score, maxScore);

        grade1.Should().Be(grade2).And.Be(grade3);
    }

    /// <summary>
    /// Tests that invalid scores return F grade.
    /// </summary>
    [Theory]
    [InlineData(-10, 100)]  // Negative score
    [InlineData(150, 100)]  // Score > max
    [InlineData(100, 0)]    // MaxScore = 0
    [InlineData(-5, -10)]   // Both negative
    public void CalculateGrade_WithInvalidScores_ReturnsFGrade(decimal score, decimal maxScore)
    {
        var grade = _gradeService.CalculateGrade(score, maxScore);
        grade.Should().Be("F");
    }

    /// <summary>
    /// Tests percentage capping at 100%.
    /// </summary>
    [Theory]
    [InlineData(100, 100)]   // Exactly 100%
    [InlineData(150, 100)]   // 150% → capped at 100%
    [InlineData(200, 100)]   // 200% → capped at 100%
    [InlineData(1000, 1)]    // 100000% → capped at 100%
    public void GetPercentage_NeverExceedsCap_MaximumIs100(decimal score, decimal maxScore)
    {
        var percentage = _gradeService.GetPercentage(score, maxScore);
        percentage.Should().BeLessThanOrEqualTo(100);
    }

    /// <summary>
    /// Tests grade scale completeness.
    /// Ensures all grade levels are achievable.
    /// </summary>
    [Fact]
    public void CalculateGrade_AllGradeLevelsAchievable()
    {
        // Test all expected grades
        var aGrade = _gradeService.CalculateGrade(90, 100);
        var bGrade = _gradeService.CalculateGrade(80, 100);
        var cGrade = _gradeService.CalculateGrade(60, 100);
        var fGrade = _gradeService.CalculateGrade(0, 100);

        aGrade.Should().Be("A");
        bGrade.Should().Be("B");
        cGrade.Should().Be("C");
        fGrade.Should().Be("F");
    }

    /// <summary>
    /// Tests boundary precision at grade thresholds.
    /// </summary>
    [Fact]
    public void CalculateGrade_BoundaryPrecisionTests()
    {
        // Test just below and at each boundary
        var justBelowA = _gradeService.CalculateGrade(89.9m, 100);  // Just below 90%
        var atA = _gradeService.CalculateGrade(90.0m, 100);         // Exactly 90%

        var justBelowB = _gradeService.CalculateGrade(74.9m, 100);  // Just below 75%
        var atB = _gradeService.CalculateGrade(75.0m, 100);         // Exactly 75%

        justBelowA.Should().Be("B");
        atA.Should().Be("A");
        justBelowB.Should().Be("C");
        atB.Should().Be("B");
    }
}
