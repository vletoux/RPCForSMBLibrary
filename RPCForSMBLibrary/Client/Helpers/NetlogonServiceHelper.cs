/* Copyright (C) 2021 Vincent LE TOUX <vincent.letoux@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using SMBLibrary.RPC;
using SMBLibrary.Services;
using Utilities;

namespace SMBLibrary.Client
{
    public class NetlogonServiceHelper
    {


        public static object DsGetDCNames(ISMBClient client, string ServerName, string DomainName, string SiteName, uint Flags, out NTStatus status)
        {
            object pipeHandle;

            ISMBFileStore namedPipeShare = client.TreeConnect("IPC$", out status);
            if (namedPipeShare == null)
            {
                return null;
            }
            status = RPCClientHelper.Bind(namedPipeShare, NetlogonService.ServicePipeName, NetlogonService.ServiceInterfaceGuid, NetlogonService.ServiceVersion, out pipeHandle);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                namedPipeShare.Disconnect();
                return null;
            }


            DsrGetDcNameRequest getDcNameRequest = new DsrGetDcNameRequest();
            getDcNameRequest.ServerName = ServerName;
            getDcNameRequest.DomainName = DomainName;
            getDcNameRequest.SiteName = SiteName;
            getDcNameRequest.Flags = Flags;

            DsrGetDcNameResponse getDcNameResponse;

            status = RPCClientHelper.ExecuteCall(namedPipeShare, pipeHandle, (ushort)NetlogonServiceOpName.DsrGetDcName, getDcNameRequest, out getDcNameResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                namedPipeShare.Disconnect();
                return null;
            }
            namedPipeShare.CloseFile(pipeHandle);
            namedPipeShare.Disconnect();

            return new DomainControllerInfo(getDcNameResponse.DCInfo);
        }

    }
}

