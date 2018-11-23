namespace Specification.Expressions.Impl
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;

    public class ObservableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly Dictionary<TKey, object> inner = new Dictionary<TKey, object>();

        public IReadOnlyDictionary<TKey, object> Inner => this.inner;
        
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public new TValue this[TKey key]
        {
            get => (TValue)this.Inner[key];

            set
            {
                bool exist = this.Inner.TryGetValue(key, out object oldValueObj);
                TValue oldValue = (TValue)oldValueObj;
                var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
                this.inner[key] = value;
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                if (exist)
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, this.Inner.Keys.ToList().IndexOf(key)));
                }
                else
                {
                    this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem, this.Inner.Keys.ToList().IndexOf(key)));
                    this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }

        public new void Add(TKey key, TValue value)
        {
            if (!this.Inner.ContainsKey(key))
            {
                var item = new KeyValuePair<TKey, TValue>(key, value);
                this.inner.Add(key, value);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, this.Inner.Keys.ToList().IndexOf(key)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            }
        }

        public new bool Remove(TKey key)
        {
            if (this.Inner.TryGetValue(key, out object value))
            {                
                var item = new KeyValuePair<TKey, TValue>(key, (TValue)value);
                bool result = this.inner.Remove(key);
                this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, this.Inner.Keys.ToList().IndexOf(key)));
                this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                return result;
            }

            return false;
        }

        public new void Clear()
        {
            this.inner.Clear();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            this.OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        public int Count => this.Inner.Count;
    }
}