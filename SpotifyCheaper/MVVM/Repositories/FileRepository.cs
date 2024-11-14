using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace SpotifyCheaper.MVVM.Repositories
{
    public class FileRepository
    {
        /// <summary>
        /// Change from object string to json file
        /// </summary>
        /// <param name="file"></param> The pc path
        /// <param name="value"></param> String 
        /// <returns></returns>
        public bool InputJsonFile(string file, string value)
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();
                string fullPath = currentPath + "\\" + file;

                JObject jsonArray = JObject.Parse(value);
                // Create the file and write the JSON content
                File.WriteAllText(fullPath, jsonArray.ToString());

                return File.Exists(fullPath);

            }
            catch (Exception ex)
            {
                // Handle exceptions such as parsing errors or file write issues
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        //public void AddStringToJson(string file, string value)
        //{
        //    try
        //    {
        //        string currentPath = Directory.GetCurrentDirectory();
        //        string fullPath = currentPath + "\\" + file;

        //        string sReadFile = File.ReadAllText(fullPath);
        //        JObject jsonObject = JObject.Parse(sReadFile);
        //        jsonObject.Add("Path", value);
        //        File.WriteAllText(fullPath, jsonObject.ToString());
        //    }
        //    catch(Exception ex) { Console.WriteLine(ex.Message); }
        //}


        public string GetJsonFile(string file, string key)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(file)
                .Build();
            return configuration[key];
        }

        public bool DeleteJsonValue(string file, string value)
        {
            try
            {
                // Get Path
                string currentPath = Directory.GetCurrentDirectory();
                string finalPath = currentPath + "\\" + file;

                //Read file to Json
                string jsonAllFile = File.ReadAllText(finalPath);
                JObject jsonAllObject = JObject.Parse(jsonAllFile);

                if (jsonAllObject.ContainsKey(value))
                {
                    jsonAllObject.Remove(value);
                    File.WriteAllText(finalPath, jsonAllObject.ToString());
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool ChangeJsonKeyValue(string file, string key, string value)
        {
            try
            {
                //Get Path
                string currentPath = Directory.GetCurrentDirectory();
                string finalPath = currentPath + "\\" + file;

                //Parse text to json
                string jsonAllFile = File.ReadAllText(finalPath);
                JObject jsonAllObject = JObject.Parse(jsonAllFile);

                //Check if the key exist then change the value
                if (jsonAllObject.ContainsKey(key))
                {
                    jsonAllObject[key] = value;
                    File.WriteAllText(finalPath, jsonAllObject.ToString());
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddJsonValue(string file, string key, string value)
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();
                string finalPath = currentPath + "\\" + file;

                string jsonAllFile = File.ReadAllText(finalPath);
                JObject jsonAllObject = JObject.Parse(jsonAllFile);

                jsonAllObject[key] = value;
                File.WriteAllText(finalPath, jsonAllObject.ToString());
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public void SaveVideoPaths(List<string> videoPaths)
        {
            string filePath = "videoLocation.txt"; // You can choose a location for the file

            // Write the list of video paths to a file
            File.WriteAllLines(filePath, videoPaths);
        }
        public List<string> LoadVideoPaths()
        {
            string filePath = "videoLocation.txt"; // Same file where paths are saved

            if (File.Exists(filePath))
            {
                // Read all lines from the file and return them as a list
                return File.ReadAllLines(filePath).ToList();
            }
            else
            {
                return new List<string>(); // If no file exists, return an empty list
            }
        }

    }
}
