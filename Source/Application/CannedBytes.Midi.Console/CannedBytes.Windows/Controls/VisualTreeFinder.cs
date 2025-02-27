﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace CannedBytes.Windows.Controls
{
    /// <summary>
    /// This class was obtained from Philipp Sumi
    /// http://www.hardcodet.net/uploads/2009/06/UIHelper.cs
    /// </summary>
    public static class VisualTreeFinder
    {
        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T TryFindParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            // Get parent item
            DependencyObject parentObject = GetParentObject(child);

            // We've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            // Check if the parent matches the type we're looking for
            T parent = parentObject as T;

            if (parent != null)
            {
                return parent;
            }

            // Use recursion to proceed with next level
            return TryFindParent<T>(parentObject);
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Keep in mind that for content element,
        /// this method falls back to the logical tree of the element!
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        public static DependencyObject GetParentObject(this DependencyObject child)
        {
            if (child == null) return null;

            // Handle content elements separately
            var contentElement = child as ContentElement;

            if (contentElement != null)
            {
                var parent = ContentOperations.GetParent(contentElement);

                if (parent != null)
                {
                    return parent;
                }

                var fce = contentElement as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            // Also try searching for parent in framework elements (such as DockPanel, etc)
            var frameworkElement = child as FrameworkElement;

            if (frameworkElement != null)
            {
                DependencyObject parent = frameworkElement.Parent;

                if (parent != null)
                {
                    return parent;
                }
            }

            // If it's not a ContentElement/FrameworkElement, rely on VisualTreeHelper
            return VisualTreeHelper.GetParent(child);
        }

        /// <summary>
        /// Analyzes both visual and logical tree in order to find all elements of a given
        /// type that are descendants of the <paramref name="source"/> item.
        /// </summary>
        /// <typeparam name="T">The type of the queried items.</typeparam>
        /// <param name="source">The root element that marks the source of the search. If the
        /// source is already of the requested type, it will not be included in the result.</param>
        /// <returns>All descendants of <paramref name="source"/> that match the requested type.</returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject source)
            where T : DependencyObject
        {
            if (source != null)
            {
                var childs = GetChildObjects(source);

                foreach (DependencyObject child in childs)
                {
                    // Analyze if children match the requested type
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    // Recurs tree
                    foreach (T descendant in FindChildren<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetChild"/> method, which also
        /// supports content elements. Keep in mind that for content elements,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="parent">The item to be processed.</param>
        /// <returns>The submitted item's child elements, if available.</returns>
        public static IEnumerable<DependencyObject> GetChildObjects(this DependencyObject parent)
        {
            if (parent != null)
            {
                if (parent is ContentElement || parent is FrameworkElement)
                {
                    // Use the logical tree for content / framework elements
                    foreach (object obj in LogicalTreeHelper.GetChildren(parent))
                    {
                        var depObj = obj as DependencyObject;

                        if (depObj != null)
                        {
                            yield return (DependencyObject)obj;
                        }
                    }
                }
                else
                {
                    // Use the visual tree per default
                    int count = VisualTreeHelper.GetChildrenCount(parent);

                    for (int i = 0; i < count; i++)
                    {
                        yield return VisualTreeHelper.GetChild(parent, i);
                    }
                }
            }
        }

        /// <summary>
        /// Tries to locate a given item within the visual tree,
        /// starting with the dependency object at a given position.
        /// </summary>
        /// <typeparam name="T">The type of the element to be found
        /// on the visual tree of the element at the given location.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        public static T TryFindFromPoint<T>(this UIElement reference, Point point)
            where T : DependencyObject
        {
            var element = reference.InputHitTest(point) as DependencyObject;

            if (element == null)
            {
                return null;
            }

            if (element is T)
            {
                return (T)element;
            }

            return TryFindParent<T>(element);
        }
    }
}