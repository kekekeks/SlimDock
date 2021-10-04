using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.VisualTree;

namespace SlimDock.Controls
{
    [PseudoClasses(":left", ":right", ":bottom", ":top", ":vertical", ":horizontal")]
    class DockSideControl : Control
    {
        public Dock Dock { get; }
        private DockTabsControl _tabs;
        private readonly Thumb _thumb;
        private readonly DockSplitterControl _splitter;
        private double? _userPreferredSize;
        private const double _minumumExpandedSize = 40;

        public DockSideSlot FirstSlot { get; }
        public DockSideSlot LastSlot { get; }
        private Orientation _orientation;
        private readonly DockPanePresenter _firstSlotPresenter;
        private readonly DockPanePresenter _lastSlotPresenter;

        public DockSideControl(Dock dock)
        {
            FirstSlot = new DockSideSlot();
            LastSlot = new DockSideSlot();
            _firstSlotPresenter = new DockPanePresenter(FirstSlot);
            _lastSlotPresenter = new DockPanePresenter(LastSlot);
            Dock = dock;
            PseudoClasses.Set(":vertical", Dock is Dock.Top or Dock.Bottom);
            PseudoClasses.Set(":horizontal", Dock is Dock.Left or Dock.Right);
            PseudoClasses.Set(":left", Dock is Dock.Left);
            PseudoClasses.Set(":right", Dock is Dock.Right);
            PseudoClasses.Set(":top", Dock is Dock.Top);
            PseudoClasses.Set(":bottom", Dock is Dock.Bottom);
            
            _tabs = new DockTabsControl(dock, FirstSlot, LastSlot);
            _thumb = new Thumb();
            _splitter = new DockSplitterControl
            {
                Orientation = dock is Dock.Bottom or Dock.Top ? Orientation.Horizontal : Orientation.Vertical,
                First = _firstSlotPresenter,
                Last = _lastSlotPresenter,
                DivideExtraSizeEqually = true
            };
            _orientation = dock is Dock.Bottom or Dock.Top ? Orientation.Vertical : Orientation.Horizontal;
            foreach (var slot in new[] { FirstSlot, LastSlot })
                slot.GetObservable(DockSideSlot.SelectedPaneProperty).Subscribe(_ => UpdateSlots());
            
            foreach (var c in new Control[] { _tabs, _splitter, _thumb })
            {
                VisualChildren.Add(c);
                LogicalChildren.Add(c);
                ((ISetLogicalParent)c).SetParent(this);
            }
            UpdateSlots();
        }
        
        private void UpdateSlots()
        {
            _firstSlotPresenter.Pane = FirstSlot.SelectedPane;
            _lastSlotPresenter.Pane = LastSlot.SelectedPane;
            _splitter.First = FirstSlot.SelectedPane == null ? null : _firstSlotPresenter;
            _splitter.Last = LastSlot.SelectedPane == null ? null : _lastSlotPresenter;
            _thumb.IsVisible = _splitter.IsVisible = FirstSlot.SelectedPane != null || LastSlot.SelectedPane != null;
            ((ILayoutable)this.GetVisualParent())?.InvalidateMeasure();
            ((ILayoutable)this.GetVisualParent())?.InvalidateArrange();
        }

        (OrientedControl self, OrientedControl tabs, OrientedControl content, OrientedControl thumb) Get() =>
        (
            new OrientedControl(this, _orientation),
            new OrientedControl(_tabs, _orientation),
            new OrientedControl(_splitter, _orientation),
            new OrientedControl(_thumb, _orientation));
        

        internal (double MinimumSize, double PreferredSize) PreMeasure(double availableForAllSlots, double otherDimension)
        {
            var (self, tabs, content, thumb) = Get();
            var availableSize = self.Flip(new Size(availableForAllSlots, otherDimension));
            tabs.Measure(new Size(double.PositiveInfinity, otherDimension));
            if (!_splitter.IsVisible)
                return (tabs.DesiredWidth, tabs.DesiredWidth);
            {
                var preferredSize = Math.Max(_minumumExpandedSize,
                    _userPreferredSize ?? (availableSize.Width - tabs.DesiredWidth) / 4);
                content.Measure(new Size(preferredSize, availableSize.Height));
                thumb.Measure(new Size(double.PositiveInfinity, availableSize.Height));
                return (tabs.DesiredWidth + _minumumExpandedSize,
                    tabs.DesiredWidth + content.DesiredWidth + thumb.DesiredWidth);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (!_splitter.IsVisible)
            {
                _tabs.Measure(availableSize);
                return _tabs.DesiredSize;
            }
            
            var (self, tabs, content, thumb) = Get();
            availableSize = self.Flip(availableSize);
            tabs.Measure(availableSize);

            thumb.Measure(availableSize);
            availableSize =
                availableSize.WithWidth(Math.Max(0, availableSize.Width - tabs.DesiredWidth - thumb.DesiredWidth));
            content.Measure(availableSize);


            return self.Flip(new Size(content.DesiredWidth + tabs.DesiredWidth + thumb.DesiredWidth,
                availableSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (!_splitter.IsVisible)
            {
                _tabs.Arrange(new Rect(default, finalSize));
                return finalSize;
            }
            
            var (self, tabs, content, thumb) = Get();
            var size = self.Flip(finalSize);
            var l = new ReversedLayout(size.Width, Dock is Dock.Bottom or Dock.Right);

            l.Arrange(tabs, 0, 0, tabs.DesiredWidth, size.Height);
            l.Arrange(thumb, size.Width - thumb.DesiredWidth, 0, thumb.DesiredWidth, size.Height);
            l.Arrange(content, tabs.DesiredWidth, 0,
                Math.Max(0, size.Width - tabs.DesiredWidth - thumb.DesiredWidth), size.Height);

            return finalSize;
        }
    }
}