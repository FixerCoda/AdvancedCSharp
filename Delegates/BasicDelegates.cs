using System;

namespace BasicDelegates
{
    class Character
    {
        public string Name { get; set; } = "Unknown";
        public int Health { get; set; } = 100;
    }

    class Player : Character
    {
        public int Score { get; set; } = 0;
    }

    class Enemy : Character
    {
        public int RewardPoints { get; set; } = 50;
    }

    class Program
    {
        delegate void GameAction(string playerName);
        delegate int DamageCalculator(int baseDamage);
        delegate void PowerUpEffect(string effectName);

        delegate Player PlayerFactory(string name);
        delegate void CharacterHandler(Character character);


        static async Task Main(string[] args)
        {
            Console.WriteLine("Example 1: Player Actions");
            GameAction playerAction;

            playerAction = Attack;
            playerAction?.Invoke("Mario");

            playerAction = Defend;
            playerAction?.Invoke("Luigi");

            playerAction = Heal;
            playerAction?.Invoke("Peach");

            Console.WriteLine("\nExample 2: Damage Calculation");
            DamageCalculator normalDamage = CalculateNormalDamage;
            DamageCalculator criticalDamage = CalculateCriticalDamage;

            int baseDamage = 50;
            Console.WriteLine($"Base Damage: {baseDamage}");
            Console.WriteLine($"Normal Hit: {normalDamage?.Invoke(baseDamage)} damage");
            Console.WriteLine($"Critical Hit: {criticalDamage?.Invoke(baseDamage)} damage");

            Console.WriteLine("\nExample 3: Power-Up Collection (Multicast)");
            PowerUpEffect? collectPowerUp = SpeedBoost;

            collectPowerUp += StrengthBoost;
            collectPowerUp += ShieldActivate;

            Console.WriteLine("Player collected a MEGA Power-Up!");
            collectPowerUp?.Invoke("MEGA Power-Up");

            Console.WriteLine("\nExample 4: Power-Up Expires");
            collectPowerUp -= ShieldActivate;
            Console.WriteLine("Shield expired! Collecting another power-up:");
            collectPowerUp?.Invoke("Health Pack");

            // Func: delegates with multiple inputs and return values
            Console.WriteLine("\nExample 5: Func<T, TResult> - With return value");
            Func<int, int, int> calculateDamage = (attack, defense) => Math.Max(0, attack - defense);
            int damage = calculateDamage(75, 25);
            Console.WriteLine($"Damage dealt: {damage}");

            Func<string, string> formatPlayerName = (name) => $"{name.ToUpper()}";
            Console.WriteLine($"Player: {formatPlayerName("Link")}");

            // Action: delegates with multiple inputs and no return value
            Console.WriteLine("\nExample 6: Action<T> - No return value");
            Action<string> logMessage = (msg) => Console.WriteLine($"[LOG] {msg}");
            logMessage("Player entered the dungeon");
            logMessage("Player found treasure chest");

            // Predicate: delegates with one input that return bool
            Console.WriteLine("\nExample 7: Predicate<T> - Boolean test");
            Predicate<int> isHealthCritical = (health) => health < 20;
            Predicate<int> hasEnoughMana = (mana) => mana >= 50;

            int currentHealth = 15;
            int currentMana = 60;

            Console.WriteLine($"Health: {currentHealth} - Critical: {isHealthCritical(currentHealth)}");
            Console.WriteLine($"Mana: {currentMana} - Can cast spell: {hasEnoughMana(currentMana)}");

            Console.WriteLine("\nExample 8: Chaining Func delegates");
            Func<int, int> addBonus = (score) => score + 100;
            Func<int, int> multiplyCombo = (score) => score * 2;
            Func<int, int> applyPenalty = (score) => score - 50;

            int baseScore = 500;
            int finalScore = applyPenalty(multiplyCombo(addBonus(baseScore)));
            Console.WriteLine($"Base Score: {baseScore} -> Final Score: {finalScore}");

            // Covariance: delegate can return a more derived type where a less derived type is expected
            Console.WriteLine("\nExample 9: Covariance - Return type compatibility");
            PlayerFactory factory = CreatePlayer;
            Player player = factory("Hero");
            Console.WriteLine($"Created player: {player.Name} with {player.Health} HP and {player.Score} score");

            Func<Character> createCharacter = CreateDefaultPlayer; // Expected: Character, Received: Player
            Character character = createCharacter();
            Console.WriteLine($"Character created: {character.Name}");

            // Contravariance: delegate can accept a more general parameter type
            Console.WriteLine("\nExample 10: Contravariance - Parameter type compatibility");
            CharacterHandler handler = DisplayCharacterInfo; // Parameter expected: Character

            Player newPlayer = new Player { Name = "Zelda", Health = 100, Score = 1500 };
            handler(newPlayer); // Parameter Received: Player

            Action<Character> characterAction = HandleAnyCharacter; // Parameter expected: Character
            characterAction(newPlayer); // Parameter Received: Player

            // Async examples
            await AsynchronousExamples();
        }

        static void Attack(string playerName)
        {
            Console.WriteLine($"[ATTACK] {playerName} is attacking!");
        }

        static void Defend(string playerName)
        {
            Console.WriteLine($"[DEFEND] {playerName} raises their shield!");
        }

        static void Heal(string playerName)
        {
            Console.WriteLine($"[HEAL] {playerName} uses a health potion! +50 HP");
        }

        static int CalculateNormalDamage(int baseDamage)
        {
            return baseDamage;
        }

        static int CalculateCriticalDamage(int baseDamage)
        {
            return baseDamage * 2;
        }

        static void SpeedBoost(string effectName)
        {
            Console.WriteLine($"[Speed Boost] Movement speed increased by 50%! ({effectName})");
        }

        static void StrengthBoost(string effectName)
        {
            Console.WriteLine($"[Strength Boost] Attack damage increased by 30%! ({effectName})");
        }

        static void ShieldActivate(string effectName)
        {
            Console.WriteLine($"[Shield Active] Damage reduction activated! ({effectName})");
        }

        static Player CreateDefaultPlayer()
        {
            return new Player { Name = "Default Player", Health = 100, Score = 0 };
        }

        static Player CreatePlayer(string name)
        {
            return new Player { Name = name, Health = 100, Score = 0 };
        }

        static void DisplayCharacterInfo(Character character)
        {
            Console.WriteLine($"[INFO] {character.Name} - HP: {character.Health}");
        }

        static void HandleAnyCharacter(Character character)
        {
            Console.WriteLine($"[HANDLER] Processing character: {character.Name}");
        }

        static async Task AsynchronousExamples()
        {
            Random random = new Random();

            Console.WriteLine("\nExample 11: Async delegates with Task");

            Func<string, Task<string>> loadAssetAsync = async (assetName) =>
            {
                Console.WriteLine($"[ASYNC] Loading {assetName}...");
                await Task.Delay(random.Next(1, 4) * 1000);
                return $"{assetName} loaded successfully!";
            };

            string result1 = await loadAssetAsync("Textures");
            Console.WriteLine($"[RESULT] {result1}");

            string result2 = await loadAssetAsync("Sound Effects");
            Console.WriteLine($"[RESULT] {result2}");

            Console.WriteLine("\nExample 12: Parallel async operations");

            Func<string, int, Task<int>> simulateBattleAsync = async (enemyName, enemyHealth) =>
            {
                Console.WriteLine($"[BATTLE] Fighting {enemyName} (HP: {enemyHealth})...");
                await Task.Delay(random.Next(1, 5) * 500);
                int damageDealt = random.Next(20, 50);
                Console.WriteLine($"[BATTLE] Dealt {damageDealt} damage to {enemyName}!");
                return damageDealt;
            };

            Task<int> battle1 = simulateBattleAsync("Orc", 100);
            Task<int> battle2 = simulateBattleAsync("Skeleton", 80);
            Task<int> battle3 = simulateBattleAsync("Dragon", 500);

            int[] results = await Task.WhenAll(battle1, battle2, battle3);
            int totalDamage = results.Sum();
            Console.WriteLine($"[SUMMARY] Total damage dealt in all battles: {totalDamage}");

            Console.WriteLine("\nExample 13: Async void delegate");

            Func<string, Task> saveGameAsync = async (saveName) =>
            {
                Console.WriteLine($"[SAVE] Saving game: {saveName}...");
                await Task.Delay(random.Next(1, 5) * 500);
                Console.WriteLine($"[SAVE] Game saved successfully: {saveName}");
            };

            await saveGameAsync("AutoSave_001");
            await saveGameAsync("QuickSave_001");
        }
    }
}