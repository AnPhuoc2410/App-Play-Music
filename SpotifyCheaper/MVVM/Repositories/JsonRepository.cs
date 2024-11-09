using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyCheaper.MVVM.Repositories
{
    public class JsonRepository
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
                string fullPath = currentPath +"\\"+ file;

                JArray jsonArray = JArray.Parse(value);

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

        public void AddStringToJson(string file, string value)
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();
                string fullPath = currentPath + "\\" + file;

                string sReadFile = File.ReadAllText(fullPath);
                JObject jsonObject = JObject.Parse(sReadFile);
                jsonObject.Add("Path", value);
                File.WriteAllText(fullPath, jsonObject.ToString());
            }
            catch(Exception ex) { Console.WriteLine(ex.Message); }
        }

        public bool InputSongFile(string file, string value)
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();
                string fullPath = currentPath + "\\" + file;

                //jsonObject.Add("Path", jsonArray.ToString());
                File.WriteAllText(fullPath, value);

                return File.Exists(fullPath);

            }
            catch (Exception ex)
            {
                // Handle exceptions such as parsing errors or file write issues
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public string GetJsonFile(string file, string value)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(file)
                .Build();
            return configuration[value];
        }

        
    }
}
