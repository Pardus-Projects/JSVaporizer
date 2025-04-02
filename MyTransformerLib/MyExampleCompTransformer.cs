using JSVaporizer;
using JSVComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Text.Json.Serialization;
using static JSVaporizer.JSVapor;

namespace MyTransformerLib;

[JsonSerializable(typeof(MyExampleCompTransformerDto))]
public partial class MyExampleCompTransformerDtoContext : JsonSerializerContext { }
public class MyExampleCompTransformerDto : TransformerDto
{
    public string? HeaderStr { get; set; }
    public string? ContentStr { get; set; }
}

public class MyExampleCompTransformer : Transformer
{
    [SupportedOSPlatform("browser")]
    public override MyExampleCompTransformerDto JsonToDto(string dtoJson)
    {
        MyExampleCompTransformerDto? dto = JsonSerializer.Deserialize(dtoJson, MyExampleCompTransformerDtoContext.Default.MyExampleCompTransformerDto);
        if (dto == null)
        {
            throw new JSVException("dto is null");
        }
        return dto;
    }

    [SupportedOSPlatform("browser")]
    public override string DtoToView(string dtoJson, string? userInfoJson = null)
    {
        MyExampleCompTransformerDto dto = JsonToDto(dtoJson);

        CompProperties compProps;
        if (userInfoJson == null)
        {
            throw new JSVException("userInfoJson can not be null");
        }
        else
        {
            CompProperties? tmpCompProps = JsonSerializer.Deserialize(userInfoJson, CompPropertiesContext.Default.CompProperties);
            if (tmpCompProps == null)
            {
                throw new ArgumentException("compProps is null");
            }
            compProps = tmpCompProps;
        }

        Dictionary<string, object?> compsDict = compProps.ToDictionary();

        Document.AssertGetElementById(compsDict["HeaderId"].ToString()).SetProperty("innerHTML", dto.HeaderStr);
        Document.AssertGetElementById(compsDict["ContentId"].ToString()).SetProperty("innerHTML", dto.ContentStr);

        return "Successfully invoked DtoToView()";
    }

    [SupportedOSPlatform("browser")]
    public override MyExampleCompTransformerDto ViewToDto()
    {
        MyExampleCompTransformerDto dto = new();

        try
        {
            return dto;
        }
        catch (Exception ex)
        {
            throw new JSVException(ex.Message);
        }
    }
    public override string DtoToJson(TransformerDto dto)
    {
        return JsonSerializer.Serialize(dto, MyExampleCompTransformerDtoContext.Default.MyExampleCompTransformerDto);
    }
}