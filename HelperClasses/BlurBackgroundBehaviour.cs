using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;

namespace Project_127
{
	public class BlurBackgroundBehavior : Behavior<Shape>
	{
		public static readonly DependencyProperty BlurContainerProperty
			= DependencyProperty.Register(
										  "BlurContainer",
										  typeof(UIElement),
										  typeof(BlurBackgroundBehavior),
										  new PropertyMetadata(OnContainerChanged));

		private static readonly DependencyProperty BrushProperty
			= DependencyProperty.Register(
										  "Brush",
										  typeof(VisualBrush),
										  typeof(BlurBackgroundBehavior),
										  new PropertyMetadata());

		private VisualBrush Brush
		{
			get { return (VisualBrush)this.GetValue(BrushProperty); }
			set { this.SetValue(BrushProperty, value); }
		}

		public UIElement BlurContainer
		{
			get { return (UIElement)this.GetValue(BlurContainerProperty); }
			set { this.SetValue(BlurContainerProperty, value); }
		}

		protected override void OnAttached()
		{
			this.AssociatedObject.Effect = new BlurEffect
			{
				Radius = 20,
				KernelType = KernelType.Gaussian,
				RenderingBias = RenderingBias.Quality
			};

			this.AssociatedObject.SetBinding(Shape.FillProperty,
											 new Binding
											 {
												 Source = this,
												 Path = new PropertyPath(BrushProperty)
											 });

			this.AssociatedObject.LayoutUpdated += (sender, args) => this.UpdateBounds();
			this.UpdateBounds();
		}

		protected override void OnDetaching()
		{
			BindingOperations.ClearBinding(this.AssociatedObject, Border.BackgroundProperty);
		}

		private static void OnContainerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			((BlurBackgroundBehavior)d).OnContainerChanged((UIElement)e.OldValue, (UIElement)e.NewValue);
		}

		private void OnContainerChanged(UIElement oldValue, UIElement newValue)
		{
			if (oldValue != null)
			{
				oldValue.LayoutUpdated -= this.OnContainerLayoutUpdated;
			}

			if (newValue != null)
			{
				this.Brush = new VisualBrush(newValue)
				{
					ViewboxUnits = BrushMappingMode.Absolute
				};

				newValue.LayoutUpdated += this.OnContainerLayoutUpdated;
				this.UpdateBounds();
			}
			else
			{
				this.Brush = null;
			}
		}

		private void OnContainerLayoutUpdated(object sender, EventArgs eventArgs)
		{
			this.UpdateBounds();
		}

		private void UpdateBounds()
		{
			if (this.AssociatedObject != null && this.BlurContainer != null && this.Brush != null)
			{
				Point difference = this.AssociatedObject.TranslatePoint(new Point(), this.BlurContainer);
				this.Brush.Viewbox = new Rect(difference, this.AssociatedObject.RenderSize);
			}
		}
	}
}