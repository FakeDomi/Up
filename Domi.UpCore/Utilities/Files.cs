using System.IO;

namespace Domi.UpCore.Utilities
{
    public static class Files
    {
        public static string FindTempFilePath(string directory, string file, int randomLength)
        {
            string tempFilePath = "";

            do
            {
                tempFilePath = Path.Combine(directory, string.Format(file, Util.GetRandomString(randomLength)));
            } while (File.Exists(tempFilePath));

            return tempFilePath;
        }
    }
}
