using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace WpfTreeView
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Contructor
        public MainWindow()
        {
            InitializeComponent();
        }
        #endregion

        #region On Loaded
        /// <summary>
        /// 어플리케이션이 처음 실행될때
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 로컬 컴퓨터에 존재하는 모든 드라이브를 가져옴 
            foreach(var drive in Directory.GetLogicalDrives())
            {
                // 트리뷰 아이템을 생성
                var item = new TreeViewItem()
                {
                    // 해더명 설정
                    Header = drive,
                    // 전체 경로 설정
                    Tag = drive
                };

                item.Items.Add(null);

                // 트리뷰 아이템이 확장되는 이벤트 발생시 처리하는 함수등록
                item.Expanded += Folder_Expanded;

                // 트리뷰에 아이템을 추가
                FolderView.Items.Add(item);
            }
        }
        #endregion

        #region Folder Expended
        private void Folder_Expanded(object sender, RoutedEventArgs e)
        {
            #region Initial Check
            var item = (TreeViewItem)sender;

            // 더미데이터만 있는 경우 리턴
            if (item.Items.Count != 1 || item.Items[0] != null)
                return;

            // 더미 데이터를 지운다.
            item.Items.Clear();
            #endregion

            #region Get Directories
            // 폴더 전체 경로를 가져온다.
            var fullPath = (string)item.Tag;

            // 디렉토리를 담기 위한 빈 리스트 생성
            var directories = new List<string>();

            // 폴더에서 하위 디렉토리를 가져온다.
            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            }
            catch { }

            // For each directory...
            directories.ForEach(directoryPath =>
            {
                // 하위디렉토리 생성
                var subItem = new TreeViewItem()
                {
                    // 헤더명 설정
                    Header = GetFileFolderName(directoryPath),
                    // 전체경로 설정
                    Tag = directoryPath
                };

                // 하위 더미 데이터 추가
                subItem.Items.Add(null);

                // 재귀이벤트 연결
                subItem.Expanded += Folder_Expanded;

                item.Items.Add(subItem);
            });
            #endregion

            #region Get Files
            var files = new List<string>();

            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                    files.AddRange(fs);
            }
            catch { }

            // For each directory...
            files.ForEach(filePath =>
            {
                // 하위디렉토리 생성
                var subItem = new TreeViewItem()
                {
                    // 헤더명 설정
                    Header = GetFileFolderName(filePath),
                    // 전체경로 설정
                    Tag = filePath
                };

                item.Items.Add(subItem);
            });
            #endregion
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Find the file or folder name from a full path
        /// </summary>
        /// <param name="path">The full path</param>
        /// <returns></returns>
        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;

            var normalizePath = path.Replace('/', '\\');

            var lastIndex = normalizePath.LastIndexOf('\\');

            if (lastIndex <= 0)
                return path;

            return path.Substring(lastIndex + 1);
        }
        #endregion

    }
}
