﻿using System;
using System.Threading.Tasks;
using DiscordChatExporter.Gui.Services;
using DiscordChatExporter.Gui.Utils;
using DiscordChatExporter.Gui.ViewModels.Components;
using DiscordChatExporter.Gui.ViewModels.Dialogs;
using DiscordChatExporter.Gui.ViewModels.Messages;
using DiscordChatExporter.Gui.ViewModels.Framework;
using MaterialDesignThemes.Wpf;
using Stylet;

namespace DiscordChatExporter.Gui.ViewModels;

public class RootViewModel : Screen, IHandle<NotificationMessage>, IDisposable
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly DialogManager _dialogManager;
    private readonly SettingsService _settingsService;
    private readonly UpdateService _updateService;
    
    public SnackbarMessageQueue Notifications { get; } = new(TimeSpan.FromSeconds(5));
    
    public DashboardViewModel Dashboard { get; }

    public RootViewModel(
        IViewModelFactory viewModelFactory,
        IEventAggregator eventAggregator,
        DialogManager dialogManager,
        SettingsService settingsService,
        UpdateService updateService)
    {
        _viewModelFactory = viewModelFactory;
        _dialogManager = dialogManager;
        _settingsService = settingsService;
        _updateService = updateService;
        
        eventAggregator.Subscribe(this);
        
        Dashboard = _viewModelFactory.CreateDashboardViewModel();

        DisplayName = $"{App.Name} v{App.VersionString}";
    }

    private async Task ShowWarInUkraineMessageAsync()
    {
        var dialog = _viewModelFactory.CreateMessageBoxViewModel(
            "Ukraine is at war!", @"
My country, Ukraine, has been invaded by Russian military forces in an act of aggression that can only be described as genocide.
Be on the right side of history! Consider supporting Ukraine in its fight for freedom.

Press LEARN MORE to find ways that you can help.".Trim(),
            "LEARN MORE", "CLOSE"
        );

        if (await _dialogManager.ShowDialogAsync(dialog) == true)
        {
            ProcessEx.StartShellExecute("https://tyrrrz.me");
        }
    }
    
    private async ValueTask CheckForUpdatesAsync()
    {
        try
        {
            var updateVersion = await _updateService.CheckForUpdatesAsync();
            if (updateVersion is null)
                return;

            Notifications.Enqueue($"Downloading update to {App.Name} v{updateVersion}...");
            await _updateService.PrepareUpdateAsync(updateVersion);

            Notifications.Enqueue(
                "Update has been downloaded and will be installed when you exit",
                "INSTALL NOW", () =>
                {
                    _updateService.FinalizeUpdate(true);
                    RequestClose();
                }
            );
        }
        catch
        {
            // Failure to update shouldn't crash the application
            Notifications.Enqueue("Failed to perform application update");
        }
    }
    
    public async void OnViewFullyLoaded()
    {
        await ShowWarInUkraineMessageAsync();
        await CheckForUpdatesAsync();
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        _settingsService.Load();

        if (_settingsService.IsDarkModeEnabled)
        {
            App.SetDarkTheme();
        }
        else
        {
            App.SetLightTheme();
        }
    }

    protected override void OnClose()
    {
        base.OnClose();

        _settingsService.Save();
        _updateService.FinalizeUpdate(false);
    }

    public void Handle(NotificationMessage message) => 
        Notifications.Enqueue(message.Text);

    public void Dispose() => Notifications.Dispose();
}