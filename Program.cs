using System;
using System.Collections.Generic;

namespace RPGConsole
{
    interface IPokemonProperties
    {
        string Name { get; }
        string Type { get; }
        int Level { get; }
        int MaxHp { get; }
        int Hp { get; set; }
        int MaxXp { get; }
        int Xp { get; set; }
        int Attack { get; set; }
        int Defense { get; set; }
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
        public string Type { get; private set; }
        public int Level { get; private set; }
        public int MaxHp { get; private set; }
        public int Hp { get; set; }
        public int MaxXp { get; private set; }
        public int Xp { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        private int BaseDefense;
        public bool CanEvolve { get; private set; }
        public bool IsAlive { get { return Hp > 0;} }
        private string EvolutionName;
        private static Random rnd = new Random();

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
            this.BaseDefense = defense;
            this.CanEvolve = canEvolve;
            this.EvolutionName = evolutionName;
        }

        public void AttackTarget(Pokemon enemy)
        {
            int damage = Attack - enemy.Defense + rnd.Next(1,5);
            if(damage < 0) damage = 0;
            Console.WriteLine("\n" + Name + " attaque " + enemy.Name + " et inflige " + damage + " dégâts !");
            enemy.ReceiveDamage(damage);
            enemy.ResetDefense();
        }

        public void Defend()
        {
            Console.WriteLine("\n" + Name + " se met en posture défensive !");
            Defense += 2;
        }

        public void ResetDefense()
        {
            Defense = BaseDefense;
        }

        public void ReceiveDamage(int damage)
        {
            Hp -= damage;
            if (Hp < 0) Hp = 0;
            Console.WriteLine(Name + " a maintenant " + Hp + "/" + MaxHp + " HP.");
            if (Hp == 0)
            {
                Console.WriteLine("\n" + Name + " est KO !");
            }
        }

        public void GainXp(int amount)
        {
            Xp += amount;
            Console.WriteLine("\n" + Name + " gagne " + amount + " XP !");
            while (Xp >= MaxXp)
            {
                LevelUp();
            }
        }

        public void LevelUp()
        {
            Xp -= MaxXp;
            Level++;
            MaxHp += 5;
            Attack += 2;
            Defense += 1;
            BaseDefense = Defense;
            Hp = MaxHp;

            MaxXp = Level * 10;

            Console.WriteLine("\n" + Name + " monte au niveau " + Level + " !");
            if (CanEvolve && Level >= 5)
            {
                Evolve();
            }
        }

        public void Evolve()
        {
            if (EvolutionName == null) return;
            Console.WriteLine("\n" + Name + " évolue en " + EvolutionName + " !");
            Name = EvolutionName;
            MaxHp += 5 * rnd.Next(1,6);
            Attack += 3 * rnd.Next(1,6);
            Defense += 2 * rnd.Next(1,6);
            BaseDefense = Defense;
            Hp = MaxHp;
            CanEvolve = false;
        }

        public override string ToString()
        {
            return Name + ": | Lvl " + Level + " | HP : " + Hp + "/" + MaxHp + " ATK : " + Attack + " DEF : " + Defense + " XP : " + Xp + "/" + MaxXp;
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
        public abstract bool Use(Trainer trainer, Pokemon target);
    }

    class Pokeball : Item
    {
        public Pokeball()
        {
            Name = "Pokéball";
        }

        public override bool Use(Trainer trainer, Pokemon target)
        {
            if (!target.IsAlive)
            {
                Console.WriteLine("Impossible de capturer un Pokémon KO !");
                return false;
            }
            if (target.Hp > target.MaxHp / 2)
            {
                Console.WriteLine("Impossible de capturer " + target.Name + ". Il n'est pas suffisamment affaibli !");
                return false;
            }

            Random rnd = new Random();
            int chance = rnd.Next(0, target.MaxHp + 1);

            if (target.Hp < chance)
            {
                Console.WriteLine("Félicitations ! " + target.Name + " a été capturé !");
                trainer.AddPokemon(target);
                return true;
            }
            else
            {
                Console.WriteLine(target.Name + " échappe à la capture.");
                return false;
            }
        }
    }

    class Potion : Item
    {
        private int healAmount;
        public Potion(int amount)
        {
            Name = "Potion (" + amount + " HP)";
            healAmount = amount;
        }

        public override bool Use(Trainer trainer, Pokemon target)
        {
            if (!target.IsAlive)
            {
                Console.WriteLine("Impossible d'utiliser une potion sur un Pokémon KO !");
                return false;
            }

            target.Hp += healAmount;
            if (target.Hp > target.MaxHp) target.Hp = target.MaxHp;
            Console.WriteLine(target.Name + " récupère " + healAmount + " HP ! HP actuel : " + target.Hp + "/" + target.MaxHp);
            return true;
        }
    }

    class BoostAttack : Item
    {
        private int boost;
        public BoostAttack(int amount)
        {
            Name = "Potion d'attaque +" + amount;
            boost = amount;
        }

        public override bool Use(Trainer trainer, Pokemon target)
        {
            target.Attack += boost;
            Console.WriteLine(target.Name + " gagne +" + boost + " en attaque pour ce combat !");
            return true;
        }
    }

    class BoostDefense : Item
    {
        private int boost;
        public BoostDefense(int amount)
        {
            Name = "Potion de défense +" + amount;
            boost = amount;
        }

        public override bool Use(Trainer trainer, Pokemon target)
        {
            target.Defense += boost;
            Console.WriteLine(target.Name + " gagne +" + boost + " en défense pour ce combat !");
            return true;
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

        public string Team
        {
            get
            {
                string result = "";
                foreach (Pokemon p in TeamList)
                {
                    result += p.Name + " ";
                }
                return result.Trim();
            }
        }

        public Trainer(string name, int age)
        {
            Name = name;
            Age = age;
            Level = 1;
            TeamList = new List<Pokemon>();
            Inventory = new List<Item>();
        }

        public void AddPokemon(Pokemon p)
        {
            TeamList.Add(p);
        }

        public void ChooseActivePokemon()
        {
            List<Pokemon> alivePokemons = TeamList.FindAll(p => p.IsAlive);
            if (alivePokemons.Count == 0)
            {
                Console.WriteLine("Tous vos Pokémon sont KO !");
                ActivePokemon = null;
                return;
            }

            Console.WriteLine("\nChoisis un Pokémon pour le combat :");
            for (int i = 0; i < alivePokemons.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + alivePokemons[i].Name + " | HP: " + alivePokemons[i].Hp + "/" + alivePokemons[i].MaxHp);
            }

            Console.Write("> ");
            if (int.TryParse(Console.ReadLine(), out int choice))
            {
                choice -= 1;
                if (choice >= 0 && choice < alivePokemons.Count)
                {
                    ActivePokemon = alivePokemons[choice];
                    return;
                }
            }
            ActivePokemon = alivePokemons[0];
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
            List<Item> usableItems = Inventory.FindAll(item => !(item is Pokeball));

            if (usableItems.Count == 0)
            {
                Console.WriteLine("Aucun item utilisable sur vos Pokémon disponibles.");
                return;
            }

            Console.WriteLine("\nInventaire (0 pour annuler) :");
            for (int i = 0; i < usableItems.Count; i++)
            {
                Console.WriteLine((i + 1) + ". " + usableItems[i].Name);
            }

            Console.Write("\nChoisis un item à utiliser : ");
            if (!int.TryParse(Console.ReadLine(), out int choice))
            {
                Console.WriteLine("Entrée invalide.");
                return;
            }

            if (choice == 0)
            {
                Console.WriteLine("Retour au menu de combat.");
                return;
            }

            choice -= 1;

            if (choice >= 0 && choice < usableItems.Count)
            {
                Item item = usableItems[choice];

                Console.WriteLine("\nSur quel Pokémon ?");
                ChooseActivePokemon();
                if (ActivePokemon != null && item.Use(this, ActivePokemon)) // Qu’il y a bien un Pokémon actif ET que l’objet (item) a bien été utilisé avec succès sur ce Pokémon
                {
                    Inventory.Remove(item);
                }
            }
            else
            {
                Console.WriteLine("Item invalide !");
            }
        }

        public void ShowTeam()
        {
            Console.WriteLine("\nÉquipe de " + Name + " :");
            foreach (Pokemon p in TeamList)
            {
                Console.WriteLine(p.Name + " | HP : " + p.Hp + "/" + p.MaxHp);
            }
        }
    }

    class EnemyTrainer : Trainer
    {
        public EnemyTrainer(string name, int age) : base(name, age){}
    }

    static class PokemonFactory
    {
        private static Random rnd = new Random();

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

        public static EnemyPokemon CreateWildEnemy(int playerLevel)
        {
            int type = rnd.Next(0, 10);
            int level = Math.Max(1, playerLevel + rnd.Next(-1, 3));

            switch (type)
            {
                case 0: return new EnemyPokemon("Chenipan", "Insecte", level, 10 + level*2, 3 + level, 1 + level);
                case 1: return new EnemyPokemon("Aspicot", "Insecte", level, 11 + level*2, 4 + level, 2 + level);
                case 2: return new EnemyPokemon("Rattata", "Normal", level, 12 + level*2, 5 + level, 2 + level);
                case 3: return new EnemyPokemon("Piafabec", "Vol", level, 13 + level*2, 6 + level, 3 + level);
                case 4: return new EnemyPokemon("Nosferapti", "Vol/Poison", level, 14 + level*2, 5 + level, 4 + level);
                case 5: return new EnemyPokemon("Magicarpe", "Eau", level, 8 + level*2, 2 + level, 1 + level);
                case 6: return new EnemyPokemon("Miaouss", "Normal", level, 15 + level*2, 6 + level, 3 + level);
                case 7: return new EnemyPokemon("Machoc", "Combat", level, 18 + level*2, 7 + level, 4 + level);
                case 8: return new EnemyPokemon("Psykokwak", "Eau", level, 16 + level*2, 6 + level, 3 + level);
                default: return new EnemyPokemon("Dracaufeu", "Feu/Vol", level, 25 + level*2, 8 + level, 6 + level);
            }
        }
    }

    class Dungeon
    {
        private int levels;
        private Trainer player;
        private Random rnd = new Random();

        public Dungeon(Trainer player, int levels)
        {
            this.player = player;
            this.levels = levels;
        }

        public void Start()
        {
            Console.WriteLine("=== Tu entres dans un donjon de " + levels + " niveaux ===");

            for(int i = 1; i <= levels; i++)
            {
                Console.WriteLine("\n--- Niveau " + i + " ---");
                EnemyTrainer ennemi = new EnemyTrainer("Dresseur Niv " + i, 20); 
                ennemi.AddPokemon(PokemonFactory.CreateWildEnemy(player.Level + i));
                ennemi.ChooseActivePokemon();

                BattleEngineTrainer combat = new BattleEngineTrainer(player, ennemi);
                combat.StartBattle();

                if (player.ActivePokemon == null || !player.ActivePokemon.IsAlive)// Vérifie si le joueur n’a pas de Pokémon actif OU si le Pokémon actif n’est plus en vie
                {
                    return;
                } 
            }

            // Boss
            Console.WriteLine("\n=== Boss du donjon ! ===");
            EnemyTrainer boss = new EnemyTrainer("Boss du donjon", 50); 
            boss.AddPokemon(PokemonFactory.CreateWildEnemy(player.Level + levels + 2));
            boss.ChooseActivePokemon();
            BattleEngineTrainer combatBoss = new BattleEngineTrainer(player, boss);
            combatBoss.StartBattle();
        }
    }

    class BattleEngine
    {
        protected Trainer player;
        protected EnemyPokemon enemy;
        protected Random rnd = new Random();
        public virtual bool CanRun => true;

        public BattleEngine(Trainer player, EnemyPokemon enemy)
        {
            this.player = player;
            this.enemy = enemy;
        }

        public virtual void StartBattle()
        {
            Console.WriteLine("Combat entre " + player.ActivePokemon.Name + " et " + enemy.Name + " !");
            Console.WriteLine();

            while (player.ActivePokemon.IsAlive && enemy.IsAlive)
            {
                Console.WriteLine("Que veux-tu faire ?");
                Console.WriteLine("1. Attaquer");
                Console.WriteLine("2. Défendre");
                Console.WriteLine("3. Utiliser un item");
                Console.WriteLine("4. Lancer une Pokéball");
                if (CanRun)
                {
                    Console.WriteLine("5. Fuir");
                }
                else
                {
                    Console.WriteLine("5. Fuir (impossible contre ce dresseur)");
                }
                Console.Write("> ");
                string choice = Console.ReadLine();

                switch(choice)
                {
                    case "1": player.ActivePokemon.AttackTarget(enemy); break;
                    case "2": player.ActivePokemon.Defend(); break;
                    case "3": player.UseItem(); break;
                    case "4":
                        Item ball = new Pokeball();
                        ball.Use(player, enemy);
                        if (player.TeamList.Contains(enemy)) return;
                        break;
                    case "5":
                        if (!CanRun)
                        {
                            Console.WriteLine("❌ Impossible de fuir un combat contre un dresseur !");
                        }
                        else
                        {
                            int chanceFuite = rnd.Next(0, 100);
                            if (chanceFuite < 60)
                            {
                                Console.WriteLine("💨 Tu prends la fuite !");
                                return;
                            }
                            else
                            {
                                Console.WriteLine("😣 La fuite a échoué !");
                            }
                        }
                        break;
                }

                if (!enemy.IsAlive)
                {
                    Console.WriteLine(player.ActivePokemon.Name + " a vaincu " + enemy.Name + " !");
                    player.ActivePokemon.GainXp(enemy.Level * 5);
                    break;
                }

                if (enemy.IsAlive)
                {
                    int enemyChoice = rnd.Next(1,3);
                    if (enemyChoice == 1) enemy.AttackTarget(player.ActivePokemon);
                    else enemy.Defend();
                }

                if (!player.ActivePokemon.IsAlive) // Vérifie si le Pokémon actif du joueur n’est pas en vie
                {
                    Console.WriteLine(player.ActivePokemon.Name + " est KO ... Vous avez perdu !");
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

    class BattleEngineTrainer : BattleEngine
    {
        private EnemyTrainer enemyTrainer;
        public override bool CanRun => false;

        public BattleEngineTrainer(Trainer player, EnemyTrainer enemyTrainer) : base(player, null)
        {
            this.enemyTrainer = enemyTrainer;
        }

        public override void StartBattle()
        {
            Console.WriteLine("Combat contre le dresseur " + enemyTrainer.Name + " !");
            while (player.ActivePokemon.IsAlive && enemyTrainer.ActivePokemon != null)
            {
                Console.WriteLine("Que veux-tu faire ?");
                Console.WriteLine("1. Attaquer");
                Console.WriteLine("2. Défendre");
                Console.WriteLine("3. Utiliser un item");
                Console.Write("> ");
                string choice = Console.ReadLine();

                switch(choice)
                {
                    case "1": player.ActivePokemon.AttackTarget(enemyTrainer.ActivePokemon); break;
                    case "2": player.ActivePokemon.Defend(); break;
                    case "3": player.UseItem(); break;
                }

                if (!enemyTrainer.ActivePokemon.IsAlive)
                {
                    Pokemon defeated = enemyTrainer.ActivePokemon;
                    Console.WriteLine(defeated.Name + " est KO !");
                    enemyTrainer.TeamList.Remove(defeated);
                    if (enemyTrainer.TeamList.Count > 0) enemyTrainer.ChooseActivePokemon();
                    else break;
                }

                if (enemyTrainer.ActivePokemon != null && enemyTrainer.ActivePokemon.IsAlive)
                {
                    int enemyChoice = rnd.Next(1,3);
                    if (enemyChoice == 1) enemyTrainer.ActivePokemon.AttackTarget(player.ActivePokemon);
                    else enemyTrainer.ActivePokemon.Defend();
                }

                if (!player.ActivePokemon.IsAlive)
                {
                    Console.WriteLine(player.ActivePokemon.Name + " est KO ... Vous avez perdu !");
                    break;
                }

                Console.WriteLine();
                Console.WriteLine("État actuel :");
                Console.WriteLine(player.ActivePokemon);
                if (enemyTrainer.ActivePokemon != null) Console.WriteLine(enemyTrainer.ActivePokemon);
                Console.WriteLine("-------------------------");
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

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Bienvenue dans le RPG Pokémon Textuel ===");
            Console.Write("Entre ton nom de dresseur : ");
            string trainerName = Console.ReadLine();
            Console.Write("Ton age : ");
            int age = int.Parse(Console.ReadLine());

            Trainer player = new Trainer(trainerName, age);
            player.AddItem(new Potion(20));
            player.AddItem(new Potion(20));
            player.AddItem(new Pokeball());
            player.AddItem(new Pokeball());

            Console.WriteLine("\nChoisis ton Pokémon de départ :");
            Console.WriteLine("1. Pikachu ⚡");
            Console.WriteLine("2. Bulbizarre 🌱");
            Console.WriteLine("3. Salamèche 🔥");
            Console.Write("> ");
            string choixStarter = Console.ReadLine();

            Pokemon starter;
            switch(choixStarter)
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

            bool playing = true;
            Random rnd = new Random();
            while (playing)
            {
                Console.WriteLine("\n=== Menu Principal ===");
                Console.WriteLine("1. Voir mon équipe");
                Console.WriteLine("2. Explorer");
                Console.WriteLine("3. Quitter");
                Console.Write("> ");
                string choix = Console.ReadLine();

                switch(choix)
                {
                    case "1": player.ShowTeam(); break;

                    case "2":
                        Console.WriteLine("\nTu explores les environs...");
                        int eventRoll = rnd.Next(0, 100);
                        if (eventRoll < 30)
                        {
                            EnemyPokemon ennemi = PokemonFactory.CreateWildEnemy(player.Level);
                            Console.WriteLine("\n🌿 Un " + ennemi.Name + " sauvage apparaît !");
                            BattleEngine combat = new BattleEngine(player, ennemi);
                            combat.StartBattle();
                        }
                        else if (eventRoll < 50)
                        {
                            EnemyTrainer rival = new EnemyTrainer("Rival", 15);
                            int nbPokemon = rnd.Next(1, 4);
                            for (int i = 0; i < nbPokemon; i++)
                            {
                                rival.AddPokemon(PokemonFactory.CreateWildEnemy(player.Level));
                            }
                            rival.ChooseActivePokemon();
                            Console.WriteLine("\n🔥 Un dresseur adverse apparaît !");
                            BattleEngineTrainer combatTrainer = new BattleEngineTrainer(player, rival);
                            combatTrainer.StartBattle();
                        }
                        else
                        {
                            Console.WriteLine("\n🏰 Tu découvres l’entrée d’un donjon !");
                            Dungeon dungeon = new Dungeon(player, 3);
                            dungeon.Start();
                        }
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
