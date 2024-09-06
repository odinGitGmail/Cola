namespace Cola.Swagger;

public class ActionNameAttribute(string name) : Attribute
{
    public string Name { get; set; } = name;
}