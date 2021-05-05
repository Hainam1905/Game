namespace UnityEngine
{
    public static class CustomTime {
        public static float LocalTimeScale = 1f;
        public static float deltaTime
        {
            get
            {
                return Time.deltaTime * LocalTimeScale;
            }
        }

        public static bool IsPaused
        {
            get
            {
                return LocalTimeScale == 0f;
            }
        }

        public static float TimeScale
        {
            get
            {
                return Time.timeScale * LocalTimeScale;
            }
        }
    }
}
