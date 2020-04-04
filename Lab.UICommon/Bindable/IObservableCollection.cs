using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Lab.UICommon.Bindable
{
    public interface IObservableCollection<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, INotifyPropertyChangedEx, INotifyPropertyChanged, INotifyCollectionChanged
    {
        /// <summary>Adds the range.</summary>
        /// <param name="items">The items.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>Removes the range.</summary>
        /// <param name="items">The items.</param>
        void RemoveRange(IEnumerable<T> items);
    }
}
