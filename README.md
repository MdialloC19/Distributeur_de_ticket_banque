# Distributeur-Ticket-Banque

## Description
Application de gestion des tickets pour file d'attente dans une banque. L'application permet aux clients de prendre un ticket numéroté selon le type d'opération souhaitée (versement, retrait ou information) et affiche leur position dans la file d'attente.

## Auteur
Moussa Diallo DIC3 INFO

## Compilation et Exécution

### Prérequis
- .NET SDK (version 5.0 ou supérieure)

### Comment compiler et exécuter

1. Ouvrez un terminal et naviguez vers le répertoire du projet :
   ```
   cd chemin/vers/Distributeur-Ticket-Banque-main/DistributeurTicket
   ```

2. Compilez et exécutez l'application en une seule commande :
   ```
   dotnet run
   ```

3. Pour générer un exécutable :
   ```
   dotnet build
   ```

### Utilisation
Après avoir exécuté l'application :
1. Sélectionnez "1" pour prendre un ticket
2. Entrez les informations du client (prénom, nom, numéro de compte)
3. Choisissez le type d'opération (versement, retrait, information)
4. Un ticket sera généré avec un numéro unique et la position dans la file d'attente
