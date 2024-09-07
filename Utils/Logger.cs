using EntityCX.Atoms.Restorables.Script;

namespace EntityCX.EntityUtils
{
    public static class Logger
    {
        public static bool Enabled { get; } = true;
        public static void Message(string msg)
        {
            if (AppearanceSwapper.Logger.val)
                // if (Enabled)
                SuperController.LogMessage(msg);
        }
        public static void Error(string msg)
        {
            if (AppearanceSwapper.Logger.val)

                // if (Enabled)
                SuperController.LogError(msg);
        }
    }
}