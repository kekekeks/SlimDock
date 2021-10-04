using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace SlimDock.Controls
{
    public class DockSideRotateControl : Decorator
    {
        private Dock _dock;

        public static readonly DirectProperty<DockSideRotateControl, Dock> DockProperty =
            AvaloniaProperty.RegisterDirect<DockSideRotateControl, Dock>(
                "Dock", o => o.Dock, (o, v) => o.Dock = v);

        public Dock Dock
        {
            get => _dock;
            set => SetAndRaise(DockProperty, ref _dock, value);
        }

        Size Flip(Size s) => Dock is Dock.Left or Dock.Right ? new Size(s.Height, s.Width) : s;

        protected override Size MeasureOverride(Size availableSize) =>
            Flip(LayoutHelper.MeasureChild(Child, Flip(availableSize), default, default));

        protected override Size ArrangeOverride(Size finalSize)
        {
            var rc = new Rect(Flip(finalSize));
            if (Dock == Dock.Left)
                rc = rc.WithY(finalSize.Height);
            if (Dock == Dock.Right)
                rc = rc.WithX(finalSize.Width);
            Child?.Arrange(rc);

            return finalSize;
        }

        static DockSideRotateControl()
        {
            ChildProperty.Changed.AddClassHandler<DockSideRotateControl>((x, e) => x.ApplyTransform());
            DockProperty.Changed.AddClassHandler<DockSideRotateControl>((x, _) => x.ApplyTransform());
        }

        private void ApplyTransform()
        {
            if (Child != null)
            {
                Child.RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative);
                Child.RenderTransform = new RotateTransform(
                    Dock == Dock.Left ? -90 : Dock == Dock.Right ? 90 : 0);
            }
        }
    }
}