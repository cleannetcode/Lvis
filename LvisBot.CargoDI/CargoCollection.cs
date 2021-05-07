using System;
using System.Collections.Generic;

namespace LvisBot.CargoDI
{
    public class CargoCollection : IDisposable, IEnumerable<Type>
    {
        Dictionary<Type, Cargo> _typeDict;
        public CargoCollection()
        {
            _typeDict = new Dictionary<Type, Cargo>(TypeComparer.Comparer);
        }
        public void Dispose()
        {
            foreach (var c in _typeDict)
            {
                c.Value.Dispose();
            }
            _typeDict = null;
        }

        public IEnumerator<Type> GetEnumerator()
        {
            var tempEn = _typeDict.GetEnumerator();
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
            if (_typeDict.TryGetValue(type, out var cargo))
            {
                var ttype = typeof(T);
                if (!_typeDict.TryAdd(ttype, cargo))
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
        public void RegisterSingleton<T, TImpl>(Func<CargoCollection, TImpl> builder) where TImpl : class, T where T : class
        {
            RegisterSingleton(builder);
            RegisterImplementation<T, TImpl>();
        }
        public void RegisterSingleton<T, TImpl>(TImpl item) where TImpl : class, T where T : class => RegisterSingleton<T, TImpl>(b => item);
        public void Register<T>(Func<CargoCollection, T> builder, Pattern pattern) where T : class
        {
            AddObject(typeof(T), () => builder(this), pattern);
        }
        public void Register<T, TImpl>(Func<CargoCollection, TImpl> builder, Pattern pattern) where TImpl : class, T where T : class
        {
            Register(builder, pattern);
            RegisterImplementation<T, TImpl>();
        }
        public void RegisterSingleton<T>() where T : class, new() => Register<T>(Pattern.Singleton);
        public void RegisterSingleton<T>(Func<CargoCollection,T> builder) where T : class => Register(builder, Pattern.Singleton);
        public void RegisterSingleton<T>(T item) where T : class
        {
            AddObject(typeof(T), () => item, Pattern.Singleton);
        }
        private void AddObject(Type type, Func<object> builder, Pattern pattern)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var error = !_typeDict.TryAdd(type, new Cargo(builder, pattern));
            if (error)
            {
                throw new ArgumentException($"{type.Name} has already been added");
            }
        }
        #endregion
        public T GetObject<T>() where T : class
        {
            var type = typeof(T);
            if (_typeDict.TryGetValue(type, out var cargo))
            {
                return cargo.GetInstance() as T;
            }
            else
            {
                throw new ArgumentException($"{type.Name} was not found");
            }
        }

        //все интерфейсы также ссылаются на один контейнер
        private class Cargo : IDisposable
        {
            object _singletonInst;
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
                    if (_singletonInst == null)
                    {
                        _singletonInst = builder();
                    }
                    return _singletonInst;
                }
                return builder();
            }
            public void Dispose()
            {
                (_singletonInst as IDisposable)?.Dispose();
                _singletonInst = null;
            }
        }
    }
}
