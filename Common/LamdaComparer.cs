using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities.Common
{
    public class LambdaComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> lambdaComparer;
        private readonly Func<T, int> lambdaHash;
        public LambdaComparer(Func<T, T, bool> lambdaComparer) :
            this(lambdaComparer, o => o.GetHashCode())
        {
        }
        public LambdaComparer(Func<T, T, bool> lambdaComparer, Func<T, int> lambdaHash)
        {
            if (lambdaComparer == null)
                throw new ArgumentNullException("lambdaComparer error JukeWeb.Foundry.Utilities.Common > LambdaComparer");
            if (lambdaHash == null)
                throw new ArgumentNullException("lambdaHash error JukeWeb.Foundry.Utilities.Common > LambdaComparer");
            this.lambdaComparer = lambdaComparer;
            this.lambdaHash = lambdaHash;
        }
        public bool Equals(T x, T y)
        {
            return lambdaComparer(x, y);
        }
        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
