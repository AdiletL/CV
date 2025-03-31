namespace Calculate
{
    public static class Convert
    {
        public static float AttackSpeedToDuration(float attackSpeed)
        {
            return 100 / attackSpeed;
        }

        public static float RegenerationToRate(float regeneration)
        {
            return 2 * regeneration;
        }

        public static float EvasionToStat(float evasion)
        {
            return 100 * evasion;
        }
    }
}