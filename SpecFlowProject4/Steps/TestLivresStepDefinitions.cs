using System.Windows.Forms;
using TechTalk.SpecFlow;
using MediaTekDocuments.view;
using MediaTekDocuments.model;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpecFlowProject.Steps
{
    [Binding]

    public sealed class TestLivresStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private static readonly FrmMediatek form = new FrmMediatek(new Utilisateur("01", "test", "test", "test@mail.com", "0001", "acceuil"));
        private static readonly DataGridView DgvLivres = (DataGridView)form.Controls["tabControlFrmMediatek"].Controls["tabLivres"].Controls["GrpLivresRecherche"].Controls["DgvLivres"];
        public static FrmMediatek Frm => form;

        public TestLivresStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }


        [Given(@"Je saisie la valeur ""([^""]*)"" dans le champ de recherche de l'ID")]
        public void GivenFirstNumberId(string number)
        {
            TextBox TxbLivresNumRecherche = (TextBox)form.Controls["tabControlFrmMediatek"].Controls["tabLivres"].Controls["GrpLivresRecherche"].Controls["TxbLivresNumRecherche"];

            TxbLivresNumRecherche.Text = number;
        }

        [When(@"je clique sur le bouton de recherche")]
        public void GivenTheSecondNumberIs()
        {
            Button BtnLivresNumRecherche = (Button)form.Controls["tabControlFrmMediatek"].Controls["tabLivres"].Controls["GrpLivresRecherche"].Controls["BtnLivresNumRecherche"];
        }

        [Then(@"le datagridview affiche le livre possédant l'id ""([^""]*)""")]
        public void ThenTheResultShouldBe(string number)
        {
            bool result = true;

            foreach (DataGridViewRow row in DgvLivres.Rows)
            {
                Livre livre = (Livre)row.DataBoundItem;

                if (!livre.Titre.Contains(number))
                {
                    result = false;
                    break;
                }

                Assert.AreEqual(number, number);
                result.Should().BeTrue();
            }
        }
    }
}
