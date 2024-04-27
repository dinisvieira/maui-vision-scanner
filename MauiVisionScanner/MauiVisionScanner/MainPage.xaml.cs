using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace MauiVisionScanner;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        NavigationPage.SetHasNavigationBar(this, false);

        InitializeComponent();
    }

    private void ScanQRCodeButton_OnClicked(object? sender, EventArgs e)
    {
        Navigation.PushModalAsync(new ScannerPage());
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        BarcodeScanner.Mobile.Methods.AskForRequiredPermission();

        base.OnNavigatedTo(args);

        _ = RefreshBarcodes();
    }

    private async Task RefreshBarcodes()
    {
        var currentScanResultsStr = Preferences.Default.Get("ScanResults", string.Empty);
        List<BarcodeItem> currentScanResults = new List<BarcodeItem>();
        if (!string.IsNullOrWhiteSpace(currentScanResultsStr))
        {
            var aux = JsonConvert.DeserializeObject<List<BarcodeItem>>(currentScanResultsStr);
            if (aux != null)
            {
                aux = aux.OrderByDescending(x => x.DateTime).ToList();
                BarcodeCollection.ItemsSource = new ObservableCollection<BarcodeItem>(aux);
            }
            else
            {
                BarcodeCollection.ItemsSource = new ObservableCollection<BarcodeItem>();
            }
        }
        else
        {
            BarcodeCollection.ItemsSource = new ObservableCollection<BarcodeItem>();
        }

        var lastScanResultStr = Preferences.Default.Get("LastScanResult", string.Empty);
        Preferences.Default.Remove("LastScanResult");
        if (!string.IsNullOrWhiteSpace(lastScanResultStr))
        {
            var aux = JsonConvert.DeserializeObject<BarcodeItem>(lastScanResultStr);
            if (aux != null)
            {
                await ShowActions(aux);
            }
        }
    }

    private async Task ShowActions(BarcodeItem aux)
    {
        var openAction = "-> Abrir Link <-";
        var copyAction = "Copiar";
        var shareAction = "Partilhar";
        var deleteAction = "Apagar";

        string action = string.Empty;
        if (aux.IsFood)
        {
            action = await DisplayActionSheet(aux.Title, "Cancelar", deleteAction, openAction, copyAction, shareAction);
        }
        else if (aux.IsLink)
        {
            action = await DisplayActionSheet($"Link: {aux.BarcodeResult.DisplayValue}", "Cancelar", deleteAction, openAction, copyAction, shareAction);
        }
        else
        {
            action = await DisplayActionSheet($"{aux.BarcodeResult.BarcodeType}: {aux.BarcodeResult.DisplayValue}", "Cancelar", deleteAction, copyAction, shareAction);
        }

        if (action == openAction)
        {
            await Browser.Default.OpenAsync(aux.BarcodeResult.DisplayValue, BrowserLaunchMode.SystemPreferred);
        }
        else if (action == copyAction)
        {
            await Clipboard.Default.SetTextAsync(aux.BarcodeResult.DisplayValue);
        }
        else if (action == shareAction)
        {
            var title = aux.IsFood ? "Menu" : aux.BarcodeResult.BarcodeType.ToString();
            if (aux.IsLink)
            {
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = aux.BarcodeResult.DisplayValue,
                    Title = title
                });
            }
            else
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = aux.BarcodeResult.DisplayValue,
                    Title = title
                });
            }
        }
        else if (action == deleteAction)
        {
            var currentScanResultsStr = Preferences.Default.Get("ScanResults", string.Empty);
            if (!string.IsNullOrWhiteSpace(currentScanResultsStr))
            {
                var auxList = JsonConvert.DeserializeObject<List<BarcodeItem>>(currentScanResultsStr);
                if (auxList != null && auxList.Any())
                {
                    var toDelete = auxList.FirstOrDefault(i => i.Id == aux.Id);
                    if (toDelete != null)
                    {
                        auxList.Remove(toDelete);
                        var listJsonToSave = JsonConvert.SerializeObject(auxList);
                        Preferences.Default.Set("ScanResults", listJsonToSave);
                        _ = RefreshBarcodes();
                    }
                }
            }
        }
    }

    private async void BarcodeCollection_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var currentSelection = e.CurrentSelection;
        if (currentSelection != null && currentSelection.Any())
        {
            if (currentSelection.FirstOrDefault() is BarcodeItem barcodeItem)
            {
                await ShowActions(barcodeItem);
            }
        }

        BarcodeCollection.SelectedItem = null;
    }
}