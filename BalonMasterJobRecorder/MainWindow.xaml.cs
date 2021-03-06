﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Logic;

/*
 * Small application for my office (simple data recorder)
 * made by [aleksa.djdj  @  gmail  .  com]
 */

namespace BalonMasterJobRecorder
{
    public partial class MainWindow : Window
    {
        private readonly BallonLogic _ballonLogic;

        public MainWindow()
        {
            InitializeComponent();

            _ballonLogic = new BallonLogic();
            WindowLoaded();
            dateTextBox.Text = DateTime.Now.ToShortDateString();
            dateTextBox.Focus();
        }

        private void WindowLoaded()
        {
            var result = _ballonLogic.TestConnection();
            
            if(result == false)
            {
                MessageBox.Show("Can't connect to Database!", "ERROR");
                this.Close();
            }
        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            if (SimpleInputCheck() == false)
                return;

            GetInputData(out string date, out string store, out string dimension,
                         out string color, out string description);

            var result = _ballonLogic.Write(date, store, dimension, color, description);

            if (result == true)
                ClearTextBoxes();
            else
                MessageBox.Show("Can't write to database!", "ERROR");

        }

        private bool SimpleInputCheck()
        {
            if (dateTextBox.Text == String.Empty)
            {
                MessageBox.Show("Job date text box empty!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (dimensionTextBox.Text == String.Empty)
            {
                MessageBox.Show("Width text box empty!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (dimensionTextBox2.Text == String.Empty)
            {
                MessageBox.Show("Height text box empty!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            if (descriptionTextBox.Text == String.Empty)
            {
                MessageBox.Show("Description text box empty!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }

            return true;
        }

        private void GetInputData(out string date, out string store, out string dimension, 
                                    out string color, out string description)
        {
            date = dateTextBox.Text.Replace('-', '.');
            var storeSelectedItem = storeListBox.SelectedItem as ListBoxItem;
            store = storeSelectedItem.Content.ToString();
            if (numberTimesTextBox.Text.Equals(String.Empty))
                dimension = String.Concat(dimensionTextBox.Text, "x", dimensionTextBox2.Text, "cm");
            else
                dimension = String.Concat(dimensionTextBox.Text, "x", dimensionTextBox2.Text, "cm x", numberTimesTextBox.Text, "kom");

            color = colorComboBox.Text;
            description = descriptionTextBox.Text;
        }

        private void ClearTextBoxes()
        {
            dimensionTextBox.Text = String.Empty;
            dimensionTextBox2.Text = String.Empty;
            numberTimesTextBox.Text = String.Empty;
            descriptionTextBox.Text = String.Empty;
        }

        private async void ShowData_Click(object sender, RoutedEventArgs e)
        {
            var task = new Task<bool>(_ballonLogic.CreateHTMLFile);
            task.Start();

            var result = await task;

            if (result == true)
            {
                var result2 = _ballonLogic.LunchHtmlFile();

                if (result2 == false)
                    MessageBox.Show("Can't open HTML file!", "ERROR");
            }
            else
            {
                MessageBox.Show("Can't create/save HTML file!", "ERROR");
            }
        }

        private void DateButton_Click(object sender, RoutedEventArgs e)
        {
            dateTextBox.Text = String.Empty;
            dateTextBox.Text = DateTime.Now.ToShortDateString();
        }
    }
}
