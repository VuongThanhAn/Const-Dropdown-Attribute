using System;
using UnityEngine;

namespace Const.Attribute.Runtime
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConstDropValueAttribute : PropertyAttribute
    {
        public Type DefaultClassType { get; }
        public Type AlternateClassType { get; }
        public string ConditionMemberName { get; } // có thể là method, field hoặc property

        public ConstDropValueAttribute(Type defaultClassType)
        {
            DefaultClassType = defaultClassType;
        }

        public ConstDropValueAttribute(Type defaultClassType, Type alternateClassType, string conditionMemberName)
        {
            DefaultClassType = defaultClassType;
            AlternateClassType = alternateClassType;
            ConditionMemberName = conditionMemberName;
        }
    }
}
