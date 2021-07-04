using System.IO;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace SignalStrength.Wpf.Extensions
{
    public static class Chart2DExtension 
    {

        /// <summary>
        /// Count all point in all series
        /// </summary>
        /// <param name="series"></param>
        /// <returns></returns>
        public static int CountAll(this LiveCharts.SeriesCollection series)
        {
            int count = 0;
            for (int i = 0; i < series.Count; i++) {

                count += series[i].Values.Count;
            }
            return count;
        }
    }
}