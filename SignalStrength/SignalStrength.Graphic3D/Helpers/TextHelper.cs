using System.Text;

namespace SignalStrength.Graphic3D.Helpers
{
    public static class TextHelper
    {
     
        /// <summary>
        /// Trim text if length larger 10 chars
        /// </summary>
        /// <param name="longName">Name to shorten</param>
        /// <returns></returns>
        public static string  TrimText(this string longName)
        {
            longName = longName.Trim();
            var firstText = longName.Substring(0, 5);
            var secText = longName.Substring(longName.Length - 5,5);
            var trimedText = $"{firstText.Trim()}...{secText.Trim()}";
            return trimedText;
        }


        /// <summary>
        /// Calculate seed from name chars ASCII value 
        /// </summary>
        /// <param name="name">Name for calculate</param>
        /// <returns></returns>
        public static int CalculateSeedFromString(this string name)
        {
            int count = 0;
            for (int i = 0; i < name.Length; i++) {
                if(name[i] < 127)
                {
                    count += name[i];
                }
            }
            return count;
        }
    }


}