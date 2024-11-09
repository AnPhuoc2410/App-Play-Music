using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
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


        public string GetJsonFile(string file, string value)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(file)
                .Build();
            return configuration[value];
        }

        public bool DeleteJsonValue (string file, string value)
        {
            try
            {
                // Get Path
                string currentPath = Directory.GetCurrentDirectory();
                string finalPath = currentPath + "\\" + file;

                //Read file to Json
                string jsonAllFile = File.ReadAllText(finalPath);
                JObject jsonAllObject = JObject.Parse(jsonAllFile);

                if (jsonAllObject.ContainsKey(value)) {
                    jsonAllObject.Remove(value);
                    File.WriteAllText(finalPath, jsonAllObject.ToString());
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message );
                return false;
            }
        }

        public bool ChangeJsonKeyValue (string file, string key, string value)
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
                    File.WriteAllText (finalPath, jsonAllObject.ToString());
                    return true;
                }
                else return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool AddJsonValue (string file, string key, string value)
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
    }
}
