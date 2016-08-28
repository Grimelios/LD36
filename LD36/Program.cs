using System;

namespace LD36
{
#if WINDOWS || LINUX
    public static class Program
    {
        [STAThread]
        internal static void Main()
        {
			using (LDGame game = new LDGame())
			{
				game.Run();
			}
        }
    }
#endif
}
