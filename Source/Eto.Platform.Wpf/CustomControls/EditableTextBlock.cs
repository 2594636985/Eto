using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Eto.Platform.Wpf.CustomControls
{
	public class EditableTextBlock : UserControl
	{
		string oldText;

		public EditableTextBlock()
		{
			var textBox = new FrameworkElementFactory(typeof(TextBox));
			textBox.SetValue(Control.PaddingProperty, new Thickness(1)); // 1px for border
			textBox.AddHandler(FrameworkElement.LoadedEvent, new RoutedEventHandler(TextBox_Loaded));
			textBox.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
			textBox.AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(TextBox_LostFocus));
			textBox.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Text") { Source = this, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
			var editTemplate = new DataTemplate { VisualTree = textBox };

			var textBlock = new FrameworkElementFactory(typeof(TextBlock));
			textBlock.SetValue(FrameworkElement.MarginProperty, new Thickness(2));
			textBlock.AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(TextBlock_MouseDown));
			textBlock.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding("Text") { Source = this });
			var viewTemplate = new DataTemplate { VisualTree = textBlock };

			var style = new System.Windows.Style(typeof(EditableTextBlock));
			var trigger = new Trigger { Property = IsInEditModeProperty, Value = true };
			trigger.Setters.Add(new Setter { Property = ContentTemplateProperty, Value = editTemplate });
			style.Triggers.Add(trigger);

			trigger = new Trigger { Property = IsInEditModeProperty, Value = false };
			trigger.Setters.Add(new Setter { Property = ContentTemplateProperty, Value = viewTemplate });
			style.Triggers.Add(trigger);
			Style = style;
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register(
			"Text",
			typeof(string),
			typeof(EditableTextBlock),
			new PropertyMetadata(""));

		public bool IsEditable
		{
			get { return (bool)GetValue(IsEditableProperty); }
			set { SetValue(IsEditableProperty, value); }
		}

		public static readonly DependencyProperty IsEditableProperty =
			DependencyProperty.Register(
			"IsEditable",
			typeof(bool),
			typeof(EditableTextBlock),
			new PropertyMetadata(true));

		public bool IsInEditMode
		{
			get
			{
				return IsEditable && (bool)GetValue(IsInEditModeProperty);
			}
			set
			{
				if (IsEditable)
				{
					if (value) oldText = Text;
					SetValue(IsInEditModeProperty, value);
				}
			}
		}
		public static readonly DependencyProperty IsInEditModeProperty =
			DependencyProperty.Register(
			"IsInEditMode",
			typeof(bool),
			typeof(EditableTextBlock),
			new PropertyMetadata(false));

		static void TextBox_Loaded(object sender, RoutedEventArgs e)
		{
			var textBox = (TextBox)sender;
			textBox.Focus();
			textBox.SelectAll();
		}

		void TextBox_LostFocus(object sender, RoutedEventArgs e)
		{
			IsInEditMode = false;
		}

		void SetParentFocus()
		{
			var item = this.GetParent<TreeViewItem>();
			if (item != null)
				item.Focus();
		}

		void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				IsInEditMode = false;
				SetParentFocus();
				e.Handled = true;
			}
			else if (e.Key == Key.Escape)
			{
				IsInEditMode = false;
				SetParentFocus();
				Text = oldText;
				e.Handled = true;
			}
		}

		void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ButtonState == MouseButtonState.Pressed && e.ClickCount >= 2 && e.ChangedButton == MouseButton.Left)
			{
				IsInEditMode = true;
				e.Handled = true;
			}
		}
	}
}
