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
                if (IsSushi)
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

        public bool IsSushi
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(BarcodeResult.DisplayValue) && BarcodeResult.DisplayValue.Equals("https://qrty.io/f5V_xa"))
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
                return int.TryParse("123", out _);
            }
        }
    }
}
