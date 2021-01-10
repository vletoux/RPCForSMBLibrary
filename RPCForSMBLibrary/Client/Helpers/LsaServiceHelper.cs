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
    public class LsaServiceHelper
    {

        public static List<string> ResolveSIDs(ISMBClient client, List<SID> sids, out NTStatus status)
        {
            List<string> output = null;
            ISMBFileStore namedPipeShare = client.TreeConnect("IPC$", out status);
            if (namedPipeShare == null)
            {
                return null;
            }
            object pipeHandle;
            LsaHandle handle = LsaOpenPolicy(namedPipeShare, (AccessMask)0x801, out pipeHandle, out status);
            if (handle != null)
            {

                output = LsaLookupSids(namedPipeShare, pipeHandle, handle, sids, out status);

                LsaClose(namedPipeShare, pipeHandle, handle, out status);
            }
            namedPipeShare.Disconnect();
            return output;
        }

        public static List<SID> ResolveNames(ISMBClient client, List<string> names, out NTStatus status)
        {
            List<SID> output = null;
            ISMBFileStore namedPipeShare = client.TreeConnect("IPC$", out status);
            if (namedPipeShare == null)
            {
                return null;
            }
            object pipeHandle;
            LsaHandle handle = LsaOpenPolicy(namedPipeShare, (AccessMask)0x801, out pipeHandle, out status);
            if (handle != null)
            {

                output = LsaLookupNames(namedPipeShare, pipeHandle, handle, names, out status);

                LsaClose(namedPipeShare, pipeHandle, handle, out status);
            }
            namedPipeShare.Disconnect();
            return output;
        }


        public static LsaHandle LsaOpenPolicy(INTFileStore namedPipeShare, AccessMask desiredAccess, out object pipeHandle, out NTStatus status)
        {
            status = RPCClientHelper.Bind(namedPipeShare, LsaRemoteService.ServicePipeName, LsaRemoteService.ServiceInterfaceGuid, LsaRemoteService.ServiceVersion, out pipeHandle);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                return null;
            }


            LsarOpenPolicyRequest openPolicyRequest = new LsarOpenPolicyRequest();
            openPolicyRequest.DesiredAccess = desiredAccess;

            LsarOpenPolicyResponse openPolicyResponse;

            status = RPCClientHelper.ExecuteCall(namedPipeShare, pipeHandle, (ushort)LsaRemoteServiceOpName.LsarOpenPolicy, openPolicyRequest, out openPolicyResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                namedPipeShare.CloseFile(pipeHandle);
                return null;
            }
            return openPolicyResponse.PolicyHandle;
        }

        public static void LsaClose(INTFileStore namedPipeShare, object pipeHandle, LsaHandle handle, out NTStatus status)
        {
            LsarCloseRequest closeRequest = new LsarCloseRequest();
            closeRequest.handle = handle;

            LsarCloseResponse closeResponse;

            status = RPCClientHelper.ExecuteCall(namedPipeShare, pipeHandle, (ushort)LsaRemoteServiceOpName.LsarClose, closeRequest, out closeResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                return;
            }

        }

        public static List<string> LsaLookupSids(INTFileStore namedPipeShare, object pipeHandle, LsaHandle handle, List<SID> sids, out NTStatus status)
        {

            LsarLookupSidsRequest lookupSidsRequest = new LsarLookupSidsRequest();
            lookupSidsRequest.handle = handle;
            lookupSidsRequest.SIDEnumBuffer = new LsaSIDEnumBuffer();
            lookupSidsRequest.SIDEnumBuffer.Entries = (uint) sids.Count;
            lookupSidsRequest.SIDEnumBuffer.SIDInfos = new LsaSIDArray();
            lookupSidsRequest.SIDEnumBuffer.SIDInfos.SIDs = sids;
            lookupSidsRequest.TranslatedNames = new LsaTranslatedArray<LsaTranslatedName>();

            LsarLookupSidsResponse lookupSidsResponse;

            status = RPCClientHelper.ExecuteCall(namedPipeShare, pipeHandle, (ushort)LsaRemoteServiceOpName.LsarLookupSids, lookupSidsRequest, out lookupSidsResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                return null;
            }

            if (sids.Count != lookupSidsResponse.TranslatedSids.Items.Count)
            {
                status = NTStatus.STATUS_NOT_SUPPORTED;
                return null;
            }
            List<string> output = new List<string>();
            foreach (LsaTranslatedName translated in lookupSidsResponse.TranslatedSids.Items)
            {
                if (translated.Use == LsaSIDNameUse.SidTypeUnknown)
                {
                    output.Add(null);
                }
                else
                {
                    string domain = lookupSidsResponse.DomainList.Names[(int)translated.DomainIndex].Name;
                    output.Add(domain + "\\" + translated.Name);
                }
            }
            return output;
        }

        public static List<SID> LsaLookupNames(INTFileStore namedPipeShare, object pipeHandle, LsaHandle handle, List<string> names, out NTStatus status)
        {
            LsarLookupNamesRequest lookupNamesRequest = new LsarLookupNamesRequest();
            lookupNamesRequest.handle = handle;
            lookupNamesRequest.Names = new NDRConformantArray<LsaUnicodeString>();
            foreach (string name in names)
            {
                lookupNamesRequest.Names.Add(new LsaUnicodeString(name));
            }
            lookupNamesRequest.TranslatedSids = new LsaTranslatedArray<LsaTranslatedSid>();

            LsarLookupNamesResponse lookupNamesResponse;

            status = RPCClientHelper.ExecuteCall(namedPipeShare, pipeHandle, (ushort)LsaRemoteServiceOpName.LsarLookupNames, lookupNamesRequest, out lookupNamesResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                return null;
            }
            if (names.Count != lookupNamesResponse.TranslatedNames.Items.Count)
            {
                status = NTStatus.STATUS_NOT_SUPPORTED;
                return null;
            }
            List<SID> output = new List<SID>();
            foreach (LsaTranslatedSid sid in lookupNamesResponse.TranslatedNames.Items)
            {
                if (sid.Use == LsaSIDNameUse.SidTypeUnknown)
                    output.Add(null);
                else
                    output.Add(lookupNamesResponse.DomainList.Names[(int)sid.DomainIndex].Sid.ChildSID(sid.RelativeId));
            }
            return output;
        }
    }
}
