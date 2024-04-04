using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class Service
    {
        public string Id { get; }
        public string Libelle { get; }


        public Service(string id, string libelle)
        {
            this.Id = id;
            this.Libelle = libelle;
        }

        public override string ToString()
        {
            return this.Libelle;
        }
    }
}
