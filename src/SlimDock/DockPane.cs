using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.LogicalTree;
using Avalonia.Metadata;

namespace SlimDock
{
    public class DockPane : StyledElement
    {
        public static readonly StyledProperty<object> ContentProperty = AvaloniaProperty.Register<DockPane, object>(
            "Content");

        [Content]
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate> ContentTemplateProperty = AvaloniaProperty.Register<DockPane, IDataTemplate>(
            "ContentTemplate");

        public IDataTemplate ContentTemplate
        {
            get => GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        public static readonly StyledProperty<object> HeaderContentProperty = AvaloniaProperty.Register<DockPane, object>(
            "HeaderContent");

        public object HeaderContent
        {
            get => GetValue(HeaderContentProperty);
            set => SetValue(HeaderContentProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate> HeaderContentTemplateProperty = AvaloniaProperty.Register<DockPane, IDataTemplate>(
            "HeaderContentTemplate");

        public IDataTemplate HeaderContentTemplate
        {
            get => GetValue(HeaderContentTemplateProperty);
            set => SetValue(HeaderContentTemplateProperty, value);
        }

        public static readonly StyledProperty<object> PaneTabHeaderContentProperty = AvaloniaProperty.Register<DockPane, object>(
            "PaneTabHeaderContent");

        public object PaneTabHeaderContent
        {
            get => GetValue(PaneTabHeaderContentProperty);
            set => SetValue(PaneTabHeaderContentProperty, value);
        }

        public static readonly StyledProperty<IDataTemplate> PaneTabHeaderContentTemplateProperty = AvaloniaProperty.Register<DockPane, IDataTemplate>(
            "PaneTabHeaderContentTemplate");

        public IDataTemplate PaneTabHeaderContentTemplate
        {
            get => GetValue(PaneTabHeaderContentTemplateProperty);
            set => SetValue(PaneTabHeaderContentTemplateProperty, value);
        }

        private string _uniqueLayoutId;

        public static readonly DirectProperty<DockPane, string> UniqueLayoutIdProperty = AvaloniaProperty.RegisterDirect<DockPane, string>(
            "UniqueLayoutId", o => o.UniqueLayoutId, (o, v) => o.UniqueLayoutId = v);

        public string UniqueLayoutId
        {
            get => _uniqueLayoutId;
            set => SetAndRaise(UniqueLayoutIdProperty, ref _uniqueLayoutId, value);
        }

        public static readonly StyledProperty<DockLayoutSlot> InitialLayoutSlotProperty = AvaloniaProperty.Register<DockPane, DockLayoutSlot>(
            "InitialLayoutSlot");

        public DockLayoutSlot InitialLayoutSlot
        {
            get => GetValue(InitialLayoutSlotProperty);
            set => SetValue(InitialLayoutSlotProperty, value);
        }

        internal IAvaloniaList<ILogical> GetLogicalChildren() => LogicalChildren;
    }
}