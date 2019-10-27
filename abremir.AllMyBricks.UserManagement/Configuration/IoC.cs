using abremir.AllMyBricks.UserManagement.Interfaces;
using abremir.AllMyBricks.UserManagement.Services;
using SimpleInjector;

namespace abremir.AllMyBricks.UserManagement.Configuration
{
    public static class IoC
    {
        public static Container Configure(Container container = null)
        {
            container = container ?? new Container();

            container.Register<IUserService, UserService>(Lifestyle.Transient);

            return container;
        }
    }
}
