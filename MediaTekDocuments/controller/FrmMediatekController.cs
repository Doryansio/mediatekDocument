using System.Collections.Generic;
using MediaTekDocuments.model;
using MediaTekDocuments.dal;
using Newtonsoft.Json;

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

        #endregion

        #region Utilitaire

        /// <summary>
        /// Permets de gérer les demandes de requêtes HTML (post update delete) concernant
        /// un document
        /// </summary>
        /// <param name="id"></param>
        /// <param name="titre"></param>
        /// <param name="image"></param>
        /// <param name="IdRayon"></param>
        /// <param name="IdPublic"></param>
        /// <param name="IdGenre"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitDocument(string id, string titre, string image, string IdRayon, string IdPublic, string IdGenre, string verbose)
        {
            Dictionary<string, string> dictDocument = new Dictionary<string, string>();
            dictDocument.Add("id", id);
            dictDocument.Add("titre", titre);
            dictDocument.Add("image", image);
            dictDocument.Add("idRayon", IdRayon);
            dictDocument.Add("idPublic", IdPublic);
            dictDocument.Add("idGenre", IdGenre);
            if (verbose == "post")
                return access.CreerEntite("document", JsonConvert.SerializeObject(dictDocument));
            if (verbose == "update")
                return access.ModifierEntite("document", id, JsonConvert.SerializeObject(dictDocument));
            if (verbose == "delete")
                return access.SupprimerEntite("document", JsonConvert.SerializeObject(dictDocument));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes HTML (post update delete) concernant
        /// un livre_dvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitDvdLivre(string id, string verbose)
        {
            Dictionary<string, string> dictLivreDvd = new Dictionary<string, string>();
            dictLivreDvd.Add("id", id);
            if (verbose == "post")
                return access.CreerEntite("livres_dvd", JsonConvert.SerializeObject(dictLivreDvd));
            if (verbose == "delete")
                return access.SupprimerEntite("livres_dvd", JsonConvert.SerializeObject(dictLivreDvd));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes HTML (post update delete) concernant
        /// un livre
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isbn"></param>
        /// <param name="auteur"></param>
        /// <param name="collection"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitLivre(string id, string isbn, string auteur, string collection, string verbose)
        {
            Dictionary<string, string> unLivre = new Dictionary<string, string>();
            unLivre.Add("id", id);
            unLivre.Add("ISBN", isbn);
            unLivre.Add("auteur", auteur);
            unLivre.Add("collection", collection);
            if (verbose == "post")
                return access.CreerEntite("livre", JsonConvert.SerializeObject(unLivre));
            if (verbose == "update")
                return access.ModifierEntite("livre", id, JsonConvert.SerializeObject(unLivre));
            if (verbose == "delete")
                return access.SupprimerEntite("livre", JsonConvert.SerializeObject(unLivre));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// un Dvd
        /// </summary>
        /// <param name="id"></param>
        /// <param name="synopsis"></param>
        /// <param name="realisateur"></param>
        /// <param name="duree"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitDvd(string id, string synopsis, string realisateur, int duree, string verbose)
        {
            Dictionary<string, object> unDvd = new Dictionary<string, object>();
            unDvd.Add("id", id);
            unDvd.Add("synopsis", synopsis);
            unDvd.Add("realisateur", realisateur);
            unDvd.Add("duree", duree);
            if (verbose == "post")
                return access.CreerEntite("dvd", JsonConvert.SerializeObject(unDvd));
            if (verbose == "update")
                return access.ModifierEntite("dvd", id, JsonConvert.SerializeObject(unDvd));
            if (verbose == "delete")
                return access.SupprimerEntite("dvd", JsonConvert.SerializeObject(unDvd));
            return false;
        }

        /// <summary>
        /// Permets de gérer les demandes de requêtes post update delete concernant
        /// une revue
        /// </summary>
        /// <param name="id"></param>
        /// <param name="periodicite"></param>
        /// <param name="delaiMiseADispo"></param>
        /// <param name="verbose"></param>
        /// <returns></returns>
        public bool utilitRevue(string id, string periodicite, int delaiMiseADispo, string verbose)
        {
            Dictionary<string, object> uneRevue = new Dictionary<string, object>();
            uneRevue.Add("id", id);
            uneRevue.Add("periodicite", periodicite);
            uneRevue.Add("delaiMiseADispo", delaiMiseADispo);
            if (verbose == "post")
                return access.CreerEntite("revue", JsonConvert.SerializeObject(uneRevue));
            if (verbose == "update")
                return access.ModifierEntite("revue", id, JsonConvert.SerializeObject(uneRevue));
            if (verbose == "delete")
                return access.SupprimerEntite("revue", JsonConvert.SerializeObject(uneRevue));
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
            bool valide = true;
            if (!utilitDocument(livre.Id, livre.Titre, livre.Image, livre.IdRayon, livre.IdPublic, livre.IdGenre, "post"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDvdLivre(livre.Id, "post"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection, "post"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// modifie un livre dans la bddd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierLivre(Livre livre)
        {
            bool valide= true;
            if (!utilitDocument(livre.Id, livre.Titre, livre.Image, livre.IdRayon, livre.IdPublic, livre.IdGenre, "update"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection, "update"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// supprime un livre dans la bdd
        /// </summary>
        /// <param name="livre"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerLivre(Livre livre)
        {
            bool valide = true;
            if (!utilitLivre(livre.Id, livre.Isbn, livre.Auteur, livre.Collection, "delete"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDvdLivre(livre.Id, "delete"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDocument(livre.Id, livre.Titre, livre.Image, livre.IdRayon, livre.IdPublic, livre.IdGenre, "delete"))
                valide = false;

            return valide;
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
        /// creer un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool CreerDvd(Dvd dvd)
        {
            bool valide = true;
            if (!utilitDocument(dvd.Id, dvd.Titre, dvd.Image, dvd.IdRayon, dvd.IdPublic, dvd.IdGenre, "post"))
                valide = false;
            if (!utilitDvdLivre(dvd.Id, "post"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree, "post"))
                valide = false;

            return valide;
        }


        /// <summary>
        /// modifie un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierDvd(Dvd dvd)
        {
            bool valide = true;
            if (!utilitDocument(dvd.Id, dvd.Titre, dvd.Image, dvd.IdRayon, dvd.IdPublic, dvd.IdGenre, "update"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree, "update"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// supprime un dvd dans la bdd
        /// </summary>
        /// <param name="dvd"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerDvd(Dvd dvd)
        {
            bool valide = true;
            if (!utilitDvd(dvd.Id, dvd.Synopsis, dvd.Realisateur, dvd.Duree, "delete"))
                valide = false;
            if (!utilitDvdLivre(dvd.Id, "delete"))
                valide = false;
            //Thread.Sleep(50) a garder pour passage en ligne de l'api ? (lag);
            if (!utilitDocument(dvd.Id, dvd.Titre, dvd.Image, dvd.IdRayon, dvd.IdPublic, dvd.IdGenre, "delete"))
                valide = false;

            return valide;
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
            bool valide = true;
            if (!utilitDocument(revue.Id, revue.Titre, revue.Image, revue.IdRayon, revue.IdPublic, revue.IdGenre, "post"))
                valide = false;
            
            if (!utilitRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo, "post"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// modifie une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool ModifierRevue(Revue revue)
        {
            bool valide = true;
            if (!utilitDocument(revue.Id, revue.Titre, revue.Image, revue.IdRayon, revue.IdPublic, revue.IdGenre, "update"))
                valide = false;
            
            if (!utilitRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo, "update"))
                valide = false;

            return valide;
        }

        /// <summary>
        /// supprime une revue dans la bdd
        /// </summary>
        /// <param name="revue"></param>
        /// <returns>true si oppration valide</returns>
        public bool SupprimerRevue(Revue revue)
        {
            bool valide = true;
            if (!utilitRevue(revue.Id, revue.Periodicite, revue.DelaiMiseADispo, "delete"))
                valide = false;
            if (!utilitDocument(revue.Id, revue.Titre, revue.Image, revue.IdRayon, revue.IdPublic, revue.IdGenre, "delete"))
                valide = false;

            return valide;
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
        
    }
}
