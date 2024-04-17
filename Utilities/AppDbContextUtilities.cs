using Microsoft.EntityFrameworkCore;

namespace yawaflua.ru.Utilities
{
    public static class AppDbContextUtilities
    {
        public static bool TryGetValue<T>(this DbSet<T> set, Func<T, bool> predicate, out T? value) where T : class
        {
            value = set.FirstOrDefault(predicate);
            return value != null;
        }
    }
}
