using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TestNETFCanvas
{
	public class SuperCanvas : Canvas
    {
        private AdornerLayer aLayer;

        private bool isDown;
        private bool isDragging;
        private double originalLeft;
        private double originalTop;
        private bool selected;
        private UIElement selectedElement;
        private ContentPresenter selectedPresenter;

        private const int snapThreshold = 50;

        private Point startPoint;

        private bool locked;

        public SuperCanvas()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
			//MouseLeftButtonDown += WorkspaceWindow_MouseLeftButtonDown;
   //         MouseLeftButtonUp += DragFinishedMouseHandler;
            MouseMove += WorkspaceWindow_MouseMove;
            MouseLeave += Window1_MouseLeave;

            MouseLeftButtonDown += myCanvas_PreviewMouseLeftButtonDown;
            MouseLeftButtonUp += DragFinishedMouseHandler;
        }

        // Handler for drag stopping on leaving the window
        private void Window1_MouseLeave(object sender, MouseEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        // Handler for drag stopping on user choice
        private void DragFinishedMouseHandler(object sender, MouseButtonEventArgs e)
        {
            StopDragging();
            e.Handled = true;
        }

        // Method for stopping dragging
        private void StopDragging()
        {
            if (isDown)
            {
                isDown = false;
                isDragging = false;

                // MAGO: find nearby elements and snap on them if close enough

                // get current selected element position
                var selectedLeft = GetLeft(selectedPresenter);
                var selectedTop = GetTop(selectedPresenter);

                // get all the other elements
                IEnumerable<ContentPresenter> elements = this.Children.OfType<ContentPresenter>().Where(x => x != selectedPresenter);

                foreach (var element in elements)
                {
                    // do something with the rectangle
                    var left = GetLeft(element);
                    var top = GetTop(element);

                    // if it's close enough (top-left corner of the selected element with the top-right coner of another element)
                    if (selectedLeft >= (left + element.ActualWidth - snapThreshold) && selectedLeft <= (left + element.ActualWidth + snapThreshold) &&
                        selectedTop >= (top - snapThreshold) && selectedTop <= (top + snapThreshold))
					{
                        SetTop(selectedPresenter, top);
                        SetLeft(selectedPresenter, left + element.ActualWidth);
                        return;
                    }

                    // if it's close enough (on the right)
                    if ((selectedLeft + selectedPresenter.ActualWidth) >= (left - snapThreshold) && (selectedLeft + selectedPresenter.ActualWidth) <= (left + snapThreshold) &&
                        selectedTop >= (top - snapThreshold) && selectedTop <= (top + snapThreshold))
                    {
                        SetTop(selectedPresenter, top);
                        SetLeft(selectedPresenter, left - selectedPresenter.ActualWidth);
                        return;
                    }
                }

                Trace.WriteLine("Found " + elements.Count() + " rectangles");

                SetZIndex(selectedPresenter, 0);
                // ***
            }
        }

        // Hanler for providing drag operation with selected element
        private void WorkspaceWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if (locked) return;

            if (isDown)
            {
                if ((isDragging == false) &&
                    ((Math.Abs(e.GetPosition(this).X - startPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                     (Math.Abs(e.GetPosition(this).Y - startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    isDragging = true;

                if (isDragging)
                {
                    Point position = Mouse.GetPosition(this);

                    // MAGO: mod
                    SetTop(selectedPresenter, position.Y - (startPoint.Y - originalTop));
                    SetLeft(selectedPresenter, position.X - (startPoint.X - originalLeft));

                    //SetTop(selectedElement, position.Y - (startPoint.Y - originalTop));
                    //SetLeft(selectedElement, position.X - (startPoint.X - originalLeft));
                    // ***
                }
            }
        }

        // Handler for clearing element selection, adorner removal
        private void WorkspaceWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (locked) return;

            if (selected)
            {
                selected = false;
                if (selectedElement != null)
                {
                    aLayer.Remove(aLayer.GetAdorners(selectedElement)[0]);
                    selectedElement = null;
                }
            }
        }

        // Handler for element selection on the canvas providing resizing adorner
        private void myCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //add code to lock dragging and dropping things.
            if (locked)
            {
                e.Handled = true;
                return;
            }

            // Remove selection on clicking anywhere the window
            if (selected)
            {
                selected = false;
                if (selectedElement != null)
                {
                    // Remove the adorner from the selected element
                    aLayer.Remove(aLayer.GetAdorners(selectedElement)[0]);
                    selectedElement = null;
                }
            }

            // If any element except canvas is clicked, 
            // assign the selected element and add the adorner
            if (e.Source != this)
            {
                isDown = true;
                startPoint = e.GetPosition(this);

                selectedElement = e.Source as UIElement;

                // MAGO: mod
                selectedPresenter = VisualTreeHelper.GetParent(selectedElement) as ContentPresenter;
                originalLeft = GetLeft(selectedPresenter);
                originalTop = GetTop(selectedPresenter);

                // set it on top
                SetZIndex(selectedPresenter, 1000);

                //originalLeft = GetLeft(selectedElement);
                //originalTop = GetTop(selectedElement);
                // ***

                aLayer = AdornerLayer.GetAdornerLayer(selectedElement);
                aLayer.Add(new ResizingAdorner(selectedElement));
                selected = true;
                e.Handled = true;
            }
        }
    }
}
