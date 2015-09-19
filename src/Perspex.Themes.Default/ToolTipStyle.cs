﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Linq;
using Perspex.Controls;
using Perspex.Controls.Presenters;
using Perspex.Controls.Primitives;
using Perspex.Controls.Templates;
using Perspex.Media;
using Perspex.Styling;

namespace Perspex.Themes.Default
{
    /// <summary>
    /// The default style for the <see cref="ToolTip"/> control.
    /// </summary>
    public class ToolTipStyle : Styles
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTipStyle"/> class.
        /// </summary>
        public ToolTipStyle()
        {
            AddRange(new[]
            {
                new Style(x => x.OfType<ToolTip>())
                {
                    Setters = new[]
                    {
                        new Setter(TemplatedControl.TemplateProperty, new ControlTemplate<ToolTip>(Template)),
                        new Setter(TemplatedControl.BackgroundProperty, new SolidColorBrush(0xffffffe1)),
                        new Setter(TemplatedControl.BorderBrushProperty, Brushes.Gray),
                        new Setter(TemplatedControl.BorderThicknessProperty, 1.0),
                        new Setter(TemplatedControl.PaddingProperty, new Thickness(4, 2)),
                    },
                },
            });
        }

        /// <summary>
        /// The default template for the <see cref="ToolTip"/> control.
        /// </summary>
        /// <param name="control">The control being styled.</param>
        /// <returns>The root of the instantiated template.</returns>
        public static Control Template(ToolTip control)
        {
            return new Border
            {
                [~Border.BackgroundProperty] = control[~TemplatedControl.BackgroundProperty],
                [~Border.BorderBrushProperty] = control[~TemplatedControl.BorderBrushProperty],
                [~Border.BorderThicknessProperty] = control[~TemplatedControl.BorderThicknessProperty],
                [~Decorator.PaddingProperty] = control[~TemplatedControl.PaddingProperty],
                Child = new ContentPresenter
                {
                    Name = "contentPresenter",
                    [~ContentPresenter.ContentProperty] = control[~ContentControl.ContentProperty],
                }
            };
        }
    }
}
