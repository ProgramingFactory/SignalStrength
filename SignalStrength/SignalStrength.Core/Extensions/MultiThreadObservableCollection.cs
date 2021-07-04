namespace SignalStrength.Core.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    public class MultiThreadObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler collectionChanged = CollectionChanged;
            if (collectionChanged != null)
            {
                foreach (Delegate del in collectionChanged.GetInvocationList())
                {
                    var notifyCollChanged = (NotifyCollectionChangedEventHandler)del;
                    var dispatcherObj = notifyCollChanged.Target as DispatcherObject;
                    Dispatcher dispatcher = dispatcherObj?.Dispatcher;
                    if ((dispatcher != null) && !dispatcher.CheckAccess())
                    {
                        dispatcher.BeginInvoke((Action)(() => notifyCollChanged.Invoke(this,new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))), DispatcherPriority.DataBind);
                        continue;
                    }

                    notifyCollChanged.Invoke(this, e);
                }
            }
        }
    }

}