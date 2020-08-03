using Plugin.Media;
using Plugin.Media.Abstractions;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using Test.MediaRecorder.Client.Services;
using Xamarin.Forms;

namespace Test.MediaRecorder.Client.ViewModels
{
  public class MainViewModel : ViewModelBase
  {
    private IPageDialogService _dialogService;
    private bool _isCameraNotSupported;
    private bool _isFrontCamera;
    private ILogService _log;
    private ImageSource _previewImage;

    public MainViewModel(INavigationService nav, ILogService log, IPageDialogService dialog)
        : base(nav)
    {
      Title = "Media Sample";

      _log = log;
      _dialogService = dialog;

      IsFrontCamera = false;
    }

    public DelegateCommand CmdPhotoCapture => new DelegateCommand(OnPhotoCaptureAsync);
    public DelegateCommand CmdPhotoPicker => new DelegateCommand(OnPhotoPickerAsync);
    public DelegateCommand CmdVideoRecorder => new DelegateCommand(OnVideoRecorderAsync);
    public DelegateCommand CmdVideoSelector => new DelegateCommand(OnVideoSelectorAsync);

    public bool IsCameraNotSupported
    {
      get => _isCameraNotSupported;
      set => SetProperty(ref _isCameraNotSupported, value);
    }

    public bool IsFrontCamera
    {
      get => _isFrontCamera;
      set => SetProperty(ref _isFrontCamera, value);
    }

    public ImageSource PreviewImage
    {
      get => _previewImage;
      set => SetProperty(ref _previewImage, value);
    }

    public override void OnNavigatedTo(INavigationParameters parameters)
    {
      base.OnNavigatedTo(parameters);

      if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
        IsCameraNotSupported = true;
      else
        IsCameraNotSupported = false;
    }

    private async void OnPhotoCaptureAsync()
    {
      // Ref:
      //  - https://mobileprogrammerblog.wordpress.com/2017/01/21/xamarin-forms-with-mvvm-light/

      if (IsCameraNotSupported)
      {
        await _dialogService.DisplayAlertAsync("Alert", "Camera is not supported on this device", "OK");
        return;
      }

      var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions {
        Directory = "Test",
        SaveToAlbum = true,
        CompressionQuality = 75,
        CustomPhotoSize = 50,
        PhotoSize = PhotoSize.MaxWidthHeight,
        MaxWidthHeight = 2000,
        DefaultCamera = IsFrontCamera ? CameraDevice.Front : CameraDevice.Rear
      });

      if (file == null)
      {
        await _dialogService.DisplayAlertAsync("Alert", "Invalid or missing file.", "OK");
        return;
      }
      else
      {
        await _dialogService.DisplayAlertAsync("Saved", "File: " + file.Path, "OK");
      }

      // Method A
      PreviewImage = ImageSource.FromStream(() =>
      {
        var stream = file.GetStream();
        file.Dispose();
        return stream;
      });
    }

    private async void OnPhotoPickerAsync()
    {
      if (IsCameraNotSupported)
      {
        await _dialogService.DisplayAlertAsync("Alert", "Camera is not supported on this device", "OK");
        return;
      }

      if (!CrossMedia.Current.IsPickPhotoSupported)
      {
        await _dialogService.DisplayAlertAsync("Photos Not Supported", ":( Permission not granted to photos.", "OK");
        return;
      }
      var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions {
        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
      });

      if (file == null)
      {
        await _dialogService.DisplayAlertAsync("Alert", "Invalid or missing file.", "OK");
        return;
      }

      PreviewImage = ImageSource.FromStream(() =>
      {
        var stream = file.GetStream();
        file.Dispose();
        return stream;
      });
    }

    private async void OnVideoRecorderAsync()
    {
      if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakeVideoSupported)
      {
        await _dialogService.DisplayAlertAsync("No Camera", ":( No camera available.", "OK");
        return;
      }

      var file = await CrossMedia.Current.TakeVideoAsync(new Plugin.Media.Abstractions.StoreVideoOptions {
        Name = "video.mp4",
        DefaultCamera = IsFrontCamera ? CameraDevice.Front : CameraDevice.Rear
        Directory = "DefaultVideos",
        SaveToAlbum = false,
      });

      if (file == null)
        return;

      await _dialogService.DisplayAlertAsync("Video Recorded", "Location: " + file.Path, "OK");

      file.Dispose();
    }

    private async void OnVideoSelectorAsync()
    {
      if (!CrossMedia.Current.IsPickVideoSupported)
      {
        await _dialogService.DisplayAlertAsync("Videos Not Supported", ":( Permission not granted to videos.", "OK");
        return;
      }
      var file = await CrossMedia.Current.PickVideoAsync();

      if (file == null)
        return;

      await _dialogService.DisplayAlertAsync("Video Selected", "Location: " + file.Path, "OK");
      file.Dispose();
    }
  }
}
