using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace TongBuilder.Helpers;

/// <summary>
///  from known:https://github.com/known/Known
/// </summary>
public sealed class TypeHelper
{
    private TypeHelper() { }

    public static object GetPropertyValue(object model, string name)
    {
        if (model == null || string.IsNullOrWhiteSpace(name))
            return default;

        var property = model.GetType().GetProperty(name);
        if (property == null || !property.CanRead)
            return default;

        return property.GetValue(model);
    }   

    /// <summary>
    /// 通过属性选择器表达式来获取指定类型的属性
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static PropertyInfo Property<T, TValue>(Expression<Func<T, TValue>> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        if (selector.Body is not MemberExpression expression || expression.Member is not PropertyInfo propInfoCandidate)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        var type = typeof(T);
        var propertyInfo = propInfoCandidate.DeclaringType != type
                         ? type.GetProperty(propInfoCandidate.Name, propInfoCandidate.PropertyType)
                         : propInfoCandidate;
        if (propertyInfo is null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        return propertyInfo;
    }

    public static PropertyInfo Property<T>(Expression<Func<T, object>> selector)
    {
        if (selector is null)
            throw new ArgumentNullException(nameof(selector));

        var expression = GetMemberExpression(selector);
        if (expression == null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        var propInfoCandidate = expression.Member as PropertyInfo;
        if (propInfoCandidate == null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        var type = typeof(T);
        var propertyInfo = propInfoCandidate.DeclaringType != type
                         ? type.GetProperty(propInfoCandidate.Name)
                         : propInfoCandidate;
        if (propertyInfo is null)
            throw new ArgumentException($"The parameter selector '{selector}' does not resolve to a public property on the type '{typeof(T)}'.", nameof(selector));

        return propertyInfo;
    }

    private static MemberExpression GetMemberExpression<T>(Expression<Func<T, object>> selector)
    {
        var member = selector.Body as MemberExpression;
        if (member != null)
            return member;

        var unary = selector.Body as UnaryExpression;
        return unary != null ? unary.Operand as MemberExpression : null;
    }

    public static Type CreateType(Dictionary<string, Type> keyValues)
    {
        var assemblyName = new AssemblyName("DynamicAssembly");
        var dyAssembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
        var dyModule = dyAssembly.DefineDynamicModule("DynamicModule");
        var dyClass = dyModule.DefineType("MyDyClass", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass);
        foreach (var item in keyValues)
        {
            var fb = dyClass.DefineField("_" + item.Key, item.Value, FieldAttributes.Private);

            var gmb = dyClass.DefineMethod("get_" + item.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, item.Value, Type.EmptyTypes);
            var getIL = gmb.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Ldfld, fb);
            getIL.Emit(OpCodes.Ret);

            var smb = dyClass.DefineMethod("set_" + item.Key, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { item.Value });
            var setIL = smb.GetILGenerator();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Stfld, fb);
            setIL.Emit(OpCodes.Ret);

            var pb = dyClass.DefineProperty(item.Key, PropertyAttributes.HasDefault, item.Value, null);
            pb.SetGetMethod(gmb);
            pb.SetSetMethod(smb);
        }
        return dyClass.CreateTypeInfo();
    }
}