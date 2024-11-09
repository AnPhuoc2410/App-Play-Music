using SpotifyCheaper.MVVM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SpotifyCheaper.MVVM.Services
{
    public class JSonService
    {
        private JsonRepository jsonRepository;
        public bool InputJson(string file, string value)
        {
            jsonRepository = new JsonRepository();
            return jsonRepository.InputJsonFile(file, value);
        }

        public string OutJsonValue (string file, string value)
        {
            jsonRepository = new JsonRepository();
            return jsonRepository.GetJsonFile(file, value);
        }

        public bool InputListSong (string file, string listSong)
        {
            jsonRepository = new();
            return jsonRepository.InputJsonFile(file, listSong);
        }

        public bool ChangeValue (string file,string key, string value)
        {
            jsonRepository = new JsonRepository();
            return jsonRepository.ChangeJsonKeyValue(file, key, value);
        }
        
        //public bool AddValue (string file,string key,string value)
        //{

        //}
    }
}