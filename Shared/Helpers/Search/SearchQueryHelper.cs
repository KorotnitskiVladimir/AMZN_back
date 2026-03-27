namespace AMZN.Shared.Helpers.Search
{
    public static class SearchQueryHelper
    {
        public static string NormalizeQuery(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return string.Empty;

            var parts = query
                .Trim()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return string.Join(' ', parts);
        }

        public static List<string> SplitTokens(string? query)
        {
            var normalizedQuery = NormalizeQuery(query);

            if (string.IsNullOrEmpty(normalizedQuery))
                return new List<string>();

            return normalizedQuery
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }
    }
}
