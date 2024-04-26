using BarcodeScanner.Mobile;

namespace MauiVisionScanner
{
    internal class BarcodeItem
    {
        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public BarcodeResult BarcodeResult { get; set; }

        public string FontIconText
        {
            get
            {
                if (IsFood)
                {
                    return "\U000F04A6";
                }
                else if (IsLink)
                {
                    return "\U000F0337";
                }
                else if (IsNumber)
                {
                    return "\U000F03A0";
                }
                return "\U000F0071";
            }
        }

        public bool IsLink
        {
            get
            {
                if (string.IsNullOrWhiteSpace(BarcodeResult.DisplayValue)) { return false; }

                Uri uriResult;
                bool result = Uri.TryCreate(BarcodeResult.DisplayValue, UriKind.Absolute, out uriResult)
                              && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
                return result;
            }
        }

        public bool IsFood
        {
            get
            {
                var daruma = "https://qrty.io/f5V_xa";
                var coreano = "https://qrco.de/beCQMF";
                if (!string.IsNullOrWhiteSpace(BarcodeResult.DisplayValue) && (BarcodeResult.DisplayValue.Equals(daruma) || BarcodeResult.DisplayValue.Equals(coreano)))
                {
                    return true;
                }

                return false;
            }
        }

        public bool IsNumber
        {
            get
            {
                var value = BarcodeResult.DisplayValue;
                var isNumberLong = long.TryParse(value, out _);
                var isNumberDouble = double.TryParse(value, out _);
                return isNumberLong || isNumberDouble;
            }
        }
    }
}
