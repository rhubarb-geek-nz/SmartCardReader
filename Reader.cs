// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.ComponentModel;

namespace RhubarbGeekNz.PowerShell.SmartCardReader
{
    public class Reader : IDisposable
    {
        readonly private IntPtr hContext;
        private IntPtr activeProtocol = IntPtr.Zero, hCard = IntPtr.Zero;
        private WinSCard.SCARD_IO_REQUEST ioSend = new WinSCard.SCARD_IO_REQUEST();
        private bool bContext, bCard;

        internal Reader()
        {
            int rc = WinSCard.SCardEstablishContext(WinSCard.SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, out hContext);
            if (rc != 0) throw new Win32Exception(rc);
            bContext = true;
        }

        public void Dispose()
        {
            if (bCard)
            {
                bCard = false;
                WinSCard.SCardDisconnect(hCard, WinSCard.SCARD_LEAVE_CARD);

            }

            if (bContext)
            {
                bContext = false;
                WinSCard.SCardReleaseContext(hContext);
            }
        }

        ~Reader()
        {
            Dispose();
        }

        internal string[] GetList()
        {
            UInt32 count = 0;
            int rc = WinSCard.SCardListReaders(hContext, null, null, ref count);
            if (rc != 0) throw new Win32Exception(rc);
            char[] mszReaders = new char[count];
            rc = WinSCard.SCardListReaders(hContext, null, mszReaders, ref count);
            if (rc != 0) throw new Win32Exception(rc);
            UInt32 i = 0, num = 0;

            while (i < count)
            {
                if (mszReaders[i] == 0) break;
                num++;
                while (mszReaders[i++] != 0) ;
            }

            string[] result = new string[num];
            num = 0;
            i = 0;
            while (i < count)
            {
                if (mszReaders[i] == 0) break;
                UInt32 off = i;
                while (mszReaders[i++] != 0) ;
                result[num++] = new string(mszReaders, (int)off, (int)(i - off - 1));
            }

            return result;
        }

        internal void Connect(string name)
        {
            if (bCard)
            {
                throw new InvalidOperationException();
            }

            int rc = WinSCard.SCardConnect(hContext, name, WinSCard.SCARD_SHARE_EXCLUSIVE, 3, ref hCard, ref activeProtocol);
            if (rc != 0) throw new Win32Exception(rc);
            ioSend.dwProtocol = (int)activeProtocol;
            ioSend.cbPciLength = 8;
            bCard = true;
        }

        internal void Disconnect()
        {
            if (bCard)
            {
                bCard = false;
                WinSCard.SCardDisconnect(hCard, WinSCard.SCARD_LEAVE_CARD);
            }
        }

        internal byte[] Transceive(byte[] apdu)
        {
            if (!bCard)
            {
                throw new InvalidOperationException();
            }

            byte[] result = null;
            byte[] resp = new byte[256];
            uint respLen = (uint)resp.Length;

            int rc = WinSCard.SCardTransmit(
                    hCard,
                    ref ioSend,
                    apdu,
                    (UInt32)apdu.Length,
                    IntPtr.Zero,
                    resp,
                    ref respLen);

            if (rc != 0) throw new Win32Exception(rc);

            if ((respLen == 2) && (resp[0] == 0x61))
            {
                apdu = new byte[] { 0x00, 0xC0, 0x00, 0x00, resp[1] };

                respLen = (uint)resp.Length;

                rc = WinSCard.SCardTransmit(
                        hCard,
                        ref ioSend,
                        apdu,
                        (UInt32)apdu.Length,
                        IntPtr.Zero,
                        resp,
                        ref respLen);

                if (rc != 0) throw new Win32Exception(rc);
            }

            result = new byte[respLen];

            Array.Copy(resp, 0, result, 0, result.Length);

            return result;
        }

        internal byte[] GetATR()
        {
            if (!bCard)
            {
                throw new InvalidOperationException();
            }

            byte[] pbAtr = new byte[64];
            uint pcbAtrLen = (uint)pbAtr.Length;
            uint pdwState = 0, pdwProtocol = 0, pcchReaderLen = 0;
            int ret = WinSCard.SCardStatus(hCard, null, ref pcchReaderLen, out pdwState, out pdwProtocol, pbAtr, ref pcbAtrLen);
            if (ret != 0) throw new Win32Exception(ret);
            byte[] atr = new byte[pcbAtrLen];

            Array.Copy(pbAtr, 0, atr, 0, atr.Length);

            return atr;
        }
    }
}
