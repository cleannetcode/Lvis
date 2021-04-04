using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace YouTubeChatBot
{
    //так как по дефолту сравниваются ссылки у Type, а они всегда разные
    class TypeComparer : IEqualityComparer<Type>
    {
        public static readonly TypeComparer Comparer = new TypeComparer();
        private TypeComparer() { }
        public bool Equals([AllowNull] Type x, [AllowNull] Type y)
        {
            return x.FullName.Equals(y.FullName);
        }
        public int GetHashCode([DisallowNull] Type obj)
        {
            return obj.FullName.GetHashCode();
        }
    }
    class DICargo : IDisposable, IEnumerable<Type>
    {
        Dictionary<Type, Cargo> typeDict;
        public DICargo()
        {
            typeDict = new Dictionary<Type, Cargo>(TypeComparer.Comparer);
        }
        public void Dispose()
        {
            foreach (var c in typeDict)
            {
                c.Value.Dispose();
            }
            typeDict = null;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            var tempEn = typeDict.GetEnumerator();
            while (tempEn.MoveNext())
            {
                yield return tempEn.Current.Key;
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #region register methods
        public void Register<T>(Pattern pattern) where T: class, new()
        {
            AddObject(typeof(T), () => new T(), pattern);
        }
        public void Register<T, TImpl>(Pattern pattern) where TImpl : class, T, new() where T : class
        {
            Register<TImpl>(pattern);
            RegisterImplementation<T, TImpl>();
        }
        public void RegisterImplementation<T, TImpl>()
            where T : class
            where TImpl : class, T
        {
            var type = typeof(TImpl);
            if (typeDict.TryGetValue(type, out var cargo))
            {
                var ttype = typeof(T);
                if (!typeDict.TryAdd(ttype, cargo))
                {
                    throw new ArgumentException($"{ttype.Name} has already been added");
                }
            }
            else
            {
                throw new ArgumentException($"{type.Name} was not added");
            }
        }
        public void RegisterSingleton<T, TImpl>() where TImpl : class, T, new() where T : class => RegisterSingleton<T, TImpl>(b => new TImpl());
        public void RegisterSingleton<T, TImpl>(Func<DICargo, TImpl> builder) where TImpl : class, T where T : class
        {
            RegisterSingleton(builder);
            RegisterImplementation<T, TImpl>();
        }
        public void RegisterSingleton<T, TImpl>(TImpl item) where TImpl : class, T where T : class => RegisterSingleton<T, TImpl>(b => item);
        public void Register<T>(Func<DICargo, T> builder, Pattern pattern) where T : class
        {
            AddObject(typeof(T), () => builder(this), pattern);
        }
        public void Register<T, TImpl>(Func<DICargo, TImpl> builder, Pattern pattern) where TImpl : class, T where T : class
        {
            Register(builder, pattern);
            RegisterImplementation<T, TImpl>();
        }
        public void RegisterSingleton<T>() where T : class, new() => Register<T>(Pattern.Singleton);
        public void RegisterSingleton<T>(Func<DICargo,T> builder) where T : class => Register(builder, Pattern.Singleton);
        public void RegisterSingleton<T>(T item) where T : class
        {
            AddObject(typeof(T), () => item, Pattern.Singleton);
        }
        private void AddObject(Type type, Func<object> builder, Pattern pattern)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var error = !typeDict.TryAdd(type, new Cargo(builder, pattern));
            if (error)
            {
                throw new ArgumentException($"{type.Name} has already been added");
            }
        }
        #endregion
        public T GetObject<T>() where T : class
        {
            var type = typeof(T);
            if (typeDict.TryGetValue(type, out var cargo))
            {
                return cargo.GetInstance() as T;
            }
            else
            {
                throw new ArgumentException($"{type.Name} was not found");
            }
        }

        //все интерфейсы также ссылаются на один контейнер
        class Cargo : IDisposable
        {
            object singletonInst;
            bool isSingleton;
            Func<object> builder;
            public Cargo(Func<object> builder, Pattern pattern)
            {
                this.builder = builder;
                isSingleton = pattern == Pattern.Singleton;
            }
            public object GetInstance()
            {
                if (isSingleton)
                {
                    if (singletonInst == null)
                    {
                        singletonInst = builder();
                    }
                    return singletonInst;
                }
                return builder();
            }
            public void Dispose()
            {
                (singletonInst as IDisposable)?.Dispose();
                singletonInst = null;
            }
        }
    }
    enum Pattern
    {
        Singleton,
        Prototype
    }
}
