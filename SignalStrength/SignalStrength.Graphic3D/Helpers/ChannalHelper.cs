namespace SignalStrength.Graphic3D.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class ChannalHelper
    {
        /// <summary>
        /// Find repeating channals and how much they repeat
        /// </summary>
        /// <param name="channels">Networks channals</param>
        /// <returns>Channal number,how much they repeating</returns>
        public static IDictionary<int,int> RepeatingChannals(this int[] channels)
        {
            IDictionary<int,int> repeating = new Dictionary<int, int>();

           var result = channels.GroupBy(i => i).Select(i => new { key = i.Key, repeatingTime = i.Count() });
            foreach (var item in result)
            {
                repeating.Add(item.key, item.repeatingTime);
            }
            return repeating;
        }


        /// <summary>
        /// Find repeating channals and how much they repeat
        /// </summary>
        /// <param name="channels">Networks channals</param>
        /// <returns>Channal number,how much they repeating and repeating names</returns>
        public static IList<Tuple<int, int, string[]>> RepeatingChannals(this ScanValues[] channels)
        {
            IList<Tuple<int, int,string[]>> repeating = new List<Tuple<int, int, string[]>>();

            var result = channels.GroupBy(i => i.Channal).Select(i => new { key = i.Key, repeatingTime = i.Count(), name=i.GroupBy(x => x.Name).Select(n=> n.Key).ToArray()});
            foreach (var item in result)
            {
                repeating.Add(new Tuple<int, int, string[]>(item.key, item.repeatingTime, item.name));
            }
            return repeating;
        }
    }

}