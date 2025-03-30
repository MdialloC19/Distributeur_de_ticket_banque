using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DistributeurTicketsBanque
{
    class Program
    {
        private static Dictionary<string, int> compteursTickets = new Dictionary<string, int>();
        private static List<Client> clients = new List<Client>();
        private static string fichierCompteurs = Path.Combine(Path.GetTempPath(), "fnumero.txt");

        static void Main(string[] args)
        {
            Console.WriteLine($"Chemin du fichier des compteurs: {fichierCompteurs}");
            InitialiserCompteurs();
            AfficherMenuPrincipal();
        }

        static void InitialiserCompteurs()
        {
            try
            {
                if (File.Exists(fichierCompteurs))
                {
                    var lignes = File.ReadAllLines(fichierCompteurs);
                    foreach (var ligne in lignes)
                    {
                        var parts = ligne.Split(':');
                        if (parts.Length == 2 && int.TryParse(parts[1], out int valeur))
                        {
                            compteursTickets[parts[0]] = valeur;
                        }
                    }
                }

                // Types de tickets : V (Versement), R (Retrait), I (Information)
                string[] typesTickets = { "V", "R", "I" };
                foreach (var type in typesTickets)
                {
                    if (!compteursTickets.ContainsKey(type))
                    {
                        compteursTickets[type] = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la lecture du fichier: {ex.Message}");
                compteursTickets = new Dictionary<string, int>
                {
                    { "V", 0 }, { "R", 0 }, { "I", 0 }
                };
            }
        }

        static void SauvegarderCompteurs()
        {
            try
            {
                File.WriteAllLines(fichierCompteurs, compteursTickets.Select(kv => $"{kv.Key}:{kv.Value}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur de sauvegarde: {ex.Message}");
            }
        }

        static void AfficherMenuPrincipal()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== DISTRIBUTEUR DE TICKETS ===");
                Console.WriteLine("1. Prendre un ticket");
                Console.WriteLine("2. Afficher les clients");
                Console.WriteLine("3. Quitter");
                Console.Write("Choix (1-3): ");

                switch (Console.ReadLine())
                {
                    case "1":
                        CreerTicket();
                        break;
                    case "2":
                        AfficherListeClients();
                        break;
                    case "3":
                        SauvegarderCompteurs();
                        Console.WriteLine("Au revoir!");
                        return;
                    default:
                        Console.WriteLine("Choix invalide!");
                        System.Threading.Thread.Sleep(1000);
                        break;
                }
            }
        }

        static void CreerTicket()
        {
            while (true)
            {
                Console.Clear();
                var client = new Client();

                while (true)
                {
                    Console.Write("Prénom (ou 'q' pour quitter): ");
                    client.Prenom = Console.ReadLine()?.Trim();
                    
                    if (string.IsNullOrEmpty(client.Prenom))
                    {
                        Console.WriteLine("Le prénom est obligatoire!");
                        continue;
                    }
                    
                    if (client.Prenom.ToLower() == "q") return;
                    break;
                }

                while (true)
                {
                    Console.Write("Nom (ou 'q' pour quitter): ");
                    client.Nom = Console.ReadLine()?.Trim();
                    
                    if (string.IsNullOrEmpty(client.Nom))
                    {
                        Console.WriteLine("Le nom est obligatoire!");
                        continue;
                    }
                    
                    if (client.Nom.ToLower() == "q") return;
                    break;
                }

                while (true)
                {
                    Console.Write("Numéro de compte (ou 'q' pour quitter): ");
                    client.NumeroCompte = Console.ReadLine()?.Trim();
                    
                    if (string.IsNullOrEmpty(client.NumeroCompte))
                    {
                        Console.WriteLine("Le numéro est obligatoire!");
                        continue;
                    }
                    
                    if (client.NumeroCompte.ToLower() == "q") return;
                    break;
                }

                string typeTicket = null;
                while (typeTicket == null)
                {
                    Console.WriteLine("\nType d'opération:");
                    Console.WriteLine("1. Versement (V)");
                    Console.WriteLine("2. Retrait (R)");
                    Console.WriteLine("3. Informations (I)");
                    Console.WriteLine("4. Annuler");
                    Console.Write("Choix (1-4): ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            typeTicket = "V";
                            break;
                        case "2":
                            typeTicket = "R";
                            break;
                        case "3":
                            typeTicket = "I";
                            break;
                        case "4":
                            return;
                        default:
                            Console.WriteLine("Choix invalide!");
                            continue;
                    }
                }

                compteursTickets[typeTicket]++;
                client.NumeroTicket = $"{typeTicket}-{compteursTickets[typeTicket]}";
                clients.Add(client);

                // Calcul du nombre de personnes en attente avant ce client (même type d'opération)
                int enAttente = clients.Count(c => c.NumeroTicket.StartsWith(typeTicket) && 
                                          int.Parse(c.NumeroTicket.Split('-')[1]) < int.Parse(client.NumeroTicket.Split('-')[1]));

                Console.Clear();
                Console.WriteLine("=== TICKET ===");
                Console.WriteLine($"Client: {client.Prenom} {client.Nom}");
                Console.WriteLine($"Compte: {client.NumeroCompte}");
                Console.WriteLine($"\nVotre numéro: {client.NumeroTicket}");
                Console.WriteLine($"Personnes avant vous: {enAttente}");

                SauvegarderCompteurs();

                Console.Write("\nNouveau ticket? (o/n): ");
                if (Console.ReadLine().ToLower() != "o") return;
            }
        }

        static void AfficherListeClients()
        {
            Console.Clear();
            Console.WriteLine("=== CLIENTS ENREGISTRÉS ===");

            if (!clients.Any())
            {
                Console.WriteLine("Aucun client");
            }
            else
            {
                // Regroupement des clients par type d'opération
                var clientsParType = new Dictionary<string, List<Client>>
                {
                    { "V", new List<Client>() },
                    { "R", new List<Client>() },
                    { "I", new List<Client>() }
                };

                foreach (var client in clients)
                {
                    string type = client.NumeroTicket.Split('-')[0];
                    if (clientsParType.ContainsKey(type))
                    {
                        clientsParType[type].Add(client);
                    }
                }

                // Affichage des clients par type d'opération
                string[] typesOperations = { "V", "R", "I" };
                string[] nomsOperations = { "Versement", "Retrait", "Information" };

                for (int i = 0; i < typesOperations.Length; i++)
                {
                    string type = typesOperations[i];
                    string nom = nomsOperations[i];
                    
                    if (clientsParType[type].Any())
                    {
                        Console.WriteLine($"\n--- {nom}s ---");
                        foreach (var c in clientsParType[type].OrderBy(c => int.Parse(c.NumeroTicket.Split('-')[1])))
                        {
                            Console.WriteLine($"{c.NumeroTicket} - {c.Nom}, {c.Prenom} ({c.NumeroCompte})");
                        }
                    }
                }

                Console.WriteLine($"\nTotal: {clients.Count} clients");
                Console.WriteLine($"Versements: {clientsParType["V"].Count}");
                Console.WriteLine($"Retraits: {clientsParType["R"].Count}");
                Console.WriteLine($"Informations: {clientsParType["I"].Count}");
            }

            Console.Write("\nAppuyez sur une touche...");
            Console.ReadKey();
        }
    }

    class Client
    {
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string NumeroCompte { get; set; }
        public string NumeroTicket { get; set; }
    }
}