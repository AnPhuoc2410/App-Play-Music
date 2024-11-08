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
        /// Change from string to json file
        /// </summary>
        /// <param name="file"></param> The pc path
        /// <param name="value"></param> String 
        /// <returns></returns>
        public bool InputJsonFile(string file, string value)
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();

                // Truyen du lieu vao json
                JObject jsonObject = new JObject();
                JObject.Parse(value);

                // Create file and paste the value
                File.WriteAllText(currentPath + file, jsonObject.ToString());

                // Get File
                string getFile = Directory.GetFiles(currentPath + file).FirstOrDefault();
                return !string.IsNullOrEmpty(getFile);
            }
            catch (FileNotFoundException fnf)
            {
                Console.WriteLine(fnf.Message);
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
