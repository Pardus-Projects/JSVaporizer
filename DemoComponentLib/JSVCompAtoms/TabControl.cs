// -------------- TabControl.cs -----------------
using JSVaporizer;
using JSVNuFlexiArch;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace DemoComponentLib;

public class TabControl : ContainerBase<TabItem>
{
    public TabControl(string uniqueName) : base(uniqueName) { }

    protected override string RenderInnerTemplate() => @"
            <ul class=""tab-headers"">
              {{#each Items}}
                <li id=""{{HeaderId}}"" data-content-id=""{{ContentId}}"" class=""tab-header"">
                    {{Title}}
                </li>
              {{/each}}
            </ul>
            <div class=""tab-contents"">
              {{#each Items}}
                <div id=""{{ContentId}}"" class=""tab-content"" style=""display:none;"">
                    {{unescaped ContentComponent}}
                </div>
              {{/each}}
            </div>
        ";

    public override void SetItems(List<ContainerItemProto> compsList)
    {
        Items = new();
        int ind = 1;
        foreach (TabItemProto tabItemPro in compsList)
        {
            TabItem tab = new(UniqueName, ind++, tabItemPro.Title, tabItemPro.Component);
            Items.Add(tab);
        }
    }

    [SupportedOSPlatform("browser")]
    public override void AfterChildrenAttached()
    {
        // 1) click listener on every header
        foreach (var it in Items)
        {
            Document.AssertGetElementById(it.HeaderId)
                    .AddEventListener("click", (_, _, _) =>
                    {
                        // deactivate all
                        foreach (var j in Items)
                        {
                            Document.AssertGetElementById(j.HeaderId)
                                    .SetAttribute("class", "tab-header");
                            Document.AssertGetElementById(j.ContentId)
                                    .SetAttribute("style", "display:none;");
                        }

                        // activate this one
                        Document.AssertGetElementById(it.HeaderId)
                                .SetAttribute("class", "tab-header active");
                        Document.AssertGetElementById(it.ContentId)
                                .SetAttribute("style", "display:block;");

                        return (int)JSVEventListenerBehavior.NoDefault_NoPropagate;
                    });
        }

        // 2) show first tab initially
        if (Items.Count > 0)
        {
            var first = Items[0];
            Document.AssertGetElementById(first.HeaderId)
                    .SetAttribute("class", "tab-header active");
            Document.AssertGetElementById(first.ContentId)
                    .SetAttribute("style", "display:block;");
        }
    }
}

public class TabItem : IContainerItem
{
    public string HeaderId { get; }
    public string ContentId { get; }
    public string Title { get; set; }
    public JSVComponent ContentComponent { get; }

    public TabItem(string parentUniq, int index, string title, JSVComponent comp)
    {
        HeaderId = $"{parentUniq}_hdr_{index}";
        ContentId = $"{parentUniq}_cnt_{index}";
        Title = title;
        ContentComponent = comp;
    }
}

public class TabItemProto : ContainerItemProto
{
    public string Title { get; set; }
    public TabItemProto(string title, JSVComponent comp)
    {
        Component = comp;
        Title = title;
    }
}
