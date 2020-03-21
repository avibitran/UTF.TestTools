using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace UTF.TestTools.Collections
{
    public class KeyedCollectionEx<TKey, TItem> : KeyedCollection<TKey, TItem>
    {
        private const string DelegateNullExceptionMessage = "Delegate passed cannot be null";
        private Func<TItem, TKey> _getKeyForItemFunction;

        public KeyedCollectionEx(Func<TItem, TKey> getKeyForItemFunction)
            : base()
        {
            if (getKeyForItemFunction == null) throw new ArgumentNullException(DelegateNullExceptionMessage);
            _getKeyForItemFunction = getKeyForItemFunction;
        }

        public KeyedCollectionEx(Func<TItem, TKey> getKeyForItemDelegate, IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
            if (getKeyForItemDelegate == null) throw new ArgumentNullException(DelegateNullExceptionMessage);
            _getKeyForItemFunction = getKeyForItemDelegate;
        }

        protected override TKey GetKeyForItem(TItem item)
        {
            return _getKeyForItemFunction(item);
        }

        public void SortByKeys()
        {
            var comparer = Comparer<TKey>.Default;
            SortByKeys(comparer);
        }

        public void SortByKeys(IComparer<TKey> keyComparer)
        {
            var comparer = new ComparerEx<TItem>((x, y) => keyComparer.Compare(GetKeyForItem(x), GetKeyForItem(y)));
            Sort(comparer);
        }

        public void SortByKeys(Comparison<TKey> keyComparison)
        {
            var comparer = new ComparerEx<TItem>((x, y) => keyComparison(GetKeyForItem(x), GetKeyForItem(y)));
            Sort(comparer);
        }

        public void Sort()
        {
            var comparer = Comparer<TItem>.Default;
            Sort(comparer);
        }

        public void Sort(Comparison<TItem> comparison)
        {
            var newComparer = new ComparerEx<TItem>((x, y) => comparison(x, y));
            Sort(newComparer);
        }

        public void Sort(IComparer<TItem> comparer)
        {
            List<TItem> list = base.Items as List<TItem>;
            if (list != null)
            {
                list.Sort(comparer);
            }
        }
    }
}
