using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace SubtitleRun
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // 오름차순 내림차순 정렬 유무 판단 bool 변수
        private bool isSortedVideoFileNameFlag = false;
        private bool isSortedVideoFileExtensionFlag = false;
        private bool isSortedSubtitleFileNameFlag = false;
        private bool isSortedSubtitleFileExtensionFlag = false;

        private enum FILETYPE
        {
            VIDEO,
            SUBTITLE
        }

        // 비디오, 자막 파일 구조체
        private struct Files
        {
            public FILETYPE FILETYPE { get; set; }
            public string Path { get; set; }
            public string Name { get; set; }
            public string Extension { get; set; }
        }

        Files files;
    }

    // [비디오] 파일 리스트뷰 동작
    public partial class MainWindow : Window
    {
        readonly LinkedList<Files> videoFilesLinkedList = new LinkedList<Files>();   // [비디오] 파일 이중연결리스트

        // [비디오] 리스트뷰에 파일을 Drag&Drop 할 때, 드랍 가능 여부를 판단하기 위해 마우스 포인터 모양을 바꾸는 이벤트
        private void VideoFileListView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        // [비디오] 리스트뷰에 파일을 Drag&Drop 하여 추가하면, 비디오 파일 리스트에 디렉토리 경로, 파일 이름, 파일 확장자를 저장 후 리스트뷰에 디렉토리 경로, 파일 이름, 파일 확장자 표시하는 이벤트
        private void VideoFileListView_Drop(object sender, DragEventArgs e)
        {
            string[] videoFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            FileAttributes fileAttributes; // 파일 속성에 관련한 변수

            foreach (string videoFilePath in videoFilePaths)
            {
                fileAttributes = File.GetAttributes(videoFilePath); // (string)videoFilePath 변수를 System.IO.File.Getattributes 메소드 인자로 fileAttributes 변수에 대입

                // [비디오] Drag&Drop 했을 때, FileAttributes.Dirctory를 검사하여 파일이 아닌 폴더이면 무시하는 조건문
                // 출처: https://docs.microsoft.com/ko-kr/dotnet/api/system.io.file.getattributes?view=net-5.0#System_IO_File_GetAttributes_System_String
                if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    continue;
                }
                else
                {
                    files.FILETYPE = FILETYPE.VIDEO;
                    files.Path = Path.GetDirectoryName(videoFilePath);
                    files.Name = Path.GetFileNameWithoutExtension(videoFilePath);
                    files.Extension = Path.GetExtension(videoFilePath);

                    // 이중연결리스트 안에 이미 항목이 존재하는지 검사해서 이미 있으면 건너뛰는 조건문
                    if (videoFilesLinkedList.Contains(files))
                    {
                        continue;
                    }
                    else
                    {
                        videoFilesLinkedList.AddLast(files);
                    }
                }
            }

            VideoFileListView.ItemsSource = videoFilesLinkedList;
            VideoFileListView.Items.Refresh();
        }

        // [비디오] 리스트뷰의 추가 버튼을 클릭하여 파일 탐색기를 실행 후 파일을 추가하는 버튼 클릭 이벤트
        private void VideoAddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Video Files|*.asf; *.avi; *.flv; *.bik; *.bik2; *.flv; *.mkv; *.mov; *.mp4; *.m4p; *.m4b; *.m4r; *.m4v; *.mpeg; *.mpg; *.ps; *.ts; *.tp; *.mts; *.m2ts; *.tod; 3gp; *.skm; *.k3g; *.lmp4 *.ogg; *.rm; *.wmv; *.webm|All files (*.*)|*.*"
                // InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) // 윈도우 파일 탐색기의 기본 위치를 내문서로 강제 변경하기
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                foreach (string videoFilePath in openFileDialog.FileNames)
                {
                    files.FILETYPE = FILETYPE.VIDEO;
                    files.Path = Path.GetDirectoryName(videoFilePath);
                    files.Name = Path.GetFileNameWithoutExtension(videoFilePath);
                    files.Extension = Path.GetExtension(videoFilePath);

                    // 이중연결리스트 안에 이미 항목이 존재하는지 검사해서 이미 있으면 건너뛰는 조건문
                    if (videoFilesLinkedList.Contains(files))
                    {
                        continue;
                    }
                    else
                    {
                        videoFilesLinkedList.AddLast(files);
                    }
                }

                VideoFileListView.ItemsSource = videoFilesLinkedList;
                VideoFileListView.Items.Refresh();
            }
        }

        // [비디오] 리스트뷰의 삭제 버튼을 클릭하여 체크박스가 체크된 항목들을 삭제하는 버튼 클릭 이벤트
        private void VideoDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0)
            {
                foreach (Files item in VideoFileListView.SelectedItems)
                {
                    videoFilesLinkedList.Remove(item);
                }

                VideoFileListView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [비디오] 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void SortByVideoFileName_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0)
            {
                List<Files> sortedList = videoFilesLinkedList.ToList();

                if (isSortedVideoFileNameFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Name).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileNameFlag = true;
                }
                else if (isSortedVideoFileNameFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Name).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileNameFlag = false;
                }

                VideoFileListView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [비디오] 리스트뷰의 확장자순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void SortByVideoFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0)
            {
                List<Files> sortedList = videoFilesLinkedList.ToList();

                if (isSortedVideoFileExtensionFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Extension).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileExtensionFlag = true;
                }
                else if (isSortedVideoFileExtensionFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Extension).ToList();

                    videoFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        videoFilesLinkedList.AddLast(item);
                    }

                    isSortedVideoFileExtensionFlag = false;
                }

                VideoFileListView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [비디오] 리스트 뷰의 항목들을 체크한 뒤, 맨 위로 버튼을 누르면 리스트 뷰의 맨 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveVideoIndexTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count == 1)
            {
                videoFilesLinkedList.Remove((Files)VideoFileListView.SelectedItem);
                videoFilesLinkedList.AddFirst((Files)VideoFileListView.SelectedItem);

                VideoFileListView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [비디오] 리스트 뷰의 항목들을 체크한 뒤, 위로 버튼을 누르면 리스트 뷰의 한단계 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveVideoIndexUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count == 1)
            {
                LinkedListNode<Files> previousNode = videoFilesLinkedList.Find((Files)VideoFileListView.SelectedItem).Previous;
                LinkedListNode<Files> currentNode = videoFilesLinkedList.Find((Files)VideoFileListView.SelectedItem);

                if (previousNode != null)
                {
                    videoFilesLinkedList.Remove(currentNode);
                    videoFilesLinkedList.AddBefore(previousNode, currentNode);
                }

                VideoFileListView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [비디오] 리스트 뷰의 항목들을 체크한 뒤, 아래로 버튼을 누르면 리스트 뷰의 한단계 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveVideoIndexDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count == 1)
            {
                LinkedListNode<Files> nextNode = videoFilesLinkedList.Find((Files)VideoFileListView.SelectedItem).Next;
                LinkedListNode<Files> currentNode = videoFilesLinkedList.Find((Files)VideoFileListView.SelectedItem);

                if (nextNode != null)
                {
                    videoFilesLinkedList.Remove(currentNode);
                    videoFilesLinkedList.AddAfter(nextNode, currentNode);
                }

                VideoFileListView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [비디오] 리스트 뷰의 항목들을 체크한 뒤, 맨 아래로 버튼을 누르면 리스트 뷰의 맨 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveVideoIndexBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count == 1)
            {
                videoFilesLinkedList.Remove((Files)VideoFileListView.SelectedItem);
                videoFilesLinkedList.AddLast((Files)VideoFileListView.SelectedItem);

                VideoFileListView.Items.Refresh();
            }
            else if (videoFilesLinkedList.Count > 0 && VideoFileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (videoFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("비디오 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // [자막] 파일 리스트뷰 동작
    public partial class MainWindow : Window
    {
        readonly LinkedList<Files> subtitleFilesLinkedList = new LinkedList<Files>();    // 자막파일 이중연결리스트

        // [자막] 리스트뷰에 파일을 Drag&Drop 할 때, 드랍 가능 여부를 판단하기 위해 마우스 포인터 모양을 바꾸는 이벤트
        private void SubtitleFileListView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }

        // [자막] 리스트뷰에 파일을 Drag&Drop 하여 추가하면, 자막 파일 리스트에 디렉토리 경로, 파일 이름, 파일 확장자를 저장 후 리스트뷰에 디렉토리 경로, 파일 이름, 파일 확장자 표시하는 이벤트
        private void SubtitleFileListView_Drop(object sender, DragEventArgs e)
        {
            string[] subtitleFilePaths = (string[])e.Data.GetData(DataFormats.FileDrop);
            FileAttributes fileAttributes; // 파일 속성에 관련한 변수

            foreach (string subtitleFilePath in subtitleFilePaths)
            {
                fileAttributes = File.GetAttributes(subtitleFilePath); // (string)subtitleFilePath 변수를 System.IO.File.Getattributes 메소드 인자로 fileAttributes 변수에 대입

                // [비디오] Drag&Drop 했을 때, FileAttributes.Dirctory를 검사하여 파일이 아닌 폴더이면 무시하는 조건문
                // 출처: https://docs.microsoft.com/ko-kr/dotnet/api/system.io.file.getattributes?view=net-5.0#System_IO_File_GetAttributes_System_String
                if ((fileAttributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    continue;
                }
                else
                {
                    files.FILETYPE = FILETYPE.SUBTITLE;
                    files.Path = Path.GetDirectoryName(subtitleFilePath);
                    files.Name = Path.GetFileNameWithoutExtension(subtitleFilePath);
                    files.Extension = Path.GetExtension(subtitleFilePath);

                    // 이중연결리스트 안에 이미 항목이 존재하는지 검사해서 이미 있으면 건너뛰는 조건문
                    if (subtitleFilesLinkedList.Contains(files))
                    {
                        continue;
                    }
                    else
                    {
                        subtitleFilesLinkedList.AddLast(files);
                    }
                }
            }

            SubtitleFileListView.ItemsSource = subtitleFilesLinkedList;
            SubtitleFileListView.Items.Refresh();
        }

        // [자막] 리스트뷰의 추가 버튼을 클릭하여 파일 탐색기를 실행 후 파일을 추가하는 버튼 클릭 이벤트
        private void SubtitleAddButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Subtitle Files|*.smi; *.srt; *.ssa; *.ass; *.lrc|All files (*.*)|*.*"
                // InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) // 윈도우 파일 탐색기의 기본 위치를 내문서로 강제 변경하기
            };

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                foreach (string subtitleFilePath in openFileDialog.FileNames)
                {
                    files.FILETYPE = FILETYPE.SUBTITLE;
                    files.Path = Path.GetDirectoryName(subtitleFilePath);
                    files.Name = Path.GetFileNameWithoutExtension(subtitleFilePath);
                    files.Extension = Path.GetExtension(subtitleFilePath);

                    // 이중연결리스트 안에 이미 항목이 존재하는지 검사해서 이미 있으면 건너뛰는 조건문
                    if (subtitleFilesLinkedList.Contains(files))
                    {
                        continue;
                    }
                    else
                    {
                        subtitleFilesLinkedList.AddLast(files);
                    }
                }

                SubtitleFileListView.ItemsSource = subtitleFilesLinkedList;
                SubtitleFileListView.Items.Refresh();
            }
        }

        // [자막] 리스트뷰의 삭제 버튼을 클릭하여 체크박스가 체크된 항목들을 삭제하는 버튼 클릭 이벤트
        private void SubtitleDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count != 0)
            {
                foreach (Files item in SubtitleFileListView.SelectedItems)
                {
                    subtitleFilesLinkedList.Remove(item);
                }

                SubtitleFileListView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count == 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [자막] 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 이름 순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void SortBySubtitleFileName_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0)
            {
                List<Files> sortedList = subtitleFilesLinkedList.ToList();

                if (isSortedSubtitleFileNameFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Name).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileNameFlag = true;
                }
                else if (isSortedSubtitleFileNameFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Name).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileNameFlag = false;
                }

                SubtitleFileListView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [자막] 리스트뷰의 이름순 정렬 버튼을 클릭하면 추가된 항목들을 확장자 이름순으로 오름차순/내림차순 정렬하는 버튼 클릭 이벤트
        private void SortBySubtitleFileExtension_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0)
            {
                List<Files> sortedList = subtitleFilesLinkedList.ToList();

                if (isSortedSubtitleFileExtensionFlag == false)
                {
                    sortedList = sortedList.OrderBy(x => x.Extension).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileExtensionFlag = true;
                }
                else if (isSortedSubtitleFileExtensionFlag == true)
                {
                    sortedList = sortedList.OrderByDescending(x => x.Extension).ToList();

                    subtitleFilesLinkedList.Clear();

                    foreach (Files item in sortedList)
                    {
                        subtitleFilesLinkedList.AddLast(item);
                    }

                    isSortedSubtitleFileExtensionFlag = false;
                }

                SubtitleFileListView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [자막] 리스트 뷰의 항목들을 체크한 뒤, 맨 위로 버튼을 누르면 리스트 뷰의 맨 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveSubtitleIndexTopButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count == 1)
            {
                subtitleFilesLinkedList.Remove((Files)SubtitleFileListView.SelectedItem);
                subtitleFilesLinkedList.AddFirst((Files)SubtitleFileListView.SelectedItem);

                SubtitleFileListView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [자막] 리스트 뷰의 항목들을 체크한 뒤, 위로 버튼을 누르면 리스트 뷰의 한단계 위쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveSubtitleIndexUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count == 1)
            {
                LinkedListNode<Files> previousNode = subtitleFilesLinkedList.Find((Files)SubtitleFileListView.SelectedItem).Previous;
                LinkedListNode<Files> currentNode = subtitleFilesLinkedList.Find((Files)SubtitleFileListView.SelectedItem);

                if (previousNode != null)
                {
                    subtitleFilesLinkedList.Remove(currentNode);
                    subtitleFilesLinkedList.AddBefore(previousNode, currentNode);
                }

                SubtitleFileListView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [자막] 리스트 뷰의 항목들을 체크한 뒤, 아래로 버튼을 누르면 리스트 뷰의 한단계 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveSubtitleIndexDownButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count == 1)
            {
                LinkedListNode<Files> nextNode = subtitleFilesLinkedList.Find((Files)SubtitleFileListView.SelectedItem).Next;
                LinkedListNode<Files> currentNode = subtitleFilesLinkedList.Find((Files)SubtitleFileListView.SelectedItem);

                if (nextNode != null)
                {
                    subtitleFilesLinkedList.Remove(currentNode);
                    subtitleFilesLinkedList.AddAfter(nextNode, currentNode);
                }

                SubtitleFileListView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count > 1)
            {
                MessageBox.Show("이동시킬 항목을 1개만 선택해 주세요!", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else if (subtitleFilesLinkedList.Count <= 0)
            {
                MessageBox.Show("자막 파일이 존재하지 않습니다.", Properties.Resources.Warning, MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // [자막] 리스트 뷰의 항목들을 체크한 뒤, 맨 아래로 버튼을 누르면 리스트 뷰의 맨 아래쪽으로 항목이 이동하는 버튼 클릭 이벤트
        private void MoveSubtitleIndexBottomButton_Click(object sender, RoutedEventArgs e)
        {
            if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count == 1)
            {
                subtitleFilesLinkedList.Remove((Files)SubtitleFileListView.SelectedItem);
                subtitleFilesLinkedList.AddLast((Files)SubtitleFileListView.SelectedItem);

                SubtitleFileListView.Items.Refresh();
            }
            else if (subtitleFilesLinkedList.Count > 0 && SubtitleFileListView.SelectedItems.Count > 1)
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
        private void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            if (videoFilesLinkedList.Count > 0 && subtitleFilesLinkedList.Count > 0 && videoFilesLinkedList.Count == subtitleFilesLinkedList.Count)
            {
                if (AutoMoveSubtitleFilesCheckBox.IsChecked == true)
                {
                    for (int i = 0; i < subtitleFilesLinkedList.Count; i++)
                    {
                        File.Move(subtitleFilesLinkedList.ElementAt(i).Path + "\\" + subtitleFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension, videoFilesLinkedList.ElementAt(i).Path + "\\" + videoFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension);
                    }
                }
                else if (AutoMoveSubtitleFilesCheckBox.IsChecked == false)
                {
                    for (int i = 0; i < subtitleFilesLinkedList.Count; i++)
                    {
                        File.Move(subtitleFilesLinkedList.ElementAt(i).Path + "\\" + subtitleFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension, subtitleFilesLinkedList.ElementAt(i).Path + "\\" + videoFilesLinkedList.ElementAt(i).Name + subtitleFilesLinkedList.ElementAt(i).Extension);
                    }
                }

                MessageBox.Show("자막 파일 이름이 비디오 파일의 이름에 맞게 변경되었습니다.\n변경된 파일을 확인해주세요.", Properties.Resources.Information, MessageBoxButton.OK, MessageBoxImage.Information);

                // 비디오 파일 리스트뷰 항목 전부 삭제 후 새로고침
                videoFilesLinkedList.Clear(); // 전부 삭제
                VideoFileListView.Items.Refresh(); // 새로고침

                // 자막 파일 리스트뷰 항목 전부 삭제 후 새로고침
                subtitleFilesLinkedList.Clear(); // 전부 삭제
                SubtitleFileListView.Items.Refresh(); // 새로고침
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
