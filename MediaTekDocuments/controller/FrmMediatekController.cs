﻿using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;
using System;
using MediaTekDocuments.view;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {


        
        #region Commun
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

       

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }


      
        
        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

       
        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        /// <summary>
        /// Getter sur les etats
        /// </summary>
        /// <returns></returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// Retourne vrai ou faux si le service de l'utilisateur
        /// est autorisé
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public bool VerifDroitAccueil(Utilisateur utilisateur)
        {
            Console.WriteLine(utilisateur.Nom);
            List<string> services = new List<string> { "compta", "biblio", "accueil" };
            if (services.Contains(utilisateur.Service))
                return true;
            return false;
        }

        /// <summary>
        /// Retourne vrai ou faux si le service de l'utilisateur
        /// est autorisé
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public bool VerifDroitModif(Utilisateur utilisateur)
        {
            Console.WriteLine(utilisateur.Nom);
            List<string> services = new List<string> { "biblio", "accueil" };
            if (services.Contains(utilisateur.Service))
                return true;
            return false;
        }

        /// <summary>
        /// Retourne vrai ou faux si le service de l'utilisateur
        /// est autorisé
        /// </summary>
        /// <param name="utilisateur"></param>
        /// <returns></returns>
        public bool VerifCommande(Utilisateur utilisateur)
        {
            List<string> services = new List<string> { "biblio" };
            if (services.Contains(utilisateur.Service))
                return true;
            return false;
        }

        #endregion

        #region Livre

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }


        /// <summary>
        /// creer un livre dans la BDD
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerLivre(Livre livre)
        {
            return access.CreerEntite("livre", JsonConvert.SerializeObject(livre));
        }

        /// <summary>
        /// modifie un livre dans la bddd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierLivre(Livre livre)
        {
            return access.ModifierEntite("livre", livre.Id, JsonConvert.SerializeObject(livre));
        }

        /// <summary>
        /// supprime un livre dans la bdd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerLivre(Livre livre)
        {
            return access.SupprimerEntite("livre", JsonConvert.SerializeObject(livre));
        }
        #endregion

        #region Dvd

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// creer un dvd dans la BDD
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerDvd(Dvd dvd)
        {
            return access.CreerEntite("dvd", JsonConvert.SerializeObject(dvd));
        }

        /// <summary>
        /// modifie un dvd dans la bddd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierDvd(Dvd dvd)
        {
            return access.ModifierEntite("dvd", dvd.Id, JsonConvert.SerializeObject(dvd));
        }

        /// <summary>
        /// supprime un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerDvd(Dvd dvd)
        {
            return access.SupprimerEntite("livre", JsonConvert.SerializeObject(dvd));
        }

        #endregion

        #region Revue


        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// creer une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerRevue(Revue revue)
        {
            return access.CreerEntite("revue", JsonConvert.SerializeObject(revue));
        }

        /// <summary>
        /// modifie une revue dans la bddd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierRevue(Revue revue)
        {
            return access.ModifierEntite("revue", revue.Id, JsonConvert.SerializeObject(revue));
        }

        /// <summary>
        /// supprime un revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerRevue(Revue revue)
        {
            return access.SupprimerEntite("revue", JsonConvert.SerializeObject(revue));
        }
        #endregion

        /// <summary>
        /// récupère les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocuement">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesRevue(string idDocuement)
        {
            return access.GetExemplairesRevue(idDocuement);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        #region Commandes de livres et Dvd
        /// <summary>
        /// Récupère les commandes d'une livre
        /// </summary>
        /// <param name="idLivre">id du livre concernée</param>
        /// <returns></returns>
        public List<CommandeDocument> GetCommandesLivres(string idLivre)
        {
            return access.GetCommandesLivres(idLivre);
        }

        public List<CommandeDocument> GetCommandesDvd(string idDvd)
        {
            return access.GetCommandesDvd(idDvd);
        }

        /// <summary>
        /// Retourne l'id max des commandes
        /// </summary>
        /// <returns></returns>
        public string GetNbCommandeMax()
        {
            return access.GetMaxIndex("maxcommande");
        }

        /// <summary>
        /// Retourne l'id max des livres
        /// </summary>
        /// <returns></returns>
        public string GetNbLivreMax()
        {
            return access.GetMaxIndex("maxlivre");
        }

        /// <summary>
        /// Retourne l'id max des Dvd
        /// </summary>
        /// <returns></returns>
        public string GetNbDvdMax()
        {
            return access.GetMaxIndex("maxdvd");
        }

        /// <summary>
        /// Retourne l'id max des revues
        /// </summary>
        /// <returns></returns>
        public string GetNbRevueMax()
        {
            return access.GetMaxIndex("maxrevue");
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// une commande de livre ou dvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="nbExemplaire"></param>
        /// <param name="idLivreDvd"></param>
        /// <param name="idSuivi"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool UtilCommandeDocument(string id, DateTime dateCommande, double montant, int nbExemplaire,
            string idLivreDvd, int idSuivi, string etat, string verbose)
        {
            Dictionary<string, object> uneCommandeDocument = new Dictionary<string, object>
            {
                { "Id", id },
                { "DateCommande", dateCommande.ToString("yyyy-MM-dd") },
                { "Montant", montant },
                { "NbExemplaire", nbExemplaire },
                { "IdLivreDvd", idLivreDvd },
                { "IdSuivi", idSuivi },
                { "Etat", etat }
            };

            if (verbose == "post")
                return access.CreerEntite("commandedocument", JsonConvert.SerializeObject(uneCommandeDocument));
            if (verbose == "update")
                return access.ModifierEntite("commandedocument", id, JsonConvert.SerializeObject(uneCommandeDocument));
            if (verbose == "delete")
                return access.SupprimerEntite("commandedocument", JsonConvert.SerializeObject(uneCommandeDocument));
            return false;
        }

        /// <summary>
        /// Creer une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool CreerLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return UtilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                    commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "post");
        }

        /// <summary>
        /// Modifie une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool ModifierLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return UtilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                   commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "update");
        }

        /// <summary>
        /// Supprime une commande livre/Dvd dans la bdd
        /// </summary>
        /// <param name="commandeLivreDvd"></param>
        /// <returns></returns>
        public bool SupprimerLivreDvdCom(CommandeDocument commandeLivreDvd)
        {
            return UtilCommandeDocument(commandeLivreDvd.Id, commandeLivreDvd.DateCommande, commandeLivreDvd.Montant, commandeLivreDvd.NbExemplaire,
                   commandeLivreDvd.IdLivreDvd, commandeLivreDvd.IdSuivi, commandeLivreDvd.Etat, "delete");
        }
        #endregion

        #region abonnements

        /// <summary>
        /// Retourne tous les abonnements d'une revue
        /// </summary>
        /// <param name="idRevue"></param>
        /// <returns></returns>
        public List<Abonnement> GetAbonnements(string idRevue)
        {
            return access.GetAbonnements(idRevue);
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// un abonnement
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dateCommande"></param>
        /// <param name="montant"></param>
        /// <param name="dateFinAbonnement"></param>
        /// <param name="idRevue"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool UtilAbonnement(string id, DateTime dateCommande, double montant, DateTime dateFinAbonnement, string idRevue, string verbose)
        {
            Dictionary<string, object> unAbonnement = new Dictionary<string, object>
            {
                { "Id", id },
                { "DateCommande", dateCommande.ToString("yyyy-MM-dd") },
                { "Montant", montant },
                { "DateFinAbonnement", dateFinAbonnement.ToString("yyyy-MM-dd") },
                { "IdRevue", idRevue }
            };

            if (verbose == "post")
                return access.CreerEntite("abonnement", JsonConvert.SerializeObject(unAbonnement));
            if (verbose == "update")
                return access.ModifierEntite("abonnement", id, JsonConvert.SerializeObject(unAbonnement));
            if (verbose == "delete")
                return access.SupprimerEntite("abonnement", JsonConvert.SerializeObject(unAbonnement));
            return false;
        }

        /// <summary>
        /// Creer un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool CreerAbonnement(Abonnement abonnement)
        {
            return UtilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "post");
        }

        /// <summary>
        /// Modifie un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool ModifierAbonnement(Abonnement abonnement)
        {
            return UtilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "update");
        }

        /// <summary>
        /// Supprime un abonnement dans la bdd
        /// </summary>
        /// <param name="abonnement"></param>
        /// <returns></returns>
        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            return UtilAbonnement(abonnement.Id, abonnement.DateCommande, abonnement.Montant, abonnement.DateFinAbonnement, abonnement.IdRevue, "delete");
        }

        #endregion
    }
}
