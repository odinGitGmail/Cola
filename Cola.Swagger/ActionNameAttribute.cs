namespace Cola.Swagger;

public class ActionNameAttribute : Attribute
{
    public string Name { get; set; }

    public ActionNameAttribute(string name)
    {
        Name = name;
    }
}