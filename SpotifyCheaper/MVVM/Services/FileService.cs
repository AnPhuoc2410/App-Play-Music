using SpotifyCheaper.MVVM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpotifyCheaper.MVVM.Services
{
    public class FileService
    {
        private FileRepository jsonRepository;
        public bool InputJson(string file, string value)
        {
            jsonRepository = new FileRepository();
            return jsonRepository.InputJsonFile(file, value);
        }

        public string OutJsonValue (string file, string value)
        {
            jsonRepository = new FileRepository();
            return jsonRepository.GetJsonFile(file, value);
        }

        //public string 

        public bool ChangeValue (string file,string key, string value)
        {
            jsonRepository = new FileRepository();
            return jsonRepository.ChangeJsonKeyValue(file, key, value);
        }
        
        //public bool AddValue (string file,string key,string value)
        //{

        //}
    }
}