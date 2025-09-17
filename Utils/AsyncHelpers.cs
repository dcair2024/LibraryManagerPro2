public static class AsyncHelpers
{
    public static Task<List<T>> ToListAsyncSafe<T>(this IEnumerable<T> source)
    {
        return Task.FromResult(source.ToList());
    }
}
