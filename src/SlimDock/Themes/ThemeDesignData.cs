using Avalonia.Collections;

namespace SlimDock.Themes
{
    public class ThemeDesignData
    {
        public static DockControl DesignInstance => Create();

        
        private static DockControl Create()
        {
            var lst = new AvaloniaList<DockPane>();
            for (var slot = DockLayoutSlot.LeftTop; slot <= DockLayoutSlot.TopLeft; slot++)
            {
                for (var c = 1; c < 4; c++)
                {
                    var name = slot + " " + c;
                    lst.Add(new DockPane
                    {
                        Content = name + " content",
                        HeaderContent = name + " header",
                        PaneTabHeaderContent = name,
                        InitialLayoutSlot = slot
                    });
                }
            }

            return new DockControl { Panes = lst };
        }
    }
}