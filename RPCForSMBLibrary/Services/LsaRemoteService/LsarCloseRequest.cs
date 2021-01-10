﻿/* Copyright (C) 2021 Vincent LE TOUX <vincent.letoux@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Text;
using SMBLibrary.RPC;
using SMBLibrary.Services;
using Utilities;

/// <summary>
/// LsarClose Request (opnum 0)
/// </summary>
public class LsarCloseRequest : IRPCRequest
{
    public LsaHandle handle;

    public byte[] GetBytes()
    {
        NDRWriter writer = new NDRWriter();
        writer.WriteStructure(handle);
        return writer.GetBytes();
    }
}