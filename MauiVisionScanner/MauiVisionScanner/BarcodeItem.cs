using BarcodeScanner.Mobile;

namespace MauiVisionScanner
{
    internal class BarcodeItem
    {
        private string _daruma = "https://qrty.io/f5V_xa";
        private string _coreano = "https://qrco.de/beCQMF";

        public Guid Id { get; set; }
        public DateTime DateTime { get; set; }
        public BarcodeResult BarcodeResult { get; set; }

        public string Title
        {
            get
            {
                if (IsFood)
                {
                    if (BarcodeResult.DisplayValue == _daruma)
                    {
                        return "Menu Daruma";
                    }
                    else if (BarcodeResult.DisplayValue == _coreano)
                    {
                        return "Menu Coreano";
                    }
                    else
                    {
                        return "Menu";
                    }
                }
                else if (IsLink)
                {
                    return "Link";
                }
                else
                {
                    return BarcodeResult.BarcodeType.ToString();
                }
            }
        }

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
                if (!string.IsNullOrWhiteSpace(BarcodeResult.DisplayValue) && (BarcodeResult.DisplayValue.Equals(_daruma) || BarcodeResult.DisplayValue.Equals(_coreano)))
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
