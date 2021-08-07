// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using SixLabors.Fonts.Exceptions;
using SixLabors.Fonts.Tables;

namespace SixLabors.Fonts
{
    /// <summary>
    /// Provides a collection of fonts.
    /// </summary>
    public sealed class FontCollection : IFontCollection
    {
        private readonly HashSet<IFontMetrics> instances = new HashSet<IFontMetrics>();

        private readonly HashSet<FileFontMetrics> fileFontMetrics = new HashSet<FileFontMetrics>();
        private readonly HashSet<FontMetrics> fontMetrics = new HashSet<FontMetrics>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FontCollection"/> class.
        /// </summary>
        public FontCollection()
        {
        }

        /// <inheritdoc />
        // TODO: Should be a method.
        public IEnumerable<FontFamily> Families => this.FamiliesByCultureInternal(CultureInfo.CurrentCulture);

        internal ICollection<FontFamily2> FontFamilies { get; } = new HashSet<FontFamily2>();

        internal ICollection<FileFontFamily> FileFontFamilies { get; } = new HashSet<FileFontFamily>();

        /// <summary>
        /// Adds a font family from the specified filesystem path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The new <see cref="FileFontFamily"/>.</returns>
        public FileFontFamily AddNouveau(string path)
        {
            var metrics = new FileFontMetrics(path);
            this.AddFileFontMetrics(metrics);
            var name = metrics.Description.FontFamily(culture);
            var family = new FileFontFamily("TODO", path, this, CultureInfo.CurrentCulture);
            this.FileFontFamilies.Add(family);
            return family;
        }

        /// <summary>
        /// Adds a font family from the specified stream.
        /// </summary>
        /// <param name="stream">The stream containing the font data.</param>
        /// <returns>The new <see cref="FontFamily"/>.</returns>
        public FontFamily AddNouveau(Stream stream)
        {
            throw new NotImplementedException();
        }

#if SUPPORTS_CULTUREINFO_LCID
        /// <inheritdoc />
        public IEnumerable<FontFamily> FamiliesByCulture(CultureInfo culture) => this.FamiliesByCultureInternal(culture);
#endif

        private IEnumerable<FontFamily> FamiliesByCultureInternal(CultureInfo culture)
                => this.instances
                        .Select(x => x.Description.FontFamily(culture))
                        .Distinct()
                        .Select(x => new FontFamily(x, this, culture))
                        .ToArray();

        /// <summary>
        /// Adds a font from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(string path)
            => this.AddInternal(path, CultureInfo.CurrentCulture);

        /// <summary>
        /// Adds a font from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="fontDescription">The font description of the installed font.</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(string path, out FontDescription fontDescription)
            => this.AddInternal(path, CultureInfo.CurrentCulture, out fontDescription);

        /// <summary>
        /// Adds the specified font stream.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(Stream fontStream)
            => this.AddInternal(fontStream, CultureInfo.InvariantCulture);

        /// <summary>
        /// Adds the specified font stream.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        /// <param name="fontDescription">The font description of the installed font.</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(Stream fontStream, out FontDescription fontDescription)
            => this.AddInternal(fontStream, CultureInfo.InvariantCulture, out fontDescription);

        /// <summary>
        /// Adds a true type font collection (.ttc) from the specified font collection stream.
        /// </summary>
        /// <param name="fontCollectionPath">The font collection path (should be typically a .ttc file like simsun.ttc).</param>
        /// <returns>The font descriptions of the installed fonts.</returns>
        public IEnumerable<FontFamily> AddCollection(string fontCollectionPath)
            => this.AddCollectionInternal(fontCollectionPath, CultureInfo.InvariantCulture);

        /// <summary>
        /// Adds a true type font collection (.ttc) from the specified font collection stream.
        /// </summary>
        /// <param name="fontCollectionPath">The font collection path (should be typically a .ttc file like simsun.ttc).</param>
        /// <param name="fontDescriptions">The descriptions of fonts installed from the collection.</param>
        /// <returns>The font descriptions of the installed fonts.</returns>
        public IEnumerable<FontFamily> AddCollection(string fontCollectionPath, out IEnumerable<FontDescription> fontDescriptions)
            => this.AddCollectionInternal(fontCollectionPath, CultureInfo.InvariantCulture, out fontDescriptions);

        /// <summary>
        /// Adds a true type font collection (.ttc) from the specified font collection stream.
        /// </summary>
        /// <param name="fontCollectionStream">The font stream.</param>
        /// <param name="fontDescriptions">The descriptions of fonts installed from the collection.</param>
        /// <returns>The font descriptions of the installed fonts.</returns>
        public IEnumerable<FontFamily> AddCollection(Stream fontCollectionStream, out IEnumerable<FontDescription> fontDescriptions)
            => this.AddCollectionInternal(fontCollectionStream, CultureInfo.InvariantCulture, out fontDescriptions);

#if SUPPORTS_CULTUREINFO_LCID
        /// <summary>
        /// Adds a font from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="culture">The culture of the retuend font family</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(string path, CultureInfo culture)
            => this.AddInternal(path, culture);

        /// <summary>
        /// Adds a font from the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="culture">The culture of the retuend font family</param>
        /// <param name="fontDescription">The font description of the installed font.</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(string path, CultureInfo culture, out FontDescription fontDescription)
            => this.AddInternal(path, culture, out fontDescription);

        /// <summary>
        /// Adds the specified font stream.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        /// <param name="culture">The culture of the retuend font family</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(Stream fontStream, CultureInfo culture)
            => this.AddInternal(fontStream, culture);

        /// <summary>
        /// Adds the specified font stream.
        /// </summary>
        /// <param name="fontStream">The font stream.</param>
        /// <param name="culture">The culture of the retuend font family</param>
        /// <param name="fontDescription">The font description of the installed font.</param>
        /// <returns>the description of the font just loaded.</returns>
        public FontFamily Add(Stream fontStream, CultureInfo culture, out FontDescription fontDescription)
            => this.AddInternal(fontStream, culture, out fontDescription);

        /// <summary>
        /// Adds a true type font collection (.ttc) from the specified font collection stream.
        /// </summary>
        /// <param name="fontCollectionPath">The font collection path (should be typically a .ttc file like simsun.ttc).</param>
        /// <returns>The font descriptions of the installed fonts.</returns>
        /// <param name="culture">The culture of the retuend font families</param>
        public IEnumerable<FontFamily> AddCollection(string fontCollectionPath, CultureInfo culture)
            => this.AddCollectionInternal(fontCollectionPath, culture);

        /// <summary>
        /// Adds a true type font collection (.ttc) from the specified font collection stream.
        /// </summary>
        /// <param name="fontCollectionPath">The font collection path (should be typically a .ttc file like simsun.ttc).</param>
        /// <param name="culture">The culture of the retuend font families</param>
        /// <param name="fontDescriptions">The descriptions of fonts installed from the collection.</param>
        /// <returns>The new collection of <see cref="FontFamily"/> items.</returns>
        public IEnumerable<FontFamily> AddCollection(
            string fontCollectionPath,
            CultureInfo culture,
            out IEnumerable<FontDescription> fontDescriptions)
            => this.AddCollectionInternal(fontCollectionPath, culture, out fontDescriptions);

        /// <summary>
        /// Adds a true type font collection (.ttc) from the specified font collection stream.
        /// </summary>
        /// <param name="fontCollectionStream">The font stream.</param>
        /// <param name="culture">The culture of the retuend font families</param>
        /// <param name="fontDescriptions">The descriptions of fonts installed from the collection.</param>
        /// <returns>The new collection of <see cref="FontFamily"/> items.</returns>
        public IEnumerable<FontFamily> AddCollection(
            Stream fontCollectionStream,
            CultureInfo culture,
            out IEnumerable<FontDescription> fontDescriptions)
            => this.AddCollectionInternal(fontCollectionStream, culture, out fontDescriptions);
#endif

        private FontFamily AddInternal(string path, CultureInfo culture)
            => this.AddInternal(path, culture, out _);

        private FontFamily AddInternal(string path, CultureInfo culture, out FontDescription fontDescription)
        {
            var instance = new FileFontMetrics(path);
            fontDescription = instance.Description;
            return this.Add(instance, culture);
        }

        private FontFamily AddInternal(Stream fontStream, CultureInfo culture)
            => this.AddInternal(fontStream, culture, out _);

        private FontFamily AddInternal(Stream fontStream, CultureInfo culture, out FontDescription fontDescription)
        {
            var instance = FontMetrics.LoadFont(fontStream);
            fontDescription = instance.Description;

            return this.Add(instance, culture);
        }

        private IEnumerable<FontFamily> AddCollectionInternal(string fontCollectionPath, CultureInfo culture)
            => this.AddCollectionInternal(fontCollectionPath, culture, out _);

        private IEnumerable<FontFamily> AddCollectionInternal(string fontCollectionPath, CultureInfo culture, out IEnumerable<FontDescription> fontDescriptions)
        {
            FileFontMetrics[] fonts = FileFontMetrics.LoadFontCollection(fontCollectionPath);

            var description = new FontDescription[fonts.Length];
            var families = new HashSet<FontFamily>();
            for (int i = 0; i < fonts.Length; i++)
            {
                description[i] = fonts[i].Description;
                FontFamily family = this.Add(fonts[i], culture);
                families.Add(family);
            }

            fontDescriptions = description;
            return families;
        }

        private IEnumerable<FontFamily> AddCollectionInternal(Stream fontCollectionStream, CultureInfo culture, out IEnumerable<FontDescription> fontDescriptions)
        {
            long startPos = fontCollectionStream.Position;
            var reader = new BigEndianBinaryReader(fontCollectionStream, true);
            var ttcHeader = TtcHeader.Read(reader);
            var result = new List<FontDescription>((int)ttcHeader.NumFonts);
            var installedFamilies = new HashSet<FontFamily>();
            for (int i = 0; i < ttcHeader.NumFonts; ++i)
            {
                fontCollectionStream.Position = startPos + ttcHeader.OffsetTable[i];
                var instance = FontMetrics.LoadFont(fontCollectionStream);
                installedFamilies.Add(this.Add(instance, culture));
                FontDescription fontDescription = instance.Description;
                result.Add(fontDescription);
            }

            fontDescriptions = result;
            return installedFamilies;
        }

#if SUPPORTS_CULTUREINFO_LCID
        /// <inheritdoc />
        public FontFamily Find(string fontFamily, CultureInfo culture)
            => this.FindInternal(fontFamily, culture);

        /// <inheritdoc />
        public bool TryFind(string fontFamily, CultureInfo culture, [NotNullWhen(true)] out FontFamily? family)
            => this.TryFindInternal(fontFamily, culture, out family);
#endif

        private FontFamily FindInternal(string fontFamily, CultureInfo culture)
        {
            if (this.TryFindInternal(fontFamily, culture, out FontFamily? result))
            {
                return result;
            }

            throw new FontFamilyNotFoundException(fontFamily);
        }

        private bool TryFindInternal(string fontFamily, CultureInfo culture, [NotNullWhen(true)] out FontFamily? family)
        {
            StringComparer? comparer = StringComparerHelpers.GetCaseInsensitiveStringComparer(culture);
            family = null!; // make the compiler shutup

            string? familyName = this.instances
                .Select(x => x.Description.FontFamily(culture))
                .FirstOrDefault(x => comparer.Equals(x, fontFamily));
            if (familyName == null)
            {
                return false;
            }

            family = new FontFamily(familyName, this, culture);
            return true;
        }

        /// <inheritdoc />
        public FontFamily Find(string fontFamily)
            => this.FindInternal(fontFamily, CultureInfo.InvariantCulture);

        /// <inheritdoc />
        public bool TryFind(string fontFamily, [NotNullWhen(true)] out FontFamily? family)
            => this.TryFindInternal(fontFamily, CultureInfo.InvariantCulture, out family);

        internal IEnumerable<FontStyle> AvailableStyles(string fontFamily, CultureInfo culture)
            => this.FindAll(fontFamily, culture).Select(x => x.Description.Style).ToArray();

        internal void AddFileFontMetrics(FileFontMetrics metrics)
        {
            lock (this.fileFontMetrics)
            {
                this.fileFontMetrics.Add(metrics);
            }
        }

        internal void AddFontMetrics(FontMetrics metrics)
        {
            lock (this.fontMetrics)
            {
                this.fontMetrics.Add(metrics);
            }
        }

        internal FontFamily Add(IFontMetrics instance, CultureInfo culture)
        {
            Guard.NotNull(instance, nameof(instance));

            if (instance.Description == null)
            {
                throw new ArgumentException("IFontInstance must have a Description.", nameof(instance));
            }

            lock (this.instances)
            {
                this.instances.Add(instance);
            }

            return new FontFamily(instance.Description.FontFamily(culture), this, culture);
        }

        internal IFontMetrics? Find(string fontFamily, CultureInfo culture, FontStyle style)
            => this.FindAll(fontFamily, culture)
            .FirstOrDefault(x => x.Description.Style == style);

        internal IEnumerable<IFontMetrics> FindAll(string name, CultureInfo culture)
        {
            StringComparer? comparer = StringComparerHelpers.GetCaseInsensitiveStringComparer(culture);

            return this.instances
                .Where(x => comparer.Equals(x.Description.FontFamily(culture), name))
                .ToArray();
        }
    }
}
