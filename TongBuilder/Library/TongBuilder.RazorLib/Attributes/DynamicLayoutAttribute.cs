using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TongBuilder.RazorLib.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DynamicLayoutAttribute : Attribute
    {
        public Type LayoutType { get; private set; }
    }
}
