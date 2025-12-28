using System;
using System.Collections.Generic;

namespace BasicGenerics
{
    // Basic generic class: Inventory that can hold any type of item
    class Inventory<T>
    {
        private List<T> _items = new List<T>();

        public void Add(T item)
        {
            _items.Add(item);
            Console.WriteLine($"[INVENTORY] Added: {item}");
        }

        public void Remove(T item)
        {
            if (_items.Remove(item))
            {
                Console.WriteLine($"[INVENTORY] Removed: {item}");
            }
        }

        public void DisplayAll()
        {
            Console.WriteLine($"[INVENTORY] Total items: {_items.Count}");
            foreach (var item in _items)
            {
                Console.WriteLine($"- {item}");
            }
        }

        public int Count => _items.Count;
    }

    // Generic class with multiple type parameters
    class KeyValueStore<TKey, TValue> where TKey : notnull
    {
        private Dictionary<TKey, TValue> _storage = new Dictionary<TKey, TValue>();

        public void Add(TKey key, TValue value)
        {
            _storage[key] = value;
            Console.WriteLine($"[STORAGE] Stored '{value}' with key '{key}'");
        }

        public TValue? Get(TKey key)
        {
            if (_storage.TryGetValue(key, out TValue? value))
            {
                return value;
            }
            return default;
        }

        public void Display()
        {
            Console.WriteLine($"[STORAGE] Total entries: {_storage.Count}");
            foreach (var kvp in _storage)
            {
                Console.WriteLine($"- {kvp.Key} => {kvp.Value}");
            }
        }
    }

    // Constraints Examples
    class GameEntity
    {
        public string Name { get; set; } = "Entity";
        public int Id { get; set; }

        public override string ToString() => $"{Name} (ID: {Id})";
    }

    class Player : GameEntity
    {
        public int Level { get; set; }
        public Player() { Name = "Player"; }
    }

    class Enemy : GameEntity
    {
        public int Damage { get; set; }
        public Enemy() { Name = "Enemy"; }
    }

    interface IDamageable
    {
        int Health { get; set; }
        void TakeDamage(int amount);
    }

    class Character : GameEntity, IDamageable
    {
        public int Health { get; set; } = 100;

        public Character()
        {
            Name = "Character";
        }

        public void TakeDamage(int amount)
        {
            Health -= amount;
            Console.WriteLine($"[{Name}] Took {amount} damage. Health: {Health}");
        }
    }

    // Generic class with class constraint (T must be a reference type)
    class ReferenceTypeContainer<T> where T : class
    {
        private T? _value;

        public void Store(T value)
        {
            _value = value;
            Console.WriteLine($"[CLASS CONSTRAINT] Stored reference type: {value}");
        }

        public T? Retrieve()
        {
            return _value;
        }
    }

    // Generic class with struct constraint (T must be a value type)
    class ValueTypeContainer<T> where T : struct
    {
        private T _value;

        public void Store(T value)
        {
            _value = value;
            Console.WriteLine($"[STRUCT CONSTRAINT] Stored value type: {value}");
        }

        public T Retrieve()
        {
            return _value;
        }
    }

    // Generic class with base class constraint (T must inherit from GameEntity)
    class EntityManager<T> where T : GameEntity
    {
        private List<T> _entities = new List<T>();

        public void Register(T entity)
        {
            _entities.Add(entity);
            Console.WriteLine($"[BASE CLASS CONSTRAINT] Registered: {entity.Name} with ID {entity.Id}");
        }

        public T? FindById(int id)
        {
            return _entities.Find(e => e.Id == id);
        }

        public void ListAll()
        {
            Console.WriteLine($"[ENTITY MANAGER] Total entities: {_entities.Count}");
            foreach (var entity in _entities)
            {
                Console.WriteLine($"- {entity}");
            }
        }
    }

    // Generic class with interface constraint (T must implement IDamageable)
    class CombatSystem<T> where T : IDamageable
    {
        public void Attack(T target, int damage)
        {
            Console.WriteLine($"[INTERFACE CONSTRAINT] Attacking target...");
            target.TakeDamage(damage);
        }

        public void MultiAttack(List<T> targets, int damage)
        {
            Console.WriteLine($"[INTERFACE CONSTRAINT] Multi-attack on {targets.Count} targets!");
            foreach (var target in targets)
            {
                target.TakeDamage(damage);
            }
        }
    }

    // Generic methods
    class GenericMethods
    {
        // Generic method to swap two values
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
            Console.WriteLine($"[GENERIC METHOD] Swapped values");
        }

        // Generic method with constraint
        public static T FindMax<T>(T a, T b) where T : IComparable<T>
        {
            return a.CompareTo(b) > 0 ? a : b;
        }

        // Generic method that works with collections
        public static void PrintCollection<T>(IEnumerable<T> collection, string label)
        {
            Console.WriteLine($"[{label}]:");
            foreach (var item in collection)
            {
                Console.WriteLine($"- {item}");
            }
        }
    }

    // Generics with events
    class ItemEventArgs<T> : EventArgs
    {
        public T Item { get; set; }
        public string Action { get; set; } = string.Empty;

        public ItemEventArgs(T item, string action)
        {
            Item = item;
            Action = action;
        }
    }

    class GenericInventory<T>
    {
        private List<T> _items = new List<T>();

        // Generic event
        public event EventHandler<ItemEventArgs<T>>? ItemChanged;

        protected virtual void OnItemChanged(T item, string action)
        {
            ItemChanged?.Invoke(this, new ItemEventArgs<T>(item, action));
        }

        public void Add(T item)
        {
            _items.Add(item);
            OnItemChanged(item, "Added");
        }

        public void Remove(T item)
        {
            if (_items.Remove(item))
            {
                OnItemChanged(item, "Removed");
            }
        }

        public int Count => _items.Count;
    }

    // Generic subscriber for inventory events
    class InventoryMonitor<T>
    {
        private string _name;

        public InventoryMonitor(string name)
        {
            _name = name;
        }

        public void OnItemChanged(object? sender, ItemEventArgs<T> e)
        {
            Console.WriteLine($"[{_name}] Item {e.Action}: {e.Item}");
        }
    }

    // Covariant generic interface (out T)
    interface IProducer<out T>
    {
        T Produce();
    }

    class PlayerProducer : IProducer<Player>
    {
        public Player Produce()
        {
            return new Player { Name = "Hero", Level = 5, Id = 1 };
        }
    }

    // Contravariant generic interface (in T)
    interface IConsumer<in T>
    {
        void Consume(T item);
    }

    class EntityConsumer : IConsumer<GameEntity>
    {
        public void Consume(GameEntity entity)
        {
            Console.WriteLine($"[CONSUMER] Consumed entity: {entity.Name}");
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Example 1: Basic Generic Class");
            Inventory<string> weaponInventory = new Inventory<string>();
            weaponInventory.Add("Sword");
            weaponInventory.Add("Bow");
            weaponInventory.Add("Staff");
            weaponInventory.DisplayAll();

            Console.WriteLine();
            Inventory<int> scoreInventory = new Inventory<int>();
            scoreInventory.Add(100);
            scoreInventory.Add(250);
            scoreInventory.Add(500);
            scoreInventory.DisplayAll();

            Console.WriteLine("\nExample 2: Multiple Type Parameters");
            KeyValueStore<string, int> playerScores = new KeyValueStore<string, int>();
            playerScores.Add("Alice", 1500);
            playerScores.Add("Bob", 2000);
            playerScores.Add("Charlie", 1750);
            playerScores.Display();

            Console.WriteLine("\nExample 3: Class Constraint");
            ReferenceTypeContainer<string> refContainer = new ReferenceTypeContainer<string>();
            refContainer.Store("Magic Potion");

            Console.WriteLine("\nExample 4: Struct Constraint");
            ValueTypeContainer<int> valContainer = new ValueTypeContainer<int>();
            valContainer.Store(42);

            Console.WriteLine("\nExample 5: Base Class Constraint");
            EntityManager<Player> playerManager = new EntityManager<Player>();
            playerManager.Register(new Player { Name = "Warrior", Id = 1, Level = 10 });
            playerManager.Register(new Player { Name = "Mage", Id = 2, Level = 8 });
            playerManager.ListAll();

            Console.WriteLine("\nExample 6: Interface Constraint");
            CombatSystem<Character> combat = new CombatSystem<Character>();
            Character hero = new Character { Name = "Hero", Health = 100 };
            combat.Attack(hero, 25);

            Console.WriteLine("\nExample 7: Generic Methods");
            int x = 5, y = 10;
            Console.WriteLine($"Before swap: x={x}, y={y}");
            GenericMethods.Swap(ref x, ref y);
            Console.WriteLine($"After swap: x={x}, y={y}");

            int max = GenericMethods.FindMax(100, 50);
            Console.WriteLine($"Max value: {max}");

            List<string> items = new List<string> { "Helmet", "Armor", "Boots" };
            GenericMethods.PrintCollection(items, "Equipment");

            Console.WriteLine("\nExample 8: Generics with Events");
            GenericInventory<string> inventory = new GenericInventory<string>();
            InventoryMonitor<string> monitor = new InventoryMonitor<string>("MONITOR");

            inventory.ItemChanged += monitor.OnItemChanged;

            inventory.Add("Health Potion");
            inventory.Add("Mana Potion");
            inventory.Remove("Health Potion");

            Console.WriteLine("\nExample 9: Covariance");
            IProducer<Player> playerProducer = new PlayerProducer();
            // Covariance allows assignment of IProducer<Player> to IProducer<GameEntity>
            IProducer<GameEntity> entityProducer = playerProducer;
            GameEntity entity = entityProducer.Produce();
            Console.WriteLine($"[COVARIANCE] Produced: {entity}");

            Console.WriteLine("\nExample 10: Contravariance");
            IConsumer<GameEntity> entityConsumer = new EntityConsumer();
            // Contravariance allows assignment of IConsumer<GameEntity> to IConsumer<Player>
            IConsumer<Player> playerConsumer = entityConsumer;
            playerConsumer.Consume(new Player { Name = "Knight", Id = 3, Level = 15 });

            Console.ReadKey();
        }
    }
}
