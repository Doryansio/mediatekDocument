Feature: TestLivres



Scenario: Test Recherche par ID
	Given Je saisie la valeur "0001" dans le champ de recherche de l'ID
	When je clique sur le bouton de recherche
	Then le datagridview affiche le livre possédant l'id "0001"