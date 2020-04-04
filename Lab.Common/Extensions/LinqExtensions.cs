﻿using System;
using System.Collections.Generic;

namespace Lab.Common.Extensions
{
    public static class LinqExtensions
    {
        public static void Foreach<TSource>(this IEnumerable<TSource> sources, Action<TSource> action)
        {
            foreach (var source in sources)
            {
                action.Invoke(source);
            }
        }
    }
}
