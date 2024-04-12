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
    public class CommandeTests
    {
        private const string id = "0008";
        private static readonly DateTime dateCommande = DateTime.Now;
        private const float montant = 52.2F;
        private static readonly Commande commande = new Commande(id, dateCommande, montant);

        [TestMethod()]
        public void CommandeTest()
        {
            Assert.AreEqual(id, commande.Id, "devrait reussir");
            Assert.AreEqual(dateCommande, commande.DateCommande, "devrait reussir");
            Assert.AreEqual(montant, commande.Montant, "devrait reussir");
        }
    }
}