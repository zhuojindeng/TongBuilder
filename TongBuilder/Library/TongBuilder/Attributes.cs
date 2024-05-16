using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Builder;

[AttributeUsage(AttributeTargets.Class)]
public class CodeInfoAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class TableAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}

[AttributeUsage(AttributeTargets.Property)]
public class FormAttribute() : Attribute
{
    public int Row { get; set; } = 1;
    public int Column { get; set; } = 1;
    public string Type { get; set; } = FieldType.Text.ToString();
    public bool ReadOnly { get; set; }
    public string Placeholder { get; set; }
}

[AttributeUsage(AttributeTargets.Property)]
public class ColumnAttribute(string columnName = null) : Attribute
{
    public string ColumnName { get; } = columnName;
    public string DateFormat { get; set; }
    public PropertyInfo Property { get; set; }
      

    private static int GetByteLength(string value)
    {
        if (string.IsNullOrEmpty(value))
            return 0;

        return Encoding.Default.GetBytes(value).Length;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class RegexAttribute(string pattern, string message) : Attribute
{
    public string Pattern { get; } = pattern;
    public string Message { get; } = message;

    internal virtual void Validate(object value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(Pattern))
            return;

        var valueString = value == null ? "" : value.ToString().Trim();
        if (string.IsNullOrWhiteSpace(valueString))
            return;

        var isMatch = Regex.IsMatch(valueString, Pattern);
        if (isMatch)
            return;

        errors.Add(Message);
    }
}