﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplainerProd.QR
{
    using global::ZXing;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Storage.Streams;
    using static ZXing.RGBLuminanceSource;

    public static class ZXingQRCodeDecoder
    {
        static BarcodeReader barcodeReader;

        static ZXingQRCodeDecoder()
        {
            barcodeReader = new BarcodeReader();
            barcodeReader.Options.PureBarcode = false;
            barcodeReader.Options.Hints.Add(DecodeHintType.TRY_HARDER, true);
            barcodeReader.Options.PossibleFormats =
              new BarcodeFormat[] { BarcodeFormat.QR_CODE };

            barcodeReader.Options.TryHarder = true;
        }
        public static Result DecodeBufferToQRCode(
          byte[] buffer,
          int width,
          int height,
          BitmapFormat bitmapFormat)
        {
            var zxingResult = barcodeReader.Decode(
              buffer,
              width,
              height,
              bitmapFormat);

            return (zxingResult);
        }
        public static Result DecodeBufferToQRCode(
          IBuffer buffer,
          int width,
          int height,
          BitmapFormat bitmapFormat)
        {
            return (DecodeBufferToQRCode(buffer.ToArray(), width, height, bitmapFormat));
        }
    }
}
