using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TechTalk.SpecFlow;
using MediaTekDocuments.view;
using MediaTekDocuments.model;
using FluentAssertions;
using System.Threading;

namespace MediatekDocumentsSpeckFlow.Steps
{
    [Binding]
    public class TestsLivresSteps

    {
        private static readonly FrmMediatek frmMediatek = new FrmMediatek(new Utilisateur("01", "pat", "pat", "pat", "0001", "accueil"));
        private static readonly DataGridView DgvLivres = (DataGridView)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["dgvLivresListe"];

        [Given(@"je saisie la valleur dans le champs de recherche de l'id : ""(.*)""")]
        public void RemplirChampId(string id)
        {
            tabControl tabonglet = (TabControl)frmMediatek.Controls["tabControl"];
            frmMediatek.Visible = true;
            tabonglet.SelectedTab = (TabPage)tabonglet.Controls["tabLivres"];
            TextBox txbLivresNumRecherche = (TextBox)frmMediatek.Controls["tabControl"].Controls["tabLivres"].Controls["grpLivresRecherche"].Controls["txbLivresNumRecherche"];

            txbLivresNumRecherche.Text = id;
        }

        [Given(@"je saisie la valleur dans le champs de recherche des titres: ""(.*)""")]
        public void GivenJeSaisieLaValleurDansLeChampsDeRechercheDesTitres(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Given(@"je selectionne le rayon : ""(.*)""")]
        public void GivenJeSelectionneLeRayon(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"je clique sur le bouton de recherche")]
        public void WhenJeCliqueSurLeBoutonDeRecherche()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"la vue affiche le livre possédant l'id : ""(.*)""")]
        public void ThenLaVueAfficheLeLivrePossedantLId(int p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"la vue affiche le livre possédant le titre  : ""(.*)""")]
        public void ThenLaVueAfficheLeLivrePossedantLeTitre(string p0)
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"la vue affiche les livres du rayon : ""(.*)""")]
        public void ThenLaVueAfficheLesLivresDuRayon(string p0)
        {
            ScenarioContext.Current.Pending();
        }
    }
}
