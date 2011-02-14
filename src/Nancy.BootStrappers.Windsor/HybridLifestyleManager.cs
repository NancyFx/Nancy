#region license
// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System.Reflection;
using System.Web;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle;

namespace Nancy.BootStrappers.Windsor
{
    /// <summary>
    /// Abstract hybrid lifestyle manager, with two underlying lifestyles
    /// </summary>
    /// <typeparam name="M1">Primary lifestyle manager</typeparam>
    /// <typeparam name="M2">Secondary lifestyle manager</typeparam>
    public abstract class HybridLifestyleManager<M1, M2> : AbstractLifestyleManager
        where M1 : ILifestyleManager, new()
        where M2 : ILifestyleManager, new()
    {
        public HybridLifestyleManager() {  }
        protected readonly M1 lifestyle1 = new M1();
        protected readonly M2 lifestyle2 = new M2();

        public override void Dispose()
        {
            lifestyle1.Dispose();
            lifestyle2.Dispose();
        }

        public override void Init(IComponentActivator componentActivator, IKernel kernel, ComponentModel model)
        {
            lifestyle1.Init(componentActivator, kernel, model);
            lifestyle2.Init(componentActivator, kernel, model);
        }

        public override bool Release(object instance)
        {
            var r1 = lifestyle1.Release(instance);
            var r2 = lifestyle2.Release(instance);
            return r1 || r2;
        }

        public abstract override object Resolve(CreationContext context);
    }

    /// <summary>
    /// Hybrid lifestyle manager where the main lifestyle is <see cref = "PerWebRequestLifestyleManager" />
    /// </summary>
    /// <typeparam name = "T">Secondary lifestyle</typeparam>
    public class HybridPerWebRequestLifestyleManager<T> : HybridLifestyleManager<PerWebRequestLifestyleManager, T>
        where T : ILifestyleManager, new()
    {

        // TODO make this public in Windsor
        private static readonly PropertyInfo PerWebRequestLifestyleModuleInitialized = typeof(PerWebRequestLifestyleModule).GetProperty("Initialized", BindingFlags.Static | BindingFlags.NonPublic);

        private static bool IsPerWebRequestLifestyleModuleInitialized
        {
            get
            {
                return (bool)PerWebRequestLifestyleModuleInitialized.GetValue(null, null);
            }
        }

        public override object Resolve(CreationContext context)
        {
            if (HttpContext.Current != null && IsPerWebRequestLifestyleModuleInitialized)
                return lifestyle1.Resolve(context);
            return lifestyle2.Resolve(context);
        }
    }
}