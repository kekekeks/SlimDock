using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using SlimDock.Controls;

namespace SlimDock
{
    class DockPaneList : IEnumerable<DockPane>
    {
        private readonly ILogical _logicalParent;
        public event Action<DockPane>? Added;
        public event Action<DockPane>? Removed;
        public event Action<DockPane, DockPane>? Replaced;
        private INotifyCollectionChanged? _collectionNotify;
        private ICollection? _source;
        private readonly Dictionary<object, DockPane> _panes = new Dictionary<object, DockPane>();
        private IDockPaneTemplate? _template;

        public IEnumerable<DockPane> Panes => _panes.Values;

        public DockPaneList(ILogical logicalParent)
        {
            _logicalParent = logicalParent;
        }

        DockPane CreateFor(object obj) => _template?.Build() ?? new DockPane();
        
        public void SetSource(IEnumerable? en)
        {
            if(ReferenceEquals(_source, en))
                return;
            
            if (en != null && en is not ICollection)
                en = en.Cast<object>().ToList();

            if (_collectionNotify != null)
                _collectionNotify.CollectionChanged -= OnCollectionChanged;
            _collectionNotify = null;
            _source = (ICollection?)en;
            if (en is INotifyCollectionChanged collectionNotify)
            {
                _collectionNotify = collectionNotify;
                _collectionNotify.CollectionChanged += OnCollectionChanged;
            }
            HandleReset();
        }

        public void SetTemplate(IDockPaneTemplate? template)
        {
            _template = template;
            foreach (var kv in _panes.ToList())
            {
                if(kv.Key is DockPane)
                    continue;
                var oldPane = kv.Value;
                var newPane = CreateFor(kv.Key);
                ChildHelper.RemoveLogicalChild(_logicalParent, oldPane);
                ChildHelper.AddLogicalChild(_logicalParent, newPane);
                _panes[kv.Key] = newPane;
                Replaced?.Invoke(oldPane, newPane);
            }
        }

        void HandleAdd(object obj)
        {
            if (!(obj is DockPane pane))
            {
                pane = CreateFor(obj);
                pane.DataContext = obj;
            }
            ChildHelper.AddLogicalChild(_logicalParent, pane);
            _panes.Add(obj, pane);
            Added?.Invoke(pane);
        }

        void HandleRemove(object obj)
        {
            if (_panes.TryGetValue(obj, out var pane))
            {
                _panes.Remove(obj);
                ChildHelper.RemoveLogicalChild(_logicalParent, pane);
                Removed?.Invoke(pane);
            }
        }
        
        private void HandleReset()
        {
            while (_panes.Count > 0)
            {
                var pane = _panes.First();
                HandleRemove(pane.Key);
            }
            if(_source != null)
                foreach (var item in _source)
                    HandleAdd(item);
        }
        
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Reset)
                HandleReset();
            // Since panels are user-draggable, we don't really care about the order of panes in the bound collection and
            // Only use it for the initial layout
            if(e.Action == NotifyCollectionChangedAction.Move)
                return;
            
            if (e.OldItems != null)
                foreach (var i in e.OldItems)
                    HandleRemove(i);
            
            if (e.NewItems != null)
                foreach (var i in e.NewItems)
                    HandleAdd(i);
        }

        public IEnumerator<DockPane> GetEnumerator()
        {
            if(_source == null)
                yield break;
            foreach (var i in _source)
                yield return _panes[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}