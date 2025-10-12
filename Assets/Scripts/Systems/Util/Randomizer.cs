using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Systems.Util
{
    public static class Randomizer
    {
        private static readonly Random Random = new();

        public static T GetRandom<T>(this ICollection<T> values)
        {
            int randomNumber = Random.Next(values.Count);
            return values.ElementAt(randomNumber);
        }
    }
}