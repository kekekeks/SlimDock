using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace SlimDock.Controls
{
    internal static class ChildHelper
    {
        public static void SetValue<T>(Control parent, ref T? child, T? value, bool addToLogicalChildren = false) where T : Control
        {
            if (ReferenceEquals(child, value))
                return;
            var children = (IAvaloniaList<IVisual>)((IVisual)parent).VisualChildren;
            var logicalChildren =  (IAvaloniaList<ILogical>)((ILogical)parent).LogicalChildren;
            if (child != null)
            {
                if (ReferenceEquals(child.GetLogicalParent(), parent))
                    ((ISetLogicalParent)child).SetParent(null);
                children.Remove(child);
                if (addToLogicalChildren)
                    logicalChildren.Remove(child);
            }

            child = value;
            if (child != null)
            {
                if(child.GetLogicalParent() == null)
                    ((ISetLogicalParent)child).SetParent(parent);
                children.Add(child);
                if(addToLogicalChildren)
                    logicalChildren.Add(child);
            }
            parent.InvalidateMeasure();
        }

        public static void AddVisualChild(Control parent, Control child, bool addToLogicalChildren)
        {
            if(child.GetLogicalParent() == null)
                ((ISetLogicalParent)child).SetParent(parent);
            var children = (IAvaloniaList<IVisual>)((IVisual)parent).VisualChildren;
            children.Add(child);
            if (addToLogicalChildren)
            {
                var logicalChildren = (IAvaloniaList<ILogical>)((ILogical)parent).LogicalChildren;
                logicalChildren.Add(child);
            }
        }

        public static void AddLogicalChild(ILogical parent, ILogical child)
        {
            if (child.GetLogicalParent() == null)
                ((ISetLogicalParent)child).SetParent(parent);
            var logicalChildren = (IAvaloniaList<ILogical>)((ILogical)parent).LogicalChildren;
            logicalChildren.Add(child);
        }
        
        public static void RemoveLogicalChild(ILogical parent, ILogical child)
        {
            if (child.GetLogicalParent() == parent)
                ((ISetLogicalParent)child).SetParent(null);
            var logicalChildren = (IAvaloniaList<ILogical>)((ILogical)parent).LogicalChildren;
            logicalChildren.Remove(child);
        }

    }
}