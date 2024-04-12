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
    public class CategorieTests
    {
        private const string id = "0001";
        private const string libelle = "libelle";
        private static readonly Categorie categorie = new Categorie(id, libelle);

        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual(libelle, categorie.ToString(), "devrait reussir");
        }

        [TestMethod()]
        public void CategorieTest()
        {
            Assert.AreEqual(id, categorie.Id, " devrait reussir");
            Assert.AreEqual(libelle, categorie.Libelle, "devrait reussir");
        }
    }
}