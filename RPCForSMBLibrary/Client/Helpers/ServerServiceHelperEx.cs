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
    public class ServerServiceHelperEx
    {


        public static DateTime NetrRemoteTOD(ISMBClient client, string ServerName, out NTStatus status)
        {
            object pipeHandle;

            ISMBFileStore namedPipeShare = client.TreeConnect("IPC$", out status);
            if (namedPipeShare == null)
            {
                return DateTime.MinValue;
            }
            status = RPCClientHelper.Bind(namedPipeShare, ServerService.ServicePipeName, ServerService.ServiceInterfaceGuid, ServerService.ServiceVersion, out pipeHandle);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                namedPipeShare.Disconnect();
                return DateTime.MinValue;
            }


            NetrRemoteTODRequest netrRemoteTODRequest = new NetrRemoteTODRequest();
            netrRemoteTODRequest.ServerName = ServerName;
            
            NetrRemoteTODResponse netrRemoteTODResponse;

            status = RPCClientHelper.ExecuteCall(namedPipeShare, pipeHandle, (ushort)ServerServiceOpName.NetrRemoteTOD, netrRemoteTODRequest, out netrRemoteTODResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                namedPipeShare.Disconnect();
                return DateTime.MinValue;
            }
            namedPipeShare.CloseFile(pipeHandle);
            namedPipeShare.Disconnect();

            return netrRemoteTODResponse.TimeOfDayInfo.ToDateTime();
        }

        public static NetrServerStatisticsGetResponse NetrServerStatisticsGet(ISMBClient client, string serverName, string service, uint level, uint options, out NTStatus status)
        {
            object pipeHandle;

            ISMBFileStore namedPipeShare = client.TreeConnect("IPC$", out status);
            if (namedPipeShare == null)
            {
                return null;
            }
            status = RPCClientHelper.Bind(namedPipeShare, ServerService.ServicePipeName, ServerService.ServiceInterfaceGuid, ServerService.ServiceVersion, out pipeHandle);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                namedPipeShare.Disconnect();
                return null;
            }


            NetrServerStatisticsGetRequest netrServerStatisticsGetRequest = new NetrServerStatisticsGetRequest();
            netrServerStatisticsGetRequest.ServerName = serverName;
            netrServerStatisticsGetRequest.Service = service;
            netrServerStatisticsGetRequest.Level = level;
            netrServerStatisticsGetRequest.Options = options;

            NetrServerStatisticsGetResponse netrServerStatisticsGetResponse;

            status = RPCClientHelper.ExecuteCall(namedPipeShare, pipeHandle, (ushort)ServerServiceOpName.NetrServerStatisticsGet, netrServerStatisticsGetRequest, out netrServerStatisticsGetResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                namedPipeShare.Disconnect();
                return null;
            }
            namedPipeShare.CloseFile(pipeHandle);
            namedPipeShare.Disconnect();

            return netrServerStatisticsGetResponse;
        }

    }
}

