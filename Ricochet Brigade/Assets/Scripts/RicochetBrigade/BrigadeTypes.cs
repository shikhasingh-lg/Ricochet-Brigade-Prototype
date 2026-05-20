using UnityEngine;

namespace RicochetBrigade
{
    public enum BrigadeColor
    {
        Red,
        Blue,
        Yellow,
        Purple
    }

    public enum HeroTier
    {
        Bronze = 1,
        Silver = 2,
        Gold = 3
    }

    public static class BrigadeTuning
    {
        public const int Columns = 8;
        public const int Rows = 12;
        public const float CellSize = 1f;
        public const float MinLaunchSpeed = 5.5f;
        public const float MaxLaunchSpeed = 17.5f;
        public const float SettleSpeed = 0.65f;
        public const float MinFlightTime = 0.35f;
        public const int UnitCap = 24;

        public static Color HeroColor(BrigadeColor color)
        {
            switch (color)
            {
                case BrigadeColor.Red:
                    return new Color(1f, 0.22f, 0.18f);
                case BrigadeColor.Blue:
                    return new Color(0.2f, 0.52f, 1f);
                case BrigadeColor.Yellow:
                    return new Color(1f, 0.88f, 0.16f);
                case BrigadeColor.Purple:
                    return new Color(0.72f, 0.28f, 1f);
                default:
                    return Color.white;
            }
        }

        public static string ColorName(BrigadeColor color)
        {
            return color.ToString();
        }

        public static float HeroRangeCells(BrigadeColor color)
        {
            switch (color)
            {
                case BrigadeColor.Red:
                    return 3f;
                case BrigadeColor.Blue:
                    return 5f;
                case BrigadeColor.Yellow:
                    return 12f;
                default:
                    return 4f;
            }
        }

        public static float HeroDamagePerSecond(BrigadeColor color, HeroTier tier)
        {
            float baseDamage;
            switch (color)
            {
                case BrigadeColor.Red:
                    baseDamage = 26f;
                    break;
                case BrigadeColor.Blue:
                    baseDamage = 17f;
                    break;
                case BrigadeColor.Yellow:
                    baseDamage = 10f;
                    break;
                default:
                    baseDamage = 12f;
                    break;
            }

            return baseDamage * (int)tier;
        }

        public static float EnemyHealth(BrigadeColor color, bool boss)
        {
            if (boss)
            {
                return 1000f;
            }

            switch (color)
            {
                case BrigadeColor.Red:
                    return 60f;
                case BrigadeColor.Blue:
                    return 100f;
                case BrigadeColor.Yellow:
                    return 150f;
                default:
                    return 220f;
            }
        }

        public static int EnemyBaseDamage(BrigadeColor color, bool boss)
        {
            if (boss)
            {
                return 35;
            }

            switch (color)
            {
                case BrigadeColor.Yellow:
                    return 15;
                default:
                    return 10;
            }
        }

        public static BrigadeColor RandomHeroColor()
        {
            int roll = Random.Range(0, 3);
            if (roll == 0)
            {
                return BrigadeColor.Red;
            }

            return roll == 1 ? BrigadeColor.Blue : BrigadeColor.Yellow;
        }
    }
}
