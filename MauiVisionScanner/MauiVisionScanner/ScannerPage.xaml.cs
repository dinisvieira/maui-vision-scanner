using BarcodeScanner.Mobile;
using Newtonsoft.Json;

namespace MauiVisionScanner
{
    public partial class ScannerPage : ContentPage
    {
        public ScannerPage()
        {
            InitializeComponent();

            BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeFormats.All);
        }

        private void FlashButton_OnClicked(object? sender, EventArgs e)
        {
            Camera.TorchOn = !Camera.TorchOn;
        }

        private async void Camera_OnOnDetected(object? sender, OnDetectedEventArg eventArgs)
        {
            if (eventArgs == null || eventArgs.BarcodeResults == null || eventArgs.BarcodeResults.Count <= 0) { return; }

            List<BarcodeResult> barcodeResults = eventArgs.BarcodeResults;

            var firstScannerResult = barcodeResults.FirstOrDefault();
            if (firstScannerResult == null || string.IsNullOrWhiteSpace(firstScannerResult.DisplayValue)) { return; }
            string resultValue = firstScannerResult.DisplayValue;

            string resultAux = string.Empty;
            foreach (var barcodeResult in barcodeResults)
            {
                resultAux += $"Type : {barcodeResult.BarcodeType}, Value : {barcodeResult.DisplayValue}\n";
            }

            System.Diagnostics.Debug.WriteLine("All Scanner Results (only first is used):\n");
            System.Diagnostics.Debug.WriteLine(resultAux);

            await MainThread.InvokeOnMainThreadAsync(async() =>
            {
                if (resultValue != null && !string.IsNullOrWhiteSpace(resultValue))
                {
                    var currentScanResultsStr = Preferences.Default.Get("ScanResults", string.Empty);
                    List<BarcodeItem> currentScanResults = new List<BarcodeItem>();
                    if (!string.IsNullOrWhiteSpace(currentScanResultsStr))
                    {
                        var aux = JsonConvert.DeserializeObject<List<BarcodeItem>>(currentScanResultsStr);
                        if (aux != null)
                        {
                            currentScanResults = aux;
                        }
                    }

                    var newBarcodeItem = new BarcodeItem()
                    {
                        Id = Guid.NewGuid(),
                        DateTime = DateTime.Now,
                        BarcodeResult = firstScannerResult
                    };
                    currentScanResults.Add(newBarcodeItem);

                    var itemJsonToSave = JsonConvert.SerializeObject(newBarcodeItem);
                    var listJsonToSave = JsonConvert.SerializeObject(currentScanResults);

                    Preferences.Default.Set("ScanResults", listJsonToSave);
                    Preferences.Default.Set("LastScanResult", itemJsonToSave);

                    Camera.IsScanning = false;
                    Camera.TorchOn = false;
                    DeviceDisplay.KeepScreenOn = false;
                    await Navigation.PopModalAsync(true);
                }
            });
        }

        protected override void OnAppearing()
        {
            BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
            DeviceDisplay.KeepScreenOn = true;
            Camera.IsScanning = true;

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            Camera.IsScanning = false;
            Camera.TorchOn = false;
            DeviceDisplay.KeepScreenOn = false;

            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed()
        {
            Camera.IsScanning = false;
            Camera.TorchOn = false;
            DeviceDisplay.KeepScreenOn = false;

            return base.OnBackButtonPressed();
        }
    }
}