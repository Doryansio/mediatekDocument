using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        

        public Utilisateur (string id, string nom, string mail, string idService, string service)
        {
            this.Id = id;
            this.Nom = nom;
            this.Mail = mail;
            this.IdService = idService;
            this.Service = service;
        }

        public string Id { get; }
        public string Nom { get; set; }
        public string Mail { get; set; }

        public string IdService { get; set; }
        public string Service { get; set; }

    }
}
