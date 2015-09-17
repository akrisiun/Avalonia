﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

namespace Perspex.Media
{
    /// <summary>
    /// A brush that draws with a linear gradient.
    /// </summary>
    public class LinearGradientBrush : GradientBrush
    {
        /// <summary>
        /// Defines the <see cref="StartPoint"/> property.
        /// </summary>
        public static readonly PerspexProperty<RelativePoint> StartPointProperty =
            PerspexProperty.Register<LinearGradientBrush, RelativePoint>(
                "StartPoint", // nameof(StartPoint),
                RelativePoint.TopLeft);

        /// <summary>
        /// Defines the <see cref="EndPoint"/> property.
        /// </summary>
        public static readonly PerspexProperty<RelativePoint> EndPointProperty =
            PerspexProperty.Register<LinearGradientBrush, RelativePoint>(
                "EndPoint", // nameof(EndPoint), 
                RelativePoint.BottomRight);

        /// <summary>
        /// Gets or sets the start point for the gradient.
        /// </summary>
        public RelativePoint StartPoint
        {
            get { return GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        /// <summary>
        /// Gets or sets the end point for the gradient.
        /// </summary>
        public RelativePoint EndPoint
        {
            get { return GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }
    }
}