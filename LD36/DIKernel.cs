using Ninject;

namespace LD36
{
	internal static class DIKernel
	{
		private static IKernel kernel = new StandardKernel();

		public static void Bind<T>()
		{
			kernel.Bind<T>().ToSelf().InSingletonScope();
		}

		public static void Bind<T>(T constant)
		{
			kernel.Bind<T>().ToConstant(constant);
		}

		public static T Get<T>()
		{
			return kernel.Get<T>();
		}
	}
}
