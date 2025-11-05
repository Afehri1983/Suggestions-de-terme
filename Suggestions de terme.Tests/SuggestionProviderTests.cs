using Suggestions_de_terme;

namespace Suggestions_de_terme.Tests;

public class SuggestionProviderTests
{
    private readonly SuggestionProvider _provider;

    public SuggestionProviderTests()
    {
        _provider = new SuggestionProvider();
    }

    [Fact]
    public void Suggest_ExactMatch_ReturnsExactMatch()
    {
        // Arrange
        var candidates = new[] { "gros", "gras", "graisse" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 1).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("gros", result[0]);
    }

    [Fact]
    public void Suggest_OneDifference_ReturnsBestMatch()
    {
        // Arrange
        var candidates = new[] { "gros", "gras", "graisse" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 2).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("gros", result[0]); // Exact match
        Assert.Equal("gras", result[1]); // 1 difference
    }

    [Fact]
    public void Suggest_WithSlidingWindow_FindsBestMatch()
    {
        // Arrange
        var candidates = new[] { "agressif", "gros", "gras" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 3).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("gros", result[0]); // Exact match
        Assert.Equal("gras", result[1]); // 1 difference, shorter
        Assert.Equal("agressif", result[2]); // 1 difference, longer
    }

    [Fact]
    public void Suggest_ShorterCandidates_Excluded()
    {
        // Arrange
        var candidates = new[] { "go", "ros", "gro", "gros", "gras" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 5).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("gros", result);
        Assert.Contains("gras", result);
        Assert.DoesNotContain("go", result);
        Assert.DoesNotContain("ros", result);
        Assert.DoesNotContain("gro", result);
    }

    [Fact]
    public void Suggest_CaseInsensitive_Works()
    {
        // Arrange
        var candidates = new[] { "GROS", "Gras", "graisse" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 3).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("GROS", result[0]);
    }

    [Fact]
    public void Suggest_WithSpecialCharacters_Normalized()
    {
        // Arrange
        var candidates = new[] { "gros!", "gras-", "graisse123" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 3).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("gros!", result[0]); // Exact match after normalization
    }

    [Fact]
    public void Suggest_EmptyCandidates_ReturnsEmpty()
    {
        // Arrange
        var candidates = Array.Empty<string>();
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 5).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Suggest_NullCandidates_ReturnsEmpty()
    {
        // Arrange
        IEnumerable<string>? candidates = null;
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates!, 5).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Suggest_EmptySearchTerm_ReturnsEmpty()
    {
        // Arrange
        var candidates = new[] { "gros", "gras" };
        var searchTerm = "";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 5).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void Suggest_RespectsCountParameter()
    {
        // Arrange
        var candidates = new[] { "gros", "gras", "graisse", "agressif" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 2).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Suggest_SortedByLengthDelta_WhenScoreEqual()
    {
        // Arrange
        var candidates = new[] { "gros", "gras", "gross", "grossir" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 4).ToList();

        // Assert
        Assert.Equal("gros", result[0]); // Exact match, 0 diff, length delta = 0
        Assert.Equal("gross", result[1]); // 0 diff (contains "gros"), length delta = 1
        Assert.Equal("grossir", result[2]); // 0 diff (contains "gros"), length delta = 3
        Assert.Equal("gras", result[3]); // 1 diff, same length
    }

    [Fact]
    public void Suggest_AlphabeticalOrder_WhenScoreAndLengthEqual()
    {
        // Arrange
        // Use candidates that have the same length (4 chars) and will have same score
        // "test" vs "abcd": 4 differences, length delta = 0
        // "test" vs "wxyz": 4 differences, length delta = 0  
        // "test" vs "xyza": 4 differences, length delta = 0
        var candidates = new[] { "xyza", "abcd", "wxyz" };
        var searchTerm = "test";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 3).ToList();

        // Assert
        // All should have same score (4 differences) and same length, sorted alphabetically
        Assert.Equal(3, result.Count);
        Assert.Equal("abcd", result[0]);
        Assert.Equal("wxyz", result[1]);
        Assert.Equal("xyza", result[2]);
    }

    [Fact]
    public void Suggest_Gros_ExampleFromReadme()
    {
        // Arrange
        // Exemple basé sur le README : recherche de "gros" dans une liste de candidats
        // gros = 0 différence (correspondance exacte)
        // gras = 1 différence (r → s)
        // graisse = 2 différences minimum (fenêtre glissante)
        // agressif = 1 différence minimum (fenêtre glissante)
        // go, ros, gro = exclus (trop courts)
        var candidates = new[] { "gros", "gras", "graisse", "agressif", "go", "ros", "gro" };
        var searchTerm = "gros";

        // Act
        var result = _provider.Suggest(searchTerm, candidates, 2).ToList();

        // Assert
        // Pour N=2 suggestions, le résultat devrait être : gros, gras
        // (gras est choisi plutôt qu'agressif car plus court)
        Assert.Equal(2, result.Count);
        Assert.Equal("gros", result[0]); // Correspondance exacte
        Assert.Equal("gras", result[1]); // 1 différence, plus court qu'agressif
    }
}
