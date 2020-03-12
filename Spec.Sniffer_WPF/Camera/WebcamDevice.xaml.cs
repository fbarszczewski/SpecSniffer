﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AForge.Video.DirectShow;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Spec.Sniffer_WPF.Camera
{
    /// <summary>
    /// Logika interakcji dla klasy WebcamDevice.xaml
    /// </summary>
    public partial class WebcamDevice : UserControl
    {
        #region Constructor

        public WebcamDevice()
        {
            InitializeComponent();

            //// Subcribe to dispatcher shutdown event and dispose all used resources gracefully.
            Dispatcher.ShutdownStarted += DispatcherShutdownStarted;

            //// Initialize take snapshot command.
            //TakeSnapshot = new TakeSnapshotCommand(TakeSnapshotCallback);
        }

        #endregion

        #region Variable declaration

        /// <summary>
        ///     Dependency property for video display width.
        /// </summary>
        public static readonly DependencyProperty VideoPreviewWidthProperty =
            DependencyProperty.Register("VideoPreviewWidth", typeof(double), typeof(WebcamDevice),
                new PropertyMetadata(VideoPreviewWidthPropertyChangedCallback));

        /// <summary>
        ///     Dependency property for video display height.
        /// </summary>
        public static readonly DependencyProperty VideoPreviewHeightProperty =
            DependencyProperty.Register("VideoPreviewHeight", typeof(double), typeof(WebcamDevice),
                new PropertyMetadata(VideoPreviewHeightPropertyChangedCallback));

        /// <summary>
        ///     Dependency property for video device source Id.
        /// </summary>
        public static readonly DependencyProperty VideoSourceIdProperty = DependencyProperty.Register("VideoSourceId",
            typeof(string), typeof(WebcamDevice),
            new PropertyMetadata(string.Empty, VideoSourceIdPropertyChangedCallback,
                VideoSourceIdPropertyCoherceValueChanged));

        /// <summary>
        ///     Dependency property for snapshot bitmap.
        /// </summary>
        public static readonly DependencyProperty SnapshotBitmapProperty = DependencyProperty.Register("SnapshotBitmap",
            typeof(Bitmap), typeof(WebcamDevice), new PropertyMetadata(SnapshotBitmapPropertyChangedCallback));

        ///// <summary>
        /////     Dependency property for take snapshot command.
        ///// </summary>
        //public static readonly DependencyProperty TakeSnapshotProperty =
        //    DependencyProperty.RegisterAttached("TakeSnapshot", typeof(ICommand), typeof(WebcamDevice),
        //        new PropertyMetadata(default(TakeSnapshotCommand)));

        /// <summary>
        ///     Instance of video capture device.
        /// </summary>
        private VideoCaptureDevice videoCaptureDevice;

        /// <summary>
        ///     The is video source initialized.
        /// </summary>
        private bool isVideoSourceInitialized;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets video device source collection current available.
        /// </summary>
        public static IEnumerable<MediaInformation> GetVideoDevices
        {
            get
            {
                var filterVideoDeviceCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                return (from FilterInfo filterInfo in filterVideoDeviceCollection
                        select new MediaInformation {DisplayName = filterInfo.Name, UsbId = filterInfo.MonikerString})
                    .ToList();
            }
        }

        /// <summary>
        ///     Gets or sets video source device display width.
        /// </summary>
        /// <remarks>
        ///     Important: Do not write any logic in dependency property stub.
        /// </remarks>
        [TypeConverter(typeof(LengthConverter))]
        public double VideoPreviewWidth
        {
            get => (double) GetValue(VideoPreviewWidthProperty);

            set => SetValue(VideoPreviewWidthProperty, value);
        }

        /// <summary>
        ///     Gets or sets video source device display height.
        /// </summary>
        /// <remarks>
        ///     Important: Do not write any logic in dependency property stub.
        /// </remarks>
        [TypeConverter(typeof(LengthConverter))]
        public double VideoPreviewHeight
        {
            get => (double) GetValue(VideoPreviewHeightProperty);

            set => SetValue(VideoPreviewHeightProperty, value);
        }

        /// <summary>
        ///     Gets or sets video device source Id / USB ID / Moniker string.
        /// </summary>
        /// <remarks>
        ///     Note: If the Id start with "Message:" followed by a message,
        ///     the following message will be display instead of the video source device.
        ///     Important: Do not write any logic in dependency property stub.
        /// </remarks>
        public string VideoSourceId
        {
            get => (string) GetValue(VideoSourceIdProperty);

            set => SetValue(VideoSourceIdProperty, value);
        }

        /// <summary>
        ///     Gets or sets snapshot image bitmap (Default: PNG format).
        /// </summary>
        public Bitmap SnapshotBitmap
        {
            get => (Bitmap) GetValue(SnapshotBitmapProperty);

            set => SetValue(SnapshotBitmapProperty, value);
        }

        ///// <summary>
        /////     Gets or sets take snapshot command.
        ///// </summary>
        //public TakeSnapshotCommand TakeSnapshot
        //{
        //    get => (TakeSnapshotCommand) GetValue(TakeSnapshotProperty);

        //    set => SetValue(TakeSnapshotProperty, value);
        //}

        #endregion

        #region Methods

        /// <summary>
        ///     Call back function for take snapshot command dependency property,
        ///     when no video device source selected show appropriate message
        ///     or message currently display in user-control based on user input.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     Wraps and throws <see cref="InvalidOperationException" /> when any
        ///     exception is thrown from this method.
        /// </exception>
        public void TakeSnapshotCallback()
        {
            try
            {
                var playerPoint = new Point();

                //// Get the position of the source video device player.
                if (string.IsNullOrWhiteSpace(VideoSourceId))
                {
                    var noVideoDeviceSourcePoint = NoVideoSourceGrid.PointToScreen(new System.Windows.Point(0, 0));
                    playerPoint.X = (int) noVideoDeviceSourcePoint.X;
                    playerPoint.Y = (int) noVideoDeviceSourcePoint.Y;
                }
                else
                {
                    playerPoint = VideoSourcePlayer.PointToScreen(new Point(VideoSourcePlayer.ClientRectangle.X,
                        VideoSourcePlayer.ClientRectangle.Y));
                }

                if (double.IsNaN(VideoPreviewWidth) || double.IsNaN(VideoPreviewHeight))
                    using (var bitmap = new Bitmap((int) VideoSourceWindowsFormsHost.ActualWidth,
                        (int) VideoSourceWindowsFormsHost.ActualHeight))
                    {
                        using (var graphicsFromImage = Graphics.FromImage(bitmap))
                        {
                            graphicsFromImage.CopyFromScreen(playerPoint, Point.Empty,
                                new Size((int) VideoSourceWindowsFormsHost.ActualWidth,
                                    (int) VideoSourceWindowsFormsHost.ActualHeight));
                        }

                        SnapshotBitmap = new Bitmap(bitmap);
                    }
                else
                    using (var bitmap = new Bitmap((int) VideoPreviewWidth, (int) VideoPreviewHeight))
                    {
                        using (var graphicsFromImage = Graphics.FromImage(bitmap))
                        {
                            graphicsFromImage.CopyFromScreen(playerPoint, Point.Empty,
                                new Size((int) VideoPreviewWidth, (int) VideoPreviewHeight));
                        }

                        SnapshotBitmap = new Bitmap(bitmap);
                    }
            }
            catch (Exception exception)
            {
                throw new InvalidOperationException(
                    "Error occurred while trying to take snapshot from currently selected source video device",
                    exception);
            }
        }

        /// <summary>
        ///     Call back function for video device source Id property changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event argument.</param>
        private static void VideoSourceIdPropertyChangedCallback(DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var oldValue = eventArgs.OldValue as string;
            var newValue = eventArgs.NewValue as string;
            var webCamDevice = sender as WebcamDevice;
            if (null == webCamDevice) return;

            if (null == eventArgs.NewValue) return;

            if (string.IsNullOrWhiteSpace(newValue))
            {
                if (!string.IsNullOrWhiteSpace(oldValue)) webCamDevice.InitializeVideoDevice(oldValue);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(oldValue))
                {
                    webCamDevice.InitializeVideoDevice(newValue);
                }
                else
                {
                    if (oldValue != newValue) webCamDevice.isVideoSourceInitialized = false;

                    webCamDevice.InitializeVideoDevice(oldValue.Equals(newValue) ? oldValue : newValue);
                }
            }
        }

        /// <summary>
        ///     Event handler for video device Id value changed event.
        /// </summary>
        /// <param name="dependencyObject">Instance of dependency object.</param>
        /// <param name="basevalue">Base value.</param>
        /// <returns>Return base value / NULL or the new Id value of the video device source.</returns>
        private static object VideoSourceIdPropertyCoherceValueChanged(DependencyObject dependencyObject,
            object basevalue)
        {
            var baseValueStringFormat = Convert.ToString(basevalue, CultureInfo.InvariantCulture);
            var availableMediaList = GetVideoDevices;
            var mediaInfos = availableMediaList as IList<MediaInformation> ?? availableMediaList.ToList();
            if (string.IsNullOrEmpty(baseValueStringFormat) || !mediaInfos.Any()) return null;

            var filteredVideoDevice = mediaInfos.FirstOrDefault(item => item.UsbId.Equals(baseValueStringFormat));
            return null != filteredVideoDevice ? filteredVideoDevice.UsbId : baseValueStringFormat;
        }

        /// <summary>
        ///     Call back function for source video device preview width property changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event argument.</param>
        private static void VideoPreviewWidthPropertyChangedCallback(DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var webCamDevice = sender as WebcamDevice;
            if (null == webCamDevice) return;

            if (null == eventArgs.NewValue) return;

            var newValue = (double) eventArgs.NewValue;
            if (double.IsNaN(newValue))
            {
                var parentControl = webCamDevice.VisualParent as Grid;
                webCamDevice.SetVideoPlayerWidth(null != parentControl ? parentControl.Width : newValue);
            }
            else
            {
                webCamDevice.SetVideoPlayerWidth(newValue);
            }
        }

        /// <summary>
        ///     Call back function for source video device preview height property changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event argument.</param>
        private static void VideoPreviewHeightPropertyChangedCallback(DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            var webCamDevice = sender as WebcamDevice;
            if (null == webCamDevice) return;

            if (null == eventArgs.NewValue) return;

            var newValue = (double) eventArgs.NewValue;
            if (double.IsNaN(newValue))
            {
                var parentControl = webCamDevice.VisualParent as Grid;
                webCamDevice.SetVideoPlayerHeight(null != parentControl ? parentControl.Height : newValue);
            }
            else
            {
                webCamDevice.SetVideoPlayerHeight(newValue);
            }
        }

        /// <summary>
        ///     Call back function for snapshot bytes property changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event argument.</param>
        private static void SnapshotBitmapPropertyChangedCallback(DependencyObject sender,
            DependencyPropertyChangedEventArgs eventArgs)
        {
            //// NOTE: Created to make the dependency property bindable from view-model.
        }

        /// <summary>
        ///     Initialize video device.
        /// </summary>
        /// <param name="videoDeviceSourceId">Video device source Id.</param>
        /// <exception cref="InvalidOperationException">Throws invalid operation exception if video device source setup fails.</exception>
        private void InitializeVideoDevice(string videoDeviceSourceId)
        {
            if (isVideoSourceInitialized) return;

            var errorAction = new Action(() => SetVideoPlayer(false, "Unable to set video device source"));
            ReleaseVideoDevice();
            if (string.IsNullOrEmpty(videoDeviceSourceId)) return;

            if (videoDeviceSourceId.StartsWith("Message:", StringComparison.OrdinalIgnoreCase))
            {
                var splitString = videoDeviceSourceId.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                if (splitString.Length == 2)
                    SetVideoPlayer(false, splitString[1]);
                else
                    SetVideoPlayer(false);
            }
            else
            {
                try
                {
                    if (!GetVideoDevices.Any(item => item.UsbId.Equals(videoDeviceSourceId))) return;

                    videoCaptureDevice = new VideoCaptureDevice(videoDeviceSourceId);
                    VideoSourcePlayer.VideoSource = videoCaptureDevice;
                    VideoSourcePlayer.Start();
                    isVideoSourceInitialized = true;
                    SetVideoPlayer(true);
                }
                catch (ArgumentNullException)
                {
                    errorAction();
                }
                catch (ArgumentException)
                {
                    errorAction();
                }
            }
        }

        /// <summary>
        ///     Set video player width.
        /// </summary>
        /// <param name="newWidth">New width value.</param>
        private void SetVideoPlayerWidth(double newWidth)
        {
            NoVideoSourceGrid.Width = newWidth;
            VideoSourceWindowsFormsHost.Width = newWidth;
        }

        /// <summary>
        ///     Set video player height.
        /// </summary>
        /// <param name="newHeight">New height value.</param>
        private void SetVideoPlayerHeight(double newHeight)
        {
            NoVideoSourceGrid.Height = newHeight;
            VideoSourceWindowsFormsHost.Height = newHeight;
        }

        /// <summary>
        ///     Event handler for camera video device on loaded event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event argument.</param>
        private void WebcamDeviceOnLoaded(object sender, RoutedEventArgs eventArgs)
        {
            //// Set controls width / height based on VideoPreviewWidth / VideoPreviewHeight binding properties.
            NoVideoSourceGrid.Width = VideoPreviewWidth;
            VideoSourceWindowsFormsHost.Width = VideoPreviewWidth;
            NoVideoSourceGrid.Height = VideoPreviewHeight;
            VideoSourceWindowsFormsHost.Height = VideoPreviewHeight;
            InitializeVideoDevice(VideoSourceId);
        }

        /// <summary>
        ///     Disconnect video source device.
        /// </summary>
        private void ReleaseVideoDevice()
        {
            isVideoSourceInitialized = false;
            SetVideoPlayer(false);
            if (null == videoCaptureDevice) return;

            videoCaptureDevice.SignalToStop();
            videoCaptureDevice.WaitForStop();
            videoCaptureDevice.Stop();
            videoCaptureDevice = null;
        }

        /// <summary>
        ///     Set video source player visibility.
        /// </summary>
        /// <param name="isVideoSourceFound">Indicates a value weather video source device found or not.</param>
        /// <param name="noVideoSourceMessage">Message to display when no video source found, optional will use empty string.</param>
        private void SetVideoPlayer(bool isVideoSourceFound, string noVideoSourceMessage = "")
        {
            //// If video source found is true show the video source player or else show no video source message.
            if (isVideoSourceFound)
            {
                VideoSourceWindowsFormsHost.Visibility = Visibility.Visible;
                NoVideoSourceGrid.Visibility = Visibility.Hidden;
                NoVideoSourceMessage.Text = string.Empty;
            }
            else
            {
                VideoSourceWindowsFormsHost.Visibility = Visibility.Hidden;
                NoVideoSourceGrid.Visibility = Visibility.Visible;
                NoVideoSourceMessage.Text = string.IsNullOrWhiteSpace(noVideoSourceMessage)
                    ? "No video source selected."
                    : noVideoSourceMessage;
            }
        }

        /// <summary>
        ///     Event handler for dispatcher shutdown started event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private void DispatcherShutdownStarted(object sender, EventArgs eventArgs)
        {
            ReleaseVideoDevice();
        }

        /// <summary>
        ///     Event handler for camera video device on unloaded event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="eventArgs">Event arguments.</param>
        private void WebcamDeviceOnUnloaded(object sender, RoutedEventArgs eventArgs)
        {
            ReleaseVideoDevice();
        }

        #endregion
    }
}
