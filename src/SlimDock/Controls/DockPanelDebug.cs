using Avalonia;
using Avalonia.Controls;

namespace SlimDock.Controls
{
    public class DockPanelDebug : DockPanel
    {
        protected override Size ArrangeOverride(Size arrangeSize)
        {
            
            return base.ArrangeOverride(arrangeSize);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override void OnMeasureInvalidated()
        {
            base.OnMeasureInvalidated();
        }
    }
}