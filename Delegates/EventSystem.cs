using System;

namespace EventSystem
{
    enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary
    }

    class GamePlayer
    {
        public delegate void AttackHandler(string attackerName, string targetName, int damage, bool isCritical);
        public delegate void HealthChangedHandler(string playerName, int currentHealth, int maxHealth);
        public delegate void ItemCollectedHandler(string itemName, ItemRarity rarity);
        public delegate void LevelUpHandler(string playerName, int newLevel);

        public AttackHandler? OnAttackPerformed;
        public HealthChangedHandler? OnHealthChanged;
        public ItemCollectedHandler? OnItemCollected;
        public LevelUpHandler? OnLevelUp;

        private readonly int _maxHealth;
        private readonly string _name;
        private int _health;
        private int _level;

        public int MaxHealth => _maxHealth;
        public string Name => _name;

        public int Health
        {
            get => _health;
            set
            {
                if (value != _health)
                {
                    _health = Math.Clamp(value, 0, _maxHealth);
                    OnHealthChanged?.Invoke(_name, _health, _maxHealth);
                }
            }
        }

        public int Level
        {
            get => _level;
            set
            {
                if (value > _level)
                {
                    _level = value;
                    OnLevelUp?.Invoke(_name, _level);
                }
            }
        }

        public GamePlayer(string name, int maxHealth = 100)
        {
            _name = name;
            _maxHealth = maxHealth;
            _health = maxHealth;
            _level = 1;
        }

        public void TakeDamage(int damage)
        {
            Console.WriteLine($"\n[ACTION] {Name} takes {damage} damage!");
            Health -= damage;
        }

        public void Heal(int amount)
        {
            Console.WriteLine($"\n[ACTION] {Name} heals for {amount} HP!");
            Health += amount;
        }

        public void Attack(string target)
        {
            Random random = new Random();
            bool isCritical = random.Next(1, 101) <= 20;
            int damage = isCritical ? random.Next(40, 60) : random.Next(20, 35);

            Console.WriteLine($"\n[ACTION] {Name} attacks {target}!");
            OnAttackPerformed?.Invoke(_name, target, damage, isCritical);
        }

        public void CollectItem(string itemName, ItemRarity rarity = ItemRarity.Common)
        {
            Console.WriteLine($"\n[ACTION] {Name} found an item!");
            OnItemCollected?.Invoke(itemName, rarity);
        }

        public void GainExperience(int amount)
        {
            Console.WriteLine($"\n[ACTION] {Name} gains {amount} experience!");
            if (amount >= 100)
            {
                Level++;
            }
        }
    }

    class GameUI
    {
        public void OnHealthChanged(string playerName, int currentHealth, int maxHealth)
        {
            double percentage = 100 * currentHealth / (double)maxHealth;
            string healthBar = GenerateHealthBar(percentage);

            Console.WriteLine($"[UI] {playerName}'s Health: {healthBar} {currentHealth}/{maxHealth} HP ({percentage:F0}%)");

            if (currentHealth == 0)
            {
                Console.WriteLine($"[UI] {playerName} has been defeated!");
            }
            else if (percentage <= 25)
            {
                Console.WriteLine($"[UI] WARNING: {playerName} is in critical condition!");
            }
        }

        public void OnLevelUp(string playerName, int newLevel)
        {
            Console.WriteLine($"[UI] LEVEL UP! {playerName} reached Level {newLevel}!");
        }

        private string GenerateHealthBar(double percentage)
        {
            int bars = (int)(percentage / 10);
            return "[" + new string('█', bars) + new string('░', 10 - bars) + "]";
        }
    }

    class CombatLogger
    {
        public void OnAttackPerformed(string attackerName, string targetName, int damage, bool isCritical)
        {
            string critText = isCritical ? " CRITICAL HIT!" : "";
            Console.WriteLine($"[COMBAT LOG] {attackerName} dealt {damage} damage to {targetName}{critText}");
        }
    }

    class InventorySystem
    {
        public void OnItemCollected(string itemName, ItemRarity rarity)
        {
            string rarityColor = rarity switch
            {
                ItemRarity.Common => "WHITE",
                ItemRarity.Uncommon => "GREEN",
                ItemRarity.Rare => "BLUE",
                ItemRarity.Epic => "PURPLE",
                ItemRarity.Legendary => "GOLD",
                _ => "WHITE"
            };

            Console.WriteLine($"[INVENTORY] +1x {itemName} ({rarityColor} - {rarity})");
            Console.WriteLine($"[INVENTORY] Item added to inventory!");
        }
    }

    class AchievementSystem
    {
        private int _attackCount = 0;

        public void OnAttackPerformed(string attackerName, string targetName, int damage, bool isCritical)
        {
            _attackCount++;

            if (_attackCount == 5)
            {
                Console.WriteLine("[ACHIEVEMENT] 'Warrior' - Perform 5 attacks!");
            }

            if (isCritical)
            {
                Console.WriteLine("[ACHIEVEMENT] 'Lucky Strike' - Land a critical hit!");
            }
        }

        public void OnLevelUp(string playerName, int newLevel)
        {
            if (newLevel == 2)
            {
                Console.WriteLine("[ACHIEVEMENT] 'Beginner's Journey' - Reach Level 2!");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            GamePlayer player = new GamePlayer("Kratos", 150);

            GameUI ui = new GameUI();
            CombatLogger combatLogger = new CombatLogger();
            InventorySystem inventory = new InventorySystem();
            AchievementSystem achievements = new AchievementSystem();

            Console.WriteLine("\nSubscribing to delegates");
            player.OnHealthChanged += ui.OnHealthChanged;
            player.OnLevelUp += ui.OnLevelUp;
            player.OnLevelUp += achievements.OnLevelUp;

            player.OnAttackPerformed += combatLogger.OnAttackPerformed;
            player.OnAttackPerformed += achievements.OnAttackPerformed;

            player.OnItemCollected += inventory.OnItemCollected;

            Console.WriteLine("All systems registered to player delegates!\n");

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

            Console.WriteLine("\nUnsubscribing combat logger");
            player.OnAttackPerformed -= combatLogger.OnAttackPerformed;
            player.Attack("Final Enemy");
            player.TakeDamage(150);

            Console.ReadKey();
        }
    }
}
