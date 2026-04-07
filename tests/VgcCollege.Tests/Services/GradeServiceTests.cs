using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using oop_s2_1_mvc_83356.Services;
using Xunit;

namespace VgcCollege.Tests.Services;

public class GradeServiceTests
{
    private readonly GradeService _service;
    private readonly Mock<ILogger<GradeService>> _loggerMock;

    public GradeServiceTests()
    {
        _loggerMock = new Mock<ILogger<GradeService>>();
        _service = new GradeService(_loggerMock.Object);
    }

    [Theory]
    [InlineData(95, 100, "A")]
    [InlineData(90, 100, "A")]
    [InlineData(89, 100, "B")]
    [InlineData(75, 100, "B")]
    [InlineData(74, 100, "C")]
    [InlineData(60, 100, "C")]
    [InlineData(59, 100, "F")]
    [InlineData(0, 100, "F")]
    public void CalculateGrade_WithValidScores_ReturnsCorrectGrade(decimal score, decimal maxScore, string expectedGrade)
    {
        // Act
        var grade = _service.CalculateGrade(score, maxScore);

        // Assert
        grade.Should().Be(expectedGrade);
    }

    [Theory]
    [InlineData(45, 50, "A")]    // 90% → A (was "F" which is incorrect)
    [InlineData(37.5, 50, "B")]  // 75% → B
    [InlineData(30, 50, "C")]    // 60% → C
    [InlineData(15, 50, "F")]    // 30% → F
    public void CalculateGrade_WithDifferentMaxScores_ReturnsCorrectGrade(decimal score, decimal maxScore, string expectedGrade)
    {
        // Act
        var grade = _service.CalculateGrade(score, maxScore);

        // Assert
        grade.Should().Be(expectedGrade);
    }

    [Theory]
    [InlineData(100, 100, 100)]
    [InlineData(90, 100, 90)]
    [InlineData(75, 100, 75)]
    [InlineData(50, 100, 50)]
    [InlineData(0, 100, 0)]
    public void GetPercentage_WithValidScores_ReturnsCorrectPercentage(decimal score, decimal maxScore, decimal expectedPercentage)
    {
        // Act
        var percentage = _service.GetPercentage(score, maxScore);

        // Assert
        percentage.Should().Be(expectedPercentage);
    }

    [Theory]
    [InlineData(95, 100, true)]
    [InlineData(100, 100, true)]
    [InlineData(0, 100, true)]
    [InlineData(101, 100, false)] // Score exceeds max
    [InlineData(-1, 100, false)]  // Negative score
    public void IsValidScore_WithVariousScores_ReturnsCorrectValidity(decimal score, decimal maxScore, bool expected)
    {
        // Act
        var isValid = _service.IsValidScore(score, maxScore);

        // Assert
        isValid.Should().Be(expected);
    }

    [Fact]
    public void CalculateGrade_WithInvalidScore_ReturnsFail()
    {
        // Act
        var grade = _service.CalculateGrade(150, 100); // Score > max

        // Assert
        grade.Should().Be("F");
    }

    [Fact]
    public void GetPercentage_WithZeroMaxScore_ReturnsZero()
    {
        // Act
        var percentage = _service.GetPercentage(50, 0);

        // Assert
        percentage.Should().Be(0);
    }

    [Fact]
    public void GetPercentage_CapsAtHundredPercent()
    {
        // Act - Even if somehow score > max, should cap at 100
        var percentage = _service.GetPercentage(150, 100);

        // Assert
        percentage.Should().Be(100);
    }
}
