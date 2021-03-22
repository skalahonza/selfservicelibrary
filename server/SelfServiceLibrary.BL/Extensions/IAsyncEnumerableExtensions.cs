using System;
using System.Collections.Generic;

namespace SelfServiceLibrary.BL.Extensions
{
    public static class IAsyncEnumerableExtensions
    {
        /// <summary>
        /// Split source into batches of limited size. Compatible with non repeatable iterators.
        /// </summary>
        /// <param name="source">Data to be split</param>
        /// <param name="limit">Batch size limit</param>
        /// <returns></returns>
        public static async IAsyncEnumerable<T[]> Batchify<T>(this IAsyncEnumerable<T> source, int limit)
        {
            var index = 0;
            var batch = new T[limit];

            await foreach (var item in source)
            {
                batch[index++] = item;
                if (index == batch.Length)
                {
                    yield return batch;
                    batch = new T[limit];
                    index = 0;
                }
            }

            if (index > 0)
            {
                Array.Resize(ref batch, index);
                yield return batch;
            }
        }
    }
}
