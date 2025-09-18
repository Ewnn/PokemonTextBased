using System;

namespace RPGConsole
{
    interface IPokemonProperties
    {
        string Name { get; }
        string Type {get; }
        int Level { get; }
        int MaxHp { get; }
        int Hp { get; set; }
        int MaxXp { get;}
        int Xp { get; set; }
        int Attack { get; }
        int Defense { get; }
        bool CanEvolve { get; }
        bool IsAlive { get; }
    }

    interface ITrainerProperties
    {
        string Name { get; }
        int Age { get; }
        int Level { get; }
        string Team { get; }
    }

    class Pokemon : IPokemonProperties
    {
        public string Name { get; private set; }
        public string Type {get; private set; }
        public int Level { get; private set; }
        public int MaxHp { get; private set; }
        public int Hp { get; set; }
        public int MaxXp { get; private set; }
        public int Xp { get; set; }
        public int Attack { get; private set; }
        public int Defense { get; private set; }
        public bool CanEvolve { get; private set; }
        public bool IsAlive { get { return Hp > 0;} }
        private string EvolutionName;

        public Pokemon(string name, string type, int level, int maxHp, int attack, int defense, bool canEvolve, string evolutionName = null)
        {
            this.Name = name;
            this.Type = type;
            this.Level = level;
            this.MaxHp = maxHp;
            this.Hp = maxHp;
            this.MaxXp = level * 10;
            this.Xp = 0;
            this.Attack = attack;
            this.Defense = defense;
            this.CanEvolve = canEvolve;
            this.EvolutionName = evolutionName;
        }

        public void AttackTarget(Pokemon enemy) // Attaquer un autre pokemon
        {
            int damage = Attack - enemy.Defense + new Random().Next(1,5);
            if(damage < 0) // Evite les degats negatifs
            {
                damage = 0;
            }
            Console.WriteLine(Name + " attaque " + enemy.Name + " et inflige " + damage + " dégâts !");
            enemy.ReceiveDamage(damage);
        }
        
        public void Defend() //Permet de se defendre d'une attaque ennemie
        {
            Console.WriteLine(Name + " se met en posture défensive !");
            Defense += 2;

        }

        public void ReceiveDamage(int damage) // Prendre des degats causer par une attaque adverse + si hp = 0 --> KO
        {
            Hp -= damage;

            if (Hp < 0) // Evite les HP negatifs
            {
                Hp = 0;
            }
            Console.WriteLine(Name + " a maintenant " + Hp + "/" + MaxHp + " HP.");

            if (Hp == 0) // SI HP = 0 alors pokemon est KO
            {
                Console.WriteLine(Name + " est KO !");
            }
        }

        public void GainXp(int amount) // Permet de gagner des XPs 
        {
            Xp += amount;
            Console.WriteLine(Name + " gagne " + amount + " XP !");
            while (Xp >= MaxXp)
            {
                LevelUp();
            }
        }

        public void LevelUp() // Permet de monter les niveaux
        {
            Xp -= MaxXp;
            Level++;
            MaxHp += 5;
            Attack += 2;
            Defense += 1;
            Console.WriteLine(Name + " monte au niveau " + Level + " !");
            if (CanEvolve && Level >= 5) // Niveau d'evolution = 5
            {
                Evolve();
            }
        }

        public void Evolve() // Permet l'evolution a partir d'un certain niveau +  amelioration Max HP / Level / Attack / Defense
        {
            if (EvolutionName == null) // Pokemon sans evolution
            {
                return;
            }

            Random rd = new Random();
            Console.WriteLine(Name + " évolue en " + EvolutionName + " !");
            Name = EvolutionName;
            MaxHp += 5 * rd.Next(1,6);
            Attack += 3 * rd.Next(1,6);
            Defense += 2 * rd.Next(1,6);
            Hp = MaxHp;
            CanEvolve = false;
        }

        public override string ToString()
        {
            
            string texte = Name + " : Lvl " + Level + "" + " HP : " + Hp + "/" + MaxHp + " ATK : " + Attack + " DEF : " + Defense + " XP : " + Xp + "/" + MaxXp;
            return texte; 
        }
    }

    class EnemyPokemon : Pokemon
    {
        public EnemyPokemon(string name, string type, int level, int maxHp, int attack, int defense)
            : base(name, type, level, maxHp, attack, defense, false) { }
    }

    class Trainer : ITrainerProperties
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
        public int Level { get; private set; }
        public List<Pokemon> TeamList { get; private set; }

        public string Team // propriété en lecture seule qui retourne les noms des pokemons d'un dresseur
        {
            get
            {
                string result = "";
                foreach (Pokemon p in TeamList)
                {
                    result += p.Name + " ";
                }
                result = result.Trim();
                return result;
            }
        }

        public Trainer(string name, int age)
        {
            this.Name = name;
            this.Age = age;
            this.Level = 1;
            this.TeamList = new List<Pokemon>();
        }

        public void AddPokemon(Pokemon p) //ajoute un pokemon a la liste de pokemon du dresseur
        {
            TeamList.Add(p);
        }

        public Pokemon ActivePokemon // permet de selectionner un pokemon pour le combat
        {
            get
            {
                if (TeamList.Count > 0)
                {
                    return TeamList[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public void ShowTeam() // Affihce les pokemons d'un dresseur
        {
            Console.WriteLine("Équipe de " + Name + " :");
            foreach (Pokemon p in TeamList)
            {
                Console.WriteLine(p.Name + " | HP : " + p.Hp + "/" + p.MaxHp);
            }
        }
    }

    static class PokemonFactory
    {
        public static Pokemon CreateStarter(string type)
        {
            switch(type.ToLower())
            {
                case "pikachu": return new Pokemon("Pikachu", "Électrique", 1, 20, 6, 2, true, "Raichu");
                case "bulbizarre": return new Pokemon("Bulbizarre", "Plante", 1, 22, 5, 3, true, "Herbizarre");
                case "salamèche": return new Pokemon("Salamèche", "Feu", 1, 21, 4, 7, true, "Reptincel");
                default: return new Pokemon("Rattata", "Normal", 1, 14, 2, 2, false);
            }
        }


        public static EnemyPokemon CreateWildEnemy(int level)
        {
            Random rnd = new Random();
            int type = rnd.Next(0,3);
            switch(type)
            {
                case 0: 
                    return new EnemyPokemon("Chenipan", "Insecte", level, 10 + level*2, 3 + level, 1 + level); 
                case 1: 
                    return new EnemyPokemon("Aspicot", "Insecte", level, 11 + level*2, 4 + level, 2 + level);
                default: 
                    return new EnemyPokemon("Dracaufeu", "Feu/Vol", level, 25 + level*2, 8 + level, 6 + level); 
            }
        }
    }

    static class TrainerFactory
    {
        public static Trainer CreateTrainer(string name, int age)
        {
            Trainer t = new Trainer(name, age);
            t.AddPokemon(PokemonFactory.CreateStarter("pikachu"));
            return t;
        }
    }

    class BattleEngine
    {
        private Trainer player;
        private EnemyPokemon enemy;
        private Random rnd;

        public BattleEngine(Trainer player, EnemyPokemon enemy)
        {
            this.player = player;
            this.enemy = enemy;
            this.rnd = new Random();
        }
        
        public void StartBattle()
        {
            Console.WriteLine("Combat entre " + player.ActivePokemon.Name + " et " + enemy.Name + " !");
            Console.WriteLine();

            while (player.ActivePokemon.IsAlive && enemy.IsAlive)
            {
                // Lorsque c'est le tour du joueur
                Console.WriteLine("Que veux-tu faire ?");
                Console.WriteLine("1. Attaquer");
                Console.WriteLine("2. Défendre");
                Console.Write("> ");
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    player.ActivePokemon.AttackTarget(enemy);
                }
                else if (choice == "2")
                {
                    player.ActivePokemon.Defend();
                }
                // Verification de l'etat de sante de l'ennemi
                if (enemy.IsAlive == false)
                {
                    Console.WriteLine(enemy.Name + " est vaincu !");
                    player.ActivePokemon.GainXp(10);
                    break;
                }

                // Lorsque c'est le tour de l'ennemi
                int enemyChoice = rnd.Next(1,3);
                if (enemyChoice == 1)
                {
                    enemy.AttackTarget(player.ActivePokemon);
                }
                else
                {
                    enemy.Defend();
                }

                // Verification de l'etat de sante du joueur
                if (player.ActivePokemon.IsAlive == false)
                {
                    Console.WriteLine(player.ActivePokemon.Name + " est KO ... Vous avez perdu, vous êtes un looser !");
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("État actuel :");
                Console.WriteLine(player.ActivePokemon);
                Console.WriteLine(enemy);
                Console.WriteLine("--------------------");

            }
        }
    }


class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Bienvenue dans le RPG Pokémon Textuel ===");
        Console.Write("Entre ton nom de dresseur : ");
        string trainerName = Console.ReadLine();
        Console.Write("Ton âge : ");
        int age = int.Parse(Console.ReadLine());

        // Création du joueur
        Trainer joueur = new Trainer(trainerName, age);

        // Choix du starter
        Console.WriteLine("\nChoisis ton Pokémon de départ :");
        Console.WriteLine("1. Pikachu ⚡");
        Console.WriteLine("2. Bulbizarre 🌱");
        Console.WriteLine("3. Salamèche 🔥");
        Console.Write("> ");
        string choixStarter = Console.ReadLine();

        Pokemon starter;
        switch (choixStarter)
        {
            case "1": starter = PokemonFactory.CreateStarter("pikachu"); break;
            case "2": starter = PokemonFactory.CreateStarter("bulbizarre"); break;
            case "3": starter = PokemonFactory.CreateStarter("salamèche"); break;
            default: starter = PokemonFactory.CreateStarter("pikachu"); break;
        }

        joueur.AddPokemon(starter);

        Console.WriteLine("\nTu commences l’aventure avec " + starter.Name + " !");
        Console.WriteLine("--------------------------------");

        // Boucle principale du jeu
        bool playing = true;
        while (playing)
        {
            Console.WriteLine("\n=== Menu Principal ===");
            Console.WriteLine("1. Voir mon équipe");
            Console.WriteLine("2. Explorer (rencontrer un Pokémon sauvage)");
            Console.WriteLine("3. Quitter");
            Console.Write("> ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    joueur.ShowTeam();
                    break;

                case "2":
                    // Rencontre d’un ennemi aléatoire
                    EnemyPokemon ennemi = PokemonFactory.CreateWildEnemy(joueur.Level);
                    Console.WriteLine("\n🌿 Un " + ennemi.Name + " sauvage apparaît !");
                    BattleEngine combat = new BattleEngine(joueur, ennemi);
                    combat.StartBattle();
                    break;

                case "3":
                    Console.WriteLine("Merci d’avoir joué !");
                    playing = false;
                    break;

                default:
                    Console.WriteLine("Choix invalide !");
                    break;
            }
        }
    }
}

}
