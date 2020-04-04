

using System.Collections.Generic;

namespace Lab.UICommon.Bindable
{
    public class BindableCollection<T> : Caliburn.Micro.BindableCollection<T>, IObservableCollection<T>
    {
        public BindableCollection()
        {
            IsNotifying = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Caliburn.Micro.BindableCollection`1" /> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public BindableCollection(IEnumerable<T> collection)
            : base(collection)
        {
            IsNotifying = true;
        }

        public new void AddRange(IEnumerable<T> items)
        {
            base.AddRange(items);
        }

        /// <summary>Removes the range.</summary>
        /// <param name="items">The items.</param>
        public new void RemoveRange(IEnumerable<T> items)
        {
            base.RemoveRange(items);
        }

        public new void NotifyOfPropertyChange(string propertyName)
        {
            base.NotifyOfPropertyChange(propertyName);
        }

        public new void Refresh()
        {
            base.Refresh();
        }
    }
}
