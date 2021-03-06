﻿using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ModdingStudio;
using Xceed.Wpf.AvalonDock.Layout.Serialization;
using System.IO;
using ModdingStudio.Commands;
using AvalonDock.Themes;
using System.ComponentModel;
using System.Windows.Media.Animation;
using System.Windows.Markup;

namespace ModdingStudio.Applications
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private MainWindowViewModel _vm;
        private Boolean isFading = false;

        public MainWindow()
        {
            _vm = new MainWindowViewModel(this);
            Application.Instance.ApplicationViewModel = this.VM;
            InitializeComponent();
            this.AllowsTransparency = true;
            this.DataContext = this.VM;
            this.VM.showSolutionExplorer(false);
            this.dockingManager.Theme = new MetroTheme();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if(e.Property.Name == "Opacity" && isFading)
            {
                double newOpac = (double)e.NewValue;
                SolidColorBrush brush = this.GlowBrush.CloneCurrentValue();
                brush.Opacity = newOpac;
                this.GlowBrush = brush;
            }
        }

        public MainWindowViewModel VM 
        {
            get { return _vm; }
            set { _vm = value; }
        }

        public void LayoutRoot_PropertyChanging(object sender, System.ComponentModel.PropertyChangingEventArgs e)
        {
            _vm.LayoutPropChanging(sender, e);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region LoadLayoutCommand
        RelayCommand _loadLayoutCommand = null;
        public ICommand LoadLayoutCommand
        {
            get
            {
                if (_loadLayoutCommand == null)
                {
                    _loadLayoutCommand = new RelayCommand((p) => OnLoadLayout(p), (p) => CanLoadLayout(p));
                }

                return _loadLayoutCommand;
            }
        }

        private bool CanLoadLayout(object parameter)
        {
            return File.Exists(@".\AvalonDock.Layout.config");
        }

        private void OnLoadLayout(object parameter)
        {
            var layoutSerializer = new XmlLayoutSerializer(dockingManager);
            //Here I've implemented the LayoutSerializationCallback just to show
            // a way to feed layout desarialization with content loaded at runtime
            //Actually I could in this case let AvalonDock to attach the contents
            //from current layout using the content ids
            //LayoutSerializationCallback should anyway be handled to attach contents
            //not currently loaded
            layoutSerializer.LayoutSerializationCallback += (s, e) =>
            {
                //if (e.Model.ContentId == FileStatsViewModel.ToolContentId)
                //    e.Content = Workspace.This.FileStats;
                //else if (!string.IsNullOrWhiteSpace(e.Model.ContentId) &&
                //    File.Exists(e.Model.ContentId))
                //    e.Content = Workspace.This.Open(e.Model.ContentId);
            };
            layoutSerializer.Deserialize(@".\AvalonDock.Layout.config");
        }

        #endregion

        #region SaveLayoutCommand
        RelayCommand _saveLayoutCommand = null;
        public ICommand SaveLayoutCommand
        {
            get
            {
                if (_saveLayoutCommand == null)
                {
                    _saveLayoutCommand = new RelayCommand((p) => OnSaveLayout(p), (p) => CanSaveLayout(p));
                }

                return _saveLayoutCommand;
            }
        }

        private bool CanSaveLayout(object parameter)
        {
            return true;
        }

        private void OnSaveLayout(object parameter)
        {
            var layoutSerializer = new XmlLayoutSerializer(dockingManager);
            layoutSerializer.Serialize(@".\AvalonDock.Layout.config");
        }

        #endregion 

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            /*if (this.LoadLayoutCommand.CanExecute(null))
            {
                this.LoadLayoutCommand.Execute(null);
            }*/
        }

        private void dockingManager_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private bool closeStoryBoardCompleted = false;

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!closeStoryBoardCompleted)
            {
                ((Storyboard)FindResource("ExitAnimation")).Begin();
                this.isFading = true;
                e.Cancel = true;
            }
        }

        private void closeStoryBoard_Completed(object sender, EventArgs e)
        {
            closeStoryBoardCompleted = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NewProjectDialog dlg = new NewProjectDialog();
            dlg.Owner = this;
            dlg.ShowDialog();
        }
    }
}
