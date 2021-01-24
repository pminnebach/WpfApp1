using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Persons = new ObservableCollection<Person>();

            InitializeComponent();

            PersonsGrid.ItemsSource = Persons;
            PersonsListBox.ItemsSource = Persons;
        }

        public ObservableCollection<Person> Persons { get; set; }        

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            RandomLabel.Content = "Started";
            await HeavyMethod(RandomLabel);
            RandomLabel.Content += "Done";

            string _f = FirstBox.Text;
            string _l = LastBox.Text;

            if (!string.IsNullOrWhiteSpace(_f) || !string.IsNullOrWhiteSpace(_l))
            {
                Persons.Add(new Person(_f, _l));
                FirstBox.Clear();
                LastBox.Clear();
            }            
        }

        internal async Task HeavyMethod(Label label)
        {
            for (int i = 0; i < 10; i++)
            {
                label.Dispatcher.Invoke(() =>
                {
                    // UI operation goes inside of Invoke
                    label.Content += ".";
                    // Note that: 
                    //    Dispatcher.Invoke() blocks the UI thread anyway
                    //    but without it you can't modify UI objects from another thread
                });

                // CPU-bound or I/O-bound operation goes outside of Invoke
                // await won't block UI thread, unless it's run in a synchronous context
                await Task.Delay(500);
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            Person toRemove = (Person)PersonsGrid.SelectedItem;
            Persons.Remove(toRemove);
        }

        private void RemoveListButton_Click(object sender, RoutedEventArgs e)
        {
            Person toRemove = (Person)PersonsListBox.SelectedItem;
            Persons.Remove(toRemove);
        }
    }
}
