/* Copyright (C) 2021 Vincent LE TOUX <vincent.letoux@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMBLibrary.RPC;
using SMBLibrary.Services;
using Utilities;

namespace SMBLibrary.Tests
{
    [TestClass]
    public class EFSServiceTests
    {

        [TestMethod]
        public void TestEfsRpcOpenFileRawResponse()
        {
            byte[] buffer = new byte[] {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x35, 0x00, 0x00, 0x00 };


            EfsRpcOpenFileRawResponse response = new EfsRpcOpenFileRawResponse(buffer);

            Assert.IsTrue(response.Return == 0x35);

        }



        public void TestAll()
        {
            TestEfsRpcOpenFileRawResponse();
        }
    }
}
