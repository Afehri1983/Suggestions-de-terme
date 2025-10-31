using System;
using System.Collections.Generic;
using System.Linq;
using Suggestions_de_terme;

var provider = new SuggestionProvider();

Console.WriteLine("=== Système de Suggestions de Terme ===");
Console.WriteLine();

while (true)
{
	// Get search term
	Console.Write("Entrez le terme à rechercher (ou 'quit' pour quitter): ");
	string? searchTerm = Console.ReadLine()?.Trim();
	
	if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Equals("quit", StringComparison.OrdinalIgnoreCase))
	{
		Console.WriteLine("Au revoir!");
		break;
	}

	// Get candidate list
	Console.Write("Entrez la liste de termes (séparés par des virgules ou espaces): ");
	string? candidatesInput = Console.ReadLine();
	
	if (string.IsNullOrWhiteSpace(candidatesInput))
	{
		Console.WriteLine("Liste vide. Veuillez réessayer.\n");
		continue;
	}

	// Parse candidates (support both comma and space separated)
	var candidates = candidatesInput
		.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
		.Select(s => s.Trim())
		.Where(s => !string.IsNullOrWhiteSpace(s))
		.ToList();

	if (candidates.Count == 0)
	{
		Console.WriteLine("Aucun terme valide trouvé. Veuillez réessayer.\n");
		continue;
	}

	// Get number of suggestions
	Console.Write("Nombre de suggestions à retourner (défaut: 3): ");
	string? countInput = Console.ReadLine()?.Trim();
	
	int count = 3;
	if (!string.IsNullOrWhiteSpace(countInput))
	{
		if (!int.TryParse(countInput, out count) || count <= 0)
		{
			Console.WriteLine("Nombre invalide, utilisation de la valeur par défaut (3).");
			count = 3;
		}
	}

	// Get suggestions
	var suggestions = provider.Suggest(searchTerm, candidates, count).ToList();

	// Display results
	Console.WriteLine();
	Console.WriteLine("--- Résultats ---");
	Console.WriteLine($"Terme recherché: '{searchTerm}'");
	Console.WriteLine($"Liste de candidats: [{string.Join(", ", candidates)}]");
	Console.WriteLine($"Nombre de suggestions demandées: {count}");
	Console.WriteLine($"Suggestions trouvées: {(suggestions.Count > 0 ? string.Join(", ", suggestions) : "Aucune suggestion")}");
	Console.WriteLine();
}
