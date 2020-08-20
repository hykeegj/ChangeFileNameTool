using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

namespace SubtitleRun
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
        private class File
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
        LinkedList<File> videoFilesList = new LinkedList<File>();   //비디오 파일 리스트

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
                videoFilesList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(videoFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(videoFilePath), Extension = System.IO.Path.GetExtension(videoFilePath) });
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
            //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // 윈도우 파일 탐색기의 기본 위치를 내문서로 강제 변경하기

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string videoFilePath in dlg.FileNames)
                {
                    videoFilesList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(videoFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(videoFilePath), Extension = System.IO.Path.GetExtension(videoFilePath) });
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
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortByVideoFileName_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0)
            {
                List<File> sortedList = videoFilesList.ToList();

                if (isSortedVideoFileNameFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Name).ToList();

                    videoFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesList.AddLast(item);
                    }

                    isSortedVideoFileNameFlag = true;
                }
                else if (isSortedVideoFileNameFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Name).ToList();

                    videoFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesList.AddLast(item);
                    }

                    isSortedVideoFileNameFlag = false;
                }

                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 확장자순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortByVideoFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0)
            {
                List<File> sortedList = videoFilesList.ToList();

                if (isSortedVideoFileExtensionFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Extension).ToList();

                    videoFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesList.AddLast(item);
                    }

                    isSortedVideoFileExtensionFlag = true;
                }
                else if (isSortedVideoFileExtensionFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Extension).ToList();

                    videoFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesList.AddLast(item);
                    }

                    isSortedVideoFileExtensionFlag = false;
                }
                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 위로 버튼을 누르면 리스트 뷰의 맨 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                videoFilesList.Remove((File)videoFile_listView.SelectedItem);
                videoFilesList.AddFirst((File)videoFile_listView.SelectedItem);

                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 위로 버튼을 누르면 리스트 뷰의 한단계 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> previousNode = videoFilesList.Find((File)videoFile_listView.SelectedItem).Previous;
                LinkedListNode<File> currentNode = videoFilesList.Find((File)videoFile_listView.SelectedItem);

                if (previousNode != null)
                {
                    videoFilesList.Remove(currentNode);
                    videoFilesList.AddBefore(previousNode, currentNode);
                }

                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if(videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 아래로 버튼을 누르면 리스트 뷰의 한단계 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> nextNode = videoFilesList.Find((File)videoFile_listView.SelectedItem).Next;
                LinkedListNode<File> currentNode = videoFilesList.Find((File)videoFile_listView.SelectedItem);

                if(nextNode != null)
                {
                    videoFilesList.Remove(currentNode);
                    videoFilesList.AddAfter(nextNode, currentNode);
                }

                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 아래로 버튼을 누르면 리스트 뷰의 맨 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                videoFilesList.Remove((File)videoFile_listView.SelectedItem);
                videoFilesList.AddLast((File)videoFile_listView.SelectedItem);

                videoFile_listView.ItemsSource = videoFilesList;
                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // 자막 파일 리스트뷰 동작
    public partial class MainWindow : Window
    {
        LinkedList<File> subtitleFilesList = new LinkedList<File>();    // 자막파일 리스트

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
                subtitleFilesList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(subtitleFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(subtitleFilePath), Extension = System.IO.Path.GetExtension(subtitleFilePath) });
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
            //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // 윈도우 파일 탐색기의 기본 위치를 내문서로 강제 변경하기

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string subtitleFilePath in dlg.FileNames)
                {
                    subtitleFilesList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(subtitleFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(subtitleFilePath), Extension = System.IO.Path.GetExtension(subtitleFilePath) });
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
            else if (subtitleFilesList.Count == 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortBySubtitleFileName_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0)
            {
                List<File> sortedList = subtitleFilesList.ToList();

                if (isSortedSubtitleFileNameFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Name).ToList();

                    subtitleFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesList.AddLast(item);
                    }

                    isSortedSubtitleFileNameFlag = true;
                }
                else if (isSortedSubtitleFileNameFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Name).ToList();

                    subtitleFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesList.AddLast(item);
                    }

                    isSortedSubtitleFileNameFlag = false;
                }

                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortBySubtitleFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0)
            {
                List<File> sortedList = subtitleFilesList.ToList();

                if (isSortedSubtitleFileExtensionFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Extension).ToList();

                    subtitleFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesList.AddLast(item);
                    }

                    isSortedSubtitleFileExtensionFlag = true;
                }
                else if (isSortedSubtitleFileExtensionFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Extension).ToList();

                    subtitleFilesList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesList.AddLast(item);
                    }

                    isSortedSubtitleFileExtensionFlag = false;
                }
                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 위로 버튼을 누르면 리스트 뷰의 맨 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                subtitleFilesList.Remove((File)subtitleFile_listView.SelectedItem);
                subtitleFilesList.AddFirst((File)subtitleFile_listView.SelectedItem);

                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 위로 버튼을 누르면 리스트 뷰의 한단계 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> previousNode = subtitleFilesList.Find((File)subtitleFile_listView.SelectedItem).Previous;
                LinkedListNode<File> currentNode = subtitleFilesList.Find((File)subtitleFile_listView.SelectedItem);

                subtitleFilesList.Remove(currentNode);
                subtitleFilesList.AddBefore(previousNode, currentNode);

                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 아래로 버튼을 누르면 리스트 뷰의 한단계 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> nextNode = subtitleFilesList.Find((File)subtitleFile_listView.SelectedItem).Next;
                LinkedListNode<File> currentNode = subtitleFilesList.Find((File)subtitleFile_listView.SelectedItem);

                subtitleFilesList.Remove(currentNode);
                subtitleFilesList.AddAfter(nextNode, currentNode);

                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 아래로 버튼을 누르면 리스트 뷰의 맨 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                subtitleFilesList.Remove((File)subtitleFile_listView.SelectedItem);
                subtitleFilesList.AddLast((File)subtitleFile_listView.SelectedItem);

                subtitleFile_listView.ItemsSource = subtitleFilesList;
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // 자막 파일명 바꾸기 버튼 동작
    public partial class MainWindow : Window
    {
        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesList.Count > 0 && subtitleFilesList.Count > 0 && videoFilesList.Count == subtitleFilesList.Count)
            {
                if (autoMoveSubtitleFilesCheckBox.IsChecked == true)
                {
                    for (int i = 0; i < subtitleFilesList.Count; i++)
                    {
                        System.IO.File.Move(subtitleFilesList.ElementAt(i).Path + "\\" + subtitleFilesList.ElementAt(i).Name + subtitleFilesList.ElementAt(i).Extension, videoFilesList.ElementAt(i).Path + "\\" + videoFilesList.ElementAt(i).Name + subtitleFilesList.ElementAt(i).Extension);
                    }
                }
                else if (autoMoveSubtitleFilesCheckBox.IsChecked == false)
                {
                    for (int i = 0; i < subtitleFilesList.Count; i++)
                    {
                        System.IO.File.Move(subtitleFilesList.ElementAt(i).Path + "\\" + subtitleFilesList.ElementAt(i).Name + subtitleFilesList.ElementAt(i).Extension, subtitleFilesList.ElementAt(i).Path + "\\" + videoFilesList.ElementAt(i).Name + subtitleFilesList.ElementAt(i).Extension);
                    }
                }

                MessageBox.Show("자막 파일 이름이 동영상 파일의 이름에 맞게 변경되었습니다.\n변경된 파일을 확인해주세요.\n프로그램을 종료합니다.", Properties.Resources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.MainWindow.Close();
            }
            else if (videoFilesList.Count <= 0 && subtitleFilesList.Count > 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.\n비디오 파일을 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesList.Count > 0 && subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.\n자막 파일을 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesList.Count <= 0 && subtitleFilesList.Count <= 0)
            {
                MessageBox.Show("파일이 존재하지 않습니다.\n파일을 다시 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesList.Count > 0 && subtitleFilesList.Count > 0 && videoFilesList.Count != subtitleFilesList.Count)
            {
                MessageBox.Show("동영상 파일과 자막 파일의 개수가 일치하지 않습니다.\n파일들을 다시 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
