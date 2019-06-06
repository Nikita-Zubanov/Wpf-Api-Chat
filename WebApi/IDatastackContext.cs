using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi
{
    interface IDatastackContext<Essence>
    {
        List<Essence> Get();
        Essence Get(int id);
        Essence Get(string value);
        void Post(Essence obj);
        void Put(int id, Essence obj);
        void Put(string value, Essence obj);
        void Delete(int id);
        void Delete(string value);
    }
}
