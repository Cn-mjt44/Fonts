// Copyright (c) Six Labors.
// Licensed under the Apache License, Version 2.0.

using System;
using System.Diagnostics;

namespace SixLabors.Fonts
{
    /// <summary>
    /// Represents the shaped bounds of a glyph.
    /// Uses a class over a struct for ease of use.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class GlyphShapingBounds
    {
        private int x;
        private int y;
        private int width;
        private int height;

        public GlyphShapingBounds(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.IsDirty = false;
        }

        public int X
        {
            get => this.x;

            set
            {
                this.x = value;
                this.IsDirty = true;
            }
        }

        public int Y
        {
            get => this.y;

            set
            {
                this.y = value;
                this.IsDirty = true;
            }
        }

        public int Width
        {
            get => this.width;

            set
            {
                this.width = value;
                this.IsDirty = true;
            }
        }

        public int Height
        {
            get => this.height;

            set
            {
                this.height = value;
                this.IsDirty = true;
            }
        }

        public bool IsDirty { get; private set; }

        private string DebuggerDisplay
            => FormattableString.Invariant($"{this.X} : {this.Y} : {this.Width} : {this.Height} : {this.IsDirty}");
    }
}
