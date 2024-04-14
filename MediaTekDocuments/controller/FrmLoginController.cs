using MediaTekDocuments.dal;
using MediaTekDocuments.model;
using MediaTekDocuments.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaTekDocuments.controller
{
    class FrmLoginController
    {

        ///<summary>
        ///objet d'acces au données
        /// </summary>
        private readonly Access access;

        private Utilisateur utilisateur = null;

        



        ///<summary>
        ///Recuperation de l'instance unique d'access aux données
        /// </summary>
        public FrmLoginController()
        {
            access = Access.GetInstance();
        }


        /// <summary>
        /// Lance la vue principale
        /// </summary>
        private void Init()
        {
             FrmMediatek mediatek = new FrmMediatek(utilisateur);
            mediatek.Show();
        }

        public bool GetLogin(string mail, string password)
        {
            password = "Mediatek" + password;
            string hash = "";
            using(SHA256 sha256Hash = SHA256.Create())
            {
                hash = GetHash(sha256Hash, password);
                MessageBox.Show(hash);
            }           
            utilisateur = access.GetLogin(mail, hash);
            if(utilisateur != null)
            {
                Init();
                return true;
            }
            return false;
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

            var sBuilder = new StringBuilder();

            for(int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
    }
}
