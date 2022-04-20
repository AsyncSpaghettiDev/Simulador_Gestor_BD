using System;
using System.Linq;
using System.Text;

namespace SimuladorBD {
    class Stuff {
        public static string Normalize(string accentedStr, bool toUpper = true) {
            byte[] tempBytes;
            tempBytes = Encoding.GetEncoding("ISO-8859-8").GetBytes(accentedStr);
            return toUpper ? Encoding.UTF8.GetString(tempBytes).ToUpper() : Encoding.UTF8.GetString(tempBytes);
        }
        public static string[] FormatedData(string originalQuery, int skip, bool toUpper = true) {
            // Uncompress the data into an array separating originalQuery by space
            string[] uncompressedData = originalQuery.Split('\u0020');
            // Skips the first n values, where is the instrucction
            uncompressedData = uncompressedData.Skip(skip).ToArray();
            // Compress all the data without spaces into an string
            string compressedData = string.Empty;
            foreach (string value in uncompressedData)
                compressedData += Normalize(value.Trim(), toUpper);
            // Uncompress the data and creates an array
            return compressedData.Split(',');
        }
    }
}
