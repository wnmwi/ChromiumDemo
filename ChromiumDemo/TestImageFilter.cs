using CefSharp;
using System;
using System.Collections.Generic;
using System.IO;

namespace ChromiumDemo
{
    class TestImageFilter : IResponseFilter
    {
        public event Action<byte[]> NotifyData;
        private int contentLength = 0;
        private List<byte> dataAll = new List<byte>();

        public void SetContentLength(int contentLength)
        {
            this.contentLength = contentLength;
        }

        public FilterStatus Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            try
            {
                if (dataIn == null)
                {
                    dataInRead = 0;
                    dataOutWritten = 0;

                    return FilterStatus.Done;
                }

                dataInRead = dataIn.Length;
                dataOutWritten = Math.Min(dataInRead, dataOut.Length);

                dataIn.CopyTo(dataOut);
                dataIn.Seek(0, SeekOrigin.Begin);
                byte[] bs = new byte[dataIn.Length];
                dataIn.Read(bs, 0, bs.Length);
                dataAll.AddRange(bs);

                if (dataAll.Count == this.contentLength)
                {
                    // 通过这里进行通知  
                    NotifyData(dataAll.ToArray());

                    return FilterStatus.Done;
                }
                else if (dataAll.Count < this.contentLength)
                {
                    dataInRead = dataIn.Length;
                    dataOutWritten = dataIn.Length;

                    return FilterStatus.NeedMoreData;
                }
                else
                {
                    return FilterStatus.Error;
                }
            }
            catch (Exception ex)
            {
                dataInRead = dataIn.Length;
                dataOutWritten = dataIn.Length;

                return FilterStatus.Done;
            }
        }

        public bool InitFilter()
        {
            return true;
        }

        public void Dispose()
        {
            dataAll.Clear();
        }
    }
}
