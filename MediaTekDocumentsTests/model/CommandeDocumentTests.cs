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
    public class CommandeDocumentTests
    {
        private const string id = "0008";
        private static readonly DateTime dateCommande = DateTime.Now;
        private const float montant = 25.8F;
        private const int nbExemplaire = 8;
        private const string idLivreDvd = "0007";
        private const int idSuivi = 4;
        private const string etat = "Neuf";
        private static readonly CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);

        [TestMethod()]
        public void CommandeDocumentTest()
        {
            Assert.AreEqual(id, commandeDocument.Id, "devrait réussir");
            Assert.AreEqual(dateCommande, commandeDocument.DateCommande, "devrait réussir ");
            Assert.AreEqual(montant, commandeDocument.Montant, "devrait réussir");
            Assert.AreEqual(nbExemplaire, commandeDocument.NbExemplaire, "devrait réussir");
            Assert.AreEqual(idLivreDvd, commandeDocument.IdLivreDvd, "devrait réussir");
            Assert.AreEqual(idSuivi, commandeDocument.IdSuivi, "devrait réussir");
            Assert.AreEqual(etat, commandeDocument.Etat, "devrait réussir");
        }
    }
}