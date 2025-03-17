namespace JSVaporizer;

public static partial class TransformerRegistry
{
    public static ITransformer MyCoolTransformerV1()
    {
        return new MyCoolTransformer();
    }
}



