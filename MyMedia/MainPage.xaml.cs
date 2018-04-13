using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MyMedia
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        MediaPlayer MyMediaPlayer = new MediaPlayer();

        

        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
            picker.FileTypeFilter.Add(".mp3");
            picker.FileTypeFilter.Add(".mp4");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                var mediaSource = MediaSource.CreateFromStorageFile(file);
                MyMediaPlayer.Source = mediaSource;
                MyMedia.SetMediaPlayer(MyMediaPlayer);
                MyMediaPlayer.Play();
                
            }
        }

        private void Play_On_Net_Button_Click(object sender, RoutedEventArgs e)
        {
            MyMediaPlayer.Source = MediaSource.CreateFromUri(new Uri("http://www.neu.edu.cn/indexsource/neusong.mp3")); 
            
            MyMedia.SetMediaPlayer(MyMediaPlayer);
            MyMediaPlayer.Play();
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            StorageFile LoadFile = await Load();
            if(LoadFile != null)
            {
                var mediaSource = MediaSource.CreateFromStorageFile(LoadFile);
                MyMediaPlayer.Source = mediaSource;
                MyMedia.SetMediaPlayer(MyMediaPlayer);
                MyMediaPlayer.Play();
            }
            

        }

        public async Task<StorageFile> Load()
        {
            try
            {
                var httpClient = new HttpClient();
                var buffer = await httpClient.GetBufferAsync(new Uri("http://www.neu.edu.cn/indexsource/neusong.mp3"));
                if (buffer != null && buffer.Length > 0u)
                {
                    var file = await KnownFolders.MusicLibrary.CreateFileAsync("neusong.mp3", CreationCollisionOption.ReplaceExisting);
                    using (var stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await stream.WriteAsync(buffer);
                        await stream.FlushAsync();
                    }
                    return file;
                }
            }
            catch { }
            return null;
        }

    }
}
