﻿/**********************************************************************
 * VLC for WinRT
 **********************************************************************
 * Copyright © 2013-2014 VideoLAN and Authors
 *
 * Licensed under GPLv2+ and MPLv2
 * Refer to COPYING file of the official project for license
 **********************************************************************/

using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Core;
using SQLite;
using VLC_WinRT.Commands.Video;
using VLC_WinRT.Common;
using VLC_WinRT.Helpers.VideoLibrary;

namespace VLC_WinRT.Model.Video
{
    public class VideoItem : BindableBase, IVLCMedia
    {
        #region private props
        private char _alphaKey;
        private string _token;
        private string _type;
        private string _title = string.Empty;
        private string _subtitle = string.Empty;
        private bool _favorite;
        private TimeSpan _duration;
        private TimeSpan _timeWatched;
        private String _thumbnailPath = "";
        private LoadingState _thumbnailLoadingState = LoadingState.NotLoaded;
        private StorageFile _file;
        private DateTime _lastWatched;
        // TVShows related
        private int _season = -1;
        private int _episode;

        #endregion

        #region private fields

        #endregion

        #region public props
        #region tvshows related
        public int Season
        {
            get { return _season; }
            private set { SetProperty(ref _season, value); }
        }

        public int Episode
        {
            get { return _episode; }
            private set { SetProperty(ref _episode, value); }
        }
        #endregion

        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Id { get; set; }

        [Ignore]
        public String ThumbnailPath
        {
            get
            {
                if (!HasThumbnail && _thumbnailLoadingState == LoadingState.NotLoaded)
                {
                    _thumbnailLoadingState = LoadingState.Loading;
                    Task.Run(() => VideoLibraryManagement.FetchVideoThumbnailOrWaitAsync(this));
                }
                else if (HasThumbnail)
                {
                    _thumbnailPath = String.Format("ms-appdata:///local/videoPic/{0}.jpg", Id);
                }
                return _thumbnailPath;
            }
            set { SetProperty(ref _thumbnailPath, value); }
        }

        [Ignore]
        public StorageFile File
        {
            get { return _file; }
            set
            {
                SetProperty(ref _file, value);
            }
        }

        [Ignore]
        public String Token
        {
            get { return _token; }
            set { SetProperty(ref _token, value); }
        }

        [Unique]
        public string Path
        {
            get { return _filePath; }
            set { SetProperty(ref _filePath, value); }
        }

        public string Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }

        [Ignore]
        public char AlphaKey
        {
            get { return _alphaKey; }
            set { SetProperty(ref _alphaKey, value); }
        }

        public string Name
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public string Subtitle
        {
            get { return _subtitle; }
            set { SetProperty(ref _subtitle, value); }
        }

        public bool Favorite
        {
            get { return _favorite; }
            set { SetProperty(ref _favorite, value); }
        }

        public TimeSpan TimeWatched
        {
            get { return _timeWatched; }
            set
            {
                SetProperty(ref _timeWatched, value);
                OnPropertyChanged("PortionWatched");
            }
        }

        public TimeSpan Duration
        {
            get { return _duration; }
            set { SetProperty(ref _duration, value); }
        }

        [Ignore]
        public FavoriteVideoCommand FavoriteVideo { get; } = new FavoriteVideoCommand();

        public VideoProperties VideoProperties;

        public DateTime LastWatched
        {
            get { return _lastWatched; }
            set { SetProperty(ref _lastWatched, value); }
        }

        public Boolean HasThumbnail { get; set; }

        public Boolean IsCameraRoll { get; set; }

        private string _filePath;

        public bool IsTvShow
        {
            get { return _season != -1; }
        }

        public bool IsCurrentPlaying { get; set; }

        #endregion

        #region public fields

        #endregion

        #region constructors
        public VideoItem()
        {
            FavoriteVideo = new FavoriteVideoCommand();
        }

        public VideoItem(string season, string episode)
        {
            FavoriteVideo = new FavoriteVideoCommand();
            Season = int.Parse(season);
            Episode = int.Parse(episode);
        }

        public async Task Initialize(StorageFile storageFile)
        {
            if (storageFile != null)
            {
                File = storageFile;
                Name = storageFile.DisplayName;
                AlphaKey = Name.ToUpper()[0];
                Subtitle = storageFile.FileType.ToUpper() + " File";
                Type = storageFile.FileType.Replace(".", "").ToLower();
                Path = storageFile.Path;
                await GetTimeInformation();
            }
        }

        public async Task InitializeFromFilePath()
        {
            try
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(Path);
                await Initialize(file);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region methods
        private async Task GetTimeInformation()
        {
            if (VideoProperties == null)
            {
                await App.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    VideoProperties = await _file.Properties.GetVideoPropertiesAsync();
                    TimeSpan duration = VideoProperties?.Duration ?? TimeSpan.FromSeconds(0);
#if WINDOWS_PHONE_APP
                    // Absolutely totally bad workaround
                    if (duration.Seconds < 1)
                        duration = TimeSpan.FromTicks(duration.Ticks * 10000);
#endif
                    Duration = duration;
                });
            }
        }
        #endregion
    }
}
