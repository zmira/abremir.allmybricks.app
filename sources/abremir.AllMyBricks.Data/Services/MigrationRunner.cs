using System;
using System.Linq;
using System.Reflection;
using abremir.AllMyBricks.Data.Interfaces;
using LiteDB;

namespace abremir.AllMyBricks.Data.Services
{
    public class MigrationRunner : IMigrationRunner
    {
        public void ApplyMigrations(ILiteDatabase liteDatabase)
        {
            var currentUserVersion = liteDatabase.UserVersion;

            var instances = (from types in Assembly.GetAssembly(typeof(MigrationRunner)).GetTypes()
                             where types.GetInterfaces().Contains(typeof(IMigration)) && types.GetConstructor(Type.EmptyTypes) is not null
                             select Activator.CreateInstance(types) as IMigration)
                            .Where(instance => instance.MigrationId > currentUserVersion)
                            .OrderBy(instance => instance.MigrationId);

            if (!instances.Any())
            {
                return;
            }

            foreach (var instance in instances)
            {
                instance.Apply(liteDatabase);
            }
        }
    }
}
