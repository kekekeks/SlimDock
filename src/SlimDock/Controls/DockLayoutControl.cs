using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace SlimDock.Controls
{
    public class DockLayoutControl : Control
    {
        static readonly AvaloniaList<DockPane> EmptyList = new();
        
        // TODO: user customization
        private readonly Dock[] _priorities = new[] { Dock.Left, Dock.Right, Dock.Bottom, Dock.Top };
        readonly DockSideControl[] _sides = new DockSideControl[4];
        //readonly DockSplitterControl[] _splitters = new DockSplitterControl[4];
        TextBlock _content = new TextBlock { Text = "Content", Background = Brushes.Magenta };
        private DockPaneList? _panes;
        private readonly List<DockSideSlot> _slots = new(); 

        public DockLayoutControl()
        {
            for (var s = Dock.Left; s <= Dock.Top; s++)
            {
                var side = _sides[(int)s] = new DockSideControl(s); 
                ChildHelper.AddVisualChild(this, side, true);
                _slots.Add(side.FirstSlot);
                _slots.Add(side.LastSlot);
            }
            ChildHelper.AddVisualChild(this, _content, true);
/*
            for (var c = 0; c < _splitters.Length; c++)
                ((ISetLogicalParent)(_splitters[c] = new DockSplitterControl())).SetParent(this);
            VisualChildren.Add(_splitters[0]);*/
            //Rebuild();
        }

        internal void SetPanes(DockPaneList? panes)
        {
            if(ReferenceEquals(_panes, panes))
                return;
            if (_panes != null)
            {
                _panes.Added -= OnPaneAdded;
                _panes.Removed -= OnPaneRemoved;
                _panes.Replaced -= OnPaneReplaced;
                foreach(var p in _panes)
                    OnPaneRemoved(p);
            }
            _panes = panes;
            if (_panes != null)
            {
                _panes.Added += OnPaneAdded;
                _panes.Removed += OnPaneRemoved;
                _panes.Replaced += OnPaneReplaced;
                foreach(var p in _panes)
                    OnPaneAdded(p);
            }
        }

        private void OnPaneReplaced(DockPane oldPane, DockPane newPane)
        {
            var slot = FindPaneSlot(oldPane);
            if (slot != null)
            {
                var idx = slot.PanesList.IndexOf(oldPane);
                slot.PanesList[idx] = newPane;
            }
            else
            // Shouldn't happen, but whatever
                OnPaneAdded(newPane);
        }

        private void OnPaneRemoved(DockPane pane) => FindPaneSlot(pane)?.PanesList.Remove(pane);

        private void OnPaneAdded(DockPane pane) => GetSlot(pane.InitialLayoutSlot).PanesList.Add(pane);

        DockSideSlot? FindPaneSlot(DockPane pane)
        {
            foreach(var slot in _slots)
                if (slot.Panes.Contains(pane))
                    return slot;
            return null;
        }
        
        DockSideSlot GetSlot(DockLayoutSlot slot)
        {
            var (dock, second) = slot switch
            {
                DockLayoutSlot.LeftTop => (Dock.Left, false),
                DockLayoutSlot.LeftBottom => (Dock.Left, true),
                DockLayoutSlot.BottomLeft => (Dock.Bottom, false),
                DockLayoutSlot.BottomRight => (Dock.Bottom, true),
                DockLayoutSlot.RightTop => (Dock.Right, false),
                DockLayoutSlot.RightBottom => (Dock.Right, true),
                DockLayoutSlot.TopLeft => (Dock.Top, false),
                DockLayoutSlot.TopRight => (Dock.Top, true)
            };
            var side = _sides[(int)dock];
            return second ? side.LastSlot : side.FirstSlot;
        }

        
        (double left, double right, double center) CalculateSizes(double available, double minimumLeft, double preferredLeft,
            double minimumRight, double preferredRight, double minimumCenter)
        {
            // This happens if we don't have any panes on both sides
            if (preferredLeft == 0 && preferredRight == 0)
                return (0, 0, available);
            
            // Happy path
            if (available - preferredRight - preferredLeft - minimumCenter > 0)
                return (preferredLeft, preferredRight, available - preferredRight - preferredLeft);
            
            // Acceptable path
            if (available - minimumLeft - minimumRight - minimumCenter > 0)
            {
                var availableWithoutMinimums = available - minimumCenter - minimumLeft - minimumRight;

                var extraLeft = preferredLeft - minimumLeft;
                var extraRight = preferredRight - minimumRight;
                
                // TODO: Potential NaN somewhere here
                
                var totalExtra = extraLeft + extraRight;
                var ratioLeft = extraLeft / totalExtra;
                var rationRight = extraRight / totalExtra;

                extraLeft = availableWithoutMinimums * ratioLeft;
                extraRight = availableWithoutMinimums * rationRight;
                return (minimumLeft + extraLeft, minimumRight + extraRight, minimumCenter);
            }

            // Return minimums
            return (minimumLeft, minimumRight, minimumCenter);

        }

        CalculatedLayout CalculateLayout(Size size)
        {
            var leftSide = _sides[(int)Dock.Left]
                .PreMeasure(size.Width, size.Height);
            var rightSide = _sides[(int)Dock.Right]
                .PreMeasure(size.Width, size.Height);
            var bottomSide = _sides[(int)Dock.Bottom]
                .PreMeasure(size.Height, size.Width);
            var topSide = _sides[(int)Dock.Top]
                .PreMeasure(size.Height, size.Width);
            var (left, right, centerWidth) = CalculateSizes(size.Width,
                leftSide.MinimumSize, leftSide.PreferredSize, rightSide.MinimumSize, rightSide.PreferredSize,
                _content.DesiredSize.Width);
            var (top, bottom, centerHeight) = CalculateSizes(size.Height,
                topSide.MinimumSize, topSide.PreferredSize, bottomSide.MinimumSize, bottomSide.PreferredSize,
                _content.DesiredSize.Height);
            return new CalculatedLayout
            {
                Left = left,
                Right = right,
                Top = top,
                Bottom = bottom
            };
        }

        struct CalculatedLayout
        {
            public double Left, Right, Top, Bottom;
        }

        CalculatedLayout _layout;
        protected override Size MeasureOverride(Size constraint)
        {
            _layout = CalculateLayout(constraint);
            var occupied = new Size();
            for (var c = Dock.Left; c <= Dock.Top; c++)
            {
                var side = _sides[(int)c];
                // We ignore what panel desires but are still required to call Measure
                var size = (c == Dock.Left || c == Dock.Right)
                    ? new Size(c == Dock.Left ? _layout.Left : _layout.Right, constraint.Height)
                    : new Size(constraint.Width, c == Dock.Top ? _layout.Top : _layout.Bottom);
                side.Measure(size);

                occupied += side.DesiredSize;
                /*
                var requiredSize = side.MinimumSize;
                if (c is Dock.Left or Dock.Right)
                    occupied += new Size(requiredSize, 0);
                else
                    occupied += new Size(0, requiredSize);*/
            }

            var newConstraint = new Size(Math.Max(0, constraint.Width - occupied.Width),
                Math.Max(0, constraint.Height - occupied.Height));
            _content.Measure(newConstraint);
            return newConstraint + occupied;
        }
        
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            /*
            var leftSide = _sides[(int)Dock.Left]; 
            var rightSide = _sides[(int)Dock.Right]; 
            var bottomSide = _sides[(int)Dock.Bottom]; 
            var topSide = _sides[(int)Dock.Top];
            
            var (left, right, centerWidth) = CalculateSizes(finalSize.Width,
                leftSide.MinimumSize, leftSide.PreferredSize, rightSide.MinimumSize, rightSide.PreferredSize,
                _content.DesiredSize.Width);
            var (top, bottom, centerHeight) = CalculateSizes(finalSize.Height,
                topSide.MinimumSize, topSide.PreferredSize, bottomSide.MinimumSize, bottomSide.PreferredSize,
                _content.DesiredSize.Height);
*/
            var (left, right, top, bottom) = (_layout.Left, _layout.Right, _layout.Top, _layout.Bottom);

            var contentRect = new Rect(finalSize);
            for (var c = 0; c < _priorities.Length; c++)
            {
                var dock = _priorities[c];
                var side = _sides[(int)dock];
                side.ZIndex = 5 - c;
                /*
                side.HorizontalAlignment = dock == Dock.Right
                    ? HorizontalAlignment.Right
                    : dock == Dock.Left
                        ? HorizontalAlignment.Left
                        : HorizontalAlignment.Stretch;
                side.VerticalAlignment = dock == Dock.Bottom
                    ? VerticalAlignment.Bottom
                    : dock == Dock.Top
                        ? VerticalAlignment.Top
                        : VerticalAlignment.Stretch;*/
                if (dock == Dock.Left)
                {
                    //side.Measure(new Size(left, contentRect.Height));
                    side.Arrange(new Rect(contentRect.Left, contentRect.Top, left, contentRect.Height));
                    contentRect = new Rect(contentRect.Left + left, contentRect.Top, contentRect.Width - left,
                        contentRect.Height);
                }
                else if (dock == Dock.Right)
                {
                    //side.Measure(new Size(right, contentRect.Height));
                    side.Arrange(new Rect(contentRect.Right - right, contentRect.Top, right, contentRect.Height));
                    contentRect = new Rect(contentRect.Left, contentRect.Top, contentRect.Width - right,
                        contentRect.Height);
                }
                else if (dock == Dock.Top)
                {
                    //side.Measure(new Size(contentRect.Width, top));
                    side.Arrange(new Rect(contentRect.Left, contentRect.Top, contentRect.Width, top));
                    contentRect = new Rect(contentRect.Left, contentRect.Top + top, contentRect.Width,
                        contentRect.Height - top);
                }
                else if (dock == Dock.Bottom)
                {
                    //side.Measure(new Size(contentRect.Width, bottom));
                    side.Arrange(new Rect(contentRect.Left, contentRect.Bottom - bottom, contentRect.Width, bottom));
                    contentRect = new Rect(contentRect.Left, contentRect.Top, contentRect.Width,
                        contentRect.Height - bottom);
                }
                if (contentRect.Width < 0)
                    contentRect = contentRect.WithWidth(0);
                if (contentRect.Height < 0)
                    contentRect.WithHeight(0);
            }
            
            _content.HorizontalAlignment = HorizontalAlignment.Stretch;
            _content.VerticalAlignment = VerticalAlignment.Stretch;
            _content.Arrange(new Rect(contentRect.Left, contentRect.Top, Math.Max(0, contentRect.Width),
                Math.Max(0, contentRect.Height)));
            try
            {
                //_skipMeasure = true;
                //Measure(finalSize);
            }
            finally
            {
                //_skipMeasure = false;
            }
            return finalSize;
        }
    }
}