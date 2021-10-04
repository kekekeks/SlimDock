using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace SlimDock.Controls
{
    public class DockPanePresenter : TemplatedControl
    {
        private readonly DockSideSlot _slot;
        private DockPane? _pane;

        public static readonly DirectProperty<DockPanePresenter, DockPane?> PaneProperty = AvaloniaProperty.RegisterDirect<DockPanePresenter, DockPane?>(
            "Pane", o => o.Pane, (o, v) => o.Pane = v);

        private Control? _header;
        private Control? _content;

        public DockPanePresenter(DockSideSlot slot)
        {
            _slot = slot;
        }

        public DockPane? Pane
        {
            get => _pane;
            set
            {
                RemoveFromLogicalTree();
                SetAndRaise(PaneProperty, ref _pane, value);
                if (VisualRoot != null)
                    AddToLogicalTree();
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            _header = e.NameScope.Find<Control>("PART_Header");
            _content = e.NameScope.Find<Control>("PART_Content");
            var close = e.NameScope.Find<Button>("PART_Close");
            if (close != null)
                close.Click += (_, _) => _slot.SelectedPane = null;
            
            if (VisualRoot != null)
                AddToLogicalTree();
            base.OnApplyTemplate(e);
        }

        void AddToLogicalTree()
        {
            if (_pane != null)
            {
                if (_header != null)
                    _pane.GetLogicalChildren().Add(_header);
                if (_content != null)
                    _pane.GetLogicalChildren().Add(_content);
            }
        }

        void RemoveFromLogicalTree()
        {
            if (_pane != null)
            {
                _pane.GetLogicalChildren().Remove(_header!);
                _pane.GetLogicalChildren().Remove(_content!);
            }
        }
        
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            AddToLogicalTree();
            base.OnAttachedToVisualTree(e);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            RemoveFromLogicalTree();
            base.OnDetachedFromVisualTree(e);
        }
    }
}