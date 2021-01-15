/* Copyright (C) 2021 Vincent LE TOUX <vincent.letoux@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Text;
using SMBLibrary.Client.Helpers;
using SMBLibrary.RPC;
using SMBLibrary.Services;
using Utilities;

/// <summary>
/// LsarOpenPolicy Request (opnum 6)
/// </summary>
public class LsarOpenPolicyRequest : IRPCRequest
{
    public SMBLibrary.AccessMask DesiredAccess;
    // according to 3.1.4.4.1 LsarOpenPolicy2 (Opnum 44) this structure is not used and should be ignored
    LsaObjectAttributes ObjectAttributes;

    public LsarOpenPolicyRequest()
    {
        ObjectAttributes = new LsaObjectAttributes();
    }

    public byte[] GetBytes()
    {
        NDRWriter writer = new NDRWriter();
        writer.WriteTopLevelUnicodeStringPointer(null);
        writer.WriteStructure(ObjectAttributes);
        writer.WriteUInt32((uint) DesiredAccess);

        return writer.GetBytes();
    }
}
