/***************************************************************

 *  类名称：        AppBuilderPipelineProcessor

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/4/24 20:10:38

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/


using System;
using System.Text;
using CenturyGame.AppBuilder.Runtime.Configuration;
using CenturyGame.Core.Pipeline;
using YamlDotNet;
using File = System.IO.File;
using UnityEngine;
using CenturyGame.AppBuilder.Editor.Builds.Contexts;
using CenturyGame.LoggerModule.Runtime;
using CenturyGame.AppBuilder.Editor.Builds.InnerLoggers;

namespace CenturyGame.AppBuilder.Editor.Builds
{
    public class AppBuilderPipelineProcessor : BasePipelineProcessor
    {
        //--------------------------------------------------------------
        #region Fields
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Properties & Events
        //--------------------------------------------------------------

        public override IPipelineContext Context { get; } = new AppBuildContext();

        #endregion

        //--------------------------------------------------------------
        #region Creation & Cleanup
        //--------------------------------------------------------------

        #endregion

        //--------------------------------------------------------------
        #region Methods
        //--------------------------------------------------------------

        public override ProcessResult Process(IPipelineInput input)
        {
            if (!this.TestAll(input))
            {
                return ProcessResult.Create(ProcessState.Error, $"Test failure , error info : {Context.Error} .");
            }

            return base.Process(input);
        }

        /// <summary>
        /// 根据编译过程配置初始化管线处理器
        /// </summary>
        /// <param name="config">编译过程配置</param>
        public void InitFormConfig(AppBuildProcessConfig config)
        {
            foreach (var filterConfig in config.Filters)
            {
                var filterType = Type.GetType(filterConfig.TypeFullName);
                var instance = (IFilter)System.Activator.CreateInstance(filterType);
                this.Register(instance);
                if (filterConfig.Action.IsActionQueue)
                {
                    var queueActionsFilter = instance as QueueActionsPipelineFilter;
                    foreach (var childAction in filterConfig.Action.Childs)
                    {
                        var actionType = Type.GetType(childAction.TypeFullName);
                        var actionInst = (IPipelineFilterAction)Activator.CreateInstance(actionType);
                        queueActionsFilter.Enqueue(actionInst);
                    }
                }
                else
                {
                    var actionType = Type.GetType(filterConfig.Action.TypeFullName);
                    var actionInst = (IPipelineFilterAction)Activator.CreateInstance(actionType);
                    BasePipelineFilter basePipelineFilter = instance as BasePipelineFilter;
                    basePipelineFilter.SetAction(actionInst);
                }
            }
        }

        private static AppBuildProcessConfig GetAppBuildProcessConfig(string configPath)
        {
            string content = File.ReadAllText(configPath, new UTF8Encoding(false, true));
            AppBuildProcessConfig config = YAMLSerializationHelper.DeSerialize<AppBuildProcessConfig>(content);
            return config;
        }

        /// <summary>
        /// 读取编译过程配置并初始化管线处理器
        /// </summary>
        /// <param name="configPath">编译过程配置的路径</param>
        /// <returns></returns>
        public static AppBuilderPipelineProcessor ReadFromBuildProcessConfig(string configPath)
        {
            var config = GetAppBuildProcessConfig(configPath);

            if (config == null)
            {
                var logger = LoggerManager.GetLogger(typeof(AppBuilderPipelineProcessor).Name);
                logger.Error($"Get build process config failure ! Config path : {configPath} .");
                return null;
            }

            var processor = new AppBuilderPipelineProcessor();
            
            processor.InitFormConfig(config);

            return processor;
        }

        #endregion

    }
}
