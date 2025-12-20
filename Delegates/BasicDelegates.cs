using System;

namespace BasicDelegates
{
    class Program
    {
        delegate void GameAction(string playerName);
        delegate int DamageCalculator(int baseDamage);
        delegate void PowerUpEffect(string effectName);

        static void Main(string[] args)
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
    }
}