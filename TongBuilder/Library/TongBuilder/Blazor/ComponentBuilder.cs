using System.Linq.Expressions;
using TongBuilder.Helpers;

namespace TongBuilder.Blazor
{
    /// <summary>
    /// from known:https://github.com/known/Known
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ComponentBuilder<T> where T : Microsoft.AspNetCore.Components.IComponent
    {
        //手动构建呈现器
        private readonly RenderTreeBuilder builder;
        //组件参数字典，设置组件参数时，先存入字典，在构建时批量添加
        private readonly Dictionary<string, object> Parameters = new(StringComparer.Ordinal);

        internal ComponentBuilder(RenderTreeBuilder builder)
        {
            this.builder = builder;
        }

        /// <summary>
        /// 添加组件参数方法
        /// </summary>
        /// <param name="name">组件参数名称</param>
        /// <param name="value">组件参数值</param>
        /// <returns></returns>
        public ComponentBuilder<T> Add(string name, object value)
        {
            Parameters[name] = value;
            return this;
        }

        /// <summary>
        /// 设置组件参数方法
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="selector">组件参数属性选择器表达式</param>
        /// <param name="value">组件参数值</param>
        /// <returns></returns>
        /// <remarks>
        /// 当组件属性名称更改时，可自动替换;通过表达式 c => c. 可以直接调出组件定义的属性，方便阅读;可通过TValue直接限定属性的类型，开发时即可编译检查
        /// </remarks>
        public ComponentBuilder<T> Set<TValue>(Expression<Func<T, TValue>> selector, TValue value)
        {
            //通过属性选择器表达式获取组件参数属性
            var property = TypeHelper.Property(selector);
            //添加组件参数
            return Add(property.Name, value);
        }

        public void Build(Action<T> action = null)
        {
            builder.OpenComponent<T>(0);
            if (Parameters.Count > 0)
                builder.AddMultipleAttributes(1, Parameters);
            if (action != null)
                builder.AddComponentReferenceCapture(2, value => action.Invoke((T)value));
            builder.CloseComponent();
        }
    }
}
