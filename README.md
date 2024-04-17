# MediatekDocuments
Cette application permet de gérer les documents (livres, DVD, revues) d'une médiathèque. Elle a été codée en C# sous Visual Studio 2019. C'est une application de bureau, prévue d'être installée sur plusieurs postes accédant à la même base de données.<br>
L'application exploite une API REST pour accéder à la BDD MySQL. Des explications sont données plus loin, ainsi que le lien de récupération.
## Présentation
cette application etait partiellement codé sur le depot github ci joint https://github.com/CNED-SLAM/MediaTekDocuments <br>
### Ajout de fonctionalités
#### Ajout, Modification et suppression des Entités.
Nous avons inclut la possibilité de gerer les entités dans l'applications
![Capture d'écran 2024-04-17 191805](https://github.com/Doryansio/mediatekDocument/assets/91891883/44e6ba51-372c-4b1f-b1de-b7a1b03a3d97)
Au moyen des methodes POST PUT et DELETE les modifications faites dans l'applications sont transmise a l'Api qui communique avec la base de données.<br>
l'ajout, la modification et la suppression fonctionne aussi sur la gestion des commandes.
![Capture d'écran 2024-04-17 192656](https://github.com/Doryansio/mediatekDocument/assets/91891883/34f06115-2205-48a4-bf45-5c049700ba1d)
<br>
####Authentification
Il est aussi possible desormais de gerer les droit d'acces aux fonctionnalité de l'application selon l'utilisateur qui se connecte. si un uilisateur n'a pas les droits necessaires certaines fonctionnalités ne seront pas accessible.
voici le formulaire d'authetification.
![Capture d'écran 2024-04-17 193601](https://github.com/Doryansio/mediatekDocument/assets/91891883/ce76cbc7-fa33-4174-a528-2d4c4392c4d2)
### Ou recuperer l'application ?
Vous pouvez recuperer l'application sur ce depot. pour recuperer l'API afin de pouvoir la consulter en local vousq pouvez la trouver sur le depot github suivant https://github.com/Doryansio/rest_mediatekdocuments
L'API en ligne est configuré sur l'application
###Comment telecharger l'executable ? 
Une fois l'application recuperer il faudra executer le fichier MediatekDocument.msi pour installer le fichier executable dans le dossier debug de l'application. voici un exemple du Path ou vous pourrez trouver le fichier executable MediaTekDocuments\MediaTekDocuments\MediaTekDocuments\bin\Debug.
