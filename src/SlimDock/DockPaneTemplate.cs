using System;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;

namespace SlimDock
{
    public class DockPaneTemplate : IDockPaneTemplate
    {
        [Content]
        [TemplateContent(TemplateResultType = typeof(DockPane))]
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public object? Content { get; set; }

        public DockPane Build() => TemplateContent.Load<DockPane>(Content).Result;
    }

    public interface IDockPaneTemplate
    {
        DockPane Build();
    }
}
