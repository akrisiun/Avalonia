// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Perspex.Input;
using Perspex.Media;
using Perspex.Platform;
using Perspex.Styling;
using Perspex.Layout;
using Perspex.Rendering;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Perspex.Collections;
using Perspex.Controls.Templates;
using Size = Perspex.Size;
using Perspex.Interactivity;
using SharpDX.Windows;
using Perspex.Threading;
using System.Runtime.InteropServices;

namespace Perspex.Controls
{
    // TopLevel
    public class TopLevelBase : ContentControl, IRenderRoot, ILogical
    {
        public TopLevelBase(Window parent) : base() // base(Locator.Current.GetService<IWindowImpl>())
        {
            SetValue(ParentProperty, parent);   // TODO: parent as IControl

            SetInterfaces(parent);
        }

        public void SetInterfaces(Window parent)
        {
            _dispatcher = Dispatcher.UIThread;

            // ITopLevelImpl PlatformImpl = impl
            Splat.IDependencyResolver dependencyResolver = Splat.Locator.Current;
            var renderInterface = TopLevel.TryGetService<IPlatformRenderInterface>(dependencyResolver);
            if (renderInterface != null)
            {
                _renderer = renderInterface.CreateRenderer(
                    new PlatformHandle(parent.Handle, parent.GetType().FullName),
                    parent.ClientSize.Width, parent.ClientSize.Height);
                // PlatformImpl.Handle, clientSize.Width, clientSize.Height);
            }

            //if (LayoutManager != null)
            //{
            //    LayoutManager.Root = this;
            //    LayoutManager.LayoutNeeded.Subscribe(_ => HandleLayoutNeeded());
            //    LayoutManager.LayoutCompleted.Subscribe(_ => HandleLayoutCompleted());
            //}

            _renderManager = TopLevel.TryGetService<IRenderManager>(dependencyResolver);
            if (_renderManager != null)
            {
                // _renderManager.RenderNeeded.Subscribe(_ => HandleRenderNeeded());
            }

            var _keyboardNavigationHandler = TopLevel.TryGetService<IKeyboardNavigationHandler>(dependencyResolver);
            if (_keyboardNavigationHandler != null)
            {
                IInputRoot root = parent as IInputRoot;
                if (root != null)
                    _keyboardNavigationHandler.SetOwner(root);
            }
        }

        Dispatcher _dispatcher;
        IRenderer _renderer;
        IRenderManager _renderManager;
        IKeyboardNavigationHandler _keyboardNavigationHandler;

        public Size DoMeasureOverride(Size availableSize)
        {
            return this.MeasureOverride(availableSize);
        }

        public bool AutoSizing { get; set; }

        protected virtual void HandleResized(Size clientSize)
        {
            if (!AutoSizing)
            {
                Width = clientSize.Width;
                Height = clientSize.Height;
            }

            (Parent as Window).ClientSize = new System.Drawing.Size((int)clientSize.Width, (int)clientSize.Height);
            if (_renderer != null)
                _renderer.Resize((int)clientSize.Width, (int)clientSize.Height);

            // LayoutManager.ExecuteLayoutPass();
            // PlatformImpl.Invalidate(new Rect(clientSize));
        }

        public void DoHandleResized(Size clientSize)
        {
            HandleResized(clientSize);
        }

        public IVisual VisualParent { get { return Parent; } }

        public IRenderer Renderer { get { return _renderer; } }
        public IRenderManager RenderManager { get { return _renderManager; } }

        public void SetContent(object value)
        {
            Content = value;

            if (value is IEnumerable<ILogical>)
            {
                var children = LogicalChildren;
                children.AddRange(value as IEnumerable<ILogical>);
            }
        }

        public Size? _PreviousMeasure() { return (this as ILayoutable).PreviousMeasure; }
        public Rect? _PreviousArrange() { return (this as ILayoutable).PreviousArrange; }

        public Point TranslatePointToScreen(Point p)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Defines the <see cref="Title"/> property.
        /// </summary>
        public static readonly PerspexProperty<string> TitleProperty =
            PerspexProperty.Register<TopLevelBase, string>("Title" // nameof(Title)
                , "Window");

        /// <summary>
        /// Defines the <see cref="SizeToContent"/> property.
        /// </summary>
        public static readonly PerspexProperty<SizeToContent> SizeToContentProperty =
            PerspexProperty.Register<TopLevelBase, SizeToContent>("SizeToContent"); // nameof(SizeToContent));

    }


    /// <summary>
    /// A top-level window.
    /// </summary>
    public class Window : Perspex.Direct2D1.Form.DirectRenderForm, IPerspexObject, IControl,
           IVisual, ILogical, IRenderRoot, ICloseable
    //     IStyleable, IFocusScope, IInputRoot, ILayoutRoot
    {
        #region Context

        public TopLevelBase Base { get; set; }

        public object DataContext { get { return Base.DataContext; } set { Base.DataContext = value; } }
        public DataTemplates DataTemplates { get { return Base.DataTemplates; } }
        IControl IControl.Parent { get { return null; } }

        public object Content
        {
            get { return VisualChildren; }
            set { Base.SetContent(value); }
        }

        #endregion

        public static readonly PerspexProperty<Brush> BackgroundProperty =
            PerspexProperty.RegisterObj<Window, Brush>("Background", Brushes.White);

        #region Values

        protected readonly Dictionary<PerspexProperty, PriorityValue> _values = new Dictionary<PerspexProperty, PriorityValue>();
        public Dictionary<PerspexProperty, PriorityValue> Values { get { return _values; } }

        public object GetDefaultValue(PerspexProperty property) { return property.GetDefaultValue(property.PropertyType); }

        public T GetValue<T>(PerspexProperty property)
        {
            return (T)_values[property].Value;
        }
        protected void SetValue(PerspexProperty property, object value)
        {
            _values[property] = PerspexObject.SetPriorityValue(this, property, value);
        }

        public void RaisePropertyChanged(PerspexProperty property, object oldValue, object newValue) { }

        #endregion

        #region ctor

        private object _dialogResult;
        private Size _maxPlatformClientSize;

        /// <summary>
        /// Initializes static members of the <see cref="Window"/> class.
        /// </summary>
        static Window()
        {
            //BackgroundProperty.OverrideDefaultValue(typeof(Window), Brushes.White);
            //TitleProperty.Changed.AddClassHandler<Window>(
            //    (s, e) => s.PlatformImpl.SetTitle((string)e.NewValue));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        public Window() : base()
        {
            Base = new TopLevelBase(this);

            var parent = (Base as ILogical).LogicalParent;
            Contract.Requires<Exception>(parent == this);

            var dependencyResolver = Splat.Locator.Current;
            _renderManager = Base.RenderManager ?? Perspex.Controls.TopLevel.TryGetService<IRenderManager>(dependencyResolver);
            _maxPlatformClientSize = MaxClientSize; // this.PlatformImpl.MaxClientSize;
        }

        const int SM_CXMAXTRACK = 59; // 0x3B
        const int SM_CYMAXTRACK = 60; // 0x3C
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int smIndex);

        static Size MaxClientSize
        {
            get
            {
                return new Size(
                    // (UnmanagedMethods.SystemMetric
                    GetSystemMetrics(SM_CXMAXTRACK),
                    GetSystemMetrics(SM_CYMAXTRACK));
                // - BorderThickness;
            }
        }

        #endregion

        #region Events

        public virtual void RenderCallback()
        {
            // RenderForm
            // RenderLoop

            this.HandleResized(ToClientSize());
        }

        event EventHandler<RoutedEventArgs> IInputElement.GotFocus
        {
            add { }
            remove { }
        }

        event EventHandler<RoutedEventArgs> IInputElement.LostFocus
        {
            add { }
            remove { }
        }

        event EventHandler<KeyEventArgs> IInputElement.KeyDown
        {
            add { }
            remove { }
        }

        event EventHandler<KeyEventArgs> IInputElement.KeyUp
        {
            add { }
            remove { }
        }

        #endregion

        #region Implement

        /// <summary>
        /// Gets the platform-specific window implementation.
        /// </summary>
        //public IWindowImpl PlatformImpl // => 
        //{ get { return (IWindowImpl)Base.PlatformImpl; } }

        /// <summary>
        /// Gets or sets a value indicating how the window will size itself to fit its content.
        /// </summary>
        public SizeToContent SizeToContent
        {
            get { return Base.GetValue<SizeToContent>(TopLevelBase.SizeToContentProperty); }
            set { Base.SetValue(TopLevelBase.SizeToContentProperty, value); }
        }

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public string Title
        {
            get { return Base.GetValue<string>(TopLevelBase.TitleProperty); }
            set { Base.SetValue(TopLevelBase.TitleProperty, value); }
        }

        /// <inheritdoc/>
        // Type IStyleable.StyleKey // => 
        public Type StyleKey { get { return typeof(Window); } }

        Rect IVisual.Bounds
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsVisible { get { return Base.IsVisible; } set { Base.IsVisible = value; } }
        public Transform RenderTransform { get { return Base.RenderTransform; } set { Base.RenderTransform = value; } }

        public bool ClipToBounds { get; set; }
        public bool IsAttachedToVisualTree { get { return (Base as IVisual).IsAttachedToVisualTree; } }

        public bool IsEffectivelyVisible { get { return Base.IsEffectivelyVisible; } }
        public RelativePoint TransformOrigin { get; set; }
        public IPerspexReadOnlyList<IVisual> VisualChildren { get; }
        public IVisual VisualParent { get { return null; } }

        public int ZIndex { get; set; }
        public IRenderer Renderer { get { return Base.Renderer; } }

        IRenderManager _renderManager;
        public IRenderManager RenderManager { get { return _renderManager; } }

        public ILogical LogicalParent { get { return null; } }

        public IPerspexReadOnlyList<ILogical> LogicalChildren { get { return (Base as ILogical).LogicalChildren; } }

        public Size DesiredSize { get; }

        double ILayoutable.Width { get { return ClientSize.Width; } }

        double ILayoutable.Height { get { return ClientSize.Height; } }

        public double MinWidth { get { return Base.MinWidth; } }
        public double MaxWidth { get { return Base.MaxWidth; } }
        public double MinHeight { get { return Base.MinHeight; } }
        public double MaxHeight { get { return Base.MinHeight; } }

        Thickness ILayoutable.Margin { get { return Base.Margin; } }

        public HorizontalAlignment HorizontalAlignment { get { return Base.HorizontalAlignment; } }
        public VerticalAlignment VerticalAlignment { get { return Base.VerticalAlignment; } }
        public bool IsMeasureValid { get { return Base.IsMeasureValid; } }
        public bool IsArrangeValid { get { return Base.IsArrangeValid; } }
        public Size? PreviousMeasure { get { return Base._PreviousMeasure(); } }
        public Rect? PreviousArrange { get { return Base._PreviousArrange(); } }

        public bool Focusable { get { return Base.Focusable; } }
        public bool IsEnabled { get { return Base.IsEnabled; } }

        Cursor IInputElement.Cursor { get { return Base.Cursor; } }

        public bool IsEnabledCore { get { return Base.IsEnabled; } }
        public bool IsFocused { get { return Base.IsFocused; } }
        public bool IsHitTestVisible { get { return Base.IsHitTestVisible; } }
        public bool IsPointerOver { get { return Base.IsPointerOver; } }
        public IInteractive InteractiveParent { get { return null; } }
        public Classes Classes { get { return Base.Classes; } }
        public ITemplatedControl TemplatedParent { get { return Base.TemplatedParent; } }
        public Styles Styles { get { return Base.Styles; } }

        #endregion

        /// <summary>
        /// Closes the window.
        /// </summary>
        public new void Close()
        {
            // PlatformImpl.Dispose();
        }

        /// <summary>
        /// Closes a dialog window with the specified result.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        /// <remarks>
        /// When the window is shown with the <see cref="ShowDialog{TResult}"/> method, the
        /// resulting task will produce the <see cref="_dialogResult"/> value when the window
        /// is closed.
        /// </remarks>
        public void Close(object dialogResult)
        {
            _dialogResult = dialogResult;
            Close();
        }

        /// <summary>
        /// Hides the window but does not close it.
        /// </summary>
        public new void Hide()
        {
            using (BeginAutoSizing())
            {
                // PlatformImpl.Hide();
            }

            Visible = false;
        }

        /// <summary>
        /// Shows the window.
        /// </summary>
        public new void Show()
        {
            // LayoutManager.ExecuteLayoutPass();

            using (BeginAutoSizing())
            {
                // PlatformImpl.Show();
            }

            base.Show();
        }

        bool AutoSizing = false;

        public event EventHandler<TextInputEventArgs> TextInput;
        public event EventHandler<PointerEventArgs> PointerEnter;
        public event EventHandler<PointerEventArgs> PointerLeave;
        public event EventHandler<PointerPressEventArgs> PointerPressed;
        public event EventHandler<PointerEventArgs> PointerMoved;
        public event EventHandler<PointerEventArgs> PointerReleased;
        public event EventHandler<PointerWheelEventArgs> PointerWheelChanged;

        protected IDisposable BeginAutoSizing()
        {
            AutoSizing = true;
            return Disposable.Create(() => this.AutoSizing = false);
        }

        /// <summary>
        /// Shows the window as a dialog.
        /// </summary>
        /// <returns>
        /// A task that can be used to track the lifetime of the dialog.
        /// </returns>
        public new Task ShowDialog()
        {
            return ShowDialog<object>();
        }

        /// <summary>
        /// Shows the window as a dialog.
        /// </summary>
        /// <typeparam name="TResult">
        /// The type of the result produced by the dialog.
        /// </typeparam>
        /// <returns>.
        /// A task that can be used to retrive the result of the dialog when it closes.
        /// </returns>
        public Task<TResult> ShowDialog<TResult>()
        {
            // LayoutManager.ExecuteLayoutPass();

            using (BeginAutoSizing())
            {
                // var modal = PlatformImpl.ShowDialog();
                var result = new TaskCompletionSource<TResult>();

                Observable.FromEventPattern(this, "Closed") // nameof(Closed))
                    .Take(1)
                    .Subscribe(_ =>
                    {
                        // modal.Dispose();
                        result.SetResult((TResult)_dialogResult);
                    });

                return result.Task;
            }
        }

        public Size ToClientSize() { return new Size(ClientSize.Width, ClientSize.Height); }

        /// <inheritdoc/>
        protected // override 
            Size MeasureOverride(Size availableSize)
        {
            var sizeToContent = SizeToContent;
            Size size = ToClientSize();
            var desired = Base.DoMeasureOverride(availableSize.Constrain(_maxPlatformClientSize));

            switch (sizeToContent)
            {
                case SizeToContent.Width:
                    size = new Size(desired.Width, ClientSize.Height);
                    break;
                case SizeToContent.Height:
                    size = ToClientSize();
                    break;
                case SizeToContent.WidthAndHeight:
                    if (desired.Width > 0 || !Base.IsVisible)
                        size = new Size(desired.Width, desired.Height);
                    else
                        size = new Size(ClientSize.Width, ClientSize.Height);
                    break;
                case SizeToContent.Manual:
                    size = ToClientSize();
                    break;
                default:
                    throw new InvalidOperationException("Invalid value for SizeToContent.");
            }

            return size;
        }

        /// <inheritdoc/>
        protected // override 
            void HandleResized(Size clientSize)
        {
            if (!Base.AutoSizing)
            {
                SizeToContent = SizeToContent.Manual;
            }

            Base.DoHandleResized(clientSize);
        }

        public void Render(IDrawingContext context) { Base.Render(context); }

        public Matrix TransformToVisual(IVisual visual)
        { return Base.TransformToVisual(visual); }

        public Point TranslatePointToScreen(Point p)
        { return (Base as IRenderRoot).TranslatePointToScreen(p); }

        public void ApplyTemplate() { Base.ApplyTemplate(); }
        public void Measure(Size availableSize, bool force = false) { Base.Measure(availableSize, force); }
        public void Arrange(Rect rect, bool force = false) { Arrange(rect, force); }
        public void InvalidateMeasure() { InvalidateMeasure(); }
        public void InvalidateArrange() { InvalidateArrange(); }

        void IInputElement.Focus()
        {
            base.Focus();
        }

        public IInputElement InputHitTest(Point p) { return Base.InputHitTest(p); }
        public IDisposable AddHandler(RoutedEvent routedEvent, Delegate handler, RoutingStrategies routes
            = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false)
        { return Base.AddHandler(routedEvent, handler, routes); }
        public IDisposable AddHandler<TEventArgs>(RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs> handler,
            RoutingStrategies routes = RoutingStrategies.Direct | RoutingStrategies.Bubble, bool handledEventsToo = false) where TEventArgs : RoutedEventArgs
        { return Base.AddHandler<TEventArgs>(routedEvent, handler, routes, handledEventsToo); }

        public void RemoveHandler(RoutedEvent routedEvent, Delegate handler) { Base.RemoveHandler(routedEvent, handler); }
        public void RemoveHandler<TEventArgs>(RoutedEvent<TEventArgs> routedEvent, EventHandler<TEventArgs> handler) where TEventArgs : RoutedEventArgs
        { Base.RemoveHandler<TEventArgs>(routedEvent, handler); }

        public void RaiseEvent(RoutedEventArgs e) { Base.RaiseEvent(e); }
        public IDisposable Bind(PerspexProperty property, IObservable<object> source, BindingPriority priority = BindingPriority.LocalValue)
        { return Base.Bind(property, source, priority); }
        public IDisposable Bind<T>(PerspexProperty<T> property, IObservable<T> source, BindingPriority priority = BindingPriority.LocalValue)
        { return Base.Bind<T>(property, source, priority); }
        public IObservable<object> GetObservable(PerspexProperty property)
        { return Base.GetObservable(property); }
        public IObservable<T> GetObservable<T>(PerspexProperty<T> property)
        { return Base.GetObservable<T>(property); }
        public void ClearValue(PerspexProperty property) { Base.ClearValue(property); }
        public object GetValue(PerspexProperty property)
        { return Base.GetValue(property); }
        public T GetValue<T>(PerspexProperty<T> property)
        { return Base.GetValue<T>(property); }
        public bool IsRegistered(PerspexProperty property)
        { return Base.IsRegistered(property); }
        public bool IsSet(PerspexProperty property)
        { return Base.IsSet(property); }
        public void SetValue(PerspexProperty property, object value, BindingPriority priority = BindingPriority.LocalValue)
        { Base.SetValue(property, value, priority); }
        public void SetValue<T>(PerspexProperty<T> property, T value, BindingPriority priority = BindingPriority.LocalValue)
        { Base.SetValue<T>(property, value, priority); }
    }

    /// <summary>
    /// Determines how a <see cref="Window"/> will size itself to fit its content.
    /// </summary>
    public enum SizeToContent
    {
        /// <summary>
        /// The window will not automatically size itself to fit its content.
        /// </summary>
        Manual,

        /// <summary>
        /// The window will size itself horizontally to fit its content.
        /// </summary>
        Width,

        /// <summary>
        /// The window will size itself vertically to fit its content.
        /// </summary>
        Height,

        /// <summary>
        /// The window will size itself horizontally and vertically to fit its content.
        /// </summary>
        WidthAndHeight,
    }

}
