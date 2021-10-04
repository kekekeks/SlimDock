using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Layout;

namespace SlimDock.Controls
{
    [PseudoClasses(":vertical", ":horizontal")]
    public sealed class DockSplitterControl : Control
    {
        internal double? UserDefinedSize { get; set; }
        // todo: internal
        public bool PrioritizeLast { get; set; }
        public bool DivideExtraSizeEqually { get; set; }
        private Orientation _orientation;

        public static readonly DirectProperty<DockSplitterControl, Orientation> OrientationProperty =
            AvaloniaProperty.RegisterDirect<DockSplitterControl, Orientation>(
                "Orientation", o => o.Orientation,
                (o, v) => throw new InvalidOperationException("Property is read-only"));

        public Orientation Orientation
        {
            get => _orientation;
            // todo: internal
            set
            {
                SetAndRaise(OrientationProperty, ref _orientation, value);
                PseudoClasses.Set(":vertical", Orientation == Orientation.Vertical);
                PseudoClasses.Set(":horizontal", Orientation == Orientation.Horizontal);
                
            }
        }

        private readonly Thumb _thumb = new();
        private Control? _first;
        private Control? _last;
        //todo: internal
        public Control? First
        {
            get => _first;
            set => ChildHelper.SetValue(this, ref _first, value);
        }
        //todo: internal
        public Control? Last
        {
            get => _last;
            set => ChildHelper.SetValue(this, ref _last, value);
        }

        public DockSplitterControl()
        {
            ChildHelper.SetValue(this, ref _thumb!, new Thumb());
            _thumb.PointerMoved += OnThumbPointerMoved;
            ClipToBounds = true;
        }

        private void OnThumbPointerMoved(object sender, PointerEventArgs e)
        {
            var pt = e.GetCurrentPoint(this);
            if (pt.Properties.IsLeftButtonPressed)
            {
                var self = new OrientedControl(this, Orientation);
                var thumb = self.Wrap(_thumb);
                var pos = Orientation == Orientation.Horizontal ? pt.Position.X : pt.Position.Y;
                if (PrioritizeLast)
                    UserDefinedSize = Math.Max(0, self.Bounds.Width - (pos + thumb.DesiredWidth / 2));
                else
                    UserDefinedSize = Math.Max(0, pos - thumb.DesiredWidth / 2);
                InvalidateArrange();
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Due to a issue in Avalonia (as of 0.10.7) not measuring children
            // after flipping IsVisible from MeasureOverride causes layout loops
            // So we are doing it here
            _thumb.IsVisible = First?.IsVisible == true && Last?.IsVisible == true;
            _thumb.ZIndex = 2;
            _thumb.Measure(availableSize);
            
            if (First?.IsVisible != true && Last?.IsVisible != true)
                return default;

            if (First?.IsVisible == true && Last?.IsVisible == true)
            {
                (First.ZIndex, Last.ZIndex) = PrioritizeLast ? (0, 1) : (1, 0);
                
                var (self, first, second, thumb, available) = Get(availableSize);

                if (PrioritizeLast)
                    (first, second) = (second, first);
                
                var thumbSize = thumb.DesiredSize;
                available = available.WithWidth(available.Width - thumbSize.Width);

                var firstSize = first.Measure(available);
                available = available.WithWidth(available.Width - firstSize.Width);
                var lastSize = second.Measure(available);
                var desiredWidth = available.Width;
                if (double.IsInfinity(desiredWidth))
                    desiredWidth = firstSize.Width + thumbSize.Width + lastSize.Width;
                
                return self.Flip(new Size(desiredWidth,
                    Math.Max(firstSize.Height, lastSize.Height)
                ));
            }

            // There is only one child, show it without a splitter
            
            var child = First?.IsVisible == true ? First : Last;
            child!.Measure(availableSize);
            return child.DesiredSize;
        }

        private (OrientedControl, OrientedControl, OrientedControl, OrientedControl, Size) Get(Size s)
        {
            var self = new OrientedControl(this, Orientation);
            var first = self.Wrap(First);
            var last = self.Wrap(Last);
            var thumb = self.Wrap(_thumb);
            return (self, first, last, thumb, self.Flip(s));
        }

        static (double, double) DecideSize(double firstMin, double secondMin, double available, double userPreference)
        {
            var first = Math.Max(firstMin, userPreference);
            var second = Math.Max(available - first, secondMin);
            first = available - second;
            return (first, second);

        }
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            if (_thumb.IsVisible)
            {
                var (self, first, last, thumb, size) = Get(finalSize);
                var availableSizeWithoutThumb = size.Width - thumb.DesiredWidth;
                var extraSize = availableSizeWithoutThumb - first.DesiredWidth - last.DesiredWidth;
                if (extraSize > 0)
                {
                    var firstSize = first.DesiredWidth;
                    var lastSize = last.DesiredWidth;
                    
                    if (UserDefinedSize == null)
                    {
                        if (DivideExtraSizeEqually)
                        {
                            firstSize += extraSize / 2;
                            lastSize += extraSize / 2;
                        }
                        else if (PrioritizeLast)
                            firstSize += extraSize;
                        else
                            lastSize += extraSize;
                    }
                    else if (!PrioritizeLast)
                        (firstSize, lastSize) = DecideSize(firstSize, lastSize,
                            availableSizeWithoutThumb, UserDefinedSize.Value);
                    else
                        (lastSize, firstSize) = DecideSize(lastSize, firstSize,
                            availableSizeWithoutThumb, UserDefinedSize.Value);
                    
                    first.Arrange(0, 0, firstSize, size.Height);
                    thumb.Arrange(first.Bounds.Width, 0, thumb.DesiredWidth, size.Height);
                    last.Arrange(thumb.Bounds.Right, 0,
                        Math.Min(lastSize, size.Width - thumb.Bounds.Right), size.Height);
                }
                // Not enough space, priority is for the first element
                else if (!PrioritizeLast)
                {
                    first.Arrange(0, 0, first.MinimumWidthAcceptedByArrangeCore, size.Height);
                    thumb.Arrange(first.Bounds.Right, 0, thumb.MinimumWidthAcceptedByArrangeCore, size.Height);
                    last.Arrange(thumb.Bounds.Right, 0, last.MinimumWidthAcceptedByArrangeCore, size.Height);
                }
                // Not enough space, priority is for the second element
                else
                {
                    last.Arrange(size.Width - last.MinimumWidthAcceptedByArrangeCore, 0, last.MinimumWidthAcceptedByArrangeCore, size.Height);
                    thumb.Arrange(last.Bounds.Left - thumb.MinimumWidthAcceptedByArrangeCore, 0, thumb.MinimumWidthAcceptedByArrangeCore, size.Height);
                    first.Arrange(0, 0, first.MinimumWidthAcceptedByArrangeCore, size.Height);

                }
            }
            else
            {
                var ch = First?.IsVisible == true ? First : Last?.IsVisible == true ? Last : null;
                if (ch != null)
                    ch.Arrange(new Rect(finalSize));
            }
            return finalSize;
        }
    }
}