using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace TEngine
{
    public static partial class Utility
    {
        public static class Random
        {
            /// <summary>
            /// 从一个集合中随机抽取一定数量的元素。
            /// </summary>
            /// <param name="data">集合。</param>
            /// <param name="count">抽取元素数量。</param>
            /// <typeparam name="T">元素类型。</typeparam>
            /// <returns>随机抽取的元素集合。</returns>
            public static List<T> GetRandomElements<T>(List<T> data, int count)
            {
                if (data == null || data.Count == 0)
                    return new List<T>();

                var copy = new List<T>(data);
                for (int i = copy.Count - 1; i > 0; i--)
                {
                    int idx = RandomNumberGenerator.GetInt32(0, i + 1);
                    (copy[i], copy[idx]) = (copy[idx], copy[i]);
                }

                count = Math.Min(count, copy.Count);
                return copy.GetRange(0, count);
            }
        }
    }
}