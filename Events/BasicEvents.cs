using System;

namespace BasicEvents
{
    // Custom EventArgs Classes: Carry data from publisher to subscribers
    class ScoreChangedEventArgs : EventArgs
    {
        public int OldScore { get; set; }
        public int NewScore { get; set; }
        public int PointsGained { get; set; }
    }

    class EnemyDefeatedEventArgs : EventArgs
    {
        public string EnemyName { get; set; } = string.Empty;
        public int Experience { get; set; }
        public int GoldReward { get; set; }
    }

    class LeveledUpEventArgs : EventArgs
    {
        public int NewLevel { get; set; }
        public int HealthBonus { get; set; }
        public int ManaBonus { get; set; }
    }

    // Publisher Class: Raises events when something happens
    class GameCharacter
    {
        private string _name;
        private int _score;
        private int _level;

        public string Name => _name;
        public int Score => _score;
        public int Level => _level;

        // Pattern: public event EventHandler<CustomEventArgs> EventName;
        public event EventHandler<ScoreChangedEventArgs>? ScoreChanged;
        public event EventHandler<EnemyDefeatedEventArgs>? EnemyDefeated;
        public event EventHandler<LeveledUpEventArgs>? LeveledUp;
        public event EventHandler? GameOver;

        public GameCharacter(string name)
        {
            _name = name;
            _score = 0;
            _level = 1;
        }

        // Protected virtual methods to raise events
        protected virtual void OnScoreChanged(ScoreChangedEventArgs e)
        {
            ScoreChanged?.Invoke(this, e);
        }

        protected virtual void OnEnemyDefeated(EnemyDefeatedEventArgs e)
        {
            EnemyDefeated?.Invoke(this, e);
        }

        protected virtual void OnLeveledUp(LeveledUpEventArgs e)
        {
            LeveledUp?.Invoke(this, e);
        }

        protected virtual void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }

        // Public methods that trigger events
        public void DefeatEnemy(string enemyName, int experience, int gold)
        {
            Console.WriteLine($"\n[{_name}] Defeated {enemyName}!");

            OnEnemyDefeated(new EnemyDefeatedEventArgs
            {
                EnemyName = enemyName,
                Experience = experience,
                GoldReward = gold
            });

            AddScore(experience + gold);

            if (_score >= _level * 100)
            {
                LevelUp();
            }
        }

        public void AddScore(int points)
        {
            int oldScore = _score;
            _score += points;

            OnScoreChanged(new ScoreChangedEventArgs
            {
                OldScore = oldScore,
                NewScore = _score,
                PointsGained = points
            });
        }

        private void LevelUp()
        {
            _level++;
            int healthBonus = _level * 10;
            int manaBonus = _level * 5;

            OnLeveledUp(new LeveledUpEventArgs
            {
                NewLevel = _level,
                HealthBonus = healthBonus,
                ManaBonus = manaBonus
            });
        }

        public void TriggerGameOver()
        {
            Console.WriteLine($"\n[{_name}] has fallen in battle!");
            OnGameOver();
        }
    }

    // Subscriber Classes: Listen to events and react when they occur
    class ScoreBoard
    {
        public void OnScoreChanged(object? sender, ScoreChangedEventArgs e)
        {
            Console.WriteLine($"[SCOREBOARD] Score: {e.OldScore} -> {e.NewScore} (+{e.PointsGained})");
        }

        public void OnLevelUp(object? sender, LeveledUpEventArgs e)
        {
            Console.WriteLine($"[SCOREBOARD] LEVEL UP! Now at Level {e.NewLevel}");
        }
    }

    class UINotifications
    {
        public void OnEnemyDefeated(object? sender, EnemyDefeatedEventArgs e)
        {
            Console.WriteLine($"[UI] Victory! Gained {e.Experience} XP and {e.GoldReward} gold");
        }

        public void OnLevelUp(object? sender, LeveledUpEventArgs e)
        {
            Console.WriteLine($"[UI] Stats increased! +{e.HealthBonus} HP, +{e.ManaBonus} Mana");
        }

        public void OnGameOver(object? sender, EventArgs e)
        {
            if (sender is GameCharacter character)
            {
                Console.WriteLine($"\n[UI] GAME OVER - {character.Name}");
                Console.WriteLine($"[UI] Final Score: {character.Score}");
                Console.WriteLine($"[UI] Final Level: {character.Level}\n");
            }
        }
    }

    class AchievementTracker
    {
        private int _enemiesDefeated = 0;

        public void OnEnemyDefeated(object? sender, EnemyDefeatedEventArgs e)
        {
            _enemiesDefeated++;

            if (_enemiesDefeated == 1)
            {
                Console.WriteLine($"[ACHIEVEMENT] First Blood - Defeat your first enemy!");
            }
            else if (_enemiesDefeated == 5)
            {
                Console.WriteLine($"[ACHIEVEMENT] Warrior - Defeat 5 enemies!");
            }
            else if (_enemiesDefeated == 10)
            {
                Console.WriteLine($"[ACHIEVEMENT] Champion - Defeat 10 enemies!");
            }
        }

        public void OnLevelUp(object? sender, LeveledUpEventArgs e)
        {
            if (e.NewLevel == 5)
            {
                Console.WriteLine($"[ACHIEVEMENT] Experienced - Reach level 5!");
            }
            else if (e.NewLevel == 10)
            {
                Console.WriteLine($"[ACHIEVEMENT] Master - Reach level 10!");
            }
        }
    }

    class SoundEffects
    {
        public void OnScoreChanged(object? sender, ScoreChangedEventArgs e)
        {
            Console.WriteLine($"[SOUND] *score sound effect*");
        }

        public void OnEnemyDefeated(object? sender, EnemyDefeatedEventArgs e)
        {
            Console.WriteLine($"[SOUND] *victory sound effect*");
        }

        public void OnLevelUp(object? sender, LeveledUpEventArgs e)
        {
            Console.WriteLine($"[SOUND] *level up music*");
        }
    }

    // Main Program
    class Program
    {
        static void Main(string[] args)
        {
            // Create publisher
            GameCharacter hero = new GameCharacter("Aragorn");

            // Create subscribers
            ScoreBoard scoreBoard = new ScoreBoard();
            UINotifications ui = new UINotifications();
            AchievementTracker achievements = new AchievementTracker();
            SoundEffects sound = new SoundEffects();

            // Suscribing to events
            // Pattern: publisher.EventName += subscriber.HandlerMethod;
            hero.ScoreChanged += scoreBoard.OnScoreChanged;
            hero.ScoreChanged += sound.OnScoreChanged;

            hero.EnemyDefeated += ui.OnEnemyDefeated;
            hero.EnemyDefeated += achievements.OnEnemyDefeated;
            hero.EnemyDefeated += sound.OnEnemyDefeated;

            hero.LeveledUp += scoreBoard.OnLevelUp;
            hero.LeveledUp += ui.OnLevelUp;
            hero.LeveledUp += achievements.OnLevelUp;
            hero.LeveledUp += sound.OnLevelUp;

            hero.GameOver += ui.OnGameOver;

            // Trigger events through gameplay
            Console.WriteLine("Gameplay:");

            hero.DefeatEnemy("Goblin", 50, 10);
            hero.DefeatEnemy("Orc", 75, 15);
            hero.DefeatEnemy("Troll", 100, 25);
            hero.DefeatEnemy("Dragon", 200, 50);
            hero.DefeatEnemy("Skeleton", 60, 12);
            hero.DefeatEnemy("Zombie", 70, 14);
            hero.DefeatEnemy("Vampire", 150, 30);

            // Unsubscribe example
            Console.WriteLine("\nUnsubscribing Sound Effects");

            hero.ScoreChanged -= sound.OnScoreChanged;
            hero.EnemyDefeated -= sound.OnEnemyDefeated;
            hero.LeveledUp -= sound.OnLevelUp;

            hero.DefeatEnemy("Ghost", 80, 16);
            hero.DefeatEnemy("Demon", 180, 40);
            hero.DefeatEnemy("Boss Monster", 300, 75);

            // Game over
            hero.TriggerGameOver();

            Console.ReadKey();
        }
    }
}
