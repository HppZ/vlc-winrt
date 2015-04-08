﻿
using VLC_WinRT.Utils;
using VLC_WinRT.ViewModels;

namespace VLC_WinRT.Commands.Music
{
    public class StartMusicIndexingCommand : AlwaysExecutableCommand
    {
        public override async void Execute(object parameter)
        {
            await Locator.MusicLibraryVM.PerformRoutineCheckIfNotBusy();
        }
    }
}
