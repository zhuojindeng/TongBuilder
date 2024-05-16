﻿namespace TongBuilder.Extensions;

/// <summary>
///  from known:https://github.com/known/Known
/// </summary>
public static class HtmlExtension
{
    public static void Label(this RenderTreeBuilder builder, string text) => builder.Label(() => builder.Markup(text));
    public static void Label(this RenderTreeBuilder builder, Action child) => builder.Label().Children(child).Close();

    public static void Div(this RenderTreeBuilder builder, Action child) => builder.Div("", child);
    public static void Div(this RenderTreeBuilder builder, string className, string text) => builder.Div(className, () => builder.Markup(text));
    public static void Div(this RenderTreeBuilder builder, string className, Action child) => builder.Div().Class(className).Children(child).Close();

    public static void Ul(this RenderTreeBuilder builder, Action child) => builder.Ul("", child);
    public static void Ul(this RenderTreeBuilder builder, string className, Action child) => builder.Ul().Class(className).Children(child).Close();

    public static void Li(this RenderTreeBuilder builder, Action child) => builder.Li("", child);
    public static void Li(this RenderTreeBuilder builder, string className, string text) => builder.Li(className, () => builder.Markup(text));
    public static void Li(this RenderTreeBuilder builder, string className, Action child) => builder.Li().Class(className).Children(child).Close();

    public static void Icon(this RenderTreeBuilder builder, string icon, EventCallback<MouseEventArgs>? onClick = null) => builder.Span(icon, "", onClick);
    public static void Span(this RenderTreeBuilder builder, string text, EventCallback<MouseEventArgs>? onClick = null) => builder.Span("", text, onClick);
    public static void Span(this RenderTreeBuilder builder, string className, string text, EventCallback<MouseEventArgs>? onClick = null)
    {
        builder.Span().Class(className).OnClick(onClick).Markup(text).Close();
    }

    public static void IFrame(this RenderTreeBuilder builder, string url)
    {
        builder.IFrame().Class("kui-frame").Src(url).Close();
    }

    public static void Link(this RenderTreeBuilder builder, string text, EventCallback onClick)
    {
        if (string.IsNullOrWhiteSpace(text))
            return;

        builder.Span().Class("kui-link").OnClick(onClick).Markup(text).Close();
    }
    
}