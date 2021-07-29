# About RPCForSMBLibrary

RPCForSMBLibrary is a complement of the project https://github.com/TalAloni/SMBLibrary made by TalAloni 

SMBLibrary is an open-source C# SMB 1.0/CIFS, SMB 2.0, SMB 2.1 and SMB 3.0 server and client implementation.
SMBLibrary gives .NET developers an easy way to share a directory / file system / virtual file system, with any operating system that supports the SMB protocol.
SMBLibrary is modular, you can take advantage of Integrated Windows Authentication and the Windows storage subsystem on a Windows host or use independent implementations that allow for cross-platform compatibility.
SMBLibrary shares can be accessed from any Windows version since Windows NT 4.0.

# RPC Calls

A subset of LSA, NetLogon and EFS have been implemented.
You can for example get the SID of a user using this way:
```
var client = new SMB2Client(); // SMB1Client can be used as well
bool isConnected = client.Connect(IPAddress.Parse("192.168.0.X"), SMBTransportType.DirectTCPTransport);
if (isConnected)
{
   NTStatus status = client.Login(String.Empty, "XXX", "XXX");
   if (status == NTStatus.STATUS_SUCCESS)
   {
      var o = LsaServiceHelper.ResolveNames(client, new List<string>() { "adiant" }, out status);
   }
}
```

Or for Petit Potam:
```
var client = new SMB2Client(); // SMB1Client can be used as well
bool isConnected = client.Connect(IPAddress.Parse("192.168.0.X"), SMBTransportType.DirectTCPTransport);
if (isConnected)
{
   NTStatus status = client.Login(String.Empty, "XXX", "XXX");
   if (status == NTStatus.STATUS_SUCCESS)
   {
      using (RPCCallHelper rpc = new RPCCallHelper(client, EFSService.ServicePipeName, EFSService.ServiceInterfaceGuid, EFSService.ServiceVersion))
      {
         status = rpc.BindPipe();
         if (status != NTStatus.STATUS_SUCCESS)
            return;
         var ooo = EFSServiceHelper.EfsRpcOpenFileRaw(rpc, out handle, "\\\\192.168.0.10\\test", 0, out status);
         Console.WriteLine(ooo);
      }
   }
}
```

You can use global helpers such as:
```
NetlogonServiceHelper.DsGetDCNames(client, "192.1680.25", "test.mysmartlogon.com", null, 0, out status);
ServerServiceHelperEx.NetrRemoteTOD(client, "192.168.0.25", out status);
ServerServiceHelperEx.NetrServerStatisticsGet(client, "192.168.0.25", "LanmanWorkstation", 0, 0, out status);
```
