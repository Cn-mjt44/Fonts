// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using SixLabors.Fonts.Exceptions;
using SixLabors.Fonts.Unicode;

namespace SixLabors.Fonts
{
    /// <summary>
    /// Defines a particular format for text, including font face, size, and style attributes.
    /// This class cannot be inherited.
    /// </summary>
    public sealed class Font
    {
        private readonly Lazy<IFontMetrics?> metrics;
        private readonly Lazy<string> fontName;

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="family">The font family.</param>
        /// <param name="size">The size of the font in PT units.</param>
        public Font(FontFamily family, float size)
            : this(family, size, FontStyle.Regular)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="family">The font family.</param>
        /// <param name="size">The size of the font in PT units.</param>
        /// <param name="style">The font style.</param>
        public Font(FontFamily family, float size, FontStyle style)
        {
            this.Family = family;
            this.RequestedStyle = style;
            this.Size = size;
            this.metrics = new Lazy<IFontMetrics?>(this.LoadInstanceInternal);
            this.fontName = new Lazy<string>(this.LoadFontName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="style">The font style.</param>
        public Font(Font prototype, FontStyle style)
            : this(prototype?.Family ?? throw new ArgumentNullException(nameof(prototype)), prototype.Size, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="size">The size of the font in PT units.</param>
        /// <param name="style">The font style.</param>
        public Font(Font prototype, float size, FontStyle style)
            : this(prototype?.Family ?? throw new ArgumentNullException(nameof(prototype)), size, style)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        /// <param name="prototype">The prototype.</param>
        /// <param name="size">The size of the font in PT units.</param>
        public Font(Font prototype, float size)
            : this(prototype.Family, size, prototype.RequestedStyle)
        {
        }

        /// <summary>
        /// Gets the family.
        /// </summary>
        public FontFamily Family { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name => this.fontName.Value;

        /// <summary>
        /// Gets the size of the font in PT units.
        /// </summary>
        public float Size { get; }

        /// <summary>
        /// Gets the font metrics.
        /// </summary>
        public IFontMetrics FontMetrics => this.metrics.Value ?? throw new FontException("Font instance not found.");

        /// <summary>
        /// Gets a value indicating whether this <see cref="Font"/> is bold.
        /// </summary>
        public bool IsBold => (this.FontMetrics.Description.Style & FontStyle.Bold) == FontStyle.Bold;

        /// <summary>
        /// Gets a value indicating whether this <see cref="Font"/> is italic.
        /// </summary>
        public bool IsItalic => (this.FontMetrics.Description.Style & FontStyle.Italic) == FontStyle.Italic;

        /// <summary>
        /// Gets the requested style.
        /// </summary>
        internal FontStyle RequestedStyle { get; }

        /// <summary>
        /// Gets the filesystem path to the font family source.
        /// </summary>
        /// <param name="path">
        /// When this method returns, contains the filesystem path to the font family source,
        /// if the path exists; otherwise, the default value for the type of the path parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the <see cref="Font" /> was created via a filesystem path; otherwise, <see langword="false" />.
        /// </returns>
        public bool TryGetPath([NotNullWhen(true)] out string? path)
        {
            if (this == default)
            {
                FontsThrowHelper.ThrowDefaultInstance();
            }

            if (this.FontMetrics is FileFontMetrics fileMetrics)
            {
                path = fileMetrics.Path;
                return true;
            }

            path = null;
            return false;
        }

        /// <summary>
        /// Gets the glyph.
        /// </summary>
        /// <param name="codePoint">The code point of the character.</param>
        /// <returns>Returns the glyph</returns>
        public Glyph GetGlyph(CodePoint codePoint)

            => new Glyph(this.FontMetrics.GetGlyphMetrics(codePoint), this.Size);

        private string LoadFontName()
            => this.metrics.Value?.Description.FontName(this.Family.Culture) ?? string.Empty;

        private IFontMetrics? LoadInstanceInternal()
        {
            if (this.Family.TryGetMetrics(this.RequestedStyle, out IFontMetrics? metrics))
            {
                return metrics;
            }

            if (this.RequestedStyle.HasFlag(FontStyle.Italic))
            {
                // Can't find style requested and they want one thats at least partial itallic.
                // Try the regual italic.
                if (this.Family.TryGetMetrics(FontStyle.Italic, out metrics))
                {
                    return metrics;
                }
            }

            if (this.RequestedStyle.HasFlag(FontStyle.Bold))
            {
                // Can't find style requested and they want one thats at least partial bold.
                // Try the regular bold.
                if (this.Family.TryGetMetrics(FontStyle.Bold, out metrics))
                {
                    return metrics;
                }
            }

            // Can't find style requested so let's just try returning the default.
            IEnumerable<FontStyle>? styles = this.Family.AvailableStyles();
            FontStyle defaultStyle = styles.Contains(FontStyle.Regular)
            ? FontStyle.Regular
            : styles.First();

            this.Family.TryGetMetrics(defaultStyle, out metrics);
            return metrics;
        }
    }
}
