﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace ShoppingWebCrawler.Host.Common.TypeFinder
{
    public interface IConstructorInvoker
    {
        object Invoke(params object[] parameters);
    }

    public class ConstructorInvoker : IConstructorInvoker
    {
        private Func<object[], object> m_invoker;

        public ConstructorInfo ConstructorInfo { get; private set; }

        public ConstructorInvoker(ConstructorInfo constructorInfo)
        {
            this.ConstructorInfo = constructorInfo;
            this.m_invoker = InitializeInvoker(constructorInfo);
        }

        private Func<object[], object> InitializeInvoker(ConstructorInfo constructorInfo)
        {
            // Target: (object)new T((T0)parameters[0], (T1)parameters[1], ...)

            // parameters to execute
            var parametersParameter = System.Linq.Expressions.Expression.Parameter(typeof(object[]), "parameters");

            // build parameter list
            var parameterExpressions = new List<System.Linq.Expressions.Expression>();
            var paramInfos = constructorInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                // (Ti)parameters[i]
                var valueObj = System.Linq.Expressions.Expression.ArrayIndex(parametersParameter, System.Linq.Expressions.Expression.Constant(i));
                var valueCast = System.Linq.Expressions.Expression.Convert(valueObj, paramInfos[i].ParameterType);

                parameterExpressions.Add(valueCast);
            }

            // new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreate = System.Linq.Expressions.Expression.New(constructorInfo, parameterExpressions);

            // (object)new T((T0)parameters[0], (T1)parameters[1], ...)
            var instanceCreateCast = System.Linq.Expressions.Expression.Convert(instanceCreate, typeof(object));

            var lambda = System.Linq.Expressions.Expression.Lambda<Func<object[], object>>(instanceCreateCast, parametersParameter);

            return lambda.Compile();
        }

        public object Invoke(params object[] parameters)
        {
            return this.m_invoker(parameters);
        }

        #region IConstructorInvoker Members

        object IConstructorInvoker.Invoke(params object[] parameters)
        {
            return this.Invoke(parameters);
        }

        #endregion
    }
    public class ConstructorInvokerFactory : IFastReflectionFactory<ConstructorInfo, IConstructorInvoker>
    {
        public IConstructorInvoker Create(ConstructorInfo key)
        {
            return new ConstructorInvoker(key);
        }

        #region IFastReflectionFactory<ConstructorInfo,IConstructorInvoker> Members

        IConstructorInvoker IFastReflectionFactory<ConstructorInfo, IConstructorInvoker>.Create(ConstructorInfo key)
        {
            return this.Create(key);
        }

        #endregion
    }
    public class ConstructorInvokerCache : FastReflectionCache<ConstructorInfo, IConstructorInvoker>
    {
        protected override IConstructorInvoker Create(ConstructorInfo key)
        {
            return FastReflectionFactories.ConstructorInvokerFactory.Create(key);
        }
    }
}
