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

            Console.ReadKey();
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