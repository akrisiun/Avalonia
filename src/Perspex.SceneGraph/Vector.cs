﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Globalization;

namespace Perspex
{
    /// <summary>
    /// Defines a vector.
    /// </summary>
    public struct Vector
    {
        /// <summary>
        /// The X vector.
        /// </summary>
        private readonly double _x;

        /// <summary>
        /// The Y vector.
        /// </summary>
        private readonly double _y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector"/> structure.
        /// </summary>
        /// <param name="x">The X vector.</param>
        /// <param name="y">The Y vector.</param>
        public Vector(double x, double y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Gets the X vector.
        /// </summary>
        public double X // => 
        { get { return  _x;}}

        /// <summary>
        /// Gets the Y vector.
        /// </summary>
        public double Y // => 
        { get { return _y; } }

        /// <summary>
        /// Converts the <see cref="Vector"/> to a <see cref="Point"/>.
        /// </summary>
        /// <param name="a">The vector.</param>
        public static explicit operator Point(Vector a)
        {
            return new Point(a._x, a._y);
        }

        /// <summary>
        /// Negates a vector.
        /// </summary>
        /// <param name="a">The vector.</param>
        /// <returns>The negated vector.</returns>
        public static Vector operator -(Vector a)
        {
            return new Vector(-a._x, -a._y);
        }

        /// <summary>
        /// Adds two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector that is the result of the addition.</returns>
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a._x + b._x, a._y + b._y);
        }

        /// <summary>
        /// Subtracts two vectors.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>A vector that is the result of the subtraction.</returns>
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a._x - b._x, a._y - b._y);
        }

        /// <summary>
        /// Returns the string representation of the point.
        /// </summary>
        /// <returns>The string representation of the point.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}, {1}", _x, _y);
        }

        /// <summary>
        /// Returns a new vector with the specified X coordinate.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <returns>The new vector.</returns>
        public Vector WithX(double x)
        {
            return new Vector(x, _y);
        }

        /// <summary>
        /// Returns a new vector with the specified Y coordinate.
        /// </summary>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The new vector.</returns>
        public Vector WithY(double y)
        {
            return new Vector(_x, y);
        }
    }
}
