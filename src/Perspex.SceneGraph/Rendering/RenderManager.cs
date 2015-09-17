﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Subjects;

namespace Perspex.Rendering
{
    /// <summary>
    /// Schedules the rendering of a tree.
    /// </summary>
    public class RenderManager : IRenderManager
    {
        private readonly Subject<Unit> _renderNeeded = new Subject<Unit>();

        private bool _renderQueued;

        /// <summary>
        /// Gets an observable that is fired whenever a render is required.
        /// </summary>
        public IObservable<Unit> RenderNeeded // => 
        { get { return  _renderNeeded;}}

        /// <summary>
        /// Gets a valuue indicating whether a render is queued.
        /// </summary>
        public bool RenderQueued // => 
        { get { return _renderQueued; } }

        /// <summary>
        /// Invalidates the render for the specified visual and raises <see cref="RenderNeeded"/>.
        /// </summary>
        /// <param name="visual">The visual.</param>
        public void InvalidateRender(IVisual visual)
        {
            if (!_renderQueued)
            {
                _renderNeeded.OnNext(Unit.Default);
                _renderQueued = true;
            }
        }

        /// <summary>
        /// Called when rendering is finished.
        /// </summary>
        public void RenderFinished()
        {
            _renderQueued = false;
        }
    }
}
