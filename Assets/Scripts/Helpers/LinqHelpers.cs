using System;
using System.Collections.Generic;

namespace Helpers
{
    public static class LinqHelpers
    {
        // Adapted from https://github.com/morelinq/MoreLINQ/blob/ec4bbd3c7ca61e3a98695aaa2afb23da001ee420/MoreLinq/MinBy.cs
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, null);
        }

        private static TSource MinBy<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            IComparer<TKey>? comparer
        )
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            comparer ??= Comparer<TKey>.Default;

            using var sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext()) throw new InvalidOperationException("Sequence contains no elements");
            var min = sourceIterator.Current;
            var minKey = selector(min);
            while (sourceIterator.MoveNext())
            {
                var candidate = sourceIterator.Current;
                var candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, minKey) < 0)
                {
                    min = candidate;
                    minKey = candidateProjected;
                }
            }

            return min;
        }

        public static TSource MinByOrElse<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            TSource elseValue
        )
        {
            return source.MinByOrElse(selector, elseValue, null);
        }

        private static TSource MinByOrElse<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            TSource elseValue,
            IComparer<TKey>? comparer
        )
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            comparer ??= Comparer<TKey>.Default;

            using var sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext()) return elseValue;
            var min = sourceIterator.Current;
            var minKey = selector(min);
            while (sourceIterator.MoveNext())
            {
                var candidate = sourceIterator.Current;
                var candidateProjected = selector(candidate);
                if (comparer.Compare(candidateProjected, minKey) < 0)
                {
                    min = candidate;
                    minKey = candidateProjected;
                }
            }

            return min;
        }

        public static TKey MaxOrElse<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            TKey elseValue
        )
        {
            return source.MaxOrElse(selector, elseValue, null);
        }

        private static TKey MaxOrElse<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> selector,
            TKey elseValue,
            IComparer<TKey>? comparer
        )
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (selector == null) throw new ArgumentNullException(nameof(selector));
            comparer ??= Comparer<TKey>.Default;

            using var sourceIterator = source.GetEnumerator();
            if (!sourceIterator.MoveNext()) return elseValue;
            var maxKey = selector(sourceIterator.Current);
            while (sourceIterator.MoveNext())
            {
                var candidateProjected = selector(sourceIterator.Current);
                if (comparer.Compare(candidateProjected, maxKey) > 0) maxKey = candidateProjected;
            }

            return maxKey;
        }
    }
}