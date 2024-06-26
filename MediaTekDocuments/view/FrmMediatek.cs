﻿using System;
using System.Windows.Forms;
using MediaTekDocuments.model;
using MediaTekDocuments.controller;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();
        private readonly Utilisateur utilisateur;
        private bool ajouterBool = false;
        private bool premierLoad = false;

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        public FrmMediatek(Utilisateur lutilisateur)
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
            this.utilisateur = lutilisateur;
            VerifDroitAcceuil(lutilisateur);
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesSuivis">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboSuivi(List<Suivi> lesSuivis, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesSuivis;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Augemente un index de type string
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string plusUnIdString(string id)
        {
            int taille = id.Length;
            int idnum = int.Parse(id) + 1;
            id = idnum.ToString();
            if (id.Length > taille)
                MessageBox.Show("Taille du registre arrivé a saturation");
            while (id.Length != taille)
            {
                id = "0" + id;
            }
            return id;
        }

        /// <summary>
        /// Ouvre une MessageBox au lancement de FrmMediatek.cs
        /// si des abonnements sont proches de se terminer
        /// </summary>
        private void AfficherAlerteAbo()
        {
            if (controller.VerifCommande(utilisateur))
            {
                bool interupteur = false;
                List<Revue> revues = controller.GetAllRevues();
                string alerteRevues = "Revues dont l'abonnement se termine dans moins de 30 jours : \n";
                foreach (Revue revue in revues)
                {
                    List<Abonnement> abonnements = controller.GetAbonnements(revue.Id);
                    abonnements = abonnements.FindAll(o => (o.DateFinAbonnement <= DateTime.Now.AddMonths(1))
                            && (o.DateFinAbonnement >= DateTime.Now));
                    if (abonnements.Count > 0)
                    {

                        alerteRevues = string.Concat(alerteRevues, " -" + revue.Titre + "\n");

                        interupteur = true;
                    }

                }

                if (interupteur)
                    MessageBox.Show(alerteRevues);
            }
        }
        /// <summary>
        /// Verifie les droit d'un uitilisateur
        /// </summary>
        /// <param name="lutilisateur"></param>
        private void VerifDroitAcceuil(Utilisateur lutilisateur)
        {
            if (!controller.VerifDroitAccueil(lutilisateur))
            {
                MessageBox.Show("Droit insuffisant");
                Application.Exit();
            }
        }
        /// <summary>
        /// Arrete le programme quand on ferme la fenetre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
            #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private readonly BindingSource bdgGenresInfo = new BindingSource();
        private readonly BindingSource bdgPublicInfo = new BindingSource();
        private readonly BindingSource bdgRayonInfo = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            lesLivres = controller.GetAllLivres();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, CbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, CbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, CbxLivresRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenresInfo, CbxLivresGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublicInfo, CbxLivresPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayonInfo, CbxLivresRayonInfos);
            
            RemplirLivresListeComplete();
            if (controller.VerifDroitModif(utilisateur))
            {
                LivreEnCoursDeModif(false);
                if (premierLoad)
                {
                    AfficherAlerteAbo();
                    premierLoad = false;
                }
            }
            else
            {
                ConsultationLivre();
            }
        }

        ///<summary>
        ///Desactive les interfaces quand l'utilisateur ne peut que lire les infos
        /// </summary>
        private void ConsultationLivre()
        {
            BtnAjouterLivres.Enabled = false;
            BtnModifierLivres.Enabled = false;
            BtnSupprimerLivres.Enabled = false;
            BtnValiderChoix.Enabled = false;
            BtnAnnulerChoix.Enabled = false;
            CbxLivresGenreInfos.Enabled = false;
            CbxLivresPublicInfos.Enabled = false;
            CbxLivresRayonInfos.Enabled = false;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            DgvLivresListe.DataSource = bdgLivresListe;
            DgvLivresListe.Columns["isbn"].Visible = false;
            DgvLivresListe.Columns["idRayon"].Visible = false;
            DgvLivresListe.Columns["idGenre"].Visible = false;
            DgvLivresListe.Columns["idPublic"].Visible = false;
            DgvLivresListe.Columns["image"].Visible = false;
            DgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DgvLivresListe.Columns["id"].DisplayIndex = 0;
            DgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!TxbLivresNumRecherche.Text.Equals(""))
            {
                TxbLivresTitreRecherche.Text = "";
                CbxLivresGenres.SelectedIndex = -1;
                CbxLivresRayons.SelectedIndex = -1;
                CbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(TxbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche avec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!TxbLivresTitreRecherche.Text.Equals(""))
            {
                CbxLivresGenres.SelectedIndex = -1;
                CbxLivresRayons.SelectedIndex = -1;
                CbxLivresPublics.SelectedIndex = -1;
                TxbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(TxbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (CbxLivresGenres.SelectedIndex < 0 && CbxLivresPublics.SelectedIndex < 0 && CbxLivresRayons.SelectedIndex < 0
                    && TxbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            TxbLivresAuteur.Text = livre.Auteur;
            TxbLivresCollection.Text = livre.Collection;
            TxbLivresImage.Text = livre.Image;
            TxbLivresIsbn.Text = livre.Isbn;
            TxbLivresNumero.Text = livre.Id;
            CbxLivresGenreInfos.SelectedIndex = CbxLivresGenreInfos.FindString(livre.Genre);
            CbxLivresPublicInfos.SelectedIndex = CbxLivresPublicInfos.FindString(livre.Public);
            CbxLivresRayonInfos.SelectedIndex = CbxLivresRayonInfos.FindString(livre.Rayon);
            TxbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                PcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                PcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            TxbLivresAuteur.Text = "";
            TxbLivresCollection.Text = "";
            TxbLivresImage.Text = "";
            TxbLivresIsbn.Text = "";
            TxbLivresNumero.Text = "";
            CbxLivresGenreInfos.SelectedIndex = -1;
            CbxLivresPublicInfos.SelectedIndex = -1;
            CbxLivresRayonInfos.SelectedIndex = -1;
            TxbLivresTitre.Text = "";
            PcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxLivresGenres.SelectedIndex >= 0)
            {
                TxbLivresTitreRecherche.Text = "";
                TxbLivresNumRecherche.Text = "";
                Genre genre = (Genre)CbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                CbxLivresRayons.SelectedIndex = -1;
                CbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxLivresPublics.SelectedIndex >= 0)
            {
                TxbLivresTitreRecherche.Text = "";
                TxbLivresNumRecherche.Text = "";
                Public lePublic = (Public)CbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                CbxLivresRayons.SelectedIndex = -1;
                CbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxLivresRayons.SelectedIndex >= 0)
            {
                TxbLivresTitreRecherche.Text = "";
                TxbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)CbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                CbxLivresGenres.SelectedIndex = -1;
                CbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            CbxLivresGenres.SelectedIndex = -1;
            CbxLivresRayons.SelectedIndex = -1;
            CbxLivresPublics.SelectedIndex = -1;
            TxbLivresNumRecherche.Text = "";
            TxbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void LivreEnCoursDeModif(bool modif)
        {
            BtnAjouterLivres.Enabled = !modif;
            BtnSupprimerLivres.Enabled = !modif;
            BtnModifierLivres.Enabled = !modif;
            BtnAnnulerChoix.Enabled = modif;
            BtnValiderChoix.Enabled = modif;
            TxbLivresTitre.ReadOnly = !modif;
            TxbLivresAuteur.ReadOnly = !modif;
            CbxLivresPublicInfos.Enabled = modif;
            TxbLivresIsbn.ReadOnly = !modif;
            TxbLivresCollection.ReadOnly = !modif;
            CbxLivresGenreInfos.Enabled = modif;
            CbxLivresRayonInfos.Enabled = modif;
            TxbLivresImage.ReadOnly = !modif;
            TxbLivresNumero.ReadOnly = true;
            DgvLivresListe.Enabled = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// enclanche la procédure d'ajout de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjouterLivres_Click(object sender, EventArgs e)
        {
            LivreEnCoursDeModif(true);
            ajouterBool = true;
            string id = plusUnIdString(controller.GetNbLivreMax());
            if (id == "0")
                id = "00001";
            TxbLivresNumero.Text = id;
            TxbLivresTitre.Text = "";
            TxbLivresAuteur.Text = "";
            CbxLivresPublicInfos.SelectedIndex = -1;
            TxbLivresCollection.Text = "";
            CbxLivresGenreInfos.SelectedIndex = -1;
            CbxLivresRayonInfos.SelectedIndex = -1;
            TxbLivresImage.Text = "";
            TxbLivresIsbn.Text = "";
        
    }

        /// <summary>
        /// enclanche la procédure de modification de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifierLivres_Click(object sender, EventArgs e)
        {
            LivreEnCoursDeModif(true);
        }

        /// <summary>
        /// enclanche la procédure de suppresion de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerLivres_Click(object sender, EventArgs e)
        {
            Livre leLivre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
            if (MessageBox.Show($"Etes vous sur de vouloir supprimer {leLivre.Titre} de {leLivre.Auteur} ?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // fonction a modifier pour prendre en charge le faite que l'on ne pourra pas supprimer un livre tant que des examplaire de se livre existe
                if (controller.SupprimerLivre(leLivre))
                {

                    lesLivres = controller.GetAllLivres();
                    RemplirLivresListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// annule les modification ou ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerChoixLivres_Click(object sender, EventArgs e)
        {
            LivreEnCoursDeModif(false);
        }


        /// <summary>
        /// valide dans la bdd les changements en cours ( ajout / modification)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderChoixLivres_Click(object sender, EventArgs e)
        {
            bool checkValid;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = TxbLivresNumero.Text;
                Genre unGenre = (Genre)CbxLivresGenreInfos.SelectedItem;
                Public unPublic = (Public)CbxLivresPublicInfos.SelectedItem;
                Rayon unRayon = (Rayon)CbxLivresRayonInfos.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = TxbLivresTitre.Text;
                string image = TxbLivresImage.Text;
                string isbn = TxbLivresIsbn.Text;
                string auteur = TxbLivresAuteur.Text;
                string collection = TxbLivresCollection.Text;
                string idGenre = unGenre?.Id;
                string genre = unGenre?.Libelle;
                string idPublic = unPublic?.Id;
                string lePublic = unPublic?.Libelle;
                string idRayon = unRayon?.Id;
                string rayon = unRayon?.Libelle;
                if (titre != "" && auteur != "" && genre != null && unPublic != null)
                {
                    Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    if (!ajouterBool)  // si on est en  modification
                        checkValid = controller.ModifierLivre(livre);
                    else      // si on est en creation
                        checkValid = controller.CreerLivre(livre);
                    if (checkValid)
                    {
                        LivreEnCoursDeModif(false);
                        lesLivres = controller.GetAllLivres();
                        RemplirLivresListeComplete();
                    }
                    else
                    {
                        if (TxbLivresNumero.ReadOnly)
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = DgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }
        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private readonly BindingSource bdgDvdGenreInfos = new BindingSource();
        private readonly BindingSource bdgDvdPublicInfo = new BindingSource();
        private readonly BindingSource bdgDvdRayonInfo = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            lesDvd = controller.GetAllDvd();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, CbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, CbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, CbxDvdRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgDvdGenreInfos, CbxDvdGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgDvdPublicInfo, CbxDvdPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgDvdRayonInfo, CbxDvdRayonInfos);
            RemplirDvdListeComplete();
            if (controller.VerifDroitModif(utilisateur))
            {
                DvdEnCoursDeModif(false);
            }
            else
            {
                ConsultationDvd();
            }
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            DgvDvdListe.DataSource = bdgDvdListe;
            DgvDvdListe.Columns["idRayon"].Visible = false;
            DgvDvdListe.Columns["idGenre"].Visible = false;
            DgvDvdListe.Columns["idPublic"].Visible = false;
            DgvDvdListe.Columns["image"].Visible = false;
            DgvDvdListe.Columns["synopsis"].Visible = false;
            DgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DgvDvdListe.Columns["id"].DisplayIndex = 0;
            DgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!TxbDvdNumRecherche.Text.Equals(""))
            {
                TxbDvdTitreRecherche.Text = "";
                CbxDvdGenres.SelectedIndex = -1;
                CbxDvdRayons.SelectedIndex = -1;
                CbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(TxbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!TxbDvdTitreRecherche.Text.Equals(""))
            {
                CbxDvdGenres.SelectedIndex = -1;
                CbxDvdRayons.SelectedIndex = -1;
                CbxDvdPublics.SelectedIndex = -1;
                TxbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(TxbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (CbxDvdGenres.SelectedIndex < 0 && CbxDvdPublics.SelectedIndex < 0 && CbxDvdRayons.SelectedIndex < 0
                    && TxbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            TxbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            TxbDvdImage.Text = dvd.Image;
            TxbDvdDuree.Text = dvd.Duree.ToString();
            TxbDvdNumero.Text = dvd.Id;
            CbxDvdGenreInfos.SelectedIndex = CbxDvdGenreInfos.FindString(dvd.Genre);
            CbxDvdPublicInfos.SelectedIndex = CbxDvdPublicInfos.FindString(dvd.Public);
            CbxDvdRayonInfos.SelectedIndex = CbxDvdRayonInfos.FindString(dvd.Rayon);
            TxbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                PcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                PcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            TxbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            TxbDvdImage.Text = "";
            TxbDvdDuree.Text = "";
            TxbDvdNumero.Text = "";
            CbxDvdGenreInfos.SelectedIndex = -1;
            CbxDvdPublicInfos.SelectedIndex = -1;
            CbxDvdRayonInfos.SelectedIndex = -1;
            TxbDvdTitre.Text = "";
            PcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxDvdGenres.SelectedIndex >= 0)
            {
                TxbDvdTitreRecherche.Text = "";
                TxbDvdNumRecherche.Text = "";
                Genre genre = (Genre)CbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                CbxDvdRayons.SelectedIndex = -1;
                CbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxDvdPublics.SelectedIndex >= 0)
            {
                TxbDvdTitreRecherche.Text = "";
                TxbDvdNumRecherche.Text = "";
                Public lePublic = (Public)CbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                CbxDvdRayons.SelectedIndex = -1;
                CbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxDvdRayons.SelectedIndex >= 0)
            {
                TxbDvdTitreRecherche.Text = "";
                TxbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)CbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                CbxDvdGenres.SelectedIndex = -1;
                CbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            CbxDvdGenres.SelectedIndex = -1;
            CbxDvdRayons.SelectedIndex = -1;
            CbxDvdPublics.SelectedIndex = -1;
            TxbDvdNumRecherche.Text = "";
            TxbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// configure l'interface en fonction de la procédure événementielle requise
        /// </summary>
        /// <param name="modif"></param>
        private void DvdEnCoursDeModif(bool modif)
        {
            BtnAjouterDvd.Enabled = !modif;
            BtnSupprimerDvd.Enabled = !modif;
            BtnModifierDvd.Enabled = !modif;
            BtnAnnulerChoixDvd.Enabled = modif;
            BtnValiderChoixDvd.Enabled = modif;
            TxbDvdTitre.ReadOnly = !modif;
            TxbDvdRealisateur.ReadOnly = !modif;
            txbDvdSynopsis.ReadOnly = !modif;
            CbxDvdPublicInfos.Enabled = modif;
            TxbDvdDuree.ReadOnly = !modif;
            CbxDvdGenreInfos.Enabled = modif;
            CbxDvdRayonInfos.Enabled = modif;
            TxbDvdImage.ReadOnly = !modif;
            DgvDvdListe.Enabled = !modif;
            TxbDvdNumero.ReadOnly = true;
        }

        ///<summary>
        ///Bloque les  interfaces quand l'utilsateur est en lecture seul
        /// </summary>
        private void ConsultationDvd()
        {
            BtnAjouterDvd.Enabled = false;
            BtnModifierDvd.Enabled = false;
            BtnSupprimerDvd.Enabled = false;
            BtnValiderChoixDvd.Enabled = false;
            BtnAnnulerChoixDvd.Enabled = false;
            CbxDvdGenreInfos.Enabled = false;
            CbxDvdPublicInfos.Enabled = false;
            CbxDvdRayonInfos.Enabled = false;
        }

        /// <summary>
        /// lance la procédure d'ajout d'un nouveau dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjouterDvd_Click(object sender, EventArgs e)
        {
            DvdEnCoursDeModif(true);
            TxbDvdNumero.ReadOnly = false;
            VideDvdInfos();
        }

        /// <summary>
        /// lance la procédure de modification du DVD sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifierDvd_Click(object sender, EventArgs e)
        {
            DvdEnCoursDeModif(true);
        }

        /// <summary>
        /// lance la procédure de suppresion du DVD sélectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerDvd_Click(object sender, EventArgs e)
        {
            Dvd leDvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
            if (MessageBox.Show($"Etes vous sur de vouloir supprimer {leDvd.Titre} de {leDvd.Realisateur} ?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // fonction a modifier pour prendre en charge le faite que l'on ne pourra pas supprimer un livre tant que des examplaire de se livre existe
                if (controller.SupprimerDvd(leDvd))
                {
                    lesDvd = controller.GetAllDvd();
                    RemplirDvdListeComplete();
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }

        /// <summary>
        /// Annule les modification en cours (ajouter / supprimer )
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulerChoixDvd_Click(object sender, EventArgs e)
        {
            DvdEnCoursDeModif(false);
        }


        /// <summary>
        /// Valide les modification en cours dans la BDD ( ajouter / supprimer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnValiderChoixDvd_Click(object sender, EventArgs e)
        {
            bool Valide;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = TxbDvdNumero.Text;
                int? a = null;  // version simplifié de nullabe <int> a

                try
                {
                    a = int.Parse(TxbDvdNumero.Text);
                }
                catch
                {
                    MessageBox.Show("Le Numéro de document doit etre un entier");
                }
                Genre unGenre = (Genre)CbxDvdGenreInfos.SelectedItem;
                Public unPublic = (Public)CbxDvdPublicInfos.SelectedItem;
                Rayon unRayon = (Rayon)CbxDvdRayonInfos.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = TxbDvdTitre.Text;
                string image = TxbDvdImage.Text;
                int duree = (TxbDvdDuree.Text == "") ? 0 : int.Parse(TxbDvdDuree.Text);
                string realisateur = TxbDvdRealisateur.Text;
                string synopsis = txbDvdSynopsis.Text;
                string idGenre = unGenre?.Id;
                string genre = unGenre?.Libelle;
                string idPublic = unPublic?.Id;
                string lePublic = unPublic?.Libelle;
                string idRayon = unRayon?.Id;
                string rayon = unRayon?.Libelle;
                if (a != null && titre != "" && realisateur != "" && genre != null && unPublic != null)
                {
                    Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);
                    if (TxbDvdNumero.ReadOnly)  // si on est en  modification
                        Valide = controller.ModifierDvd(dvd);
                    else    // si on est en creation
                        Valide = controller.CreerDvd(dvd);
                    if (Valide)
                    {
                        DvdEnCoursDeModif(false);
                        lesDvd = controller.GetAllDvd();
                        RemplirDvdListeComplete();
                    }
                    else
                    {
                        if (TxbDvdNumero.ReadOnly)
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = DgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }
        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();
        private readonly BindingSource bdgRevuesGenreInfos = new BindingSource();
        private readonly BindingSource bdgRevuesPublicInfos = new BindingSource();
        private readonly BindingSource bdgRevuesRayonInfos = new BindingSource();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, CbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, CbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, CbxRevuesRayons);
            RemplirComboCategorie(controller.GetAllGenres(), bdgRevuesGenreInfos, CbxRevuesGenreInfos);
            RemplirComboCategorie(controller.GetAllPublics(), bdgRevuesPublicInfos, CbxRevuesPublicInfos);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRevuesRayonInfos, CbxRevuesRayonInfos);
            RemplirRevuesListeComplete();

            if (controller.VerifDroitModif(utilisateur))
            {
                RevueEnCoursDeModif(false);
            }
            else
            {
                ConsultationRevue();
            }
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            DgvRevuesListe.DataSource = bdgRevuesListe;
            DgvRevuesListe.Columns["idRayon"].Visible = false;
            DgvRevuesListe.Columns["idGenre"].Visible = false;
            DgvRevuesListe.Columns["idPublic"].Visible = false;
            DgvRevuesListe.Columns["image"].Visible = false;
            DgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DgvRevuesListe.Columns["id"].DisplayIndex = 0;
            DgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!TxbRevuesNumRecherche.Text.Equals(""))
            {
                TxbRevuesTitreRecherche.Text = "";
                CbxRevuesGenres.SelectedIndex = -1;
                CbxRevuesRayons.SelectedIndex = -1;
                CbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(TxbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!TxbRevuesTitreRecherche.Text.Equals(""))
            {
                CbxRevuesGenres.SelectedIndex = -1;
                CbxRevuesRayons.SelectedIndex = -1;
                CbxRevuesPublics.SelectedIndex = -1;
                TxbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(TxbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (CbxRevuesGenres.SelectedIndex < 0 && CbxRevuesPublics.SelectedIndex < 0 && CbxRevuesRayons.SelectedIndex < 0
                    && TxbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            TxbRevuesPeriodicite.Text = revue.Periodicite;
            TxbRevuesImage.Text = revue.Image;
            TxbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            TxbRevuesNumero.Text = revue.Id;
            CbxRevuesGenreInfos.SelectedIndex = CbxRevuesGenreInfos.FindString(revue.Genre);
            CbxRevuesPublicInfos.SelectedIndex = CbxRevuesPublicInfos.FindString(revue.Public);
            CbxRevuesRayonInfos.SelectedIndex = CbxRevuesRayonInfos.FindString(revue.Rayon);
            TxbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                PcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                PcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            TxbRevuesPeriodicite.Text = "";
            TxbRevuesImage.Text = "";
            TxbRevuesDateMiseADispo.Text = "";
            TxbRevuesNumero.Text = "";
            CbxRevuesGenreInfos.SelectedIndex = -1;
            CbxRevuesPublicInfos.SelectedIndex = -1;
            CbxRevuesRayonInfos.SelectedIndex = -1;
            TxbRevuesTitre.Text = "";
            PcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxRevuesGenres.SelectedIndex >= 0)
            {
                TxbRevuesTitreRecherche.Text = "";
                TxbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)CbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                CbxRevuesRayons.SelectedIndex = -1;
                CbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxRevuesPublics.SelectedIndex >= 0)
            {
                TxbRevuesTitreRecherche.Text = "";
                TxbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)CbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                CbxRevuesRayons.SelectedIndex = -1;
                CbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CbxRevuesRayons.SelectedIndex >= 0)
            {
                TxbRevuesTitreRecherche.Text = "";
                TxbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)CbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                CbxRevuesGenres.SelectedIndex = -1;
                CbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            CbxRevuesGenres.SelectedIndex = -1;
            CbxRevuesRayons.SelectedIndex = -1;
            CbxRevuesPublics.SelectedIndex = -1;
            TxbRevuesNumRecherche.Text = "";
            TxbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// configure l'interface en fonction de la procédure événementielle requise
        /// </summary>
        /// <param name="modif"></param>
        private void RevueEnCoursDeModif(bool modif)
        {
            BtnAjouterRevue.Enabled = !modif;
            BtnSupprimerRevue.Enabled = !modif;
            BtnModifierRevue.Enabled = !modif;
            BtnAnnulerChoixRevue.Enabled = modif;
            BtnValiderChoixRevue.Enabled = modif;
            TxbRevuesTitre.ReadOnly = !modif;
            TxbRevuesPeriodicite.ReadOnly = !modif;
            CbxRevuesPublicInfos.Enabled = modif;
            TxbRevuesDateMiseADispo.ReadOnly = !modif;
            TxbLivresCollection.ReadOnly = !modif;
            CbxRevuesGenreInfos.Enabled = modif;
            CbxRevuesRayonInfos.Enabled = modif;
            TxbRevuesImage.ReadOnly = !modif;
            TxbRevuesNumero.ReadOnly = true;
            DgvRevuesListe.Enabled = !modif;
        }

        ///<summary>
        ///Desactive les interface si l'utilisateur n'a acces qu'en lecture
        /// </summary>
        private void ConsultationRevue()
        {
            BtnSupprimerRevue.Enabled = false;
            BtnAjouterRevue.Enabled = false;
            BtnModifierRevue.Enabled = false;
            BtnValiderChoixRevue.Enabled = false;
            BtnAnnulerChoixRevue.Enabled = false;
            CbxRevuesGenreInfos.Enabled = false;
            CbxRevuesPublicInfos.Enabled = false;
            CbxRevuesRayonInfos.Enabled = false;
        }

        /// <summary>
        /// lance la procédure d'ajout d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAjouterRevues_Click(object sender, EventArgs e)
        {
            RevueEnCoursDeModif(true);
            ajouterBool = true;
            string id = plusUnIdString(controller.GetNbRevueMax());
            if (id == "1")
                id = "10001";
            TxbRevuesNumero.Text = id;
            TxbRevuesTitre.Text = "";
            TxbRevuesPeriodicite.Text = "";
            TxbRevuesDateMiseADispo.Text = "";
            TxbRevuesImage.Text = "";
            CbxRevuesPublicInfos.SelectedIndex = -1;
            CbxRevuesGenreInfos.SelectedIndex = -1;
            CbxRevuesRayonInfos.SelectedIndex = -1;

        }

        /// <summary>
        /// lance la procédure de modification d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifierRevues_Click(object sender, EventArgs e)
        {
            RevueEnCoursDeModif(true);
        }

        /// <summary>
        /// lance la procédure de suppresion d'une revue dans la bdd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerRevues_Click(object sender, EventArgs e)
        {
            Revue laRevue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
            if (MessageBox.Show($"Etes vous sur de vouloir supprimer {laRevue.Titre}?",
                "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (controller.GetExemplairesRevue(laRevue.Id).Count == 0)
                {

                    if (controller.SupprimerRevue(laRevue))
                    {
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Des parutions sont rattachées à cette revue, vous ne pouvez pas la supprimer");
                }

            }
        }

        /// <summary>
        /// annule les modifications en cours (ajout / suppresion)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnnulerChoixRevues_Click(object sender, EventArgs e)
        {
            RevueEnCoursDeModif(false);
        }

        /// <summary>
        /// valide les modifications en cours dans la bdd ( ajout / suppresion)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValiderChoixRevues_Click(object sender, EventArgs e)
        {
            bool checkValid;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = TxbRevuesNumero.Text;
                int delaiMiseADispo = 0;
                int? a = null;
                int? b = null;
                try
                {
                    a = int.Parse(TxbRevuesNumero.Text);
                    delaiMiseADispo = int.Parse(TxbRevuesDateMiseADispo.Text);
                    b = delaiMiseADispo;
                }
                catch
                {
                    MessageBox.Show("Le Numéro de document et le delai de mise a dispo doivent etre des entiers");
                }
                Genre unGenre = (Genre)CbxRevuesGenreInfos.SelectedItem;
                Public unPublic = (Public)CbxRevuesPublicInfos.SelectedItem;
                Rayon unRayon = (Rayon)CbxRevuesRayonInfos.SelectedItem;
                if (unGenre == null)
                    MessageBox.Show("Genre invalide");
                if (unPublic == null)
                    MessageBox.Show("Public invalide");
                if (unRayon == null)
                    MessageBox.Show("Rayon invalide");
                string titre = TxbRevuesTitre.Text;
                string image = TxbRevuesImage.Text;
                string idGenre = unGenre?.Id;
                string genre = unGenre?.Libelle;
                string idPublic = unPublic?.Id;
                string lePublic = unPublic?.Libelle;
                string idRayon = unRayon?.Id;
                string rayon = unRayon?.Libelle;
                string periodicite = TxbRevuesPeriodicite.Text;
                if (a != null && b != null && titre != "" && genre != null && unPublic != null)
                {
                    Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);
                    if (TxbRevuesNumero.ReadOnly)  // si on est en  modification
                        checkValid = controller.ModifierRevue(revue);
                    else      // si on est en creation
                        checkValid = controller.CreerRevue(revue);
                    if (checkValid)
                    {
                        RevueEnCoursDeModif(false);
                        lesRevues = controller.GetAllRevues();
                        RemplirRevuesListeComplete();
                    }
                    else
                    {
                        if (TxbRevuesNumero.ReadOnly)
                            MessageBox.Show("numéro de publication déjà existant", "Erreur");
                        else
                            MessageBox.Show("Erreur");
                    }
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = DgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Paarutions
        private readonly BindingSource bdgExemplairesListe = new BindingSource();
        private List<Exemplaire> lesExemplaires = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            TxbReceptionRevueNumero.Text = "";
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires != null)
            {
                bdgExemplairesListe.DataSource = exemplaires;
                DgvReceptionExemplairesListe.DataSource = bdgExemplairesListe;
                DgvReceptionExemplairesListe.Columns["idEtat"].Visible = false;
                DgvReceptionExemplairesListe.Columns["id"].Visible = false;
                DgvReceptionExemplairesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                DgvReceptionExemplairesListe.Columns["numero"].DisplayIndex = 0;
                DgvReceptionExemplairesListe.Columns["dateAchat"].DisplayIndex = 1;
            }
            else
            {
                bdgExemplairesListe.DataSource = null;
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!TxbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(TxbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            TxbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            TxbReceptionRevueDelaiMiseADispo.Text = "";
            TxbReceptionRevueGenre.Text = "";
            TxbReceptionRevuePublic.Text = "";
            TxbReceptionRevueRayon.Text = "";
            TxbReceptionRevueTitre.Text = "";
            PcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            TxbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            TxbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            TxbReceptionRevueNumero.Text = revue.Id;
            TxbReceptionRevueGenre.Text = revue.Genre;
            TxbReceptionRevuePublic.Text = revue.Public;
            TxbReceptionRevueRayon.Text = revue.Rayon;
            TxbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                PcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                PcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = TxbReceptionRevueNumero.Text;
            lesExemplaires = controller.GetExemplairesRevue(idDocuement);
            RemplirReceptionExemplairesListe(lesExemplaires);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            GrpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            TxbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            DtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!TxbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(TxbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = DtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string idDocument = TxbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    TxbReceptionExemplaireNumero.Text = "";
                    TxbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = DgvReceptionExemplairesListe.Columns[e.ColumnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();
            switch (titreColonne)
            {
                case "Numero":
                    sortedList = lesExemplaires.OrderBy(o => o.Numero).Reverse().ToList();
                    break;
                case "DateAchat":
                    sortedList = lesExemplaires.OrderBy(o => o.DateAchat).Reverse().ToList();
                    break;
                case "Photo":
                    sortedList = lesExemplaires.OrderBy(o => o.Photo).ToList();
                    break;
            }
            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvReceptionExemplairesListe.CurrentCell != null)
            {
                Exemplaire exemplaire = (Exemplaire)bdgExemplairesListe.List[bdgExemplairesListe.Position];
                string image = exemplaire.Photo;
                try
                {
                    PcbReceptionExemplaireRevueImage.Image = Image.FromFile(image);
                }
                catch
                {
                    PcbReceptionExemplaireRevueImage.Image = null;
                }
            }
            else
            {
                PcbReceptionExemplaireRevueImage.Image = null;
            }
        }
        #endregion

        #region Onglet Commande Livres

        //Information du premier dgv et des infos du Livre  selectionné
        private readonly BindingSource bdgLesLivresListe = new BindingSource();
        private readonly BindingSource bdgComLivreGenresInfo = new BindingSource();
        private readonly BindingSource bdgComLivrePublicInfo = new BindingSource();
        private readonly BindingSource bdgComLivreRayonInfo = new BindingSource();
        private List<Livre> lesComLivres = new List<Livre>();
        // informations du second dgv et des infos des la commandes selectionné
        private readonly BindingSource bdgListeCommandeLivre = new BindingSource();
        private readonly BindingSource bdgComLivreEtat = new BindingSource();
        private List<CommandeDocument> lesCommandes = new List<CommandeDocument>();

        /// <summary>
        /// Ouverture de l'onglet Commande de Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeLivres_Enter(object sender, EventArgs e)
        {
            if (!controller.VerifCommande(utilisateur))
            {
                MessageBox.Show("Droits insuffisant pour acceder a cette fonctionnaité");
                tabControl.SelectedIndex = 0;
            }
            else
            {
                lesComLivres = controller.GetAllLivres();
                RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxComLivresGenres);
                RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxComLivresPublics);
                RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxComLivresRayons);
                RemplirComboCategorie(controller.GetAllGenres(), bdgComLivreGenresInfo, cbxComLivresGenreInfos);
                RemplirComboCategorie(controller.GetAllPublics(), bdgComLivrePublicInfo, cbxComLivresPublicInfos);
                RemplirComboCategorie(controller.GetAllRayons(), bdgComLivreRayonInfo, cbxComLivresRayonInfos);
                RemplirComboSuivi(controller.GetAllSuivis(), bdgComLivreEtat, cbxCommandeLivreEtat);
                CommandeLivreEnCoursDeModif(false);
                RemplirComLivresListeComplete();
            }
            
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// affiche la liste des livres
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirComLivresListe(List<Livre> livres)
        {
            bdgLesLivresListe.DataSource = livres;
            DgvLesLivresListe.DataSource = bdgLesLivresListe;
            DgvLesLivresListe.Columns["isbn"].Visible = false;
            DgvLesLivresListe.Columns["idRayon"].Visible = false;
            DgvLesLivresListe.Columns["idGenre"].Visible = false;
            DgvLesLivresListe.Columns["idPublic"].Visible = false;
            DgvLesLivresListe.Columns["image"].Visible = false;
            DgvLesLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            DgvLesLivresListe.Columns["id"].DisplayIndex = 0;
            DgvLesLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// affiche la liste des commandes associé a un livre
        /// </summary>
        /// <param name="lesCommandes">liste des commandes d'un livres</param>
        private void RemplirLivresComListeCommandes(List<CommandeDocument> lesCommandes)
        {
            if (lesCommandes.Count > 0)
            {
                bdgListeCommandeLivre.DataSource = lesCommandes;
                dgvLivresCommande.DataSource = bdgListeCommandeLivre;
                dgvLivresCommande.Columns["idLivreDvd"].Visible = false;
                dgvLivresCommande.Columns["idSuivi"].Visible = false;
                dgvLivresCommande.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvLivresCommande.Columns["id"].DisplayIndex = 0;
                dgvLivresCommande.Columns["dateCommande"].DisplayIndex = 1;
                dgvLivresCommande.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvLivresCommande.Columns.Clear();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbComLivresNumRecherche.Text.Equals(""))
            {
                TxbComLivresTitreRecherche.Text = "";
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                Livre livre = lesComLivres.Find(x => x.Id.Equals(txbComLivresNumRecherche.Text));
                if (livre!= null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirComLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirComLivresListeComplete();
                }
            }
            else
            {
                RemplirComLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche avec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbComLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!TxbComLivresTitreRecherche.Text.Equals(""))
            {
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                txbComLivresNumRecherche.Text = "";
                List<Livre> ComLivresParTitre;
                ComLivresParTitre = lesComLivres.FindAll(x => x.Titre.ToLower().Contains(TxbComLivresTitreRecherche.Text.ToLower()));
                RemplirComLivresListe(ComLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxComLivresGenres.SelectedIndex < 0 && cbxComLivresPublics.SelectedIndex < 0 && cbxComLivresRayons.SelectedIndex < 0
                    && txbComLivresNumRecherche.Text.Equals(""))
                {
                    RemplirComLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheComLivresInfos(Livre livre)
        {
            txbComLivresAuteur.Text = livre.Auteur;
            txbComLivresCollection.Text = livre.Collection;
            txbComLivresImage.Text = livre.Image;
            txbComLivresIsbn.Text = livre.Isbn;
            txbComLivresNumero.Text = livre.Id;
            cbxComLivresGenreInfos.SelectedIndex = cbxComLivresGenreInfos.FindString(livre.Genre);
            cbxComLivresPublicInfos.SelectedIndex = cbxComLivresPublicInfos.FindString(livre.Public);
            cbxComLivresRayonInfos.SelectedIndex = cbxComLivresRayonInfos.FindString(livre.Rayon);
            txbComLivresTitre.Text = livre.Titre;
            

        }



        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresGenres.SelectedIndex >= 0)
            {
                TxbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                DgvLesLivresListe.ClearSelection();
                Genre genre = (Genre)cbxComLivresGenres.SelectedItem;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                List<Livre> livres = lesComLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirComLivresListe(livres);
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresPublics.SelectedIndex >= 0)
            {
                TxbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                DgvLesLivresListe.ClearSelection();
                Public lePublic = (Public)cbxComLivresPublics.SelectedItem;
                cbxComLivresRayons.SelectedIndex = -1;
                cbxComLivresGenres.SelectedIndex = -1;
                List<Livre> livres = lesComLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirComLivresListe(livres);
                
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComLivresRayons.SelectedIndex >= 0)
            {
                TxbComLivresTitreRecherche.Text = "";
                txbComLivresNumRecherche.Text = "";
                DgvLesLivresListe.ClearSelection();
                Rayon rayon = (Rayon)cbxComLivresRayons.SelectedItem;
                cbxComLivresGenres.SelectedIndex = -1;
                cbxComLivresPublics.SelectedIndex = -1;
                List<Livre> livres = lesComLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComLivresListe(livres);
                
            }
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un livre
        /// </summary>
        /// <param name="livre"></param>
        private void AfficheLivresCommandeInfos(Livre livre)
        {
            string idLivre = livre.Id;
            VideLivresComInfos();
            Console.WriteLine("test d'appel de methode");
            lesCommandes = controller.GetCommandesLivres(idLivre);
            GrpLivreCommande.Text = livre.Titre + " de " + livre.Auteur;
            Console.WriteLine("lesCommandes.count = " + lesCommandes.Count.ToString());

            if (lesCommandes.Count == 0)
                VideLivresComInfos();
            RemplirLivresComListeCommandes(lesCommandes);
        }


        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirComLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirComLivresListeComplete()
        {
            RemplirComLivresListe(lesComLivres);
            VideComLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideComLivresZones()
        {
            cbxComLivresGenres.SelectedIndex = -1;
            cbxComLivresRayons.SelectedIndex = -1;
            cbxComLivresPublics.SelectedIndex = -1;
            txbComLivresNumRecherche.Text = "";
            TxbComLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresComZones()
        {
            cbxComLivresGenres.SelectedIndex = -1;
            cbxComLivresRayons.SelectedIndex = -1;
            cbxComLivresPublics.SelectedIndex = -1;
            txbComLivresNumRecherche.Text = "";
            TxbComLivresTitreRecherche.Text = "";
            GrpLivreCommande.Text = "";
        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideLivresComInfos()
        {
            txbCommandeNumeroCommande.Text = "";
            DtpComandeLivre.Value = DateTime.Now.Date;
            txbCommandeLivreMontant.Text = "";
            txbCommandeExemplaireLivre.Text = "";
            cbxCommandeLivreEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void CommandeLivreEnCoursDeModif(bool modif)
        {
            BtnLivreAjouterCom.Enabled = !modif;
            BtnLivreSupprimerCom.Enabled = !modif;
            BtnLivreModifierCom.Enabled = !modif;
            BtnAnnulerChoix.Enabled = modif;
            BtnValiderChoix.Enabled = modif;
            BtnComLivresNumRecherche.Enabled = !modif;
            BtnComLivresAnnulGenres.Enabled = !modif;
            BtnComLivresAnnulPublic.Enabled = !modif;
            BtnComLivresAnnulRayon.Enabled = !modif;
            TxbLivresTitre.ReadOnly = !modif;
            TxbLivresAuteur.ReadOnly = !modif;
            txbComLivresNumRecherche.Enabled = !modif;
            TxbComLivresTitreRecherche.Enabled = !modif;
            CbxLivresPublicInfos.Enabled = modif;
            txbComLivresNumero.ReadOnly = !modif;
            TxbLivresCollection.ReadOnly = !modif;
            CbxLivresGenreInfos.Enabled = modif;
            CbxLivresRayonInfos.Enabled = modif;
            TxbLivresImage.ReadOnly = !modif;
            txbComLivresNumero.ReadOnly = true;
            DgvLesLivresListe.Enabled = !modif;
            dgvLivresCommande.Enabled = !modif;
            txbCommandeNumeroCommande.ReadOnly = true;
            DtpComandeLivre.Enabled = !modif;
            txbCommandeLivreMontant.ReadOnly = !modif;
            txbCommandeExemplaireLivre.ReadOnly = !modif;
            txbCommandeNumeroLivre.ReadOnly = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// affiche les détails d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheLivresComInfo(CommandeDocument laCommande)
        {
            txbCommandeNumeroCommande.Text = laCommande.Id;
            txbCommandeNumeroLivre.Text = laCommande.IdLivreDvd;
            DtpComandeLivre.Value = laCommande.DateCommande;
            txbCommandeLivreMontant.Text = laCommande.Montant.ToString();
            txbCommandeExemplaireLivre.Text = laCommande.NbExemplaire.ToString();
            cbxCommandeLivreEtat.SelectedIndex = cbxCommandeLivreEtat.FindString(laCommande.Etat);
        }


        /// <summary>
        /// démarre la procédure d'ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAjouter_Click(object sender, EventArgs e)
        {
            CommandeLivreEnCoursDeModif(true);
            txbCommandeNumeroLivre.ReadOnly = true;
            ajouterBool = true;
            string id = plusUnIdString(controller.GetNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideLivresComInfos();
            cbxCommandeLivreEtat.SelectedIndex = 0;
            txbCommandeNumeroCommande.Text = id;
            cbxCommandeLivreEtat.Enabled = false;
            
        }

        /// <summary>
        /// démarre la procédure de modification de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComModifier_Click(object sender, EventArgs e)
        {
            if (dgvLivresCommande.CurrentCell != null && txbCommandeNumeroCommande.Text != "")
            {
                List<Suivi> lesSuivi = controller.GetAllSuivis().FindAll(o => o.Id >= ((Suivi)cbxCommandeLivreEtat.SelectedItem).Id).ToList();
                if (lesSuivi.Count > 2)
                    lesSuivi = lesSuivi.FindAll(o => o.Id < 4).ToList();
                CommandeLivreEnCoursDeModif(true);
                RemplirComboSuivi(lesSuivi, bdgComLivreEtat, cbxCommandeLivreEtat);
                cbxCommandeLivreEtat.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Aucune commande sélectionné");
            }
        }

        /// <summary>
        /// supprime une commande de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComSupprimer_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeLivre[bdgListeCommandeLivre.Position];
            if (dgvLivresCommande.CurrentCell != null && txbCommandeNumeroCommande.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                    MessageBox.Show("Une commande livrée ou réglée ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sur de vouloir supprimer la commande n°" + commandeDocument.Id +
                    " concernant " + lesComLivres.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre + " ?",
                    "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {

                        try
                        {
                            Livre livre = (Livre)bdgLesLivresListe.List[bdgLesLivresListe.Position];
                            AfficheLivresCommandeInfos(livre);
                            txbCommandeNumeroCommande.Text = livre.Id;
                        }
                        catch
                        {
                            VideLivresComZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une commande");
            }



        }


        /// <summary>
        /// annule la modifications ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComAnnuler_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgComLivreEtat, cbxCommandeLivreEtat);
            CommandeLivreEnCoursDeModif(false);
            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeLivre[bdgListeCommandeLivre.Position];
                AfficheLivresComInfo(commandeDocument);
            }
            catch
            {
                VideLivresComInfos();
            }
        }


        /// <summary>
        /// valide la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLivresComValider_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbCommandeNumeroCommande.Text;
                bool checkValid = false;
                DateTime dateCommande = DtpComandeLivre.Value;
                float montant = -1;
                int nbExemplaire = -1;
                try
                {
                    montant = float.Parse(txbCommandeLivreMontant.Text);
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un nombre a virgule");
                }
                try
                {
                    nbExemplaire = int.Parse(txbCommandeExemplaireLivre.Text);
                }
                catch
                {
                    MessageBox.Show("Le nombre d'exemplaire doit etre un nombre a entier");
                }
                string idLivreDvd = txbCommandeNumeroLivre.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxCommandeLivreEtat.SelectedItem;
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                else
                    MessageBox.Show("Veuillez selectionner un etat");
                if (montant <= -1 && nbExemplaire != -1 && etat != "")
                {
                    CommandeDocument commandeLivre = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                    if (!ajouterBool)
                        checkValid = controller.ModifierLivreDvdCom(commandeLivre);
                    else
                        checkValid = controller.CreerLivreDvdCom(commandeLivre);
                    if (checkValid)
                    {
                        if (!ajouterBool)
                            RemplirComboSuivi(controller.GetAllSuivis(), bdgComLivreEtat, cbxCommandeLivreEtat);
                        CommandeLivreEnCoursDeModif(false);
                        try
                        {
                            Livre livre = (Livre)bdgLesLivresListe[bdgLesLivresListe.Position];
                            AfficheLivresCommandeInfos(livre);
                            txbCommandeNumeroCommande.Text = livre.Id;
                        }
                        catch
                        {
                            VideLivresComZones();
                        }

                    }
                    else
                        MessageBox.Show("Erreur");
                }

            }

        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage les commandes relative a un livre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLesLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (DgvLesLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLesLivresListe.List[bdgLesLivresListe.Position];
                    AfficheLivresCommandeInfos(livre);
                    txbComLivresNumero.Text = livre.Id;
                }
                catch
                {
                    VideComLivresZones();
                }
            }
            else
            {
                txbCommandeNumeroLivre.Text = "";
                VideLivresComInfos();

            }
            if (DgvLesLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLesLivresListe.List[bdgLesLivresListe.Position];
                    AfficheComLivresInfos(livre);
                }
                catch
                {
                    VideComLivresZones();
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLesLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideComLivresZones();
            string titreColonne = DgvLesLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesComLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesComLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesComLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesComLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesComLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesComLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesComLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirComLivresListe(sortedList);
        }

        

       
        ///<summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage les information d'une commande .
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        private void dgvLivresCommande_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLivresCommande.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeLivre[bdgListeCommandeLivre.Position];
                    AfficheLivresComInfo(commandeDocument);
                }
                catch
                {
                    VideLivresComInfos();
                }

            }
            else
            {
                VideLivresComInfos();
            }
        }


        /// <summary>
        /// tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvLivresCommande_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandes.Count > 0 && dgvLivresCommande != null)
            {
                VideLivresComInfos();
                string titreColonne = dgvLivresCommande.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "IdLivre":
                        sortedList = lesCommandes.OrderBy(o => o.Id).ToList();
                        
                        break;
                    case "DateCommande":
                        sortedList = lesCommandes.OrderBy(o => o.DateCommande).ToList();
                        
                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandes.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandes.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandes.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirLivresComListeCommandes(sortedList);
            }
        }

        #endregion

        #region Onglet Commande Dvd

        //Information du premier dgv et des infos du Livre  selectionné
        private readonly BindingSource bdgLesDvdListe = new BindingSource();
        private readonly BindingSource bdgComDvdGenresInfo = new BindingSource();
        private readonly BindingSource bdgComDvdPublicInfo = new BindingSource();
        private readonly BindingSource bdgComDvdRayonInfo = new BindingSource();
        private List<Dvd> lesComDvd = new List<Dvd>();
        // informations du second dgv et des infos des la commandes selectionné
        private readonly BindingSource bdgListeCommandeDvd = new BindingSource();
        private readonly BindingSource bdgComDvdEtat = new BindingSource();
        private List<CommandeDocument> lesCommandesDvd = new List<CommandeDocument>();

        /// <summary>
        /// Ouverture de l'onglet Commande de Dvd : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabCommandeDvd_Enter(object sender, EventArgs e)
        {
            if (!controller.VerifCommande(utilisateur))
            {
                MessageBox.Show("Droit insuffisant pour acceder a cette fonctionalité");
                tabControl.SelectedIndex = 0;
            }
            else
            {
                lesComDvd = controller.GetAllDvd();
                RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxComDvdGenres);
                RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxComDvdPublics);
                RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxComDvdRayons);
                RemplirComboCategorie(controller.GetAllGenres(), bdgComDvdGenresInfo, cbxComDvdGenreInfos);
                RemplirComboCategorie(controller.GetAllPublics(), bdgComDvdPublicInfo, cbxComDvdPublicInfos);
                RemplirComboCategorie(controller.GetAllRayons(), bdgComDvdRayonInfo, cbxComDvdRayonInfos);
                RemplirComboSuivi(controller.GetAllSuivis(), bdgComDvdEtat, cbxCommandeDvdEtat);
                CommandeDvdEnCoursDeModif(false);
                RemplirComDvdListeComplete();
            }
            
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// affiche la liste des Dvd
        /// </summary>
        /// <param name="dvd">liste de livres</param>
        private void RemplirComDvdListe(List<Dvd> dvd)
        {
            bdgLesDvdListe.DataSource = dvd;
            dgvComDvdListe.DataSource = bdgLesDvdListe;
            dgvComDvdListe.Columns["Duree"].Visible = false;
            dgvComDvdListe.Columns["idRayon"].Visible = false;
            dgvComDvdListe.Columns["idGenre"].Visible = false;
            dgvComDvdListe.Columns["idPublic"].Visible = false;
            dgvComDvdListe.Columns["image"].Visible = false;
            dgvComDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvComDvdListe.Columns["id"].DisplayIndex = 0;
            dgvComDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// affiche la liste des commandes associé a un Dvd
        /// </summary>
        /// <param name="lesCommandesDvd">liste des commandes d'un livres</param>
        private void RemplirDvdComListeCommandes(List<CommandeDocument> lesCommandesDvd)
        {
            if (lesCommandesDvd.Count > 0)
            {
                bdgListeCommandeDvd.DataSource = lesCommandesDvd;
                dgvCommandeDvd.DataSource = bdgListeCommandeDvd;
                dgvCommandeDvd.Columns["idLivreDvd"].Visible = false;
                dgvCommandeDvd.Columns["idSuivi"].Visible = false;
                dgvCommandeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvCommandeDvd.Columns["id"].DisplayIndex = 0;
                dgvCommandeDvd.Columns["dateCommande"].DisplayIndex = 1;
                dgvCommandeDvd.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvCommandeDvd.Columns.Clear();
            }

        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbComDvdNumRecherche.Text.Equals(""))
            {
                txbComDvdTitreRecherche.Text = "";
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesComDvd.Find(x => x.Id.Equals(txbComDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> dvds = new List<Dvd>() { dvd };
                    RemplirComDvdListe(dvds);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirComDvdListeComplete();
                }
            }
            else
            {
                RemplirComDvdListeComplete();
            }
        }


        /// <summary>
        /// Recherche et affichage des livres dont le titre matche avec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbComDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbComDvdTitreRecherche.Text.Equals(""))
            {
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                txbComDvdNumRecherche.Text = "";
                List<Dvd> ComDvdParTitre;
                ComDvdParTitre = lesComDvd.FindAll(x => x.Titre.ToLower().Contains(txbComDvdTitreRecherche.Text.ToLower()));
                RemplirComDvdListe(ComDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxComDvdGenres.SelectedIndex < 0 && cbxComDvdPublics.SelectedIndex < 0 && cbxComDvdRayons.SelectedIndex < 0
                    && txbComDvdNumRecherche.Text.Equals(""))
                {
                    RemplirComDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="dvd">le livre</param>
        private void AfficheComDvdInfos(Dvd dvd)
        {
            txbComDvdRealisateur.Text = dvd.Realisateur;
            txbComDvdDuree.Text = dvd.Duree.ToString();
            txbComDvdImage.Text = dvd.Image;
            txbComDvdSynopsis.Text = dvd.Synopsis;
            txbComDvdNumero.Text = dvd.Id;
            cbxComDvdGenreInfos.SelectedIndex = cbxComDvdGenreInfos.FindString(dvd.Genre);
            cbxComDvdPublicInfos.SelectedIndex = cbxComDvdPublicInfos.FindString(dvd.Public);
            cbxComDvdRayonInfos.SelectedIndex = cbxComDvdRayonInfos.FindString(dvd.Rayon);
            txbComDvdTitre.Text = dvd.Titre;
            

        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdGenres.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                dgvComDvdListe.ClearSelection();
                Genre genre = (Genre)cbxComDvdGenres.SelectedItem;
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                List<Dvd> dvds = lesComDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirComDvdListe(dvds);
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdPublics.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                dgvComDvdListe.ClearSelection();
                Public lePublic = (Public)cbxComDvdPublics.SelectedItem;
                cbxComDvdRayons.SelectedIndex = -1;
                cbxComDvdGenres.SelectedIndex = -1;
                List<Dvd> dvds = lesComDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirComDvdListe(dvds);

            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxComDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxComDvdRayons.SelectedIndex >= 0)
            {
                txbComDvdTitreRecherche.Text = "";
                txbComDvdNumRecherche.Text = "";
                dgvComDvdListe.ClearSelection();
                Rayon rayon = (Rayon)cbxComLivresRayons.SelectedItem;
                cbxComDvdGenres.SelectedIndex = -1;
                cbxComDvdPublics.SelectedIndex = -1;
                List<Dvd> dvds = lesComDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirComDvdListe(dvds);

            }
        }

        /// <summary>
        /// Récupère et affiche les commandes d'un dvd
        /// </summary>
        /// <param name="dvd"></param>
        private void AfficheDvdCommandeInfos(Dvd dvd)
        {
            string idDvd = dvd.Id;
            VideDvdComInfos();
            Console.WriteLine("test d'appel de methode");
            lesCommandesDvd = controller.GetCommandesDvd(idDvd);
            GrpDvdCommande.Text = dvd.Titre + " de " + dvd.Realisateur;
            Console.WriteLine("lesCommandesDvd.count = " + lesCommandesDvd.Count.ToString());

            if (lesCommandesDvd.Count == 0)
                VideDvdComInfos();
            RemplirDvdComListeCommandes(lesCommandesDvd);
        }


        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDvdComAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirComDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirComDvdListeComplete()
        {
            RemplirComDvdListe(lesComDvd);
            VideComDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideComDvdZones()
        {
            cbxComDvdGenres.SelectedIndex = -1;
            cbxComDvdRayons.SelectedIndex = -1;
            cbxComDvdPublics.SelectedIndex = -1;
            txbComDvdNumRecherche.Text = "";
            txbComDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdComZones()
        {
            cbxComDvdGenres.SelectedIndex = -1;
            cbxComDvdRayons.SelectedIndex = -1;
            cbxComDvdPublics.SelectedIndex = -1;
            txbComDvdNumRecherche.Text = "";
            txbComDvdTitreRecherche.Text = "";
            GrpDvdCommande.Text = "";
        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideDvdComInfos()
        {
            txbCommandeDvdNumeroCommande.Text = "";
            dtpCommandeDvdDate.Value = DateTime.Now.Date;
            txbCommandeDvdMontant.Text = "";
            txbCommandeDvdNbExemplaire.Text = "";
            cbxCommandeLivreEtat.SelectedIndex = -1;
        }

        /// <summary>
        /// applique des droits sur l'interface en fonction de la situation
        /// </summary>
        /// <param name="modif"></param>
        private void CommandeDvdEnCoursDeModif(bool modif)
        {
            BtnAjouterDvdCom.Enabled = !modif;
            BtnSupprimerDvdCom.Enabled = !modif;
            BtnModifierDvdCom.Enabled = !modif;
            BtnAnnulerDvdCom.Enabled = modif;
            BtnValiderDvdCom.Enabled = modif;
            btnComDvdNumRecherche.Enabled = !modif;
            BtnComLivresAnnulGenres.Enabled = !modif;
            BtnComLivresAnnulPublic.Enabled = !modif;
            BtnComLivresAnnulRayon.Enabled = !modif;
            TxbDvdTitre.ReadOnly = !modif;
            TxbDvdRealisateur.ReadOnly = !modif;
            txbComDvdNumRecherche.Enabled = !modif;
            txbComDvdTitreRecherche.Enabled = !modif;
            CbxDvdPublicInfos.Enabled = modif;
            txbCommandeDvdNumero.ReadOnly = !modif;
            TxbDvdDuree.ReadOnly = !modif;
            CbxDvdGenreInfos.Enabled = modif;
            CbxDvdRayonInfos.Enabled = modif;
            TxbDvdImage.ReadOnly = !modif;
            txbComDvdNumero.ReadOnly = true;
            dgvComDvdListe.Enabled = !modif;
            dgvCommandeDvd.Enabled = !modif;
            txbCommandeDvdNumeroCommande.ReadOnly = true;
            dtpCommandeDvdDate.Enabled = !modif;
            txbCommandeDvdMontant.ReadOnly = !modif;
            txbCommandeDvdNbExemplaire.ReadOnly = !modif;
            txbComDvdSynopsis.ReadOnly = !modif;
            ajouterBool = false;
        }

        /// <summary>
        /// affiche les détails d'une commande
        /// </summary>
        /// <param name="laCommande"></param>
        private void AfficheDvdComInfo(CommandeDocument laCommande)
        {
            txbCommandeDvdNumeroCommande.Text = laCommande.Id;
            txbCommandeDvdNumero.Text = laCommande.IdLivreDvd;
            dtpCommandeDvdDate.Value = laCommande.DateCommande;
            txbCommandeDvdMontant.Text = laCommande.Montant.ToString();
            txbCommandeDvdNbExemplaire.Text = laCommande.NbExemplaire.ToString();
            cbxCommandeDvdEtat.SelectedIndex = cbxCommandeDvdEtat.FindString(laCommande.Etat);
        }


        /// <summary>
        /// démarre la procédure d'ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComAjouterDvd_Click(object sender, EventArgs e)
        {
            CommandeDvdEnCoursDeModif(true);
            txbCommandeDvdNumero.ReadOnly = true;
            ajouterBool = true;
            string id = plusUnIdString(controller.GetNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideDvdComInfos();
            cbxCommandeDvdEtat.SelectedIndex = 0;
            txbCommandeDvdNumeroCommande.Text = id;
            cbxCommandeDvdEtat.Enabled = false;

        }

        /// <summary>
        /// démarre la procédure de modification de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnComModifierDvd_Click(object sender, EventArgs e)
        {
            if (dgvCommandeDvd.CurrentCell != null && txbCommandeDvdNumeroCommande.Text != "")
            {
                List<Suivi> lesSuivi = controller.GetAllSuivis().FindAll(o => o.Id >= ((Suivi)cbxCommandeDvdEtat.SelectedItem).Id).ToList();
                if (lesSuivi.Count > 2)
                    lesSuivi = lesSuivi.FindAll(o => o.Id < 4).ToList();
                CommandeDvdEnCoursDeModif(true);
                RemplirComboSuivi(lesSuivi, bdgComDvdEtat, cbxCommandeDvdEtat);
                cbxCommandeDvdEtat.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Aucune commande sélectionné");
            }
        }

       
        /// <summary>
        /// supprime une commande de livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerDvdCom_Click(object sender, EventArgs e)
        {
            CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeDvd[bdgListeCommandeDvd.Position];
            if (dgvCommandeDvd.CurrentCell != null && txbCommandeDvdNumeroCommande.Text != "")
            {
                if (commandeDocument.IdSuivi > 2)
                    MessageBox.Show("Une commande livrée ou réglée ne peut etre supprimée");
                else if (MessageBox.Show("Etes vous sur de vouloir supprimer la commande n°" + commandeDocument.Id +
                    " concernant " + lesComDvd.Find(o => o.Id == commandeDocument.IdLivreDvd).Titre + " ?",
                    "Validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerLivreDvdCom(commandeDocument))
                    {

                        try
                        {
                            Dvd dvd = (Dvd)bdgLesDvdListe.List[bdgLesDvdListe.Position];
                            AfficheDvdCommandeInfos(dvd);
                            txbCommandeDvdNumeroCommande.Text = dvd.Id;
                        }
                        catch
                        {
                            VideDvdComZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner une commande");
            }



        }
        /// <summary>
        /// annule la modifications ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnnulerDvdCom_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            RemplirComboSuivi(controller.GetAllSuivis(), bdgComLivreEtat, cbxCommandeLivreEtat);
            CommandeLivreEnCoursDeModif(false);
            try
            {
                CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeDvd[bdgListeCommandeDvd.Position];
                AfficheDvdComInfo(commandeDocument);
            }
            catch
            {
                VideLivresComInfos();
            }
        }


        /// <summary>
        /// valide la modification ou l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValiderDvdCom_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbCommandeDvdNumeroCommande.Text;
                bool checkValid = false;
                DateTime dateCommande = dtpCommandeDvdDate.Value;
                float montant = -1;
                int nbExemplaire = -1;
                try
                {
                    montant = float.Parse(txbCommandeDvdMontant.Text);
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un nombre a virgule");
                }
                try
                {
                    nbExemplaire = int.Parse(txbCommandeDvdNbExemplaire.Text);
                }
                catch
                {
                    MessageBox.Show("Le nombre d'exemplaire doit etre un nombre a entier");
                }
                string idLivreDvd = txbCommandeDvdNumero.Text;
                int idSuivi = 0;
                string etat = "";
                Suivi suivi = (Suivi)cbxCommandeDvdEtat.SelectedItem;
                if (suivi != null)
                {
                    idSuivi = suivi.Id;
                    etat = suivi.Etat;
                }
                else
                    MessageBox.Show("Veuillez selectionner un etat");
                if (montant <= -1 && nbExemplaire != -1 && etat != "")
                {
                    CommandeDocument commandeDvd = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi, etat);
                    if (!ajouterBool)
                        checkValid = controller.ModifierLivreDvdCom(commandeDvd);
                    else
                        checkValid = controller.CreerLivreDvdCom(commandeDvd);
                    if (checkValid)
                    {
                        if (!ajouterBool)
                            RemplirComboSuivi(controller.GetAllSuivis(), bdgComDvdEtat, cbxCommandeDvdEtat);
                        CommandeDvdEnCoursDeModif(false);
                        try
                        {
                            Dvd dvd = (Dvd)bdgLesDvdListe[bdgLesDvdListe.Position];
                            AfficheDvdCommandeInfos(dvd);
                            txbCommandeNumeroCommande.Text = dvd.Id;
                        }
                        catch
                        {
                            VideDvdComZones();
                        }

                    }
                    else
                        MessageBox.Show("Erreur");
                }

            }

        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage les commandes relative a un dvd.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvComDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvComDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgLesDvdListe.List[bdgLesDvdListe.Position];
                    AfficheDvdCommandeInfos(dvd);
                    txbComDvdNumero.Text = dvd.Id;
                }
                catch
                {
                    VideComDvdZones();
                }
            }
            else
            {
                txbCommandeDvdNumero.Text = "";
                VideDvdComInfos();

            }
            if (dgvComDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgLesDvdListe.List[bdgLesDvdListe.Position];
                    AfficheComDvdInfos(dvd);
                }
                catch
                {
                    VideComDvdZones();
                }
            }
        }


        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvComDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideComDvdZones();
            string titreColonne = dgvComDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesComDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesComDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Synopsis":
                    sortedList = lesComDvd.OrderBy(o => o.Synopsis).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesComDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesComDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesComDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesComDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirComDvdListe(sortedList);
        }




        ///<summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage les information d'une commande .
        /// </summary>
        /// <param name="e"></param>
        /// <param name="sender"></param>
        private void dgvCommandeDvd_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCommandeDvd.CurrentCell != null)
            {
                try
                {
                    CommandeDocument commandeDocument = (CommandeDocument)bdgListeCommandeDvd[bdgListeCommandeDvd.Position];
                    AfficheDvdComInfo(commandeDocument);
                }
                catch
                {
                    VideDvdComInfos();
                }

            }
            else
            {
                VideDvdComInfos();
            }
        }


        /// <summary>
        /// tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvCommandeDvd_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (lesCommandesDvd.Count > 0 && dgvCommandeDvd != null)
            {
                VideDvdComInfos();
                string titreColonne = dgvCommandeDvd.Columns[e.ColumnIndex].HeaderText;
                List<CommandeDocument> sortedList = new List<CommandeDocument>();
                switch (titreColonne)
                {
                    case "IdDvd":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Id).ToList();

                        break;
                    case "DateCommande":
                        sortedList = lesCommandesDvd.OrderBy(o => o.DateCommande).ToList();

                        break;
                    case "NbExemplaire":
                        sortedList = lesCommandesDvd.OrderBy(o => o.NbExemplaire).ToList();
                        break;
                    case "Etat":
                        sortedList = lesCommandesDvd.OrderBy(o => o.IdSuivi).ToList();
                        break;
                    case "Montant":
                        sortedList = lesCommandesDvd.OrderBy(o => o.Montant).ToList();
                        break;
                }
                RemplirLivresComListeCommandes(sortedList);
            }
        }

        #endregion

        #region Abonnements

        private readonly BindingSource bdgListeAbo = new BindingSource();
        private readonly BindingSource bdgListeAboCommande = new BindingSource();
        private readonly BindingSource bdgRevueInfoGenre = new BindingSource();
        private readonly BindingSource bdgRevueInfoPublic = new BindingSource();
        private readonly BindingSource bdgRevueInfoRayon = new BindingSource();
        private List<Revue> lesAboRevue = new List<Revue>();
        private List<Abonnement> LesAbonnements = new List<Abonnement>();
        bool filtre;

        /// <summary>
        /// Ouverture de l'onglet Abonnement:
        /// Appel de methodes pour remplir les datagrid et les combo box des revues 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabAbonnement_Enter(object sender, EventArgs e)
        {

            if (!controller.VerifCommande(utilisateur))
            {
                MessageBox.Show("Droit insuffisant pour acceder a cette fonctionalité");
                tabControl.SelectedIndex = 0;
            }
            else
            {
                lesAboRevue = controller.GetAllRevues();
                RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxAboRevueGenre);
                RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxAboRevuePublic);
                RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxAboRevueRayon);
                RemplirComboCategorie(controller.GetAllGenres(), bdgRevueInfoGenre, cbxInfoRevueGenre);
                RemplirComboCategorie(controller.GetAllPublics(), bdgRevueInfoPublic, cbxInfoRevuePublic);
                RemplirComboCategorie(controller.GetAllRayons(), bdgRevueInfoRayon, cbxInfoRevueRayon);
                AbonnementEnCoursDeModif(false);
                RemplirListeAboComplete();
                filtre = false;
            }
            
        }

        /// <summary>
        /// Remplit le data grid des revue
        /// </summary>
        /// <param name="revue"></param>
        private void RemplirListeAbo(List<Revue> revues)
        {
            bdgListeAbo.DataSource = revues;
            dgvListeAbo.DataSource = bdgListeAbo;
            dgvListeAbo.Columns["idRayon"].Visible = false;
            dgvListeAbo.Columns["idPublic"].Visible = false;
            dgvListeAbo.Columns["idGenre"].Visible = false;
            dgvListeAbo.Columns["image"].Visible = false;
            dgvListeAbo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvListeAbo.Columns["id"].DisplayIndex = 0;
            dgvListeAbo.Columns["titre"].DisplayIndex = 1;
            dgvListeAbo.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        }

        /// <summary>
        /// Remplit le data grid des abonnements aux revues
        /// </summary>
        /// <param name="LesAbonnements"></param>
        private void RemplirListeAboCommande(List<Abonnement> LesAbonnements)
        {
            if (LesAbonnements.Count > 0)
            {
                bdgListeAboCommande.DataSource = LesAbonnements;
                dgvListeAboCommande.DataSource = bdgListeAboCommande;
                dgvListeAboCommande.Columns["idRevue"].Visible = false;
                dgvListeAboCommande.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                dgvListeAboCommande.Columns["id"].DisplayIndex = 0;
                dgvListeAboCommande.Columns["dateCommande"].DisplayIndex = 1;
                dgvListeAboCommande.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
            else
            {
                dgvListeAboCommande.Columns.Clear();
            }
        }



        /// <summary>
        /// Recherche et affiche la revue qui match avec la saisie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnRevueAboRecherche_Click(object sender, EventArgs e)
        {
            if (!txbAboRevueNumRecherche.Text.Equals(""))
            {
                txbAboRevueTitreRecherche.Text = "";
                cbxAboRevueGenre.SelectedIndex = -1;
                cbxAboRevuePublic.SelectedIndex = -1;
                cbxAboRevueRayon.SelectedIndex = -1;
                Revue revue = lesAboRevue.Find(x => x.Id.Equals(txbAboRevueNumRecherche.Text));
                if(revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirListeAbo(revues);
                }
                else
                {
                    MessageBox.Show("Numero introuvable");
                    RemplirListeAboComplete();
                }
            }
            else
            {
                RemplirListeAboComplete();
            }
        }

        /// <summary>
        /// Recherche et  affiche la revue qui match avec la saisie
        /// cette proicedure s'applique a chaque ajout ou suppression de caractere.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbAboRevueTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbAboRevueTitreRecherche.Text.Equals(""))
            {
                cbxAboRevueGenre.SelectedIndex = -1;
                cbxAboRevuePublic.SelectedIndex = -1;
                cbxAboRevueRayon.SelectedIndex = -1;
                txbAboRevueNumRecherche.Text = " ";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesAboRevue.FindAll(x => x.Titre.ToLower().Contains(txbAboRevueTitreRecherche.Text.ToLower()));
                RemplirListeAbo(lesRevuesParTitre);
            }
            else
            {
               if(cbxAboRevueGenre.SelectedIndex < 0 && cbxAboRevuePublic.SelectedIndex < 0 && cbxAboRevueRayon.SelectedIndex < 0 && 
                    txbAboRevueNumRecherche.Text.Equals(" "))
                {
                    RemplirListeAboComplete();
                }
            }
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxAboRevueGenre_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cbxAboRevueGenre.SelectedIndex >= 0)
            {
                txbAboRevueTitreRecherche.Text = "";
                txbAboRevueNumRecherche.Text = "";
                dgvListeAbo.ClearSelection();
                Genre genre = (Genre)cbxAboRevueGenre.SelectedItem;
                cbxAboRevuePublic.SelectedIndex = -1;
                cbxAboRevueRayon.SelectedIndex = -1;
                List<Revue> lesRevues = lesAboRevue.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirListeAbo(lesRevues);
            }
        }

        /// <summary>
        /// filtre sur le public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxAboRevuePublic_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboRevuePublic.SelectedIndex >= 0)
            {
                txbAboRevueTitreRecherche.Text = "";
                txbAboRevueNumRecherche.Text = "";
                dgvListeAbo.ClearSelection();
                Public lePublic = (Public)cbxAboRevuePublic.SelectedItem;
                cbxAboRevueRayon.SelectedIndex = -1;
                cbxAboRevueGenre.SelectedIndex = -1;
                List<Revue> lesRevues = lesAboRevue.FindAll(x => x.Genre.Equals(lePublic.Libelle));
                RemplirListeAbo(lesRevues);
            }
        }
         /// <summary>
         /// filtre sur le rayon
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void cbxAboRevueRayon_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxAboRevueRayon.SelectedIndex >= 0)
            {
                txbAboRevueTitreRecherche.Text = "";
                txbAboRevueNumRecherche.Text = "";
                dgvListeAbo.ClearSelection();
                Rayon rayon = (Rayon)cbxAboRevueRayon.SelectedItem;
                cbxAboRevuePublic.SelectedIndex = -1;
                cbxAboRevueGenre.SelectedIndex = -1;
                List<Revue> lesRevues = lesAboRevue.FindAll(x => x.Genre.Equals(rayon.Libelle));
                RemplirListeAbo(lesRevues);
            }
        }


        /// <summary>
        /// Recupere et affiche les abonnements d'une revue
        /// </summary>
        /// <param name="revue"></param>
        private void AfficherInfoAbo(Revue revue)
        {
            string idRevue = revue.Id;
            VideAboCommandeInfo();
            LesAbonnements = controller.GetAbonnements(idRevue);
            grpGestionAbo.Text = revue.Titre;
            if(LesAbonnements.Count == 0)
            {
                VideAboCommandeInfo();
            }
            RemplirListeAboCommande(LesAbonnements);
        }
        /// <summary>
        /// Filtre les revue dont l'abonnement se termine dans moins de 30 jours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAboRevueFilter_Click(object sender, EventArgs e)
        {
            if (filtre)
            {
                lesAboRevue = controller.GetAllRevues();
                BtnAboRevueFilter.Text = "Filter";
                filtre = false;
            }
            else
            {
                List<Revue> listeTrie = new List<Revue>();
                foreach (Revue revue in lesAboRevue)
                {
                    List<Abonnement> abonnements = controller.GetAbonnements(revue.Id);
                    abonnements = abonnements.FindAll(x => (x.DateFinAbonnement <= DateTime.Now.AddMonths(1))
                    && (x.DateFinAbonnement >= DateTime.Now));
                    if(abonnements.Count > 0)
                    {
                        listeTrie.Add(revue);
                    }
                        
                }
                lesAboRevue = listeTrie;
                BtnAboRevueFilter.Text = "X";
                filtre = true;
            }
            RemplirListeAboComplete();
        }
        
        /// <summary>
        /// filtre les abonnements qui se termine dans moins de 30 jours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAboGestionFilter_Click(object sender, EventArgs e)
        {
            LesAbonnements = LesAbonnements.FindAll(x => (x.DateFinAbonnement <= DateTime.Now.AddMonths(1))
            && (x.DateFinAbonnement >= DateTime.Now));
            if (LesAbonnements.Count == 0)
            {
                VideAboCommandeInfo();
                RemplirListeAboCommande(LesAbonnements);
            }
        }
        /// <summary>
        /// Sur le clic d'annulation affichage de la liste complete des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulGenreRevue_Click(object sender, EventArgs e)
        {
            RemplirListeAboComplete();
        }
        /// <summary>
        /// Sur le clic d'annulation affichage de la liste complete des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnnulRayonRevue_Click(object sender, EventArgs e)
        {
            RemplirListeAboComplete();
        }
        /// <summary>
        /// Sur le clic d'annulation affichage de la liste complete des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnnulPublicRevue_Click(object sender, EventArgs e)
        {
            RemplirListeAboComplete();
        }

        /// <summary>
        /// Affichage de la liste complete des revues
        /// et annulation de tous les filtres de recherches
        /// </summary>
        private void RemplirListeAboComplete()
        {
            RemplirListeAbo(lesAboRevue);
            VideAboZones();
        }

        /// <summary>
        /// Vide les zones de recherches et les filtres
        /// </summary>
        private void VideAboZones()
        {
            cbxAboRevueGenre.SelectedIndex = -1;
            cbxAboRevuePublic.SelectedIndex = -1;
            cbxAboRevueRayon.SelectedIndex = -1;
            txbAboRevueNumRecherche.Text = "";
            txbAboRevueTitreRecherche.Text = "";
            grpGestionAbo.Text = "";
        }

        /// <summary>
        /// vide les zones d'affichage d'une commande
        /// </summary>
        private void VideAboCommandeInfo()
        {
            txbAboNumCommande.Text = "";
            dtpAboStart.Value = DateTime.Now.Date;
            txbAboCommandeMontant.Text = "";
            dtpAboEnd.Value = DateTime.Now.Date.AddMonths(6);
        }

        /// <summary>
        /// Applique des droits sur l'interface selon le contexte
        /// </summary>
        /// <param name="modif"></param>
        private void AbonnementEnCoursDeModif(bool modif)
        {
            btnAjouterAboRevue.Enabled = !modif;
            BtnModifierAboRevue.Enabled = !modif;
            BtnSupprimerAboRevue.Enabled = !modif;
            BtnValiderChoixAbo.Enabled = modif;
            BtnAnnulChoixAbo.Enabled = modif;
            btnAboGestionFilter.Enabled = !modif;
            txbAboNumCommande.ReadOnly = true;
            dtpAboStart.Enabled = modif;
            txbAboNumRevue.ReadOnly = modif;
            txbAboCommandeMontant.ReadOnly = !modif;
            dtpAboEnd.Enabled = modif;
            dgvListeAbo.Enabled = !modif;
            dgvListeAboCommande.Enabled = !modif;
            cbxAboRevueGenre.Enabled = !modif;
            cbxAboRevuePublic.Enabled = !modif;
            cbxAboRevueRayon.Enabled = !modif;
            btnRevueAboRecherche.Enabled = !modif;
            txbAboRevueTitreRecherche.Enabled = !modif;
            btnAnnulGenreRevue.Enabled = !modif;
            btnAnnulRayonRevue.Enabled = !modif;
            BtnAnnulPublicRevue.Enabled = !modif;
            ajouterBool = false;

        }

        /// <summary>
        /// Affiche les detail d'une commande
        /// </summary>
        /// <param name="labonnement"></param>
        private void AfficheAboCommandeInfo(Abonnement labonnement)
        {
            txbAboNumCommande.Text = labonnement.Id;
            txbAboNumRevue.Text = labonnement.IdRevue;
            dtpAboStart.Value = labonnement.DateCommande;
            dtpAboEnd.Value = labonnement.DateFinAbonnement;
            txbAboCommandeMontant.Text = labonnement.Montant.ToString();
        }

        /// <summary>
        /// Affichage des informations d'une sélectionné
        /// </summary>
        /// <param name="revue">le dvd</param>
        private void AfficheAboRevueInfos(Revue revue)
        {
            txbInfoNumRevue.Text = revue.Id;
            txbInfoTitreRevue.Text = revue.Titre;
            txbInfoPeriodicite.Text = revue.Periodicite;
            txbInfoDateDispo.Text = revue.DelaiMiseADispo.ToString();
            cbxInfoRevueGenre.SelectedIndex = cbxInfoRevueGenre.FindString(revue.Genre);
            cbxInfoRevuePublic.SelectedIndex = cbxInfoRevuePublic.FindString(revue.Public);
            cbxInfoRevueRayon.SelectedIndex = cbxInfoRevueRayon.FindString(revue.Rayon);
            
            
        }

        /// <summary>
        /// Demarre la procedure d'ajout d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAjouterAboRevue_Click(object sender, EventArgs e)
        {
            AbonnementEnCoursDeModif(true);
            ajouterBool = true;
            string id = plusUnIdString(controller.GetNbCommandeMax());
            if (id == "1")
                id = "00001";
            VideAboCommandeInfo();
            dtpAboStart.Value = DateTime.Now;
            dtpAboEnd.Value = DateTime.Now.AddMonths(2);
            txbAboNumCommande.Text = id;

        }
        /// <summary>
        /// demarre la procedure de modification de commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnModifierAboRevue_Click(object sender, EventArgs e)
        {
            if(dgvListeAboCommande.CurrentCell != null && txbAboNumCommande.Text != "")
            {
                AbonnementEnCoursDeModif(true);
            }
            else
            {
                MessageBox.Show("Aucune revue selectionnée");
            }
        }

        /// <summary>
        /// renvoie vrai si un exemplaire a ete recu pendant la periode de l'abonnement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public bool verriExemplaireAbo(Abonnement abonnement)
        {
            List<Exemplaire> lesExemplaires = controller.GetExemplairesRevue(abonnement.IdRevue);
            return lesExemplaires.FindAll(x => (x.DateAchat >= abonnement.DateCommande) && (x.DateAchat <= abonnement.DateCommande)).Count > 0;
        }

        /// <summary>
        /// Supprime la revue sleectionnée si verriExemplaireAbo retourne false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSupprimerAboRevue_Click(object sender, EventArgs e)
        {
            Abonnement abonnement = (Abonnement)bdgListeAboCommande[bdgListeAboCommande.Position];
            if(dgvListeAboCommande.CurrentCell != null && txbAboNumCommande.Text != "")
            {
                if (verriExemplaireAbo(abonnement))
                {
                    MessageBox.Show("Une revue a ete livrée le temps de cet abonnement, il ne peut pas etre supprimé");

                }
                else if (MessageBox.Show($"Etes vous sur de vouloir supprimer la commande n° {abonnement.Id}  concernant ? "
                    + lesAboRevue.Find(x => x.Id == abonnement.IdRevue).Titre + "?", "validation suppresion", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (controller.SupprimerAbonnement(abonnement))
                    {
                        try
                        {
                            Revue Revue = (Revue)bdgListeAbo.List[bdgListeAbo.Position];
                            AfficherInfoAbo(Revue);
                            txbAboNumRevue.Text = Revue.Id;

                        }
                        catch
                        {
                            VideAboZones();
                        }
                    }
                    else
                    {
                        MessageBox.Show("erreur");
                    }
                }
            }
            else
            {
                MessageBox.Show("Veuillez selectionner un abonnement.");
            }
        }
        /// <summary>
        /// Valide la modification de l'ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnValiderChoixAbo_Click(object sender, EventArgs e)
        {
            bool valide;
            if (MessageBox.Show("Etes vous sur ?", "oui ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string id = txbAboNumCommande.Text;
                float montant = 0;
                float? b = null;
                try
                {
                    montant = float.Parse(txbAboCommandeMontant.Text);
                    b = montant;
                }
                catch
                {
                    MessageBox.Show("Le montant doit etre un chiffre a virgule.");
                }
                DateTime dateDeCommande = dtpAboStart.Value;
                DateTime dateDeFinCommande = dtpAboEnd.Value;
                string idRevue = txbAboNumRevue.Text;
                if (b != null && dateDeCommande <= dateDeFinCommande && idRevue != null)
                {
                    Abonnement abonnement = new Abonnement(id, dateDeCommande, montant, dateDeFinCommande, idRevue);
                    if (!ajouterBool)
                        valide = controller.ModifierAbonnement(abonnement);
                    else
                    {
                        valide = controller.CreerAbonnement(abonnement);
                    }
                    if (valide)
                    {
                        AbonnementEnCoursDeModif(false);
                        lesAboRevue = controller.GetAllRevues();
                        RemplirListeAboComplete();
                    }
                    else
                    {
                        MessageBox.Show("Erreur");
                    }
                }
                else
                {
                    MessageBox.Show("Erreur");
                }
            }
        }
        /// <summary>
        /// Annule les modifaction/ajout en cours
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAnnulChoixAbo_Click(object sender, EventArgs e)
        {
            ajouterBool = false;
            AbonnementEnCoursDeModif(false);
            try
            {
                Abonnement abonnement = (Abonnement)bdgListeAboCommande[bdgListeAboCommande.Position];
                AfficheAboCommandeInfo(abonnement);
            }
            catch
            {
                VideAboCommandeInfo();
            }
        }

        
        /// <summary>
        /// Sur la selection d'une ligne ou cellule dans le grid affiche les abonnements et les informations aux revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeAbo_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvListeAbo.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgListeAbo.List[bdgListeAbo.Position];
                    AfficherInfoAbo(revue);
                    txbAboNumRevue.Text = revue.Id;
                }
                catch
                {
                    VideAboZones();
                }
            }
            else
            {
                txbInfoNumRevue.Text = "";
                VideAboZones();
            }
            if(dgvListeAbo.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgListeAbo.List[bdgListeAbo.Position];
                    AfficheAboRevueInfos(revue);
                }
                catch
                {
                    VideAboZones();
                }
            }


        }
        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeAbo_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideAboZones();
            string titreColonne = dgvListeAbo.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesAboRevue.OrderBy(x => x.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesAboRevue.OrderBy(x => x.Titre).ToList();
                    break;
                case "Periodicité":
                    sortedList = lesAboRevue.OrderBy(x => x.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesAboRevue.OrderBy(x => x.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesAboRevue.OrderBy(x => x.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesAboRevue.OrderBy(x => x.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesAboRevue.OrderBy(x => x.Rayon).ToList();
                    break;
            }
            RemplirListeAbo(sortedList); 
        }
        /// <summary>
        /// Sur la selection d'une ligne ou d'une commande affiche les detail d'une commande
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeAboCommande_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvListeAboCommande.CurrentCell != null)
            {
                try
                {
                    Abonnement abonnement = (Abonnement)bdgListeAboCommande[bdgListeAboCommande.Position];
                    AfficheAboCommandeInfo(abonnement);
                }
                catch
                {
                    VideAboCommandeInfo();
                }
            }
            else
            {
                VideAboCommandeInfo();
            }
        }
        /// <summary>
        /// tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvListeAboCommande_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if(LesAbonnements.Count > 0 && dgvListeAboCommande != null)
            {
                VideAboCommandeInfo();
                string titreColonne = dgvListeAboCommande.Columns[e.ColumnIndex].HeaderText;
                List<Abonnement> sortedList = new List<Abonnement>();
                switch (titreColonne)
                {
                    case "Id":
                        sortedList = LesAbonnements.OrderBy(x => x.Id).ToList();
                        break;
                    case "DateCommande":
                        sortedList = LesAbonnements.OrderBy(x => x.DateCommande).ToList();
                        break;
                    case "Montant":
                        sortedList = LesAbonnements.OrderBy(x => x.Montant).ToList();
                        break;
                    case "DateFinAbonnement":
                        sortedList = LesAbonnements.OrderBy(x => x.DateFinAbonnement).ToList();
                        break;
                }
                RemplirListeAboCommande(sortedList);
            }
        }


        #endregion


    }
}

