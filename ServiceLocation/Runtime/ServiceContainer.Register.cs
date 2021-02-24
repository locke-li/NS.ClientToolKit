/***************************************************************

 *  类名称：        ServiceContainer

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/19 16:46:07

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/


using System;
using System.Reflection;

namespace CenturyGame.ServiceLocation.Runtime
{
    public sealed partial class ServiceContainer
    {
		//--------------------------------------------------------------
		#region Fields
		//--------------------------------------------------------------

		#endregion
		
		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------

		#endregion
		
		//--------------------------------------------------------------
		#region Creation & Cleanup
		//--------------------------------------------------------------

		#endregion
		
		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------

        public void Register<T>(string key , object instance)
        {
            ThrowIfDispose();
            Register(typeof(T) , key , instance);
        }

        public void Register(Type serviceType, string key, object instance)
        {
            ThrowIfDispose();
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");

            if (instance == null)
                throw new ArgumentNullException("instance");

            if (!serviceType.IsInstanceOfType(instance))
            {
                throw new InvalidOperationException($"The instance(name : {instance.GetType().Name}) is not assignable to the serviceType (name : {serviceType.Name}) reference.");
            }

            ServiceRegistration registration = new ServiceRegistration(serviceType , key);

            if (this._registry.TryGetValue(registration,out var entry))
            {
                throw new InvalidOperationException($"The service that type is \"{serviceType.Name}\" is already registed in container .");
            }

            object CreateInstance(ServiceContainer container) => instance;

            RegisterInternal(registration,CreateInstance);
        }


        public void Register<T>(string key , Func<T> create)
        {
            ThrowIfDispose();
            if (create == null)
                throw new ArgumentNullException("create");

            var serviceType = typeof(T);

            ServiceRegistration registration = new ServiceRegistration(serviceType, key);

            if (this._registry.TryGetValue(registration, out var entry))
            {
                throw new InvalidOperationException($"The service that type is \"{serviceType.Name}\" is already registed in container .");
            }

            object CreateInstanceFunc(ServiceContainer container) => create();

            RegisterInternal(registration , CreateInstanceFunc);
        }


        private void RegisterInternal(ServiceRegistration registration , Func<ServiceContainer, object> createInstance)
        {
            var entry = new ServiceEntry(createInstance);

            this._registry[registration] = entry;
        }

        #endregion

    }
}
