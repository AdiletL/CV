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
            return 10 * regeneration;
        }
    }
}