using Ninject;

namespace AudioManager10.DependencyInjector
{
    public class NinjectKernel
    {
        private static IKernel _standardKernel;

        public static IKernel StandardKernel
        {
            get
            {
                if (_standardKernel != null) return _standardKernel;
                _standardKernel = new StandardKernel();
                _standardKernel.Load("AudioManager10.*.dll");
                _standardKernel.Load("NAudioWrapper.dll");
                return _standardKernel;
            }
        }
    }
}