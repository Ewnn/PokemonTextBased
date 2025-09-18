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
            Console.WriteLine("\n" + Name + " attaque " + enemy.Name + " et inflige " + damage + " dégâts !");
            enemy.ReceiveDamage(damage);
        }
        
        public void Defend() //Permet de se defendre d'une attaque ennemie
        {
            Console.WriteLine("\n" + Name + " se met en posture défensive !");
            Defense += 2;

        }

        public void ReceiveDamage(int damage) // Prendre des degats causer par une attaque adverse + si hp = 0 --> KO
        {
            Hp -= damage;

            if (Hp < 0) // Evite les HP negatifs
            {
                Hp = 0;
            }
            Console.WriteLine( Name + " a maintenant " + Hp + "/" + MaxHp + " HP.");

            if (Hp == 0) // SI HP = 0 alors pokemon est KO
            {
                Console.WriteLine("\n" + Name + " est KO !");
            }
        }

        public void GainXp(int amount) // Permet de gagner des XPs 
        {
            Xp += amount;
            Console.WriteLine("\n" + Name + " gagne " + amount + " XP !");
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
            Console.WriteLine("\n" + Name + " monte au niveau " + Level + " !");
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
            Console.WriteLine("\n" + Name + " évolue en " + EvolutionName + " !");
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

    abstract class Item
    {
        public string Name { get; protected set; }
        public abstract void Use(Trainer trainer, Pokemon target);
    }

    class Pokeball : Item
    {
        public Pokeball()
        {
            Name = "Pokéball";
        }

        public override void Use(Trainer trainer, Pokemon target)
        {
            if (target.Hp > target.MaxHp / 2)
            {
                Console.WriteLine("Impossible de capturer " + target.Name + ". Il n'est pas suffisamment affaibli !");
                return;
            }

            Random rnd = new Random();
            int chance = rnd.Next(0, target.MaxHp + 1);

            if (!target.IsAlive || target.Hp < chance)
            {
                Console.WriteLine(" Félicitations ! " + target.Name + " a été capturé !");
                trainer.AddPokemon(target);
            }
            else
            {
                Console.WriteLine(target.Name + " échappe à la capture.");
            }
        }
    }

    class Potion : Item
    {
        private int healAmount;
        public Potion(int amount = 20)
        {
            Name = "Potion";
            healAmount = amount;
        }

        public override void Use(Trainer trainer, Pokemon target)
        {
            if (!target.IsAlive)
            {
                Console.WriteLine("Impossible d'utiliser une potion sur un Pokémon KO !");
                return;
            }

            target.Hp += healAmount;
            if (target.Hp > target.MaxHp)
            {
                target.Hp = target.MaxHp;
            }
            Console.WriteLine(target.Name + " récupère " + healAmount + " HP ! HP actuel : " + target.Hp + "/" + target.MaxHp);
        }
    }

    class Trainer : ITrainerProperties
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
        public int Level { get; private set; }
        public List<Pokemon> TeamList { get; private set; }
        public List<Item> Inventory { get; private set; }
        public Pokemon ActivePokemon { get; private set; }

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
            this.Inventory = new List<Item>();
        }

        public void AddPokemon(Pokemon p) //ajoute un pokemon a la liste de pokemon du dresseur
        {
            TeamList.Add(p);
        }

        public void ChooseActivePokemon() // permet de selectionner un pokemon pour le combat
        {
            Console.WriteLine("\nChoisis un Pokémon pour le combat :");
            for (int i = 0; i < TeamList.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + TeamList[i].Name + " | HP: " + TeamList[i].Hp + "/" + TeamList[i].MaxHp);
            }
            Console.Write("> ");
            int choice = int.Parse(Console.ReadLine()) - 1;
            if (choice >= 0 && choice < TeamList.Count)
            {
                ActivePokemon = TeamList[choice];
            }
            else if (TeamList.Count > 0)
            {
                ActivePokemon = TeamList[0]; // valeur par défaut
            }
        }
 
        public void AddItem(Item item)
        {
            Inventory.Add(item);
        }

        public void ShowInventory()
        {
            Console.WriteLine("\nInventaire :");
            for (int i = 0; i < Inventory.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + Inventory[i].Name);
            }
        }

        public void UseItem()
        {
            ShowInventory();
            Console.WriteLine("\nChoissis un item à utiliser : ");
            int choice = int.Parse(Console.ReadLine()) - 1;
            if (choice >= 0 && choice < Inventory.Count) // verifi que le choix est supérieur à 0 et inferieur au nombre total d'item dans le sac
            {
                Item item = Inventory[choice];
                Console.WriteLine("\nSur quel Pokémon ?");
                ChooseActivePokemon();
                item.Use(this, ActivePokemon); // this → le dresseur qui utilise l’objet sur le pokement cible qui est le pokemon actif
                Inventory.RemoveAt(choice); // objet supprime de l'inventaire apres usage
            }
            else
            {
                Console.WriteLine("Item invalide !");
            }

        }

        public void ShowTeam() // Affihce les pokemons d'un dresseur
        {
            Console.WriteLine("\nÉquipe de " + Name + " :");
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
                Console.WriteLine("\nQue veux-tu faire ?");
                Console.WriteLine("1. Attaquer");
                Console.WriteLine("2. Défendre");
                Console.WriteLine("3. Utiliser un item");
                Console.WriteLine("4. Lancer une Pokéball");
                Console.Write("> ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        player.ActivePokemon.AttackTarget(enemy);
                        break;
                    case "2":
                        player.ActivePokemon.Defend();
                        break;
                    case "3":
                        player.UseItem();
                        break;
                    case "4":
                        Item ball = new Pokeball();
                        ball.Use(player, enemy);
                        if (player.TeamList.Contains(enemy))
                            return; // fin du combat si capture réussie
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
                Console.WriteLine("-------------------------");

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
        Trainer player = new Trainer(trainerName, age);

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

        player.AddPokemon(starter);
        player.ChooseActivePokemon();

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
                    player.ShowTeam();
                    break;

                case "2":
                    // Rencontre d’un ennemi aléatoire
                    EnemyPokemon ennemi = PokemonFactory.CreateWildEnemy(player.Level);
                    Console.WriteLine("\n🌿 Un " + ennemi.Name + " sauvage apparaît !");
                    BattleEngine combat = new BattleEngine(player, ennemi);
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
