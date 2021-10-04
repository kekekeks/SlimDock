using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;
using System;

namespace SlimDock.Controls
{
    // This is a dummy control that supplies layout information to its parent
    /*
    public sealed class DockSideControlInfo : Control
    {
        DockSideControl? _parent;
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _parent = this.GetVisualAncestors().OfType<DockSideControl>().FirstOrDefault();
            _parent?.UpdateTabBounds(TabBounds);
        }

        public static readonly StyledProperty<Rect> TabBoundsProperty = AvaloniaProperty.Register<DockSideControlInfo, Rect>(
            "TabBounds");

        public Rect TabBounds
        {
            get => GetValue(TabBoundsProperty);
            set => SetValue(TabBoundsProperty, value);
        }

        static DockSideControlInfo()
        {
            TabBoundsProperty.Changed.Subscribe(e =>
            {
                ((DockSideControlInfo)e.Sender)._parent?.UpdateTabBounds(e.NewValue.GetValueOrDefault());
            });
        }
    }*/
}