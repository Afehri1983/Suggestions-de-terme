# Suggestions de terme

Un système de suggestions de termes qui trouve les mots les plus similaires dans une liste de candidats en fonction d'un terme de recherche.

## Description

Ce projet implémente un algorithme de suggestion de termes qui :
- Prend un terme de recherche et une liste de candidats
- Retourne les N termes les plus similaires basés sur le nombre de lettres à remplacer
- Utilise une normalisation (minuscules, alphanumériques uniquement)
- Trie les résultats par similarité, puis par longueur, puis par ordre alphabétique

## Fonctionnalités

- **Normalisation automatique** : Convertit les termes en minuscules et garde uniquement les caractères alphanumériques
- **Algorithme de similarité** : Utilise une fenêtre glissante pour trouver la meilleure correspondance dans chaque candidat
- **Tri intelligent** : 
  1. Par nombre de différences (moins = mieux)
  2. Par différence de longueur avec le terme recherché (plus proche = mieux)
  3. Par ordre alphabétique (en cas d'égalité)

## Comment ça marche

L'algorithme compare le terme recherché avec toutes les sous-chaînes de même longueur dans chaque candidat. Il calcule le nombre de lettres à remplacer (distance de Hamming) et retourne les meilleures correspondances.

### Exemple

Pour le terme `gros` dans la liste `[gros, gras, graisse, agressif, go, ros, gro]` :

- `gros` = 0 différence (correspondance exacte)
- `gras` = 1 différence (r → s)
- `graisse` = 2 différences minimum (fenêtre glissante)
- `agressif` = 1 différence minimum (fenêtre glissante)
- `go`, `ros`, `gro` = exclus (trop courts)

Pour N=2 suggestions, le résultat sera : **gros, gras** (gras est choisi plutôt qu'agressif car plus court).

## Utilisation

### Interface interactive

Le programme propose une interface en ligne de commande interactive :

```bash
dotnet run
```

Vous serez invité à :
1. Entrer le terme à rechercher
2. Entrer la liste de candidats (séparés par des virgules ou des espaces)
3. Spécifier le nombre de suggestions souhaitées

Exemple de session :
```
=== Système de Suggestions de Terme ===

Entrez le terme à rechercher (ou 'quit' pour quitter): gros
Entrez la liste de termes (séparés par des virgules ou espaces): gros, gras, graisse, agressif, go, ros, gro
Nombre de suggestions à retourner (défaut: 3): 2

--- Résultats ---
Terme recherché: 'gros'
Liste de candidats: [gros, gras, graisse, agressif, go, ros, gro]
Nombre de suggestions demandées: 2
Suggestions trouvées: gros, gras
```

### Utilisation programmatique

```csharp
using Suggestions_de_terme;

var provider = new SuggestionProvider();
var candidates = new List<string> { "gros", "gras", "graisse", "agressif" };
var suggestions = provider.Suggest("gros", candidates, 2);

foreach (var suggestion in suggestions)
{
    Console.WriteLine(suggestion);
}
```

## Architecture

### Interface `ISuggestionProvider`

```csharp
public interface ISuggestionProvider
{
    IEnumerable<string> Suggest(string searchTerm, IEnumerable<string> candidates, int count);
}
```

### Implémentation `SuggestionProvider`

La classe `SuggestionProvider` implémente l'algorithme :
- **Normalize()** : Normalise les termes (minuscules, alphanumériques uniquement)
- **GetDifferenceScore()** : Calcule le nombre de différences entre deux chaînes de même longueur
- **Suggest()** : Algorithme principal qui trouve les meilleures suggestions

## Règles de l'algorithme

1. Les candidats plus courts que le terme recherché sont exclus
2. La similarité est calculée uniquement par remplacement de lettres (pas d'insertion)
3. Une fenêtre glissante trouve la meilleure correspondance dans chaque candidat
4. En cas d'égalité du score de différence :
   - Priorité aux termes de longueur similaire au terme recherché
   - Puis tri alphabétique

## Technologies

- .NET 8.0
- C#

## Auteur

Projet créé pour implémenter un système de suggestions de termes basé sur la similarité par remplacement de lettres.

