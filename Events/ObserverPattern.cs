using System;
using System.Collections.Generic;

namespace ObserverPattern
{
    enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    abstract class GameEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }

    class HealthChangedEvent : GameEvent
    {
        public string PlayerName { get; set; } = string.Empty;
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; set; }
    }

    class AttackEvent : GameEvent
    {
        public string AttackerName { get; set; } = string.Empty;
        public string TargetName { get; set; } = string.Empty;
        public int Damage { get; set; }
        public bool IsCritical { get; set; }
    }

    class ItemCollectedEvent : GameEvent
    {
        public string ItemName { get; set; } = string.Empty;
        public ItemRarity Rarity { get; set; }
    }

    class LevelUpEvent : GameEvent
    {
        public string PlayerName { get; set; } = string.Empty;
        public int NewLevel { get; set; }
    }

    // Unsubscriber helper class: Implements IDisposable to allow unsubscription
    class Unsubscriber<T> : IDisposable
    {
        private readonly List<IObserver<T>> _observers;
        private readonly IObserver<T> _observer;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            _observers = observers;
            _observer = observer;
        }

        public void Dispose()
        {
            if (_observers.Contains(_observer))
            {
                _observers.Remove(_observer);
            }
        }
    }

    // Observable Provider: Implements IObservable<T> to allow observers to subscribe
    class GamePlayer : IObservable<GameEvent>
    {
        private readonly List<IObserver<GameEvent>> _observers;
        private readonly string _name;
        private readonly int _maxHealth;
        private int _health;
        private int _level;

        public string Name => _name;
        public int Health => _health;
        public int Level => _level;

        public GamePlayer(string name, int maxHealth = 100)
        {
            _name = name;
            _maxHealth = maxHealth;
            _health = maxHealth;
            _level = 1;
            _observers = new List<IObserver<GameEvent>>();
        }

        // IObservable<T> implementation
        public IDisposable Subscribe(IObserver<GameEvent> observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
            return new Unsubscriber<GameEvent>(_observers, observer);
        }

        // Helper method to notify all observers
        private void NotifyObservers(GameEvent gameEvent)
        {
            foreach (var observer in _observers)
            {
                observer.OnNext(gameEvent);
            }
        }

        // Game actions that trigger events
        public void TakeDamage(int damage)
        {
            Console.WriteLine($"\n[ACTION] {_name} takes {damage} damage!");
            _health = Math.Max(0, _health - damage);

            NotifyObservers(new HealthChangedEvent
            {
                PlayerName = _name,
                CurrentHealth = _health,
                MaxHealth = _maxHealth
            });

            if (_health == 0)
            {
                EndGame();
            }
        }

        public void Heal(int amount)
        {
            Console.WriteLine($"\n[ACTION] {_name} heals for {amount} HP!");
            _health = Math.Min(_maxHealth, _health + amount);

            NotifyObservers(new HealthChangedEvent
            {
                PlayerName = _name,
                CurrentHealth = _health,
                MaxHealth = _maxHealth
            });
        }

        public void Attack(string target)
        {
            Random random = new Random();
            bool isCritical = random.Next(1, 101) <= 20;
            int damage = isCritical ? random.Next(40, 60) : random.Next(20, 35);

            Console.WriteLine($"\n[ACTION] {_name} attacks {target}!");

            NotifyObservers(new AttackEvent
            {
                AttackerName = _name,
                TargetName = target,
                Damage = damage,
                IsCritical = isCritical
            });
        }

        public void CollectItem(string itemName, ItemRarity rarity = ItemRarity.Common)
        {
            Console.WriteLine($"\n[ACTION] {_name} found an item!");

            NotifyObservers(new ItemCollectedEvent
            {
                ItemName = itemName,
                Rarity = rarity
            });
        }

        public void GainExperience(int amount)
        {
            Console.WriteLine($"\n[ACTION] {_name} gains {amount} experience!");

            if (amount >= 100)
            {
                _level++;

                NotifyObservers(new LevelUpEvent
                {
                    PlayerName = _name,
                    NewLevel = _level
                });
            }
        }

        private void EndGame()
        {
            // Notify all observers that the sequence is complete
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }
    }

    // Observer Subscribers: Each implements IObserver<GameEvent> to receive notifications

    class GameUIObserver : IObserver<GameEvent>
    {
        public void OnNext(GameEvent value)
        {
            switch (value)
            {
                case HealthChangedEvent health:
                    double percentage = 100 * health.CurrentHealth / (double)health.MaxHealth;
                    string healthBar = GenerateHealthBar(percentage);
                    Console.WriteLine($"[UI] {health.PlayerName}'s Health: {healthBar} {health.CurrentHealth}/{health.MaxHealth} HP ({percentage:F0}%)");

                    if (health.CurrentHealth == 0)
                    {
                        Console.WriteLine($"[UI] {health.PlayerName} has been defeated!");
                    }
                    else if (percentage <= 25)
                    {
                        Console.WriteLine($"[UI] WARNING: {health.PlayerName} is in critical condition!");
                    }
                    break;

                case LevelUpEvent levelUp:
                    Console.WriteLine($"[UI] LEVEL UP! {levelUp.PlayerName} reached Level {levelUp.NewLevel}!");
                    break;
            }
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"[UI ERROR] {error.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine("[UI] Game session ended.");
        }

        private string GenerateHealthBar(double percentage)
        {
            int bars = (int)(percentage / 10);
            return "[" + new string('█', bars) + new string('░', 10 - bars) + "]";
        }
    }

    class CombatLoggerObserver : IObserver<GameEvent>
    {
        public void OnNext(GameEvent value)
        {
            if (value is AttackEvent attack)
            {
                string critText = attack.IsCritical ? " CRITICAL HIT!" : "";
                Console.WriteLine($"[COMBAT LOG] {attack.AttackerName} dealt {attack.Damage} damage to {attack.TargetName}{critText}");
            }
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"[COMBAT LOG ERROR] {error.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine("[COMBAT LOG] Combat session ended.");
        }
    }

    class InventoryObserver : IObserver<GameEvent>
    {
        public void OnNext(GameEvent value)
        {
            if (value is ItemCollectedEvent item)
            {
                string rarityColor = item.Rarity switch
                {
                    ItemRarity.Common => "WHITE",
                    ItemRarity.Uncommon => "GREEN",
                    ItemRarity.Rare => "BLUE",
                    ItemRarity.Epic => "PURPLE",
                    ItemRarity.Legendary => "GOLD",
                    _ => "WHITE"
                };

                Console.WriteLine($"[INVENTORY] +1x {item.ItemName} ({rarityColor} - {item.Rarity})");
                Console.WriteLine($"[INVENTORY] Item added to inventory!");
            }
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"[INVENTORY ERROR] {error.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine("[INVENTORY] Inventory system closed.");
        }
    }

    class AchievementObserver : IObserver<GameEvent>
    {
        private int _attackCount = 0;

        public void OnNext(GameEvent value)
        {
            switch (value)
            {
                case AttackEvent attack:
                    _attackCount++;

                    if (_attackCount == 1)
                    {
                        Console.WriteLine($"[ACHIEVEMENT] First Strike - Perform your first attack!");
                    }
                    else if (_attackCount == 5)
                    {
                        Console.WriteLine($"[ACHIEVEMENT] Warrior - Perform 5 attacks!");
                    }

                    if (attack.IsCritical)
                    {
                        Console.WriteLine($"[ACHIEVEMENT] Lucky Strike - Land a critical hit!");
                    }
                    break;

                case LevelUpEvent levelUp:
                    if (levelUp.NewLevel == 2)
                    {
                        Console.WriteLine($"[ACHIEVEMENT] Beginner's Journey - Reach Level 2!");
                    }
                    break;
            }
        }

        public void OnError(Exception error)
        {
            Console.WriteLine($"[ACHIEVEMENT ERROR] {error.Message}");
        }

        public void OnCompleted()
        {
            Console.WriteLine("[ACHIEVEMENT] Achievement tracking ended.");
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            // Create the observable provider
            GamePlayer player = new GamePlayer("Kratos", 150);

            // Create observers
            GameUIObserver ui = new GameUIObserver();
            CombatLoggerObserver combatLogger = new CombatLoggerObserver();
            InventoryObserver inventory = new InventoryObserver();
            AchievementObserver achievements = new AchievementObserver();

            // Subscribe observers to the observable
            IDisposable uiSubscription = player.Subscribe(ui);
            IDisposable combatSubscription = player.Subscribe(combatLogger);
            IDisposable inventorySubscription = player.Subscribe(inventory);
            IDisposable achievementSubscription = player.Subscribe(achievements);

            // Trigger game events
            Console.WriteLine("Gameplay:");

            player.Attack("Goblin");
            player.Attack("Orc");
            player.Attack("Troll");

            player.TakeDamage(30);
            player.TakeDamage(50);
            player.Heal(40);
            player.TakeDamage(80);

            player.CollectItem("Health Potion", ItemRarity.Common);
            player.CollectItem("Sword of Destiny", ItemRarity.Legendary);
            player.CollectItem("Magic Ring", ItemRarity.Epic);

            player.GainExperience(120);

            player.Attack("Dragon");
            player.Attack("Boss");

            // Unsubscribe example
            Console.WriteLine("\nUnsubscribing Combat Logger");

            combatSubscription.Dispose();

            player.Attack("Final Enemy");
            player.TakeDamage(150);

            Console.ReadKey();
        }
    }
}
