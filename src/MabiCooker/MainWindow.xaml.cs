using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MabiCooker.Properties;

namespace MabiCooker
{
    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window
    {
        private const double ProgressBarMargin = 25; //23;
        private const double ProgressBarWidthMultiplier = 2.43; //2.32;
        private bool _preventChange;

        public MainWindow()
        {
            InitializeComponent();
            Top = Settings.Default.Top;
            Left = Settings.Default.Left;

            var items = new List<Recipe>
            {
                new Recipe("Custom", 0, 0, 0),
                new Recipe("Basil Tea", 15, 85, 0),
                new Recipe("Vegetable Canape", 40, 50, 10),
                new Recipe("Tomato Basil Salad", 50, 30, 20),
                new Recipe("Steamed Potato", 42, 58, 0),
            };

            ComboBoxRecipes.ItemsSource = items;

            DrawBars(0, 0, 0);
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            var x = Left;
            var y = Top;

            switch (e.Key)
            {
                case Key.Left:
                    --x;
                    break;

                case Key.Up:
                    --y;
                    break;

                case Key.Right:
                    ++x;
                    break;

                case Key.Down:
                    ++y;
                    break;
            }

            if (Math.Abs(x - Left) < 0.001 && Math.Abs(y - Top) < 0.001)
            {
                return;
            }

            Left = x;
            Top = y;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.Top = Top;
            Settings.Default.Left = Left;
            Settings.Default.Save();
        }

        private void ComboBoxRecipes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _preventChange = true;

            var comboBox = sender as ComboBox;
            if (comboBox == null)
            {
                return;
            }

            var recipe = comboBox.SelectedItem as Recipe;

            if (recipe.Ingredient1 == 0 && recipe.Ingredient2 == 0 && recipe.Ingredient3 == 0)
            {
                TextBoxIngredient1.IsEnabled = true;
                TextBoxIngredient2.IsEnabled = true;

                TextBoxIngredient1.Text = string.Empty;
                TextBoxIngredient2.Text = string.Empty;
                TextBoxIngredient3.Text = string.Empty;

                DrawBars(0, 0, 0);
            }
            else
            {
                TextBoxIngredient1.IsEnabled = false;
                TextBoxIngredient2.IsEnabled = false;

                TextBoxIngredient1.Text = recipe.Ingredient1.ToString();
                TextBoxIngredient2.Text = recipe.Ingredient2.ToString();
                TextBoxIngredient3.Text = recipe.Ingredient3.ToString();

                DrawBars(recipe.Ingredient1, recipe.Ingredient2, recipe.Ingredient3);
            }

            _preventChange = false;
        }

        private void DrawBars(double num1, double num2, double num3)
        {
            Bar1.Width = (int)Math.Round(num1 * ProgressBarWidthMultiplier);
            Bar2.Width = (int)Math.Round(num2 * ProgressBarWidthMultiplier);
            Bar3.Width = (int)Math.Round(num3 * ProgressBarWidthMultiplier);

            var top = Bar1.Margin.Top;
            Bar1.Margin = new Thickness(ProgressBarMargin, top, 0, 0);
            Bar2.Margin = new Thickness(Bar1.Margin.Left + Bar1.Width, top, 0, 0);
            Bar3.Margin = new Thickness(Bar2.Margin.Left + Bar2.Width, top, 0, 0);
        }

        private void HandleInput()
        {
            var input1 = TextBoxIngredient1.Text != string.Empty ? TextBoxIngredient1.Text : "0";
            var input2 = TextBoxIngredient2.Text != string.Empty ? TextBoxIngredient2.Text : "0";

            var num1 = Convert.ToDouble(input1);
            var num2 = Convert.ToDouble(input2);
            var num3 = 100.0 - num1 - num2;

            num1 = LimitToRange(num1, 0, 100);
            num2 = LimitToRange(num2, 0, 100);
            num3 = LimitToRange(num3, 0, 100);

            var total = num1 + num2 + num3;

            if (num1 > 0 && num2 > 0 && total >= 0 && total <= 100)
            {
                TextBoxIngredient3.Text = num3.ToString(CultureInfo.InvariantCulture);
                DrawBars(num1, num2, num3);
            }
            else
            {
                TextBoxIngredient3.Text = string.Empty;
                DrawBars(0, 0, 0);
            }
        }

        private static bool IsTextAllowed(string text)
        {
            var regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return !regex.IsMatch(text);
        }

        private void TextBoxIngredient_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var isTextAllowed = IsTextAllowed(e.Text);
            e.Handled = !isTextAllowed;
        }

        private void TextBoxIngredient_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_preventChange)
            {
                HandleInput();
            }
        }

        public static double LimitToRange(double value, double inclusiveMinimum, double inclusiveMaximum)
        {
            if (value < inclusiveMinimum) { return inclusiveMinimum; }
            if (value > inclusiveMaximum) { return inclusiveMaximum; }
            return value;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // Begin dragging the window
            this.DragMove();
        }

        private void CommandBinding_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void CommandBinding_Executed_3(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void ComboBoxRecipes_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Right)
            {
                e.Handled = true;

                // perform awesomeness
            }
            else
            {
                base.OnPreviewKeyDown(e);
            }
        }
    }
}