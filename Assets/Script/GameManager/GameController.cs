using System;

namespace UnityEngine
{
    public static class GameController
    {
        public static int characterAmount, enemyAmount;

        public static void InitLevel()
        {
            characterAmount = 1;
            enemyAmount = 1;
        }
        public static void InitLevel(int CharacterAmount, int EnemyAmount)
        {
            characterAmount = CharacterAmount;
            enemyAmount = EnemyAmount;
        }

        public static bool isWon()
        {
            return (enemyAmount == 0);
        }

        public static bool isLost()
        {
            return (characterAmount == 0);
        }

        public static void DestroyACharacter()
        {
            if (characterAmount > 0)
            {
                characterAmount--;
            }
            else throw new ArgumentOutOfRangeException();
        }

        public static void EarningACharacter()
        {
            characterAmount++;
        }

        public static void DestroyAnEnemy()
        {
            if (enemyAmount > 0)
            {
                enemyAmount--;
            }
            else throw new ArgumentOutOfRangeException();
        }

        public static void EarningAnEnemy()
        {
            enemyAmount++;
        }
    }
}
