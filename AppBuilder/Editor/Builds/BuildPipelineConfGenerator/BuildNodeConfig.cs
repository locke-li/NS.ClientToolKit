/***************************************************************

 *  类名称：        BuildNodeConfig

 *  描述：				

 *  作者：          Chico(wuyuanbing)

 *  创建时间：      2020/6/9 17:04:54

 *  最后修改人：

 *  版权所有 （C）:   diandiangames

***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CenturyGame.AppBuilder.Editor.Builds.Actions;
using CenturyGame.AppBuilder.Runtime.BuildPipelineConfGenerator;
using CenturyGame.Core.Pipeline;

namespace CenturyGame.AppBuilder.Editor.Builds.BuildPipelineConfGenerator
{
    public static class BuildNodeBaseTypeDic
    {
        public readonly static Dictionary<BuildNodeType, Type> Data = new Dictionary<BuildNodeType, Type>
        {
            { BuildNodeType.Action,typeof(BaseBuildFilterAction)},
            {BuildNodeType.Filter,typeof(BasePipelineFilter)}
        };
    }
    

}
