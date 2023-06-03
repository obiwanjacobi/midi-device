using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace CannedBytes.Windows.Controls
{
    [TemplatePart(Name = "PART_ScrollViewer", Type = typeof(ScrollViewer))]
    public class Panorama : ItemsControl
    {
        private ScrollViewer _scrollViewer;
        private Point _scrollTarget;
        private Point _scrollStart;
        private Point _scrollStartOffset;
        private Point _previousPoint;
        private Vector _velocity;
        private double _friction;
        private DispatcherTimer _animationTimer = new DispatcherTimer(DispatcherPriority.DataBind);
        private const int PixelsToMoveToBeConsideredScroll = 5;
        private const int PixelsToMoveToBeConsideredClick = 2;

        static Panorama()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Panorama), new FrameworkPropertyMetadata(typeof(Panorama)));
        }

        public Panorama()
        {
            _friction = 0.85;

            _animationTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            _animationTimer.Tick += new EventHandler(HandleWorldTimerTick);
            //_animationTimer.Start();
        }

        public override void OnApplyTemplate()
        {
            _scrollViewer = (ScrollViewer)Template.FindName("PART_ScrollViewer", this);
            base.OnApplyTemplate();
        }

        bool _mouseDownFlag;
        Cursor _savedCursor;

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (_scrollViewer != null &&
                _scrollViewer.IsMouseOver)
            {
                _mouseDownFlag = true;

                // Save starting point, used later when determining how much to scroll.
                _scrollStart = e.GetPosition(this);
                _scrollStartOffset.X = _scrollViewer.HorizontalOffset;
                _scrollStartOffset.Y = _scrollViewer.VerticalOffset;
            }

            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            if (_mouseDownFlag)
            {
                Point currentPoint = e.GetPosition(this);

                // Determine the new amount to scroll.
                Point delta = new Point(_scrollStart.X - currentPoint.X, _scrollStart.Y - currentPoint.Y);

                if (Math.Abs(delta.X) > PixelsToMoveToBeConsideredScroll ||
                    Math.Abs(delta.Y) > PixelsToMoveToBeConsideredScroll)
                {
                    _scrollTarget.X = _scrollStartOffset.X + delta.X;
                    _scrollTarget.Y = _scrollStartOffset.Y + delta.Y;

                    // Scroll to the new position.
                    _scrollViewer.ScrollToHorizontalOffset(_scrollTarget.X);
                    _scrollViewer.ScrollToVerticalOffset(_scrollTarget.Y);

                    if (!this.IsMouseCaptured)
                    {
                        if ((_scrollViewer.ExtentWidth > _scrollViewer.ViewportWidth) ||
                            (_scrollViewer.ExtentHeight > _scrollViewer.ViewportHeight))
                        {
                            _savedCursor = this.Cursor;
                            this.Cursor = Cursors.ScrollWE;
                        }

                        this.CaptureMouse();
                    }

                    if (!_animationTimer.IsEnabled)
                    {
                        _animationTimer.Start();
                    }
                }
            }

            base.OnPreviewMouseMove(e);
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            bool mouseDownFlag = _mouseDownFlag;
            // mouse move events may trigger while inside this handler.
            _mouseDownFlag = false;

            if (this.IsMouseCaptured)
            {
                // scroll action stopped
                this.Cursor = _savedCursor;
                this.ReleaseMouseCapture();
            }
            else if (mouseDownFlag)
            {
                // click action stopped
            }

            _savedCursor = null;

            base.OnPreviewMouseLeftButtonUp(e);
        }

        private void HandleWorldTimerTick(object sender, EventArgs e)
        {
            if (ControlExtensions.IsInDesignMode)
            {
                return;
            }

            if (this.IsMouseCaptured)
            {
                Point currentPoint = Mouse.GetPosition(this);
                _velocity = _previousPoint - currentPoint;
                _previousPoint = currentPoint;
            }
            else
            {
                DoAutoScrolling();

                if (_velocity.Length < 1.0 && _animationTimer.IsEnabled)
                {
                    _animationTimer.Stop();
                }
            }
        }

        private void DoAutoScrolling()
        {
            if (_scrollViewer == null) return;

            _scrollViewer.ScrollToHorizontalOffset(_scrollTarget.X);
            _scrollViewer.ScrollToVerticalOffset(_scrollTarget.Y);

            _scrollTarget.X += _velocity.X;
            _scrollTarget.Y += _velocity.Y;

            _velocity *= _friction;
        }
    }
}