using System;
using System.Collections.Generic;
using System.Linq;

namespace Suggestions_de_terme
{
	public class SuggestionProvider : ISuggestionProvider
	{
		public IEnumerable<string> Suggest(string searchTerm, IEnumerable<string> candidates, int count)
		{
			if (candidates == null) return Array.Empty<string>();
			var normalizedSearch = Normalize(searchTerm);
			if (string.IsNullOrEmpty(normalizedSearch)) return Array.Empty<string>();

			int searchLength = normalizedSearch.Length;

			var scored = new List<(string term, int score, int lengthDelta)>();

			foreach (var rawCandidate in candidates)
			{
				if (string.IsNullOrWhiteSpace(rawCandidate)) continue;
				var candidate = Normalize(rawCandidate);
				if (candidate.Length < searchLength) continue;

				int minScore = int.MaxValue;
				for (int i = 0; i <= candidate.Length - searchLength; i++)
				{
					int score = GetDifferenceScore(candidate.AsSpan(i, searchLength), normalizedSearch.AsSpan());
					if (score < minScore) minScore = score;
					if (minScore == 0) break;
				}

				scored.Add((rawCandidate, minScore, Math.Abs(candidate.Length - searchLength)));
			}

			return scored
				.OrderBy(s => s.score)
				.ThenBy(s => s.lengthDelta)
				.ThenBy(s => s.term, StringComparer.Ordinal)
				.Take(count)
				.Select(s => s.term)
				.ToArray();
		}

		private static string Normalize(string? text)
		{
			if (string.IsNullOrEmpty(text)) return string.Empty;
			Span<char> buffer = text.ToLowerInvariant().ToCharArray();
			// Keep only alphanumeric characters
			var filtered = new List<char>(buffer.Length);
			for (int i = 0; i < buffer.Length; i++)
			{
				char c = buffer[i];
				if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9'))
				{
					filtered.Add(c);
				}
			}
			return new string(filtered.ToArray());
		}

		// Hamming-like distance for equal-length spans
		private static int GetDifferenceScore(ReadOnlySpan<char> dest, ReadOnlySpan<char> src)
		{
			int differences = 0;
			for (int i = 0; i < dest.Length; i++)
			{
				if (dest[i] != src[i]) differences++;
			}
			return differences;
		}
	}
}


