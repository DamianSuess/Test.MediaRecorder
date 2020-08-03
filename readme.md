# Sample Media Recorder - Xamarin.Forms

Sample of how to record video, take pictures, and display results using Xamarin.Forms.

Sponsored by Xeno Innovations, this project was made with nerd-love using _Xamarin.Forms and Prism with DryIoc.

## To Do
* [X] Take pictures and save
* [X] Record video and save
* [X] Preview pictures
* [X] Choose front or rear camera
* [ ] Preview video
* [ ] Display live camera on screen
* [ ] Swipe right/left for camera or video recorder
* [ ] Swipe up/down for front or rear camera

## Permissions

### Android

* ``WRITE_EXTERNAL_STORAGE``
* ``READ_EXTERNAL_STORAGE``
* ``AssemblyInfo.cs``
```cs
[assembly: UsesFeature("android.hardware.camera", Required = true)]
[assembly: UsesFeature("android.hardware.camera.autofocus", Required = true)]
```

### UWP
* ``Webcam``

## References
* [Xam.Plugin.Media](https://github.com/jamesmontemagno/MediaPlugin)
* [How to record video with Xamarin.Forms](https://forums.xamarin.com/discussion/124773/how-to-record-video-in-xamarin-forms-for-15-seconds)
