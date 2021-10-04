using System;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls.Primitives;

namespace SlimDock.Controls
{
    public class DockSideSlot : AvaloniaObject
    {
        internal AvaloniaList<DockPane> PanesList { get; } = new();

        public IAvaloniaReadOnlyList<DockPane> Panes => PanesList;

        private DockPane? _selectedPane;

        public static readonly DirectProperty<DockSideSlot, DockPane?> SelectedPaneProperty = AvaloniaProperty.RegisterDirect<DockSideSlot, DockPane?>(
            "SelectedPane", o => o.SelectedPane, (o, v) => o.SelectedPane = v);

        public DockPane? SelectedPane
        {
            get => _selectedPane;
            set => SetAndRaise(SelectedPaneProperty, ref _selectedPane, value);
        }
    }
}