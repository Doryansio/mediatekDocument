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
    public class ServiceTests
    {
        private const string id = "009";
        private const string libelle = "Fenouil";
        private static readonly Service service = new Service(id, libelle);
        [TestMethod()]
        public void ServiceTest()
        {
            Assert.AreEqual(id, service.Id, "devrait réussir");
            Assert.AreEqual(libelle, service.Libelle, "devrait réussir");
        }
        [TestMethod()]
        public void ToStringTest()
        {
            Assert.AreEqual(libelle, service.ToString(), "devrait réussir ");
        }
    }
}