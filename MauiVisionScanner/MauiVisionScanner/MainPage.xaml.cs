using BarcodeScanner.Mobile;

namespace MauiVisionScanner
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
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
                    Camera.IsScanning = false;
                    await DisplayAlert("Result", resultValue, "OK");
                    Camera.IsScanning = true;
                }
            });
        }

        protected override void OnAppearing()
        {
            DeviceDisplay.KeepScreenOn = true;
            BarcodeScanner.Mobile.Methods.AskForRequiredPermission();

            Camera.IsScanning = true;

            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            DeviceDisplay.KeepScreenOn = false;

            base.OnDisappearing();
        }
    }
}