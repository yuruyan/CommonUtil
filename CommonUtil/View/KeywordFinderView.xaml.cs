﻿using CommonUtil.Core;
using NLog;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace CommonUtil.View {
    public partial class KeywordFinderView : Page {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static readonly DependencyProperty SearchDirectoryProperty = DependencyProperty.Register("SearchDirectory", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
        public static readonly DependencyProperty ExcludeDirectoryProperty = DependencyProperty.Register("ExcludeDirectory", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
        public static readonly DependencyProperty ExcludeFileProperty = DependencyProperty.Register("ExcludeFile", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
        public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register("SearchText", typeof(string), typeof(KeywordFinderView), new PropertyMetadata(""));
        public static readonly DependencyProperty KeywordResultsProperty = DependencyProperty.Register("KeywordResults", typeof(ObservableCollection<KeywordResult>), typeof(KeywordFinderView), new PropertyMetadata());
        public static readonly DependencyProperty IsExcludeDirectorySelectedProperty = DependencyProperty.Register("IsExcludeDirectorySelected", typeof(bool), typeof(KeywordFinderView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsExcludeFileSelectedProperty = DependencyProperty.Register("IsExcludeFileSelected", typeof(bool), typeof(KeywordFinderView), new PropertyMetadata(false));
        public static readonly DependencyProperty IsSearchingFinishedProperty = DependencyProperty.Register("IsSearchingFinished", typeof(bool), typeof(KeywordFinderView), new PropertyMetadata(true));

        /// <summary>
        /// 搜索目录
        /// </summary>
        public string SearchDirectory {
            get { return (string)GetValue(SearchDirectoryProperty); }
            set { SetValue(SearchDirectoryProperty, value); }
        }
        /// <summary>
        /// 排除目录
        /// </summary>
        public string ExcludeDirectory {
            get { return (string)GetValue(ExcludeDirectoryProperty); }
            set { SetValue(ExcludeDirectoryProperty, value); }
        }
        /// <summary>
        /// 排除文件
        /// </summary>
        public string ExcludeFile {
            get { return (string)GetValue(ExcludeFileProperty); }
            set { SetValue(ExcludeFileProperty, value); }
        }
        /// <summary>
        /// 关键字
        /// </summary>
        public string SearchText {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }
        /// <summary>
        /// 排除目录是否勾选
        /// </summary>
        public bool IsExcludeDirectorySelected {
            get { return (bool)GetValue(IsExcludeDirectorySelectedProperty); }
            set { SetValue(IsExcludeDirectorySelectedProperty, value); }
        }
        /// <summary>
        /// 排除文件是否勾选
        /// </summary>
        public bool IsExcludeFileSelected {
            get { return (bool)GetValue(IsExcludeFileSelectedProperty); }
            set { SetValue(IsExcludeFileSelectedProperty, value); }
        }
        /// <summary>
        /// 查询结果
        /// </summary>
        public ObservableCollection<KeywordResult> KeywordResults {
            get { return (ObservableCollection<KeywordResult>)GetValue(KeywordResultsProperty); }
            set { SetValue(KeywordResultsProperty, value); }
        }
        /// <summary>
        /// 搜索是否结束
        /// </summary>
        public bool IsSearchingFinished {
            get { return (bool)GetValue(IsSearchingFinishedProperty); }
            set { SetValue(IsSearchingFinishedProperty, value); }
        }
        private KeywordFinder KeywordFinder;
        /// <summary>
        /// 上次查询目录
        /// </summary>
        private string LastSearchDirectory = string.Empty;

        public KeywordFinderView() {
            KeywordResults = new();
            KeywordResults.CollectionChanged += KeywordResultsCollectionChanged;
            InitializeComponent();
        }

        private void KeywordResultsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action == NotifyCollectionChangedAction.Add) {
                // 获取相对路径
                if (e.NewItems != null) {
                    foreach (var item in e.NewItems) {
                        if (item is KeywordResult result) {
                            result.File = result.File.Replace('/', '\\')[(SearchDirectory.Replace('/', '\\').Length + 1)..];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 查询点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FindKeywordClick(object sender, RoutedEventArgs e) {
            if (string.IsNullOrEmpty(SearchDirectory.Trim())) {
                Widget.MessageBox.Info("请选择查询目录！");
                return;
            }
            //if (string.IsNullOrEmpty(SearchText.Trim())) {
            //    Widget.MessageBox.Info("搜索文本不能为空！");
            //    return;
            //}
            if (!IsSearchingFinished) {
                Widget.MessageBox.Info("正在搜索...");
                return;
            }
            ResultNumber.Visibility = Visibility.Visible;
            IsSearchingFinished = false;
            var excludeDirs = new List<string>();
            var excludeFiles = new List<string>();
            if (IsExcludeDirectorySelected) {
                excludeDirs.AddRange(ExcludeDirectory.Split('\n').Where(s => s.Trim() != "").Select(s => s.Trim()));
            }
            if (IsExcludeFileSelected) {
                excludeFiles.AddRange(ExcludeFile.Split('\n').Where(s => s.Trim() != "").Select(s => s.Trim()));
            }
            // 检查搜索目录是否改变
            if (LastSearchDirectory != SearchDirectory) {
                LastSearchDirectory = SearchDirectory;
                var searchDirectory = SearchDirectory;
                ThreadPool.QueueUserWorkItem(o => {
                    try {
                        var keywordFinder = new KeywordFinder(searchDirectory, excludeDirs, excludeFiles);
                        Dispatcher.Invoke(() => KeywordFinder = keywordFinder);
                        FindKeyword(excludeDirs, excludeFiles);
                    } catch (Exception error) {
                        Dispatcher.Invoke(() => Widget.MessageBox.Error(error.Message));
                        Logger.Error(error);
                    }
                });
                return;
            }
            FindKeyword(excludeDirs, excludeFiles);
        }

        private void FindKeyword(List<string> excludeDirs, List<string> excludeFiles) {
            var searchText = "";
            ObservableCollection<KeywordResult> keywordResults = null;
            Dispatcher.Invoke(() => {
                searchText = SearchText;
                keywordResults = KeywordResults;
            });
            ThreadPool.QueueUserWorkItem(o => {
                try {
                    KeywordFinder.FindKeyword(searchText, excludeDirs, excludeFiles, keywordResults, Dispatcher);
                } catch (Exception error) {
                    Dispatcher.Invoke(() => Widget.MessageBox.Error(error.Message));
                    Logger.Error(error);
                } finally {
                    Dispatcher.Invoke(() => IsSearchingFinished = true);
                }
            });
        }

        /// <summary>
        /// 选择搜索目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSearchDirectoryClick(object sender, RoutedEventArgs e) {
            var dialog = new VistaFolderBrowserDialog();
            dialog.Description = "选择搜索目录";
            dialog.UseDescriptionForTitle = true;
            if (dialog.ShowDialog(Application.Current.MainWindow) == true) {
                SearchDirectory = dialog.SelectedPath;
            }
        }
    }
}