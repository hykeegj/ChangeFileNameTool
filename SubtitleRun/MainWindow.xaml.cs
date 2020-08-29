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
using Microsoft.Win32;

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
                get { return path; }
                set { path = value; }
            }

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public string Extension
            {
                get { return extension; }
                set { extension = value; }
            }
        }
    }

    // 비디오 파일 리스트뷰 동작
    public partial class MainWindow : Window
    {
        LinkedList<File> videoFilesLinkedList = new LinkedList<File>();   //비디오 파일 이중연결리스트

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
                // 파일이 아닌 폴더를 Drag&Drop 했을 때, 확장자가 존재하는지를 검사하여 확장자가 없는 폴더를 Drag&Drop하면 무시하는 조건문
                if (String.IsNullOrEmpty(System.IO.Path.GetExtension(videoFilePath)))
                {
                    continue;
                }
                else
                {
                    videoFilesLinkedList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(videoFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(videoFilePath), Extension = System.IO.Path.GetExtension(videoFilePath) });
                }
            }

            videoFile_listView.ItemsSource = videoFilesLinkedList;
            videoFile_listView.Items.Refresh();
        }

        // 리스트뷰의 추가 버튼을 클릭하여 파일 탐색기를 실행 후 파일을 추가하는 버튼 클릭 이벤트
        private void videoAddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = "Video Files|*.asf; *.avi; *.flv; *.bik; *.bik2; *.flv; *.mkv; *.mov; *.mp4; *.m4p; *.m4b; *.m4r; *.m4v; *.mpeg; *.mpg; *.ps; *.ts; *.tp; *.mts; *.m2ts; *.tod; 3gp; *.skm; *.k3g; *.lmp4 *.ogg; *.rm; *.wmv; *.webm|All files (*.*)|*.*";
            // fileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // 윈도우 파일 탐색기의 기본 위치를 내문서로 강제 변경하기

            bool? result = fileDialog.ShowDialog();

            if (result == true)
            {
                foreach (string videoFilePath in fileDialog.FileNames)
                {
                    videoFilesLinkedList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(videoFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(videoFilePath), Extension = System.IO.Path.GetExtension(videoFilePath) });
                }

                videoFile_listView.ItemsSource = videoFilesLinkedList;
                videoFile_listView.Items.Refresh();
            }
        }

        // 리스트뷰의 삭제 버튼을 클릭하여 체크박스가 체크된 항목들을 삭제하는 버튼 클릭 이벤트
        private void videoDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0)
            {
                foreach (File item in videoFile_listView.SelectedItems)
                {
                    videoFilesLinkedList.Remove(item);
                }

                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortByVideoFileName_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0)
            {
                List<File> sortedList = videoFilesLinkedList.ToList();

                if (isSortedVideoFileNameFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Name).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileNameFlag = true;
                }
                else if (isSortedVideoFileNameFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Name).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileNameFlag = false;
                }

                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 확장자순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortByVideoFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0)
            {
                List<File> sortedList = videoFilesLinkedList.ToList();

                if (isSortedVideoFileExtensionFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Extension).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileExtensionFlag = true;
                }
                else if (isSortedVideoFileExtensionFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Extension).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileExtensionFlag = false;
                }

                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 위로 버튼을 누르면 리스트 뷰의 맨 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                videoFilesLinkedList.Remove((File)videoFile_listView.SelectedItem);
                videoFilesLinkedList.AddFirst((File)videoFile_listView.SelectedItem);

                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 위로 버튼을 누르면 리스트 뷰의 한단계 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> previousNode = videoFilesLinkedList.Find((File)videoFile_listView.SelectedItem).Previous;
                LinkedListNode<File> currentNode = videoFilesLinkedList.Find((File)videoFile_listView.SelectedItem);

                if (previousNode != null)
                {
                    videoFilesLinkedList.Remove(currentNode);
                    videoFilesLinkedList.AddBefore(previousNode, currentNode);
                }

                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 아래로 버튼을 누르면 리스트 뷰의 한단계 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> nextNode = videoFilesLinkedList.Find((File)videoFile_listView.SelectedItem).Next;
                LinkedListNode<File> currentNode = videoFilesLinkedList.Find((File)videoFile_listView.SelectedItem);

                if (nextNode != null)
                {
                    videoFilesLinkedList.Remove(currentNode);
                    videoFilesLinkedList.AddAfter(nextNode, currentNode);
                }

                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 아래로 버튼을 누르면 리스트 뷰의 맨 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveVideoIndexBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count == 1)
            {
                videoFilesLinkedList.Remove((File)videoFile_listView.SelectedItem);
                videoFilesLinkedList.AddLast((File)videoFile_listView.SelectedItem);

                videoFile_listView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && videoFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // 자막 파일 리스트뷰 동작
    public partial class MainWindow : Window
    {
        LinkedList<File> subtitleFilesLinkedList = new LinkedList<File>();    // 자막파일 이중연결리스트

        // 리스트뷰에 파일을 Drag&Drop 할 때, 드랍 가능 여부를 판단하기 위해 마우스 포인터 모양을 바꾸는 이벤트
        private void subtitleFile_listView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        // 리스트뷰에 파일을 Drag&Drop 하여 추가하면, 자막 파일 리스트에 디렉토리 경로, 파일 이름, 파일 확장자를 저장 후 리스트뷰에 디렉토리 경로, 파일 이름, 파일 확장자 표시하는 이벤트
        private void subtitleFile_listView_Drop(object sender, DragEventArgs e)
        {
            string[] subtitleFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string subtitleFilePath in subtitleFilePaths)
            {
                // 파일이 아닌 폴더를 Drag&Drop 했을 때, 확장자가 존재하는지를 검사하여 확장자가 없는 폴더를 Drag&Drop하면 무시하는 조건문
                if (String.IsNullOrEmpty(System.IO.Path.GetExtension(subtitleFilePath)))
                {
                    continue;
                }
                else
                {
                    subtitleFilesLinkedList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(subtitleFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(subtitleFilePath), Extension = System.IO.Path.GetExtension(subtitleFilePath) });
                }
            }

            subtitleFile_listView.ItemsSource = subtitleFilesLinkedList;
            subtitleFile_listView.Items.Refresh();
        }

        // 리스트뷰의 추가 버튼을 클릭하여 파일 탐색기를 실행 후 파일을 추가하는 버튼 클릭 이벤트
        private void subtitleAddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Subtitle Files|*.smi; *.srt; *.ssa; *.ass; *.lrc|All files (*.*)|*.*";
            //dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); // 윈도우 파일 탐색기의 기본 위치를 내문서로 강제 변경하기

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                foreach (string subtitleFilePath in dlg.FileNames)
                {
                    subtitleFilesLinkedList.AddLast(new File() { Path = System.IO.Path.GetDirectoryName(subtitleFilePath), Name = System.IO.Path.GetFileNameWithoutExtension(subtitleFilePath), Extension = System.IO.Path.GetExtension(subtitleFilePath) });
                }

                subtitleFile_listView.ItemsSource = subtitleFilesLinkedList;
                subtitleFile_listView.Items.Refresh();
            }
        }

        // 리스트뷰의 삭제 버튼을 클릭하여 체크박스가 체크된 항목들을 삭제하는 버튼 클릭 이벤트
        private void subtitleDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count != 0)
            {
                foreach (File item in subtitleFile_listView.SelectedItems)
                {
                    subtitleFilesLinkedList.Remove(item);
                }

                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count == 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortBySubtitleFileName_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0)
            {
                List<File> sortedList = subtitleFilesLinkedList.ToList();

                if (isSortedSubtitleFileNameFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Name).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileNameFlag = true;
                }
                else if (isSortedSubtitleFileNameFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Name).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileNameFlag = false;
                }

                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void sortBySubtitleFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0)
            {
                List<File> sortedList = subtitleFilesLinkedList.ToList();

                if (isSortedSubtitleFileExtensionFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Extension).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileExtensionFlag = true;
                }
                else if (isSortedSubtitleFileExtensionFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Extension).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (File item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileExtensionFlag = false;
                }

                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 위로 버튼을 누르면 리스트 뷰의 맨 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                subtitleFilesLinkedList.Remove((File)subtitleFile_listView.SelectedItem);
                subtitleFilesLinkedList.AddFirst((File)subtitleFile_listView.SelectedItem);

                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 위로 버튼을 누르면 리스트 뷰의 한단계 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> previousNode = subtitleFilesLinkedList.Find((File)subtitleFile_listView.SelectedItem).Previous;
                LinkedListNode<File> currentNode = subtitleFilesLinkedList.Find((File)subtitleFile_listView.SelectedItem);

                if (previousNode != null)
                {
                    subtitleFilesLinkedList.Remove(currentNode);
                    subtitleFilesLinkedList.AddBefore(previousNode, currentNode);
                }

                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 아래로 버튼을 누르면 리스트 뷰의 한단계 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                LinkedListNode<File> nextNode = subtitleFilesLinkedList.Find((File)subtitleFile_listView.SelectedItem).Next;
                LinkedListNode<File> currentNode = subtitleFilesLinkedList.Find((File)subtitleFile_listView.SelectedItem);

                if (nextNode != null)
                {
                    subtitleFilesLinkedList.Remove(currentNode);
                    subtitleFilesLinkedList.AddAfter(nextNode, currentNode);
                }
                
                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // 리스트 뷰의 항목들을 체크한 뒤, 맨 아래로 버튼을 누르면 리스트 뷰의 맨 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void moveSubtitleIndexBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count == 1)
            {
                subtitleFilesLinkedList.Remove((File)subtitleFile_listView.SelectedItem);
                subtitleFilesLinkedList.AddLast((File)subtitleFile_listView.SelectedItem);

                subtitleFile_listView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && subtitleFile_listView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // 자막 파일명 바꾸기 버튼 동작
    public partial class MainWindow : Window
    {
        private void convertButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && subtitleFilesLinkedList.Count > 0 && videoFilesLinkedList.Count == subtitleFilesLinkedList.Count)
            {
                if (autoMoveSubtitleFilesCheckBox.IsChecked == true)
                {
                    for (int i = 0; i < subtitleFilesLinkedList.Count; i++)
                    {
                        System.IO.File.Move(subtitleFilesLinkedList.ElementAt(i).Path + "\\" + subtitleFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension, videoFilesLinkedList.ElementAt(i).Path + "\\" + videoFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension);
                    }
                }
                else if (autoMoveSubtitleFilesCheckBox.IsChecked == false)
                {
                    for (int i = 0; i < subtitleFilesLinkedList.Count; i++)
                    {
                        System.IO.File.Move(subtitleFilesLinkedList.ElementAt(i).Path + "\\" + subtitleFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension, subtitleFilesLinkedList.ElementAt(i).Path + "\\" + videoFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension);
                    }
                }

                MessageBox.Show("자막 파일 이름이 비디오 파일의 이름에 맞게 변경되었습니다.\n변경된 파일을 확인해주세요.", Properties.Resources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
                
                // 비디오 파일 리스트뷰 항목 전부 삭제 후 새로고침
                videoFilesLinkedList.Clear(); // 전부 삭제
                videoFile_listView.Items.Refresh(); // 새로고침

                // 자막 파일 리스트뷰 항목 전부 삭제 후 새로고침
                subtitleFilesLinkedList.Clear(); // 전부 삭제
                subtitleFile_listView.Items.Refresh(); // 새로고침
            }
            else if (videoFilesLinkedList.Count <= 0 && subtitleFilesLinkedList.Count > 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.\n비디오 파일을 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesLinkedList.Count > 0 && subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.\n자막 파일을 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesLinkedList.Count <= 0 && subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("파일이 존재하지 않습니다.\n파일을 다시 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (videoFilesLinkedList.Count > 0 && subtitleFilesLinkedList.Count > 0 && videoFilesLinkedList.Count != subtitleFilesLinkedList.Count)
            {
                MessageBox.Show("비디오 파일과 자막 파일의 개수가 일치하지 않습니다.\n파일들을 다시 확인해 주세요.", Properties.Resources.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
