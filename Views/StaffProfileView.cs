using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Data;
using System;

namespace HR_Applicant_System.Views
{
    public class StaffProfileView : Window
    {
        public StaffProfileView()
        {
            Width = 450;
            Height = 550;
            Title = "Edit Staff Profile";
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = new SolidColorBrush(Color.Parse("#1e1e1e"));

            StackPanel formStack = new StackPanel
            {
                Spacing = 15,
                Margin = new Thickness(40)
            };

            formStack.Children.Add(new TextBlock 
            { 
                Text = "Personal Information", 
                FontSize = 24, 
                FontWeight = FontWeight.Bold,
                Margin = new Thickness(0,0,0,10),
                Foreground = Brushes.White
            });

            formStack.Children.Add(new TextBlock { Text = "Full Name", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White });
            var nameBox = new TextBox();
            nameBox.Bind(TextBox.TextProperty, new Binding("FullName"));
            formStack.Children.Add(nameBox);

            formStack.Children.Add(new TextBlock { Text = "Staff Email (Read-Only)", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White });
            var emailBox = new TextBox { IsReadOnly = true, Opacity = 0.6 };
            emailBox.Bind(TextBox.TextProperty, new Binding("StaffEmail"));
            formStack.Children.Add(emailBox);

            formStack.Children.Add(new TextBlock { Text = "Department", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White });
            var deptBox = new TextBox();
            deptBox.Bind(TextBox.TextProperty, new Binding("Department"));
            formStack.Children.Add(deptBox);

            formStack.Children.Add(new TextBlock { Text = "Professional Bio", FontWeight = FontWeight.SemiBold, Foreground = Brushes.White });
            var bioBox = new TextBox { Height = 120, AcceptsReturn = true, TextWrapping = TextWrapping.Wrap };
            bioBox.Bind(TextBox.TextProperty, new Binding("Bio"));
            formStack.Children.Add(bioBox);

            Button btnSave = new Button
            {
                Content = "Save Profile Changes",
                Height = 45,
                Background = new SolidColorBrush(Color.Parse("#10B981")),
                Foreground = Brushes.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                FontWeight = FontWeight.Bold,
                Margin = new Thickness(0, 15, 0, 0)
            };
            btnSave.Bind(Button.CommandProperty, new Binding("SaveProfile"));
            formStack.Children.Add(btnSave);

            Content = new Border
            {
                Background = new SolidColorBrush(Color.Parse("#252525")),
                CornerRadius = new CornerRadius(12),
                Padding = new Thickness(5),
                Margin = new Thickness(15),
                Child = formStack
            };
        }
    }
}
