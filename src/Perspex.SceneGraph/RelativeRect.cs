﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace Perspex
{
    /// <summary>
    /// Defines a rectangle that may be defined relative to a containing element.
    /// </summary>
    public struct RelativeRect
    {
        /// <summary>
        /// A rectangle that represents 100% of an area.
        /// </summary>
        public static readonly RelativeRect Fill = new RelativeRect(0, 0, 1, 1, RelativeUnit.Relative);

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeRect"/> structure.
        /// </summary>
        /// <param name="x">The X position.</param>
        /// <param name="y">The Y position.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="unit">The unit of the rect.</param>
        public RelativeRect(double x, double y, double width, double height, RelativeUnit unit)
        {
            _rect = new Rect(x, y, width, height);
            _unit = unit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeRect"/> structure.
        /// </summary>
        /// <param name="rect">The rectangle.</param>
        /// <param name="unit">The unit of the rect.</param>
        public RelativeRect(Rect rect, RelativeUnit unit)
        {
            _rect = rect;
            _unit = unit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeRect"/> structure.
        /// </summary>
        /// <param name="size">The size of the rectangle.</param>
        /// <param name="unit">The unit of the rect.</param>
        public RelativeRect(Size size, RelativeUnit unit)
        {
            _rect = new Rect(size);
            _unit = unit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeRect"/> structure.
        /// </summary>
        /// <param name="position">The position of the rectangle.</param>
        /// <param name="size">The size of the rectangle.</param>
        /// <param name="unit">The unit of the rect.</param>
        public RelativeRect(Point position, Size size, RelativeUnit unit)
        {
            _rect = new Rect(position, size);
            _unit = unit;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeRect"/> structure.
        /// </summary>
        /// <param name="topLeft">The top left position of the rectangle.</param>
        /// <param name="bottomRight">The bottom right position of the rectangle.</param>
        /// <param name="unit">The unit of the rect.</param>
        public RelativeRect(Point topLeft, Point bottomRight, RelativeUnit unit)
        {
            _rect = new Rect(topLeft, bottomRight);
            _unit = unit;
        }

        /// <summary>
        /// Gets the unit of the rectangle.
        /// </summary>
        RelativeUnit _unit;
        public RelativeUnit Unit { get { return _unit; }}

        /// <summary>
        /// Gets the rectangle.
        /// </summary>
        Rect _rect;
        public Rect Rect { get { return _rect; } }

        /// <summary>
        /// Converts a <see cref="RelativeRect"/> into pixels.
        /// </summary>
        /// <param name="size">The size of the visual.</param>
        /// <returns>The origin point in pixels.</returns>
        public Rect ToPixels(Size size)
        {
            return Unit == RelativeUnit.Absolute ?
                Rect :
                new Rect(
                    Rect.X * size.Width,
                    Rect.Y * size.Height,
                    Rect.Width * size.Width,
                    Rect.Height * size.Height);
        }
    }
}
