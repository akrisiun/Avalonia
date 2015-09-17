﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;

namespace Perspex.Controls.Primitives
{
    /// <summary>
    /// A scrollbar control.
    /// </summary>
    public class ScrollBar : RangeBase
    {
        /// <summary>
        /// Defines the <see cref="ViewportSize"/> property.
        /// </summary>
        public static readonly PerspexProperty<double> ViewportSizeProperty =
            PerspexProperty.Register<ScrollBar, double>("ViewportSize"  // nameof(ViewportSize)
                , defaultValue: double.NaN);

        /// <summary>
        /// Defines the <see cref="Visibility"/> property.
        /// </summary>
        public static readonly PerspexProperty<ScrollBarVisibility> VisibilityProperty =
            PerspexProperty.Register<ScrollBar, ScrollBarVisibility>("Visibility"); // nameof(Visibility));

        /// <summary>
        /// Defines the <see cref="Orientation"/> property.
        /// </summary>
        public static readonly PerspexProperty<Orientation> OrientationProperty =
            PerspexProperty.Register<ScrollBar, Orientation>("Orientation"); // nameof(Orientation));

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollBar"/> class.
        /// </summary>
        public ScrollBar()
        {
            var isVisible = Observable.Merge(
                GetObservable(MinimumProperty).Select(_ => Unit.Default),
                GetObservable(MaximumProperty).Select(_ => Unit.Default),
                GetObservable(ViewportSizeProperty).Select(_ => Unit.Default),
                GetObservable(VisibilityProperty).Select(_ => Unit.Default))
                .Select(_ => CalculateIsVisible());
            Bind(IsVisibleProperty, isVisible, BindingPriority.Style);
        }

        /// <summary>
        /// Gets or sets the amount of the scrollable content that is currently visible.
        /// </summary>
        public double ViewportSize
        {
            get { return GetValue(ViewportSizeProperty); }
            set { SetValue(ViewportSizeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that indicates whether the scrollbar should hide itself when it
        /// is not needed.
        /// </summary>
        public ScrollBarVisibility Visibility
        {
            get { return GetValue(VisibilityProperty); }
            set { SetValue(VisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the orientation of the scrollbar.
        /// </summary>
        public Orientation Orientation
        {
            get { return GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        /// <summary>
        /// Calculates whether the scrollbar should be visible.
        /// </summary>
        /// <returns>The scrollbar's visibility.</returns>
        private bool CalculateIsVisible()
        {
            switch (Visibility)
            {
                case ScrollBarVisibility.Visible:
                    return true;

                case ScrollBarVisibility.Hidden:
                    return false;

                case ScrollBarVisibility.Auto:
                    var viewportSize = ViewportSize;
                    return double.IsNaN(viewportSize) || viewportSize < Maximum - Minimum;

                default:
                    throw new InvalidOperationException("Invalid value for ScrollBar.Visibility.");
            }
        }
    }
}
