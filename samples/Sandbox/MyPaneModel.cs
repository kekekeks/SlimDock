using System.Collections.Generic;
using Avalonia.Collections;
using SlimDock;
using SlimDock.Controls;

namespace Sandbox
{
    public class MyPaneModel
    {
        public DockLayoutSlot Slot { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string Tab { get; set; }

        public static AvaloniaList<MyPaneModel> DesignItems
        {
            get
            {
                var lst = new AvaloniaList<MyPaneModel>();
                for (var slot = DockLayoutSlot.LeftTop; slot <= DockLayoutSlot.TopLeft; slot++)
                {
                    for (var c = 1; c < 4; c++)
                    {
                        var name = slot + " " + c;
                        lst.Add(new MyPaneModel()
                        {
                            Content = name + " content",
                            Header = name + " header",
                            Tab = name,
                            Slot = slot
                        });
                    }
                }

                return lst;
            }
        }
    }
}