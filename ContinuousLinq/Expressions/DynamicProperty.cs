using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq.Expressions;

namespace ContinuousLinq.Expressions
{
    public class DynamicProperty
    {
        #region Fields

        private static readonly Dictionary<PropertyInfo, Func<object, object>> _getterDelegateCache;

        private static readonly Dictionary<PropertyInfo, DynamicProperty> _dynamicPropertyCache;

        private readonly PropertyInfo _property;
        private readonly MemberExpression _expression;

        private Func<object, object> _getterDelegate;

        #endregion

        #region Constructor

        static DynamicProperty()
        {
            _getterDelegateCache = new Dictionary<PropertyInfo, Func<object, object>>();
            _dynamicPropertyCache = new Dictionary<PropertyInfo, DynamicProperty>();
        }

        public DynamicProperty(PropertyInfo property)
        {
            _property = property;
            CreateDynamicGetterDelegate();
        }

        public DynamicProperty(MemberExpression expression) {
            _property = (PropertyInfo)expression.Member;
            _expression = expression;
            CreateDynamicGetterDelegate();
        }

        #endregion

        #region Methods

        public static DynamicProperty Create(Type sourceType, string propertyName)
        {
            PropertyInfo propertyInfo = sourceType.GetProperty(propertyName);

            return Create(propertyInfo);
        }

        public static DynamicProperty Create(PropertyInfo propertyInfo)
        {
            DynamicProperty cached;

            lock (_dynamicPropertyCache)
            {
                if (!_dynamicPropertyCache.TryGetValue(propertyInfo, out cached))
                {
                    cached = new DynamicProperty(propertyInfo);
                    _dynamicPropertyCache.Add(propertyInfo, cached);
                }
            }
            return cached;
        }

        public static DynamicProperty Create(MemberExpression expression) {
            DynamicProperty cached;
            var propertyInfo = (PropertyInfo)expression.Member;
            lock (_dynamicPropertyCache) {
                if (!_dynamicPropertyCache.TryGetValue(propertyInfo, out cached)) {
                    cached = new DynamicProperty(expression);
                    _dynamicPropertyCache.Add(propertyInfo, cached);
                }
            }
            return cached;
        }

        public object GetValue(object obj)
        {
            return _getterDelegate(obj);
        }
        private static Func<object, object> CreateDynamicExpressionGettingDelegate(MemberExpression member) {
            var parameter = member.Expression as ParameterExpression;
            if (parameter == null) {
                parameter = Expression.Parameter(member.Expression.Type, "me");
                member = Expression.MakeMemberAccess(parameter, member.Member);
            }
            var objectParamater = Expression.Parameter(typeof(object), "other");

            var casted = Expression.Convert(objectParamater, parameter.Type);
            var lambda = Expression.Lambda(member, parameter);

            var invoke = Expression.Invoke(lambda, casted);

            var oLambda = Expression.Lambda<Func<object, object>>(invoke, objectParamater);
            Func<object, object> del = oLambda.Compile();

            return del;
        }

        private static Func<object, object> CreateDynamicILGetterDelegate(PropertyInfo property) {
            MethodInfo getterMethodInfo = property.GetGetMethod();

            if (getterMethodInfo == null)
                throw new InvalidOperationException("No getter method found on property");

            DynamicMethod dynamicMethod = new DynamicMethod(
                string.Empty,
                typeof(object),
                new[] { typeof(object) });

            ILGenerator il = dynamicMethod.GetILGenerator();

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, property.DeclaringType);

            il.EmitCall(OpCodes.Callvirt, getterMethodInfo, null);

            if (property.PropertyType.IsValueType) {
                il.Emit(OpCodes.Box, property.PropertyType);
            }

            il.Emit(OpCodes.Ret);

            var getter = (Func<object, object>)dynamicMethod.CreateDelegate(typeof(Func<object, object>));

            return getter;
        }


        private void CreateDynamicGetterDelegate()
        {
            lock (_getterDelegateCache)
            {
                if (_getterDelegateCache.TryGetValue(_property, out _getterDelegate))
                {
                    return;
                }
                if (_expression != null)
                    _getterDelegate = CreateDynamicExpressionGettingDelegate(_expression);
                else
                    _getterDelegate = CreateDynamicILGetterDelegate(_property);

                _getterDelegateCache.Add(_property, _getterDelegate);
            }
        }

        internal static void ClearCaches()
        {
            _dynamicPropertyCache.Clear();
            _getterDelegateCache.Clear();
        }
        #endregion
    }
}
