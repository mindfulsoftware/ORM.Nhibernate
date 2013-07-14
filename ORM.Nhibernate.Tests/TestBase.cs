using System;
using NHibernate;
using NHibernate.Cfg;

namespace ORM.Nhibernate {
    public abstract class BaseTests {
        public abstract string ConnectionString { get; }
        protected ISession Session;

        public BaseTests() {
            var cfg = new Configuration();
            cfg.SetProperty(NHibernate.Cfg.Environment.Dialect, typeof(NHibernate.Dialect.SQLiteDialect).AssemblyQualifiedName);
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, typeof(NHibernate.Driver.SqlClientDriver).AssemblyQualifiedName);
            cfg.SetProperty(NHibernate.Cfg.Environment.ProxyFactoryFactoryClass, typeof(NHibernate.ByteCode.Castle.ProxyFactoryFactory).AssemblyQualifiedName);
            cfg.SetProperty(NHibernate.Cfg.Environment.ConnectionString, ConnectionString);
            cfg.AddAssembly("ORM.Nhibernate");

            Session = cfg
                .BuildSessionFactory()
                .OpenSession();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~BaseTests() {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {

                if (Session != null) {
                    if (Session.IsOpen)
                        Session.Close();

                    Session.Dispose();
                    Session = null;
                }
            }
        }
    }
}
