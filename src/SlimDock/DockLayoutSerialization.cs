using System.Collections.Generic;

namespace SlimDock
{
    public class SerializedDockLayout
    {
        public Dictionary<SerializedDockLayoutSlot, List<string>> Slots { get; } =
            new Dictionary<SerializedDockLayoutSlot, List<string>>();

        public List<SerializedDockLayoutSide> SideLayoutPriorities { get; } = 
            new List<SerializedDockLayoutSide>();
    }

    public enum SerializedDockLayoutSide
    {
        Left, Bottom, Top
    }

    public enum SerializedDockLayoutSlot
    {
        LeftTop, LeftBottom, RightTop, RightBottom, BottomLeft, BottomCenter, BottomRight
    }
}