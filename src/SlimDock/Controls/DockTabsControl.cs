using System;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace SlimDock.Controls
{
    [PseudoClasses(":left", ":right", ":bottom", ":top", ":vertical", ":horizontal")]
    public class DockTabsControl : TemplatedControl
    {
        public Dock Dock { get; }
        public DockSideSlot FirstSlot { get; }
        public DockSideSlot LastSlot { get; }

        internal DockTabsControl(Dock dock, DockSideSlot firstSlot, DockSideSlot lastSlot)
        {
            Dock = dock;
            FirstSlot = firstSlot;
            LastSlot = lastSlot;
            PseudoClasses.Set(":vertical", Dock is Dock.Top or Dock.Bottom);
            PseudoClasses.Set(":horizontal", Dock is Dock.Left or Dock.Right);
            PseudoClasses.Set(":left", Dock is Dock.Left);
            PseudoClasses.Set(":right", Dock is Dock.Right);
            PseudoClasses.Set(":top", Dock is Dock.Top);
            PseudoClasses.Set(":bottom", Dock is Dock.Bottom);
        }

        internal double MinimumCollapsedSize => 40;
        internal double MinimumExpandedSize => 80;
        
        internal bool IsExpanded { get; set; }
        internal double UserPreferredSize { get; set; }
        internal double MinimumSize => IsExpanded ? MinimumExpandedSize : MinimumCollapsedSize;
        internal double PreferredSize => Math.Max(UserPreferredSize, MinimumSize);

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            //e.NameScope.Find<>()
            base.OnApplyTemplate(e);
        }
    }
}