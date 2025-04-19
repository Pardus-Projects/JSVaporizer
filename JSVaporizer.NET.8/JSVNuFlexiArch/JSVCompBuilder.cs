using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using static JSVaporizer.JSVapor;

namespace JSVNuFlexiArch;

public delegate void PostAttachToDOMSetup();
public abstract class JSVCompBuilder
{
    public PostAttachToDOMSetup? PostAttachToDOMSetup;
    public string AssemblyQualifiedName { get; set; }

    protected JSVCompBuilder()
    {
        AssemblyQualifiedName = GetAssemblyQualifiedName();
    }

    public abstract JSVComponent Build(string uniqueName);
    public void PostAttachToDOM()
    {
        if (PostAttachToDOMSetup != null)
        {
            PostAttachToDOMSetup();
        }
    }

    public static JSVCompBuilder Instantiate(string assemblyQualifiedName)
    {
        Type? builderType = Type.GetType(assemblyQualifiedName);
        if (builderType == null)
        {
            throw new ArgumentException($"assemblyQualifiedName = \"{assemblyQualifiedName}\" was not found.");
        }

        JSVCompBuilder? cBuilder = (JSVCompBuilder?)Activator.CreateInstance(builderType);
        if (cBuilder == null)
        {
            throw new ArgumentException($"CreateInstance() failed for builderType = \"{builderType}\"");
        }

        return cBuilder;
    }

    [SupportedOSPlatform("browser")]
    public static void Invoke(string uniqueName, string assemblyQualifiedName, string placeholderElementId)
    {
        JSVCompBuilder cBuilder = Instantiate(assemblyQualifiedName);
        JSVComponent comp = cBuilder.Build(uniqueName);

        Element referenceElem = Document.AssertGetElementById(placeholderElementId);
        referenceElem.SetOuterHtml(comp.Render(), alreadySafe: true);

        cBuilder.PostAttachToDOM();
    }

    private string GetAssemblyQualifiedName()
    {
        string? nFqn = GetType().AssemblyQualifiedName;
        if (nFqn == null)
        {
            throw new ArgumentNullException("nFqn is null");
        }
        return nFqn;
    }
}

