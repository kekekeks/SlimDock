using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace SlimDock.Controls
{
    struct OrientedControl
    {
        private readonly Control Control;
        public readonly Orientation Orientation;

        public OrientedControl(Control control, Orientation orientation)
        {
            Control = control;
            Orientation = orientation;
        }

        public Size Flip(Size s)
        {
            if (Orientation == Orientation.Vertical)
                return new Size(s.Height, s.Width);
            return s;
        }

        public Rect Flip(Rect r)
        {
            if (Orientation == Orientation.Vertical)
                return new Rect(r.Top, r.Left, r.Height, r.Width);
            return r;
        }

        public Size DesiredSize => Flip(Control.DesiredSize);
        public double DesiredWidth => DesiredSize.Width;
        public double MinWidth => Orientation == Orientation.Horizontal ? Control.MinWidth : Control.MinHeight;
        public double MinimumWidthAcceptedByArrangeCore => Math.Max(DesiredWidth, MinWidth);
        public Size Measure(Size availableSize)
        {
            Control.Measure(Flip(availableSize));
            return DesiredSize;
        }

        public void Arrange(Rect r) => Control.Arrange(Flip(r));

        public void Arrange(double x, double y, double w, double h) =>
            Arrange(new Rect(x, y, Math.Max(0, w), Math.Max(0, h)));
        public Rect Bounds => Flip(Control.Bounds);
        public OrientedControl Wrap(Control other) => new OrientedControl(other, Orientation);
    }

    struct ReversedLayout
    {
        private readonly double _size;
        private readonly bool _reverseX;

        public ReversedLayout(double size, bool reverseX)
        {
            _size = size;
            _reverseX = reverseX;
        }

        public void Arrange(OrientedControl control, double x, double y, double w, double h)
        {
            if (_reverseX)
                control.Arrange(_size - x - w, y, w, h);
            else
                control.Arrange(x, y, w, h);
        }
    }
}