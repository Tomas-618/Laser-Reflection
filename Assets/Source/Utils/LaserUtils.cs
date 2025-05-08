namespace Source.Utils
{
    public static class LaserUtils
    {
        public static int CalculateVerticesCount(int redirectionsCount)
        {
            int extraVerticesCount = 2;

            return 2 * redirectionsCount + extraVerticesCount;;
        }
    }
}
