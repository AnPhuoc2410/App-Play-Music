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

        public string InputListSong (string file, string listSong)
        {
            jsonRepository = new();
            return (jsonRepository.InputSongFile(file, listSong))? "Success": "Fail";
        }

    }
}