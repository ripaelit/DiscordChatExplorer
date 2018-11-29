﻿using System.IO;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Stylet;

namespace DiscordChatExporter.Gui.ViewModels.Framework
{
    public class DialogManager
    {
        private readonly IViewManager _viewManager;

        public DialogManager(IViewManager viewManager)
        {
            _viewManager = viewManager;
        }

        public async Task<T> ShowDialogAsync<T>(DialogScreen<T> dialogScreen)
        {
            // Get the view that renders this viewmodel
            var view = _viewManager.CreateAndBindViewForModelIfNecessary(dialogScreen);

            // Set up event routing that will close the view when called from viewmodel
            DialogOpenedEventHandler onDialogOpened = (sender, e) =>
            {
                // Delegate to close the dialog and unregister event handler
                void OnScreenClosed(object o, CloseEventArgs args)
                {
                    e.Session.Close();
                    dialogScreen.Closed -= OnScreenClosed;
                }

                dialogScreen.Closed += OnScreenClosed;
            };

            // Show view
            await DialogHost.Show(view, onDialogOpened);

            // Return the result
            return dialogScreen.DialogResult;
        }

        public string PromptSaveFilePath(string filter = "All files|*.*", string initialFilePath = "")
        {
            // Create dialog
            var dialog = new SaveFileDialog
            {
                Filter = filter,
                AddExtension = true,
                FileName = initialFilePath,
                DefaultExt = Path.GetExtension(initialFilePath) ?? ""
            };

            // Show dialog and return result
            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }
    }
}