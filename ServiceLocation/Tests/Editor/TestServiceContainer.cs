using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CenturyGame.LoggerModule.Runtime;
using CenturyGame.Log4NetForUnity.Runtime;
using CenturyGame.ServiceLocation.Runtime;
using CommonServiceLocator;
using Log4Net.Unity;
using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tests
{
    public class TestServiceContainer
    {
        #region Services

        public interface IServiceA
        {
            void PrintName();

            void Print(string msg);
        }


        public class ServiceA : IServiceA , IDisposable
        {
            private string name;
            public ServiceA(string name)
            {
                this.name = name;
            }

            public void PrintName()
            {
                var logger = LoggerManager.GetLogger("ServiceA.PrintName");
                logger.Debug($"ServiceA : {this.name} .");
            }

            public void Print(string msg)
            {
                var logger = LoggerManager.GetLogger("ServiceA.Print");
                logger.Info(msg);
            }

            public void Dispose()
            {
                var logger = LoggerManager.GetLogger("ServiceA.Print");
                logger.Info($"{ this.name}'s dispose is invoke !");
            }
        }


        public class ServiceB: IServiceA, IDisposable
        {
            private string name;
            public ServiceB(string name)
            {
                this.name = name;
            }

            public void PrintName()
            {
                var logger = LoggerManager.GetLogger("ServiceB.PrintName");
                logger.Debug($"ServiceB : {this.name} .");
            }

            public void Print(string msg)
            {
                var logger = LoggerManager.GetLogger("ServiceB.Print");
                logger.Info(msg);
            }

            public void Dispose()
            {
                var logger = LoggerManager.GetLogger("ServiceB.Print");
                logger.Info($"{ this.name}'s dispose is invoke !");
            }
        }

        #endregion

        private ServiceContainer GetServiceContainer()
        {
            var initLogger = typeof(RuntimeInitializer).GetMethod("InitLog4Net", BindingFlags.Static | BindingFlags.NonPublic);
            initLogger.Invoke(null, null);

            LoggerManager.SetCurrentLoggerProvider(new Log4NetLoggerProvider());

            var container = new ServiceContainer();
            ServiceLocator.SetLocatorProvider(() => container);
            return container;
        }

        [Test]
        public void RegisterGeneric()
        {
            var container = GetServiceContainer();
            container.Register<IServiceA>("ServiceA", new ServiceA("RegisterGeneric.ServiceA"));
        }

        [Test]
        public void RegisterGenericNoKey()
        {
            var container = GetServiceContainer();
            container.Register<IServiceA>(null, new ServiceA("RegisterGenericNoKey.ServiceA"));
        }


        [Test]
        public void RegisterNormal()
        {
            var container = GetServiceContainer();
            container.Register(typeof(IServiceA), "ServiceA", new ServiceA("RegisterNormal.ServiceA"));
        }

        [Test]
        public void RegisterNoKey()
        {
            var container = GetServiceContainer();
            container.Register(typeof(IServiceA), null, new ServiceA("RegisterNormal.ServiceA"));
        }

        [Test]
        public void GetInstanceGeneric()
        {

            var container = GetServiceContainer();

            container.Register<IServiceA>("ServiceA", new ServiceA("A"));
            var serviceA = container.GetInstance<IServiceA>("ServiceA") as IServiceA;
            serviceA.PrintName();

            container.Register<IServiceA>(null, new ServiceA("B"));
            //container.Register<IServiceA>("ServiceA", new ServiceA("A"));

            serviceA = container.GetInstance<IServiceA>() as IServiceA;
            serviceA.PrintName();

            container.Dispose();
        }


        [Test]
        public void GetInstanceRByType()
        {
            var container = GetServiceContainer();

            container.Register<IServiceA>("ServiceA", new ServiceA("A"));
            var serviceA = container.GetInstance<IServiceA>("ServiceA") as IServiceA;
            serviceA.PrintName();

            container.Register<IServiceA>(null, new ServiceA("B"));
            //container.Register<IServiceA>("ServiceA", new ServiceA("A"));

            serviceA = container.GetInstance<IServiceA>() as IServiceA;
            serviceA.PrintName();

            container.Dispose();
        }

        [Test]
        public void GetAllInstancesGeneric()
        {
            var container = GetServiceContainer();

            container.Register<IServiceA>("ServiceA", new ServiceA("A"));
            container.Register<IServiceA>(null, new ServiceA("A no key"));

            container.Register<IServiceA>("ServiceB", new ServiceB("B"));
            //container.Register<IServiceA>(null, new ServiceB("B no key"));

            var services = container.GetAllInstances<IServiceA>();
            Debug.Log($"service length : {services.ToArray().Length}");
            foreach (var service in services)
            {
                service.PrintName();
            }

            container.Dispose();
        }


        [Test]
        public void GetAllInstancesNormal()
        {
            var container = GetServiceContainer();

            container.Register(typeof(IServiceA),"ServiceA", new ServiceA("A"));
            container.Register(typeof(IServiceA),null, new ServiceA("A no key"));

            container.Register(typeof(IServiceA),"ServiceB", new ServiceB("B"));
            //container.Register<IServiceA>(null, new ServiceB("B no key"));

            var services = container.GetAllInstances(typeof(IServiceA));
            Debug.Log($"service length : {services.ToArray().Length}");
            foreach (var service in services)
            {   
                ((IServiceA)service).PrintName();
            }

            container.Dispose();
        }

    }
}
