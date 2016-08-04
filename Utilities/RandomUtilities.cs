using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities
{
    public class RandomUtilities
    {
        private static Random _Random = new Random();
        /// <summary>
        /// Returns an object of type T randomly selected from the specified list 
        /// based on its weight (statistical frequency of occurrence).
        /// </summary>
        /// <param name="list">A collection whose values are the objects to be 
        /// randomly returned and whose keys are their distribution weights.</param>
        public static T WeightedRandom<T>(SortedList<int, T> list)
        {
            int max = list.Keys[list.Keys.Count - 1];
            int random = _Random.Next(max);
            foreach (int key in list.Keys)
            {
                if (random <= key)
                    return list[key];
            }
            return default(T);
        }
    }
}
