using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;

namespace GameLogic
{
    /// <summary>
    /// 数据类的基类。
    /// 用于对象比较、深拷贝。
    /// </summary>
    public abstract class BaseDataClass : IEquatable<BaseDataClass>
    {
        #region 缓存

        private static readonly Dictionary<Type, TypeMemberCache> _cache = new();

        private class TypeMemberCache
        {
            public FieldInfo[] Fields;
            public PropertyInfo[] Props;
        }

        private static TypeMemberCache GetCache(Type type)
        {
            if (_cache.TryGetValue(type, out var c)) return c;

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
                .ToArray();

            c = new TypeMemberCache
            {
                Fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance),
                Props = props
            };

            _cache[type] = c;
            return c;
        }

        #endregion

        #region 深拷贝

        public T DeepCopy<T>() where T : BaseDataClass, new()
        {
            var target = new T();
            CopyTo(target);
            return target;
        }

        protected void CopyTo(BaseDataClass target)
        {
            var type = GetType();
            if (target.GetType() != type)
                throw new ArgumentException("类型不一致");

            var cache = GetCache(type);

            foreach (var f in cache.Fields)
            {
                var v = f.GetValue(this);
                f.SetValue(target, CopyValue(v));
            }

            foreach (var p in cache.Props)
            {
                var v = p.GetValue(this);
                p.SetValue(target, CopyValue(v));
            }
        }

        #endregion

        #region CopyValue

        private static readonly Dictionary<Type, MethodInfo> _methodCache = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object CopyValue(object value)
        {
            if (value == null) return null;

            var t = value.GetType();

            if (t.IsValueType || t == typeof(string))
                return value;

            if (value is BaseDataClass bdc)
            {
                var newObj = Activator.CreateInstance(t) as BaseDataClass
                             ?? throw new Exception("创建实例失败");

                bdc.CopyTo(newObj);
                return newObj;
            }

            if (t.IsArray)
                return CopyArray((Array)value, t);

            if (!t.IsGenericType)
                throw new NotSupportedException($"不支持类型: {t.FullName}");

            var def = t.GetGenericTypeDefinition();

            if (def == typeof(List<>))
                return CopyList(value, t);

            if (def == typeof(Dictionary<,>))
                return CopyDictionary(value, t);

            if (def == typeof(Queue<>))
                return CopyQueue(value, t);

            if (def == typeof(Stack<>))
                return CopyStack(value, t);

            throw new NotSupportedException($"不支持泛型类型: {t.FullName}");
        }

        #endregion

        #region 容器拷贝

        private static object CopyArray(Array arr, Type t)
        {
            var newArr = Array.CreateInstance(t.GetElementType()!, arr.Length);

            for (int i = 0; i < arr.Length; i++)
                newArr.SetValue(CopyValue(arr.GetValue(i)), i);

            return newArr;
        }

        private static object CopyList(object listObj, Type t)
        {
            var newList = (IList)Activator.CreateInstance(t)!;

            foreach (var item in (IEnumerable)listObj)
            {
                var copy = CopyValue(item);
                newList.Add(copy);
            }

            return newList;
        }

        private static object CopyDictionary(object dictObj, Type t)
        {
            var newDict = (IDictionary)Activator.CreateInstance(t)!;
            var dict = (IDictionary)dictObj;

            foreach (DictionaryEntry e in dict)
            {
                var key = CopyValue(e.Key);

                if (key == null)
                    throw new InvalidOperationException($"Dictionary Key 不允许为 null: {t.FullName}");

                var val = CopyValue(e.Value);

                newDict.Add(key, val);
            }

            return newDict;
        }

        private static object CopyQueue(object queueObj, Type t)
        {
            var queue = (IEnumerable)queueObj;
            var newQueue = Activator.CreateInstance(t)!;

            var enqueue = GetMethod(t, "Enqueue");

            foreach (var item in queue)
            {
                var copy = CopyValue(item);
                enqueue.Invoke(newQueue, new object[] { copy });
            }

            return newQueue;
        }

        private static object CopyStack(object stackObj, Type t)
        {
            var stack = (IEnumerable)stackObj;
            var newStack = Activator.CreateInstance(t)!;

            var push = GetMethod(t, "Push");

            var temp = new List<object>();

            foreach (var item in stack)
                temp.Add(item);

            for (int i = temp.Count - 1; i >= 0; i--)
            {
                var copy = CopyValue(temp[i]);
                push.Invoke(newStack, new[] { copy });
            }

            return newStack;
        }

        #endregion

        #region Method缓存

        private static MethodInfo GetMethod(Type t, string name)
        {
            if (_methodCache.TryGetValue(t, out var m))
                return m;

            m = t.GetMethod(name)!;
            _methodCache[t] = m;
            return m;
        }

        #endregion

        #region Equals

        public override bool Equals(object obj) => Equals(obj as BaseDataClass);

        public bool Equals(BaseDataClass other)
        {
            if (other == null) return false;
            if (ReferenceEquals(this, other)) return true;
            if (GetType() != other.GetType()) return false;

            var cache = GetCache(GetType());

            foreach (var f in cache.Fields)
            {
                if (!ValueEqual(f.GetValue(this), f.GetValue(other)))
                    return false;
            }

            foreach (var p in cache.Props)
            {
                if (!ValueEqual(p.GetValue(this), p.GetValue(other)))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var cache = GetCache(GetType());
            var hash = new HashCode();

            foreach (var f in cache.Fields)
                hash.Add(f.GetValue(this));

            foreach (var p in cache.Props)
                hash.Add(p.GetValue(this));

            return hash.ToHashCode();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool ValueEqual(object a, object b)
        {
            if (a == b) return true;
            if (a == null || b == null) return false;

            var t = a.GetType();
            if (t != b.GetType()) return false;

            if (t.IsValueType || t == typeof(string))
                return a.Equals(b);

            if (a is BaseDataClass da && b is BaseDataClass db)
                return da.Equals(db);

            if (a is Array arr1 && b is Array arr2)
            {
                if (arr1.Length != arr2.Length) return false;

                for (int i = 0; i < arr1.Length; i++)
                    if (!ValueEqual(arr1.GetValue(i), arr2.GetValue(i)))
                        return false;

                return true;
            }

            if (t.IsGenericType)
            {
                var def = t.GetGenericTypeDefinition();

                if (def == typeof(List<>) ||
                    def == typeof(Queue<>) ||
                    def == typeof(Stack<>))
                {
                    return SequenceEqualFast((IEnumerable)a, (IEnumerable)b);
                }

                if (def == typeof(Dictionary<,>))
                {
                    var d1 = (IDictionary)a;
                    var d2 = (IDictionary)b;

                    if (d1.Count != d2.Count) return false;

                    foreach (DictionaryEntry e in d1)
                    {
                        if (!d2.Contains(e.Key)) return false;

                        if (!ValueEqual(e.Value, d2[e.Key]))
                            return false;
                    }

                    return true;
                }
            }

            throw new NotSupportedException($"不支持类型: {t.FullName}");
        }

        private static bool SequenceEqualFast(IEnumerable a, IEnumerable b)
        {
            var ea = a.GetEnumerator();
            using var ea1 = ea as IDisposable;
            var eb = b.GetEnumerator();
            using var eb1 = eb as IDisposable;

            while (true)
            {
                var ha = ea.MoveNext();
                var hb = eb.MoveNext();

                if (ha != hb) return false;
                if (!ha) return true;

                if (!ValueEqual(ea.Current, eb.Current))
                    return false;
            }
        }

        public static bool operator ==(BaseDataClass a, BaseDataClass b)
            => Equals(a, b);

        public static bool operator !=(BaseDataClass a, BaseDataClass b)
            => !Equals(a, b);

        #endregion
    }
}