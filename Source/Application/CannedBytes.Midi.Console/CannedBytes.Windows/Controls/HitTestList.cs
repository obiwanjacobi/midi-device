using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace CannedBytes.Windows.Controls
{
    public class HitTestList
    {
        private List<DependencyObject> _hits = new List<DependencyObject>();

        public HitTestList(Visual rootVisual, Point point)
        {
            VisualTreeHelper.HitTest(rootVisual, OnHitTestFilter, OnHitTestResult, new PointHitTestParameters(point));
        }

        protected virtual HitTestFilterBehavior OnHitTestFilter(DependencyObject potentialHitTestTarget)
        {
            return potentialHitTestTarget == null ? HitTestFilterBehavior.ContinueSkipSelfAndChildren : HitTestFilterBehavior.Continue;
        }

        protected virtual HitTestResultBehavior OnHitTestResult(HitTestResult result)
        {
            AddHit(result.VisualHit);
            return HitTestResultBehavior.Continue;
        }

        protected void AddHit(DependencyObject hitObject)
        {
            _hits.Add(hitObject);
        }

        public IEnumerable<DependencyObject> Hits()
        {
            return _hits;
        }

        public IEnumerable<T> Hits<T>() where T : DependencyObject
        {
            return from hobj in _hits
                   where hobj is T
                   select hobj as T;
        }
    }
}