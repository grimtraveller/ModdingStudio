﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModdingStudio;
using System.Windows;

namespace ModdingStudio.Documents
{
    public class JavaSourceViewModel : DependencyObject , IFileVM
    {
        private JavaSource _view;
        private string _filePath;
        private string _fileName;
        private bool _existsOnFileSys;
        private bool _isUnsaved;

        public JavaSourceViewModel(JavaSource view, string path)
        {
            _view = view;
            this.ExistsOnFileSystem = true;
            this.FilePath = path;
        }

        public JavaSourceViewModel(JavaSource view)
        {
            _view = view;
            this.ExistsOnFileSystem = false;
            this.IsUnsaved = true;
        }

        public IDocumentView GetView()
        {
            return _view;
        }

        public string FilePath 
        {
            get { return ExistsOnFileSystem ? _filePath : _fileName; }
            set { _filePath = value; }
        }
        public string FileName 
        {
            get { return _fileName; }
            set { _fileName = value;
                  this._view.Title = value;
            }
        }

        public void SaveFile()
        {
            if (_existsOnFileSys)
            {
                JavaSourceProvider.saveAllLinesToFile(FilePath, this._view.textBox.Text);
                IsUnsaved = false;
            }
            else
            {

            }
        }

        public bool ExistsOnFileSystem
        {
            get { return _existsOnFileSys; }
            set { _existsOnFileSys = value; }
        }

        public bool IsUnsaved
        {
            get { return _isUnsaved; }
            set
            {
                _isUnsaved = value;
                this._view.Title = value ? this.FileName + "*" : this.FileName;
            }
        }

        public string FileExt
        {
            get { return ".java"; }
            set { ; }
        }

        public void OnLoaded()
        {
            if (ExistsOnFileSystem)
            {
                this._view.textBox.Text = JavaSourceProvider.readAllLinesFromFile(this.FilePath);
                this.IsUnsaved = false;
            }
            else
            {
                this.IsUnsaved = true;
            }
        }

        public void OnClosing()
        {
            
        }

        public Application.IAppVM GetSuperiorVM()
        {
            return _view._supVM;
        }

        public void OnTextChanged(object sender, EventArgs e)
        {
            this.IsUnsaved = true;
        }

        public string TitleName
        {
            get
            {
                return FileName;
            }
            set
            {
                this.FileName = value;
            }
        }

        public override string ToString()
        {
            return this.FileName;
        }
    }
}
