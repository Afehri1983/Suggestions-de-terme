using System.Collections.Generic;

namespace Suggestions_de_terme
{
	public interface ISuggestionProvider
	{
		IEnumerable<string> Suggest(string searchTerm, IEnumerable<string> candidates, int count);
	}
}


