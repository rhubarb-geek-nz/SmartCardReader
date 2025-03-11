// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;

namespace RhubarbGeekNz.PowerShell.SmartCardReader
{
    internal class WinSCard
    {
        internal const int
            SCARD_E_TIMEOUT = -2146435062;

        internal const UInt32
            SCARD_STATE_UNAWARE = 0,
            SCARD_STATE_IGNORE = 1,
            SCARD_STATE_CHANGED = 2,
            SCARD_STATE_UNKNOWN = 4,
            SCARD_STATE_UNAVAILABLE = 8,
            SCARD_STATE_EMPTY = 0x10,
            SCARD_STATE_PRESENT = 0x20,
            SCARD_STATE_ATRMATCH = 0x40,
            SCARD_STATE_EXCLUSIVE = 0x80,
            SCARD_STATE_INUSE = 0x100,
            SCARD_STATE_MUTE = 0x200,
            SCARD_STATE_UNPOWERED = 0x400;

        internal const UInt32
            SCARD_SCOPE_USER = 0,
            SCARD_SCOPE_TERMINAL = 1,
            SCARD_SCOPE_SYSTEM = 2;

        internal const UInt32
            SCARD_SHARE_EXCLUSIVE = 1,
            SCARD_SHARE_SHARED = 2,
            SCARD_SHARE_DIRECT = 3;

        internal const UInt32
            SCARD_LEAVE_CARD = 0,
            SCARD_RESET_CARD = 1,
            SCARD_UNPOWER_CARD = 2,
            SCARD_EJECT_CARD = 3;

        [DllImport("WinSCard.dll")]
        internal static extern int SCardEstablishContext(
            uint dwScope,
            IntPtr notUsed1,
            IntPtr notUsed2,
            out IntPtr phContext);

        [DllImport("WinSCard.dll")]
        internal static extern int SCardReleaseContext(
            IntPtr phContext);

        [DllImport("WinSCard.dll")]
        internal static extern int SCardConnect(
            IntPtr hContext,
            string cReaderName,
            uint dwShareMode,
            uint dwPrefProtocol,
            ref IntPtr phCard,
            ref IntPtr ActiveProtocol);

        [DllImport("WinSCard.dll")]
        internal static extern int SCardDisconnect(
            IntPtr hCard,
            uint Disposition);

        [DllImport("WinSCard.dll", EntryPoint = "SCardListReadersW", CharSet = CharSet.Unicode)]
        internal static extern int SCardListReaders(
            IntPtr hContext,
            char[] mszGroups,
            char[] mszReaders,
            ref UInt32 pcchReaders
            );

        [StructLayout(LayoutKind.Sequential)]
        internal struct SCARD_IO_REQUEST
        {
            internal int dwProtocol;
            internal int cbPciLength;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct SCARD_READERSTATE
        {
            internal string szReader;
            internal IntPtr pvUserData;
            internal UInt32 dwCurrentState;
            internal UInt32 dwEventState;
            internal UInt32 cbAtr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
            internal byte[] m_rgbAtr;
        }

        [DllImport("WinSCard.dll", EntryPoint = "SCardGetStatusChangeW", CharSet = CharSet.Unicode)]
        static extern int SCardGetStatusChange(
            IntPtr hContext,
            UInt32 dwTimeout,
            [In, Out] SCARD_READERSTATE[] rgReaderStates,
            UInt32 cReaders);

        [DllImport("WinSCard.dll")]
        internal static extern int SCardTransmit(IntPtr hCard,
            [In] ref SCARD_IO_REQUEST pioSendPci,
            byte[] pbSendBuffer,
            UInt32 cbSendLength,
            IntPtr pioRecvPci,
            [Out] byte[] pbRecvBuffer,
            ref UInt32 pcbRecvLength);

        [DllImport("WinSCard.dll", EntryPoint = "SCardStatusW", CharSet = CharSet.Unicode)]
        internal static extern int SCardStatus(IntPtr hCard,
            [Out] char[] szReaderName,
            ref UInt32 pcchReaderLen,
            out UInt32 pdwState,
            out UInt32 pdwProtocol,
            [Out] byte[] pbAtr,
            ref UInt32 pcbAtrLen);
    }
}
