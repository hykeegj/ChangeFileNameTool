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

namespace ChangeFileNameTool
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 오름차순 내림차순 정렬 유무 판단 bool 변수
        bool isSortedVideoFileNameFlag = false;
        bool isSortedVideoFileExtensionFlag = false;
        bool isSortedSubtitleFileNameFlag = false;
        bool isSortedSubtitleFileExtensionFlag = false;

        // 비디오, 자막 파일 클래스
        public class File
        {
            private string path { get; set; }        //경로(디렉토리 경로)
            private string name { get; set; }        //파일 이름
            private string extension { get; set; }   //확장자 이름

            public string Path
            {
                get { return this.path; }
                set { this.path = value; }
            }

            public string Name
            {
                get { return this.name; }
                set { this.name = value; }
            }

            public string Extension
            {
                get { return this.extension; }
                set { this.extension = value; }
            }
        }
    }

    // 비디오 파일 리스트뷰 동작
    public partial class MainWindow : Window
    {
        List<File> videoFilesList = new List<File>();   //비디오 파일 리스트

        // 리스트뷰에 파일을 Drag&Drop 할 때, 드랍 가능 여부를 판단하기 위해 마우스 포인터 모양을 바꾸는 이벤트
        private void videoFile_listView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        // 리스트뷰에 파일을 Drag&Drop 하여 추가하면, 비디오 파일 리스트에 디렉토리 경로, 파일 이름, 파일 확장자를 저장 후 리스트뷰에 디렉토리 경로, 파일 이름, 파일 확장자 표시하는 이벤트
        private void videoFile_listView_Drop(object sender, DragEventArgs e)
        {
            string[] videoFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string videoFilePath in videoFilePaths)
            {
                videoFilesList.Add(new File() { Path = System.IO.Path.GetDirectoryName(videoFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(videoFilePath), Extension = System.IO.Path.GetExtension(videoFilePath) });
            }
            videoFile_listView.ItemsSource = videoFilesList;
            videoFile_listView.Items.Refresh();
        }

        // 리스트뷰의 추가 버튼을 클릭하여 파일 탐색기를 실행 후 파일을 추가하는 버튼 클릭 이벤트
        private void videoAddButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "All files (*.*)|*.*";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string videoFilePath in dlg.FileNames)
                {
                    videoFilesList.Add(new File() { Path = System.IO.Path.GetDirectoryName(videoFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(videoFilePath), Extension = System.IO.Path.GetExtension(videoFilePath) });
                }
                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
        }

        // 리스트뷰의 삭제 버튼을 클릭하여 체크박스가 체크된 항목들을 삭제하는 버튼 클릭 이벤트
        private void videoDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0)
            {
                foreach (File item in videoFile_listView.SelectedItems)
                {
                    videoFilesList.Remove(item);
                }
                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortByVideoFileName_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0)
            {
                List<File> sortedList = new List<File>();

                if (isSortedVideoFileNameFlag == false)
                {
                    sortedList = videoFilesList.OrderBy(x => x.Name).ToList();
                    videoFilesList = sortedList;
                    isSortedVideoFileNameFlag = true;
                }
                else if (isSortedVideoFileNameFlag == true)
                {
                    sortedList = videoFilesList.OrderByDescending(x => x.Name).ToList();
                    videoFilesList = sortedList;
                    isSortedVideoFileNameFlag = false;
                }
                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortByVideoFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0)
            {
                List<File> sortedList = new List<File>();

                if (isSortedVideoFileExtensionFlag == false)
                {
                    sortedList = videoFilesList.OrderBy(x => x.Extension).ToList();
                    videoFilesList = sortedList;
                    isSortedVideoFileExtensionFlag = true;
                }
                else if (isSortedVideoFileExtensionFlag == true)
                {
                    sortedList = videoFilesList.OrderByDescending(x => x.Extension).ToList();
                    videoFilesList = sortedList;
                    isSortedVideoFileExtensionFlag = false;
                }
                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // 자막 파일 리스트뷰 동작
    public partial class MainWindow : Window
    {
        List<File> subtitleFilesList = new List<File>();    // 자막파일 리스트

        // 리스트뷰에 파일을 Drag&Drop 할 때, 드랍 가능 여부를 판단하기 위해 마우스 포인터 모양을 바꾸는 이벤트
        private void subtitleFile_listView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        // 리스트뷰에 파일을 Drag&Drop 하여 추가하면, 비디오 파일 리스트에 디렉토리 경로, 파일 이름, 파일 확장자를 저장 후 리스트뷰에 디렉토리 경로, 파일 이름, 파일 확장자 표시하는 이벤트
        private void subtitleFile_listView_Drop(object sender, DragEventArgs e)
        {
            string[] subtitleFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string subtitleFilePath in subtitleFilePaths)
            {
                subtitleFilesList.Add(new File() { Path = System.IO.Path.GetDirectoryName(subtitleFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(subtitleFilePath), Extension = System.IO.Path.GetExtension(subtitleFilePath) });
            }
            subtitleFile_listView.ItemsSource = subtitleFilesList;
            subtitleFile_listView.Items.Refresh();
        }

        // 리스트뷰의 추가 버튼을 클릭하여 파일 탐색기를 실행 후 파일을 추가하는 버튼 클릭 이벤트
        private void subtitleAddButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "All files (*.*)|*.*";
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string subtitleFilePath in dlg.FileNames)
                {
                    subtitleFilesList.Add(new File() { Path = System.IO.Path.GetDirectoryName(subtitleFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(subtitleFilePath), Extension = System.IO.Path.GetExtension(subtitleFilePath) });
                }
                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
        }

        // 리스트뷰의 삭제 버튼을 클릭하여 체크박스가 체크된 항목들을 삭제하는 버튼 클릭 이벤트
        private void subtitleDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count != 0)
            {
                foreach (File item in subtitleFile_listView.SelectedItems)
                {
                    subtitleFilesList.Remove(item);
                }
                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count == 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortBySubtitleFileName_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0)
            {
                List<File> sortedList = new List<File>();

                if (isSortedSubtitleFileNameFlag == false)
                {
                    sortedList = subtitleFilesList.OrderBy(x => x.Name).ToList();
                    subtitleFilesList = sortedList;
                    isSortedSubtitleFileNameFlag = true;
                }
                else if (isSortedSubtitleFileNameFlag == true)
                {
                    sortedList = subtitleFilesList.OrderByDescending(x => x.Name).ToList();
                    subtitleFilesList = sortedList;
                    isSortedSubtitleFileNameFlag = false;
                }
                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortBySubtitleFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0)
            {
                List<File> sortedList = new List<File>();

                if (isSortedSubtitleFileExtensionFlag == false)
                {
                    sortedList = subtitleFilesList.OrderBy(x => x.Extension).ToList();
                    subtitleFilesList = sortedList;
                    isSortedSubtitleFileExtensionFlag = true;
                }
                else if (isSortedSubtitleFileExtensionFlag == true)
                {
                    sortedList = subtitleFilesList.OrderByDescending(x => x.Extension).ToList();
                    subtitleFilesList = sortedList;
                    isSortedSubtitleFileExtensionFlag = false;
                }
                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // 바꾸기 버튼 동작
    public partial class MainWindow : Window
    {
        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0 && subtitleFilesList.Count > 0 && videoFilesList.Count == subtitleFilesList.Count)
            {
                for (int i = 0; i < subtitleFilesList.Count; i++)
                {
                    System.IO.File.Move(subtitleFilesList[i].Path + "\\" + subtitleFilesList[i].Name + subtitleFilesList[i].Extension, videoFilesList[i].Path + "\\" + videoFilesList[i].Name + subtitleFilesList[i].Extension);
                }
                MessageBox.Show("자막 파일 이름이 동영상 파일의 이름에 맞게 변경되었습니다.\n변경된 파일을 확인해주세요.\n프로그램을 종료합니다.", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.MainWindow.Close();
            }
            else if (videoFilesList.Count <= 0 && subtitleFilesList.Count > 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.\n비디오 파일을 확인해 주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesList.Count > 0 && subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.\n자막 파일을 확인해 주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesList.Count <= 0 && subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("파일이 존재하지 않습니다.\n파일을 다시 확인해 주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesList.Count > 0 && subtitleFilesList.Count > 0 && videoFilesList.Count != subtitleFilesList.Count)
            {
                MessageBox.Show("동영상 파일과 자막 파일의 개수가 일치하지 않습니다.\n파일들을 다시 확인해 주세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
