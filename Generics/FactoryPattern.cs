using System;
using System.Collections.Generic;

namespace FactoryPattern
{
    // Base classes and interfaces
    abstract class GameEntity
    {
        public string Name { get; set; } = "Unknown";
        public int Id { get; set; }
        public abstract void Initialize();
        public virtual void Display()
        {
            Console.WriteLine($"- [{GetType().Name}] {Name} (ID: {Id})");
        }
    }

    interface IConfigurable
    {
        void Configure(Dictionary<string, object> config);
    }

    interface ICombat
    {
        int Health { get; set; }
        int AttackPower { get; set; }
        void Attack(string target);
    }

    // Concrete entity types
    class Warrior : GameEntity, IConfigurable, ICombat
    {
        public int Health { get; set; } = 100;
        public int AttackPower { get; set; } = 50;
        public int Defense { get; set; } = 30;

        public override void Initialize()
        {
            Name = "Warrior";
            Console.WriteLine($"[INIT] {Name} initialized with {Health} HP and {AttackPower} ATK");
        }

        public void Configure(Dictionary<string, object> config)
        {
            if (config.ContainsKey("Health")) Health = (int)config["Health"];
            if (config.ContainsKey("AttackPower")) AttackPower = (int)config["AttackPower"];
            if (config.ContainsKey("Defense")) Defense = (int)config["Defense"];
            Console.WriteLine($"[CONFIG] {Name} configured: HP={Health}, ATK={AttackPower}, DEF={Defense}");
        }

        public void Attack(string target)
        {
            Console.WriteLine($"[ATTACK] {Name} slashes {target} for {AttackPower} damage!");
        }
    }

    class Mage : GameEntity, IConfigurable, ICombat
    {
        public int Health { get; set; } = 70;
        public int AttackPower { get; set; } = 80;
        public int Mana { get; set; } = 100;

        public override void Initialize()
        {
            Name = "Mage";
            Console.WriteLine($"[INIT] {Name} initialized with {Health} HP and {Mana} MP");
        }

        public void Configure(Dictionary<string, object> config)
        {
            if (config.ContainsKey("Health")) Health = (int)config["Health"];
            if (config.ContainsKey("AttackPower")) AttackPower = (int)config["AttackPower"];
            if (config.ContainsKey("Mana")) Mana = (int)config["Mana"];
            Console.WriteLine($"[CONFIG] {Name} configured: HP={Health}, ATK={AttackPower}, MP={Mana}");
        }

        public void Attack(string target)
        {
            Console.WriteLine($"[ATTACK] {Name} casts fireball at {target} for {AttackPower} damage!");
        }
    }

    class Archer : GameEntity, IConfigurable, ICombat
    {
        public int Health { get; set; } = 80;
        public int AttackPower { get; set; } = 60;
        public int Range { get; set; } = 10;

        public override void Initialize()
        {
            Name = "Archer";
            Console.WriteLine($"[INIT] {Name} initialized with {Health} HP and range {Range}");
        }

        public void Configure(Dictionary<string, object> config)
        {
            if (config.ContainsKey("Health")) Health = (int)config["Health"];
            if (config.ContainsKey("AttackPower")) AttackPower = (int)config["AttackPower"];
            if (config.ContainsKey("Range")) Range = (int)config["Range"];
            Console.WriteLine($"[CONFIG] {Name} configured: HP={Health}, ATK={AttackPower}, Range={Range}");
        }

        public void Attack(string target)
        {
            Console.WriteLine($"[ATTACK] {Name} shoots arrow at {target} for {AttackPower} damage!");
        }
    }

    // Simple item classes
    class Weapon : GameEntity
    {
        public int Damage { get; set; } = 10;

        public override void Initialize()
        {
            Name = "Weapon";
            Console.WriteLine($"[INIT] {Name} created with {Damage} damage");
        }
    }

    class Potion : GameEntity
    {
        public int HealAmount { get; set; } = 50;

        public override void Initialize()
        {
            Name = "Health Potion";
            Console.WriteLine($"[INIT] {Name} created (heals {HealAmount} HP)");
        }
    }

    // Generic Factory with new() constraint
    class SimpleFactory<T> where T : new()
    {
        public T Create()
        {
            T instance = new T();
            Console.WriteLine($"[SIMPLE FACTORY] Created instance of {typeof(T).Name}");
            return instance;
        }

        public List<T> CreateBatch(int count)
        {
            List<T> instances = new List<T>();
            for (int i = 0; i < count; i++)
            {
                instances.Add(new T());
            }
            Console.WriteLine($"[SIMPLE FACTORY] Created {count} instances of {typeof(T).Name}");
            return instances;
        }
    }

    // Generic Factory with base class constraint
    class EntityFactory<T> where T : GameEntity, new()
    {
        private static int _idCounter = 1;

        public T Create()
        {
            T entity = new T();
            entity.Id = _idCounter++;
            entity.Initialize();
            return entity;
        }

        public T CreateWithName(string name)
        {
            T entity = Create();
            entity.Name = name;
            Console.WriteLine($"[ENTITY FACTORY] Set name to: {name}");
            return entity;
        }
    }

    // Generic Factories with multiple constraints
    class ConfigurableEntityFactory<T> where T : GameEntity, IConfigurable, new()
    {
        private static int _idCounter = 100;

        public T Create(Dictionary<string, object> config)
        {
            T entity = new T();
            entity.Id = _idCounter++;
            entity.Initialize();
            entity.Configure(config);
            return entity;
        }

        public T CreateDefault()
        {
            T entity = new T();
            entity.Id = _idCounter++;
            entity.Initialize();
            Console.WriteLine($"[CONFIGURABLE FACTORY] Created default {typeof(T).Name}");
            return entity;
        }

        public List<T> CreateArmy(int count, Dictionary<string, object> config)
        {
            List<T> army = new List<T>();
            Console.WriteLine($"[CONFIGURABLE FACTORY] Creating army of {count} {typeof(T).Name}s");

            for (int i = 0; i < count; i++)
            {
                T entity = Create(config);
                army.Add(entity);
            }

            return army;
        }
    }

    class CombatEntityFactory<T> where T : GameEntity, ICombat, IConfigurable, new()
    {
        private static int _idCounter = 200;

        public T CreateCombatUnit(string name, int health, int attackPower)
        {
            T unit = new T();
            unit.Id = _idCounter++;
            unit.Name = name;
            unit.Initialize();

            Dictionary<string, object> config = new Dictionary<string, object>
            {
                { "Health", health },
                { "AttackPower", attackPower }
            };

            unit.Configure(config);
            return unit;
        }

        public void SimulateCombat(T attacker, string target)
        {
            Console.WriteLine($"\n[COMBAT SIMULATION]");
            attacker.Display();
            attacker.Attack(target);
        }
    }

    // Abstract Factory Pattern
    interface ICharacterFactory
    {
        GameEntity CreateCharacter();
        Weapon CreateWeapon();
        string GetFactionName();
    }

    class HumanFactory : ICharacterFactory
    {
        public GameEntity CreateCharacter()
        {
            Warrior warrior = new Warrior { Name = "Human Warrior", Id = 1 };
            warrior.Initialize();
            return warrior;
        }

        public Weapon CreateWeapon()
        {
            Weapon sword = new Weapon { Name = "Iron Sword", Damage = 25, Id = 101 };
            sword.Initialize();
            return sword;
        }

        public string GetFactionName() => "Human Kingdom";
    }

    class ElfFactory : ICharacterFactory
    {
        public GameEntity CreateCharacter()
        {
            Archer archer = new Archer { Name = "Elf Archer", Id = 2, Range = 15 };
            archer.Initialize();
            return archer;
        }

        public Weapon CreateWeapon()
        {
            Weapon bow = new Weapon { Name = "Elven Bow", Damage = 30, Id = 102 };
            bow.Initialize();
            return bow;
        }

        public string GetFactionName() => "Elf Alliance";
    }

    class OrcFactory : ICharacterFactory
    {
        public GameEntity CreateCharacter()
        {
            Warrior warrior = new Warrior { Name = "Orc Warrior", Id = 3, Defense = 50 };
            warrior.Initialize();
            return warrior;
        }

        public Weapon CreateWeapon()
        {
            Weapon axe = new Weapon { Name = "Battle Axe", Damage = 35, Id = 103 };
            axe.Initialize();
            return axe;
        }

        public string GetFactionName() => "Orc Horde";
    }

    // Singleton Factory Pattern
    class SingletonFactory<T> where T : new()
    {
        private static T? _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                            Console.WriteLine($"[SINGLETON FACTORY] Created singleton instance of {typeof(T).Name}");
                        }
                    }
                }
                return _instance;
            }
        }
    }

    class GameManager
    {
        public string GameName { get; set; } = "RPG Game";

        public void Start()
        {
            Console.WriteLine($"[GAME MANAGER] {GameName} started!");
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example 1: Simple Factory (new() constraint)");
            SimpleFactory<Potion> potionFactory = new SimpleFactory<Potion>();
            Potion potion = potionFactory.Create();

            Console.WriteLine("\nExample 2: Entity Factory (base class constraint)");
            EntityFactory<Warrior> warriorFactory = new EntityFactory<Warrior>();
            Warrior warrior1 = warriorFactory.Create();
            Warrior warrior2 = warriorFactory.CreateWithName("Conan");

            Console.WriteLine("\nExample 3: Configurable Factory (multiple constraints)");
            ConfigurableEntityFactory<Mage> mageFactory = new ConfigurableEntityFactory<Mage>();

            Dictionary<string, object> mageConfig = new Dictionary<string, object>
            {
                { "Health", 100 },
                { "AttackPower", 120 },
                { "Mana", 150 }
            };

            Mage powerfulMage = mageFactory.Create(mageConfig);
            powerfulMage.Name = "Gandalf";

            Console.WriteLine("\nExample 4: Creating an Army");
            ConfigurableEntityFactory<Archer> archerFactory = new ConfigurableEntityFactory<Archer>();

            Dictionary<string, object> archerConfig = new Dictionary<string, object>
            {
                { "Health", 90 },
                { "AttackPower", 70 },
                { "Range", 12 }
            };

            List<Archer> archerArmy = archerFactory.CreateArmy(3, archerConfig);

            Console.WriteLine("\nExample 5: Combat Entity Factory");
            CombatEntityFactory<Warrior> combatFactory = new CombatEntityFactory<Warrior>();
            Warrior champion = combatFactory.CreateCombatUnit("Champion", 150, 80);
            combatFactory.SimulateCombat(champion, "Dragon");

            Console.WriteLine("\nExample 6: Abstract Factory Pattern");
            List<ICharacterFactory> factionFactories = new List<ICharacterFactory>
            {
                new HumanFactory(),
                new ElfFactory(),
                new OrcFactory()
            };

            foreach (var factory in factionFactories)
            {
                Console.WriteLine($"\n[FACTION] {factory.GetFactionName()}");
                GameEntity character = factory.CreateCharacter();
                Weapon weapon = factory.CreateWeapon();
                character.Display();
                weapon.Display();
            }

            Console.WriteLine("\nExample 7: Singleton Factory Pattern");
            GameManager manager1 = SingletonFactory<GameManager>.Instance;
            GameManager manager2 = SingletonFactory<GameManager>.Instance;

            Console.WriteLine($"Managers are same instance: {ReferenceEquals(manager1, manager2)}");
            manager1.Start();

            Console.ReadKey();
        }
    }
}
