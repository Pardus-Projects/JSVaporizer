// -------------- ContainerComponent.cs -----------------
using JSVNuFlexiArch;
using System.Collections.Generic;
using System.Runtime.Versioning;

namespace DemoComponentLib;

public abstract class ContainerBase<TItem> : JSVComponent where TItem : IContainerItem
{
    public List<TItem> Items { get; set; } = new();

    protected ContainerBase(string uniqueName) : base(uniqueName) { }

    public abstract void SetItems(List<ContainerItemProto> compsList);

    protected abstract string RenderInnerTemplate();

    protected override string GetTemplate() => $@"
            <div id=""{{{UniqueName}}}"" class=""container"">
                {RenderInnerTemplate()}
            </div>
        ";

    // container‑specific DOM wiring
    // use inside PostAttachToDOMSetup / PostAttachToDOM()
    [SupportedOSPlatform("browser")]
    public virtual void AfterChildrenAttached() { }
}

public interface IContainerItem
{
    public string HeaderId { get; }
    public string ContentId { get; }
    public JSVComponent ContentComponent { get; }
}

public abstract class ContainerItemProto
{
    public JSVComponent Component { get; set; }
}