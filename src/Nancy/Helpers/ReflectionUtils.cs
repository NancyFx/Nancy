#region License
// Copyright (c) 2007 James Newton-King
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

namespace Nancy.Helpers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    internal static class ReflectionUtils
    {
        public static bool IsInstantiatableType(Type t)
        {
            if (t == null)
                throw new ArgumentNullException("t");

            if (t.GetTypeInfo().IsAbstract || t.GetTypeInfo().IsInterface || t.IsArray)
                return false;

            if (t.GetTypeInfo().IsGenericType)
                return false;

            if (!HasDefaultConstructor(t))
                return false;

            return true;
        }

        public static bool HasDefaultConstructor(Type t)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }

            var hasDefaultConstructor =
                t.GetTypeInfo().DeclaredConstructors.Any(ctor => !ctor.GetParameters().Any());

            return hasDefaultConstructor;
        }

        public static bool IsAssignable(Type to, Type from)
        {
            if (to == null)
            {
                throw new ArgumentNullException("to");
            }

            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            if (to.GetTypeInfo().IsGenericType && from.GetTypeInfo().IsGenericTypeDefinition)
            {
                return to.IsAssignableFrom(from.MakeGenericType(to.GetGenericArguments()));
            }

            return false;
        }

        public static bool IsSubClass(Type type, Type check)
        {
            if (type == null || check == null)
            {
                return false;
            }

            if (type == check)
            {
                return true;
            }

            if (check.GetTypeInfo().IsInterface)
            {
                foreach (Type t in type.GetInterfaces())
                {
                    if (IsSubClass(t, check)) return true;
                }
            }
            if (type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().IsGenericTypeDefinition)
            {
                if (IsSubClass(type.GetGenericTypeDefinition(), check))
                    return true;
            }
            return IsSubClass(type.GetTypeInfo().BaseType, check);
        }

        static readonly Type GenericListType = typeof(List<>);

        /// <summary>
        /// Gets the type of the typed list's items.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The type of the typed list's items.</returns>
        public static Type GetTypedListItemType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (type.IsArray)
                return type.GetElementType();
            else if (type.GetTypeInfo().IsGenericType
                         && GenericListType.IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return type.GetGenericArguments()[0];
            }
            else
            {
                throw new Exception("Bad type");
            }
        }

        public static Type GetTypedDictionaryValueType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var genDictType = GetGenericDictionary(type);

            if (genDictType != null)
            {
                return genDictType.GetGenericArguments()[1];
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return null;
            }
            else
            {
                throw new Exception("Bad type");
            }
        }

        static readonly Type GenericDictionaryType = typeof(IDictionary<,>);

        public static Type GetGenericDictionary(Type type)
        {
            if (type.GetTypeInfo().IsGenericType
                && GenericDictionaryType.IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return type;
            }

            var ifaces = type.GetInterfaces();
            if (ifaces != null)
            {
                for (int i = 0; i < ifaces.Length; i++)
                {
                    Type current = GetGenericDictionary(ifaces[i]);
                    if (current != null)
                        return current;
                }
            }

            return null;
        }

        public static Type GetMemberUnderlyingType(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).FieldType;
            }
            else if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).PropertyType;
            }
            else if (member is EventInfo)
            {
                return ((EventInfo)member).EventHandlerType;
            }
            else
            {
                throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
            }
        }


        /// <summary>
        /// Determines whether the member is an indexed property.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <returns>
        /// 	<c>true</c> if the member is an indexed property; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIndexedProperty(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            var propertyInfo = member as PropertyInfo;

            if (propertyInfo != null)
            {
                return IsIndexedProperty(propertyInfo);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the property is an indexed property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// 	<c>true</c> if the property is an indexed property; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIndexedProperty(PropertyInfo property)
        {
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }

            return (property.GetIndexParameters().Length > 0);
        }

        /// <summary>
        /// Gets the member's value on the object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target object.</param>
        /// <returns>The member's value on the object.</returns>
        public static object GetMemberValue(MemberInfo member, object target)
        {
            if (member is FieldInfo)
            {
                return ((FieldInfo)member).GetValue(target);
            }
            else if (member is PropertyInfo)
            {
                try
                {
                    return ((PropertyInfo)member).GetValue(target, null);
                }
                catch (TargetParameterCountException e)
                {
                    throw new ArgumentException("MemberInfo has index parameters", "member", e);
                }
            }
            throw new ArgumentException("MemberInfo is not of type FieldInfo or PropertyInfo", "member");
        }

        /// <summary>
        /// Sets the member's value on the target object.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="target">The target.</param>
        /// <param name="value">The value.</param>
        public static void SetMemberValue(MemberInfo member, object target, object value)
        {
            if (member is FieldInfo)
            {
                ((FieldInfo)member).SetValue(target, value);
                return;
            }
            else if (member is PropertyInfo)
            {
                ((PropertyInfo)member).SetValue(target, value, null);
                return;
            }
            throw new ArgumentException("MemberInfo must be of type FieldInfo or PropertyInfo", "member");
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be read.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be read.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be read; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanReadMemberValue(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return true;
            }
            else if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).CanRead;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified MemberInfo can be set.
        /// </summary>
        /// <param name="member">The MemberInfo to determine whether can be set.</param>
        /// <returns>
        /// 	<c>true</c> if the specified MemberInfo can be set; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanSetMemberValue(MemberInfo member)
        {
            if (member is FieldInfo)
            {
                return true;
            }
            else if (member is PropertyInfo)
            {
                return ((PropertyInfo)member).CanWrite;
            }
            return false;
        }

        public static IEnumerable<MemberInfo> GetFieldsAndProperties(Type type, BindingFlags bindingAttr)
        {

            MemberInfo[] members = type.GetFields(bindingAttr);
            for (int i = 0; i < members.Length; i++)
            {
                yield return members[i];
            }
            members = type.GetProperties(bindingAttr);
            for (int i = 0; i < members.Length; i++)
            {
                yield return members[i];
            }
        }
    }
}
