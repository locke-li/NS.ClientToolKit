/***************************************************************

 *  类名称：        ResourcePool

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/11 10:57:19

 *  最后修改人：

 *  版权所有 （C）:   CenturyGames

***************************************************************/

using CenturyGame.Core.Functional;
using System;
using System.Collections.Generic;

namespace CenturyGame.Core.ResourcePools
{
    public abstract class ResourcePool
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------
        private static readonly List<ResourcePool> PoolsInternal = new List<ResourcePool>();

        private static event EventHandler<EventArgs> _ClearingAllEvent = (sender,e)=>{};

        #endregion

        //--------------------------------------------------------------

        #region Properties & Events

        //--------------------------------------------------------------

        public static bool Enabled
        {
            get { return _enabled; }

            set
            {
                if (_enabled != value)
                {
                    ClearAll();
                    _enabled = false;
                }
            }
        }
        private static bool _enabled = true;


        internal static event EventHandler<EventArgs> ClearingAll
        {
            add
            {
                lock (_ClearingAllEvent)
                {
                    _ClearingAllEvent += value;
                }
            }
            remove
            {
                lock (_ClearingAllEvent)
                {
                    _ClearingAllEvent -= value;
                }
            }
        }

        #endregion

        //--------------------------------------------------------------

        #region Creation & Cleanup

        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------

        #region Methods

        //--------------------------------------------------------------

        internal static void Register(ResourcePool pool)
        {
            lock (PoolsInternal)
            {
                PoolsInternal.Add(pool);
            }
        }

        internal static void UnRegister(ResourcePool pool)
        {
            lock (PoolsInternal)
            {
                PoolsInternal.Remove(pool);
            }
        }

        public abstract void Clear();

        public static void ClearAll()
        {
            lock (PoolsInternal)
            {
                OnClearingAll(EventArgs.Empty);
                PoolsInternal.ForeachCall(x => x.Clear());
            }
        }


        private static void OnClearingAll(EventArgs eventArgs)
        {
            _ClearingAllEvent?.Invoke(null, eventArgs);
        }

        #endregion

    }
}
