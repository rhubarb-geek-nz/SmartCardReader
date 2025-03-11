// Copyright (c) 2024 Roger Brown.
// Licensed under the MIT License.

using System.ComponentModel;
using System.Management.Automation;

namespace RhubarbGeekNz.PowerShell.SmartCardReader
{
    [Cmdlet(VerbsCommon.Open, "SmartCardReader")]
    [OutputType(typeof(Reader))]
    public class OpenSmartCardReader : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(new Reader());
        }
    }

    [Cmdlet(VerbsCommon.Get, "SmartCardReader")]
    [OutputType(typeof(string))]
    public class GetSmartCardReader : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public Reader Reader { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                string[] list = Reader.GetList();
                foreach (string name in list)
                {
                    WriteObject(name);
                }
            }
            catch (Win32Exception)
            {
            }
        }
    }

    [Cmdlet(VerbsCommon.Close, "SmartCardReader")]
    [OutputType(typeof(void))]
    public class CloseSmartCardReader : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public Reader Reader { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Reader.Dispose();
            }
            catch (Win32Exception ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.NotSpecified, null));
            }
        }
    }

    [Cmdlet(VerbsCommunications.Disconnect, "SmartCardReader")]
    [OutputType(typeof(void))]
    public class DisconnectSmartCardReader : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public Reader Reader { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Reader.Disconnect();
            }
            catch (Win32Exception ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.NotSpecified, null));
            }
        }
    }

    [Cmdlet(VerbsCommunications.Connect, "SmartCardReader")]
    [OutputType(typeof(byte[]))]
    public class ConnectSmartCardReader : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public Reader Reader { get; set; }
        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                Reader.Connect(Name);
                WriteObject(Reader.GetATR());
            }
            catch (Win32Exception ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.NotSpecified, null));
            }
        }
    }

    [Cmdlet(VerbsLifecycle.Invoke, "SmartCardReader")]
    [OutputType(typeof(byte[]))]
    public class InvokeSmartCardReader : PSCmdlet
    {
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public Reader Reader { get; set; }
        [Parameter(
            Mandatory = true,
            Position = 1,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true)]
        public byte[] Command { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                WriteObject(Reader.Transceive(Command));
            }
            catch (Win32Exception ex)
            {
                WriteError(new ErrorRecord(ex, ex.GetType().Name, ErrorCategory.NotSpecified, null));
            }
        }
    }
}
