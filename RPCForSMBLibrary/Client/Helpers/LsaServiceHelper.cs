/* Copyright (C) 2021 Vincent LE TOUX <vincent.letoux@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using SMBLibrary.Client.Helpers;
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
            using (RPCCallHelper rpc = new RPCCallHelper(client, LsaRemoteService.ServicePipeName, LsaRemoteService.ServiceInterfaceGuid, LsaRemoteService.ServiceVersion))
            {
                status = rpc.BindPipe();
                if (status != NTStatus.STATUS_SUCCESS)
                    return null;

                LsaHandle handle = LsaOpenPolicy(rpc, (AccessMask)0x801, out status);
                if (handle != null)
                {

                    output = LsaLookupSids(rpc, handle, sids, out status);

                    LsaClose(rpc, handle, out status);
                }
            }
            return output;
        }

        public static List<SID> ResolveNames(ISMBClient client, List<string> names, out NTStatus status)
        {
            List<SID> output = null;
            using (RPCCallHelper rpc = new RPCCallHelper(client, LsaRemoteService.ServicePipeName, LsaRemoteService.ServiceInterfaceGuid, LsaRemoteService.ServiceVersion))
            {
                status = rpc.BindPipe();
                if (status != NTStatus.STATUS_SUCCESS)
                    return null;

                LsaHandle handle = LsaOpenPolicy(rpc, (AccessMask)0x801, out status);
                if (handle != null)
                {

                    output = LsaLookupNames(rpc, handle, names, out status);

                    LsaClose(rpc, handle, out status);
                }
            }
            return output;
        }


        public static LsaHandle LsaOpenPolicy(RPCCallHelper rpc, AccessMask desiredAccess, out NTStatus status)
        {
            LsarOpenPolicyRequest openPolicyRequest = new LsarOpenPolicyRequest();
            openPolicyRequest.DesiredAccess = desiredAccess;

            LsarOpenPolicyResponse openPolicyResponse;

            status = rpc.ExecuteCall((ushort)LsaRemoteServiceOpName.LsarOpenPolicy, openPolicyRequest, out openPolicyResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                return null;
            }
            return openPolicyResponse.PolicyHandle;
        }

        public static void LsaClose(RPCCallHelper rpc, LsaHandle handle, out NTStatus status)
        {
            LsarCloseRequest closeRequest = new LsarCloseRequest();
            closeRequest.handle = handle;

            LsarCloseResponse closeResponse;

            status = rpc.ExecuteCall((ushort)LsaRemoteServiceOpName.LsarClose, closeRequest, out closeResponse);
            if (status != NTStatus.STATUS_SUCCESS)
            {
                return;
            }

        }

        public static List<string> LsaLookupSids(RPCCallHelper rpc, LsaHandle handle, List<SID> sids, out NTStatus status)
        {

            LsarLookupSidsRequest lookupSidsRequest = new LsarLookupSidsRequest();
            lookupSidsRequest.handle = handle;
            lookupSidsRequest.SIDEnumBuffer = new LsaSIDEnumBuffer();
            lookupSidsRequest.SIDEnumBuffer.Entries = (uint) sids.Count;
            lookupSidsRequest.SIDEnumBuffer.SIDInfos = new LsaSIDArray();
            lookupSidsRequest.SIDEnumBuffer.SIDInfos.SIDs = sids;
            lookupSidsRequest.TranslatedNames = new LsaTranslatedArray<LsaTranslatedName>();

            LsarLookupSidsResponse lookupSidsResponse;

            status = rpc.ExecuteCall((ushort)LsaRemoteServiceOpName.LsarLookupSids, lookupSidsRequest, out lookupSidsResponse);
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

        public static List<SID> LsaLookupNames(RPCCallHelper rpc, LsaHandle handle, List<string> names, out NTStatus status)
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

            status = rpc.ExecuteCall((ushort)LsaRemoteServiceOpName.LsarLookupNames, lookupNamesRequest, out lookupNamesResponse);
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
                    output.Add(sid.GetSID(lookupNamesResponse.DomainList.Names[(int)sid.DomainIndex].Sid));
            }
            return output;
        }
    }
}
