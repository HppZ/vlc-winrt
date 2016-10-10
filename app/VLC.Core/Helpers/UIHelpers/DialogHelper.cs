﻿using System;
using System.Collections.Generic;
using System.Text;
using libVLCX;
using VLC.UI.Views.UserControls.Shell;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace VLC.Helpers.UIHelpers
{
    public static class DialogHelper
    {
        static SemaphoreSlim DialogDisplaySemaphoreSlim = new SemaphoreSlim(1);
        static VLCDialog CurrentDialog;
        private static void Dialog_Closed(Windows.UI.Xaml.Controls.ContentDialog sender, Windows.UI.Xaml.Controls.ContentDialogClosedEventArgs args)
        {
            CurrentDialog = null;
            DialogDisplaySemaphoreSlim.Release();
        }

        public static async Task DisplayDialog(string title, string desc)
        {
            await DialogDisplaySemaphoreSlim.WaitAsync();
            Debug.Assert(CurrentDialog == null);
            CurrentDialog = new VLCDialog();
            CurrentDialog.Closed += Dialog_Closed;
            CurrentDialog.Initialize(title, desc);
            await CurrentDialog.ShowAsync();
        }

        public static async Task DisplayDialog(string title, string desc, Dialog d, string username = null, bool askStore = false)
        {
            await DialogDisplaySemaphoreSlim.WaitAsync();
            Debug.Assert(CurrentDialog == null);
            CurrentDialog = new VLCDialog();
            CurrentDialog.Closed += Dialog_Closed;
            CurrentDialog.Initialize(title, desc, d, username, askStore);
            await CurrentDialog.ShowAsync();
        }

        public static async Task DisplayDialog(string title, string desc, Dialog d, Question questionType, string cancel, string action1, string action2)
        {
            if (questionType == Question.warning)
            {
                d.postAction(1);
                return;
            }

            await DialogDisplaySemaphoreSlim.WaitAsync();
            Debug.Assert(CurrentDialog == null);
            CurrentDialog = new VLCDialog();
            CurrentDialog.Closed += Dialog_Closed;
            CurrentDialog.Initialize(title, desc, d, questionType, cancel, action1, action2);
            await CurrentDialog.ShowAsync();
        }

        public static void CancelDialog(Dialog d)
        {
            d.dismiss();
            Debug.Assert(CurrentDialog != null);
            CurrentDialog.Hide();
            // Let the Dialog_Closed event cleanup afterward
        }
    }
}