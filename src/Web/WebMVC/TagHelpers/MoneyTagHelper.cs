using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Globalization;

namespace RTCodingExercise.Microservices.TagHelpers;

public class MoneyTagHelper : TagHelper
{
    private static readonly CultureInfo CultureInfo = CultureInfo.CreateSpecificCulture("en-GB");

    public decimal Value { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "span";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Content.SetContent(Value.ToString("C", CultureInfo));
    }
}
