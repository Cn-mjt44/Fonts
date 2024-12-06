// Copyright (c) Six Labors.
// Licensed under the Six Labors Split License.

namespace SixLabors.Fonts;

/// <summary>
/// Encapsulated logic for laying out and then rendering text to a <see cref="IGlyphRenderer"/> surface.
/// </summary>
public class TextRenderer
{
    private readonly IGlyphRenderer renderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderer"/> class.
    /// </summary>
    /// <param name="renderer">The renderer.</param>
    public TextRenderer(IGlyphRenderer renderer) => this.renderer = renderer;

    /// <summary>
    /// Renders the text to the <paramref name="renderer"/>.
    /// </summary>
    /// <param name="renderer">The target renderer.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="options">The text options.</param>
    public static void RenderTextTo(IGlyphRenderer renderer, ReadOnlySpan<char> text, TextOptions options)
        => new TextRenderer(renderer).RenderText(text, options);

    /// <summary>
    /// Renders the text to the <paramref name="renderer"/>.
    /// </summary>
    /// <param name="renderer">The target renderer.</param>
    /// <param name="text">The text to render.</param>
    /// <param name="options">The text option.</param>
    public static void RenderTextTo(IGlyphRenderer renderer, string text, TextOptions options)
        => new TextRenderer(renderer).RenderText(text, options);

    /// <summary>
    /// Renders the text to the default renderer.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <param name="options">The text options.</param>
    public void RenderText(string text, TextOptions options)
        => this.RenderText(text.AsSpan(), options);

    /// <summary>
    /// Renders the text to the default renderer.
    /// </summary>
    /// <param name="text">The text to render.</param>
    /// <param name="options">The style.</param>
    public void RenderText(ReadOnlySpan<char> text, TextOptions options)
    {
        using (ArrayBuilder<GlyphLayout> glyphsToRender = TextLayout.GenerateLayout(text, options))
        {
            FontRectangle rect = TextMeasurer.GetBounds(glyphsToRender, options.Dpi);

            this.renderer.BeginText(in rect);

            foreach (GlyphLayout g in glyphsToRender)
            {
                g.Glyph.RenderTo(this.renderer, g.PenLocation, g.Offset, g.LayoutMode, options);
            }

            this.renderer.EndText();
        }
    }
}
