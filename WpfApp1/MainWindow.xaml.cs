using Ookii.Dialogs.Wpf;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Windows;
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
            InitializeComponent();

            Persons = new ObservableCollection<Person>();
            PersonsGrid.ItemsSource = Persons;
            PersonsListBox.ItemsSource = Persons;

            Paths = new ObservableCollection<Path>();
            PathsGrid.ItemsSource = Paths;

            Users = new ObservableCollection<User>();
            UsersComboBox.ItemsSource = Users;

            Groups = new ObservableCollection<Group>();
            GroupsListBox.ItemsSource = Groups;
        }

        public ObservableCollection<Person> Persons { get; set; }
        public ObservableCollection<Path> Paths { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<Group> Groups { get; set; }

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

        internal static async Task HeavyMethod(Label label)
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

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            Paths.Clear();
            char[] quotes = { '\"', '\'' };
            string path = null;

            try
            {
                path = System.IO.Path.GetFullPath(PathTextBox.Text.Trim(quotes));
            }
            catch (System.ArgumentException ex)
            {
                string message = ex.Message;
                MessageBox.Show(message);
            }
            catch (System.Exception ex)
            {
                string message = ex.Message;
                MessageBox.Show(message);
            }

            PathTextBox.Clear();

            while (path != null)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (System.IO.File.Exists(path))
                    {
                        var type = "File";
                        Paths.Add(new Path(path, type));
                    }
                    else if (System.IO.Directory.Exists(path))
                    {
                        var type = "Directory";
                        DirectoryInfo dir = new DirectoryInfo(path);

                        try
                        {
                            DirectorySecurity sec = dir.GetAccessControl();
                            foreach (FileSystemAccessRule acl in sec.GetAccessRules(true, true, typeof(NTAccount)))
                            {
                                IdentityReference i = acl.IdentityReference;
                                AccessControlType a = acl.AccessControlType;
                                FileSystemRights f = acl.FileSystemRights;

                                Paths.Add(new Path(path, type, i, a, f));
                            }
                        }
                        catch (System.UnauthorizedAccessException ex)
                        {
                            string message = ex.Message;
                            MessageBox.Show(message);
                        }
                        catch (System.Exception ex)
                        {
                            string message = ex.Message;
                            MessageBox.Show(message);
                        }
                    }
                }

                var _p = Directory.GetParent(path);
                path = _p?.FullName;
            }
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VistaFolderBrowserDialog folderDialog = new VistaFolderBrowserDialog();
                folderDialog.Reset();

                //folderDialog.RootFolder = System.Environment.SpecialFolder.Desktop;
                folderDialog.ShowNewFolderButton = true;
                folderDialog.Description = "Select a folder!";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowDialog();

                PathTextBox.Text = folderDialog.SelectedPath;
                folderDialog.Reset();
            }
            catch (System.Exception ex)
            {
                string message = ex.Message;
                MessageBox.Show(message);
            }
        }

        private void SearcheButton_Click(object sender, RoutedEventArgs e)
        {
            Users.Clear();

            string usr = UserTextBox.Text;
            SearchResultCollection results = Ldap.SearchForUsers(usr);

            foreach (SearchResult sr in results)
            {
                Users.Add(new User(
                    sr.GetPropertyValue("name"),
                    sr.GetPropertyValue("samAccountName"),
                    sr.GetPropertyValue("UserPrincipalName"),
                    sr.GetPropertyValue("DistinguishedName")
                    ));
            }

            UsersComboBox.SelectedIndex = 0;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            Paths.Clear();
            Users.Clear();
            Groups.Clear();

            UserTextBox.Clear();
            PathTextBox.Clear();
            DistinguishedNameLabel.Content = null;
        }

        private void GroupsButton_Click(object sender, RoutedEventArgs e)
        {
            Groups.Clear();
            var index = UsersComboBox.SelectedIndex;
            string usr = Users[index].DistinguishedName;
            DistinguishedNameLabel.Content = usr;
            SearchResultCollection results = Ldap.GetAllNestedGroups(usr);

            foreach (SearchResult sr in results)
            {
                Groups.Add(new Group(sr.GetPropertyValue("name")));
            }
        }
    }
}
