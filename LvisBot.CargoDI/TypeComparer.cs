using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LvisBot.CargoDI
{
    //так как по дефолту сравниваются ссылки у Type, а они всегда разные
    public class TypeComparer : IEqualityComparer<Type>
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
}