﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using VLC_WINRT_APP.Helpers;

namespace VLC_WINRT_APP.Model.Video
{
    public enum TemplateSize
    {
        Normal,
        Compact
    }
    public static class TemplateSizer
    {
#if WINDOWS_PHONE_APP
        public static void ComputeCompactVideo(ItemsWrapGrid wrapGrid)
        {
            var width = Window.Current.Bounds.Width;
            var splitScreen = 2;
            if (!DisplayHelper.IsPortrait())
                splitScreen = 4;
            var itemWidth = (width / splitScreen);
            //var itemHeight = (itemWidth*1.33) - 40;
            var itemHeight = (itemWidth);
            wrapGrid.ItemWidth = itemWidth - 15;
            wrapGrid.ItemHeight = itemHeight;
        }

        public static void ComputeAlbums(ItemsWrapGrid wrapGrid, TemplateSize size = TemplateSize.Compact)
        {
            var width = Window.Current.Bounds.Width;
            var splitScreen = (size == TemplateSize.Compact) ? 3 : 2;
            if (!DisplayHelper.IsPortrait())
                splitScreen = 5;
            var itemWidth = (width / splitScreen);
            var itemHeight = itemWidth*1.33 - 11;
            wrapGrid.ItemWidth = itemWidth - 12;
            wrapGrid.ItemHeight = itemHeight;
        }
#endif
    }
}
