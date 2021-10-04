using System;
using System.Collections;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Metadata;
using Avalonia.Styling;
using SlimDock.Controls;

namespace SlimDock
{
    public class DockControl : TemplatedControl, ISupportInitialize
    {
        private readonly DockPaneList _list;
        private int _initializing;
        private IEnumerable? _panes;
        private DockLayoutControl? _layoutControl;
        public static readonly DirectProperty<DockControl, IEnumerable?> PanesProperty = AvaloniaProperty.RegisterDirect<DockControl, IEnumerable?>(
            "Panes", o => o.Panes, (o, v) => o.Panes = v);

        [Content]
        public IEnumerable? Panes
        {
            get => _panes;
            set => SetAndRaise(PanesProperty, ref _panes, value);
        }

        public static readonly StyledProperty<DockPaneTemplate?> PaneTemplateProperty = AvaloniaProperty.Register<DockControl, DockPaneTemplate?>(
            "PaneTemplate");

        public DockPaneTemplate? PaneTemplate
        {
            get => GetValue(PaneTemplateProperty);
            set => SetValue(PaneTemplateProperty, value);
        }

        static DockControl()
        {
            PanesProperty.Changed.Subscribe(e =>
            {
                var c = (DockControl)e.Sender;
                if (c._initializing > 0)
                    return;
                c._list.SetSource(e.NewValue.GetValueOrDefault());
            });
            PaneTemplateProperty.Changed.Subscribe(e =>
            {
                var c = (DockControl)e.Sender;
                if (c._initializing > 0)
                    return;
                c._list.SetTemplate(e.NewValue.GetValueOrDefault());
            });
        }
        
        public DockControl()
        {
            _list = new DockPaneList(this);
        }

        public override void BeginInit()
        {
            _initializing++;
            base.BeginInit();
        }

        public override void EndInit()
        {
            base.EndInit();
            _initializing--;
            if (_initializing == 0)
            {
                _list.SetTemplate(PaneTemplate);
                _list.SetSource(Panes);
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            var layout = e.NameScope.Find<DockLayoutControl>("PART_DockLayout");
            if (!ReferenceEquals(_layoutControl, layout))
            {
                _layoutControl?.SetPanes(null);
                _layoutControl = layout;
                _layoutControl?.SetPanes(_list);
            }
            base.OnApplyTemplate(e);
        }
    }
}