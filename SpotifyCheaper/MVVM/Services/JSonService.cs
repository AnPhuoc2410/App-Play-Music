using SpotifyCheaper.MVVM.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public string OutJson(string file, string value)
        {
            jsonRepository = new JsonRepository();
            return jsonRepository.GetJsonFile(file, value);
        }
    }
}