using Microsoft.VisualStudio.TestTools.UnitTesting;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.model.Tests
{
    [TestClass()]
    public class UtilisateurTests
    {
        private const string id = "11111";
        private const string nom = "michel ";
        private const string prenom = "Michelle";
        private const string mail = "michmich@michou.com";
        private const string idService = "01010";
        private const string service = "test";
        private static readonly Utilisateur utilisateur = new Utilisateur(id, nom, prenom, mail, idService, service);
        [TestMethod()]
        public void UtilisateurTest()
        {
            Assert.AreEqual(id, utilisateur.Id, "devrait réussir");
            Assert.AreEqual(nom, utilisateur.Nom, "devrait réussir");
            Assert.AreEqual(prenom, utilisateur.Prenom, "devrait réussir");
            Assert.AreEqual(mail, utilisateur.Mail, "devrait réussir");
            Assert.AreEqual(idService, utilisateur.IdService, "devrait reussir");
            Assert.AreEqual(service, utilisateur.Service, "devrait réussir");
        }
    }
}