/* Copyright (C) 2021 Vincent LE TOUX <vincent.letoux@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SMBLibrary.Client.Helpers;
using SMBLibrary.RPC;
using SMBLibrary.Services;
using Utilities;

namespace SMBLibrary.Tests
{
    [TestClass]
    public class LsaRemoteServiceTests
    {

        [TestMethod]
        public void TestLsarLookupSidsResponse()
        {
            byte[] buffer = new byte[] {0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x02, 0x00, 0x20, 0x00, 0x00, 0x00,
                                        0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x0a, 0x00, 0x08, 0x00, 0x02, 0x00, 0x0c, 0x00, 0x02, 0x00,
                                        0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x54, 0x00, 0x45, 0x00,
                                        0x53, 0x00, 0x54, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05,
                                        0x15, 0x00, 0x00, 0x00, 0x8f, 0xa8, 0xb9, 0xee, 0x08, 0xeb, 0x59, 0xeb, 0x87, 0xfd, 0xc8, 0x97,
                                        0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                        0x0c, 0x00, 0x0c, 0x00, 0x14, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00,
                                        0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0x41, 0x00, 0x64, 0x00, 0x69, 0x00, 0x61, 0x00,
                                        0x6e, 0x00, 0x74, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            LsarLookupSidsResponse response = new LsarLookupSidsResponse(buffer);

            Assert.IsTrue(response.Count == 1);
            Assert.IsTrue(response.DomainList.Entries == 1);
            Assert.IsTrue(response.DomainList.Names[0].Name == "TEST");
            Assert.IsTrue(SIDHelper.ToString(response.DomainList.Names[0].Sid) == "S-1-5-21-4005144719-3948538632-2546531719");
            Assert.IsTrue(response.TranslatedSids.Items.Count == 1);
            Assert.IsTrue(response.TranslatedSids.Items[0].DomainIndex == 0);
            Assert.IsTrue(response.TranslatedSids.Items[0].Name == "Adiant");
            Assert.IsTrue(response.TranslatedSids.Items[0].Use == LsaSIDNameUse.SidTypeUser);

        }

        [TestMethod]
        public void TestLsarLookupNamesResponse()
        {
            byte[] buffer = new byte[] {0x00, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x04, 0x00, 0x02, 0x00, 0x20, 0x00, 0x00, 0x00,
                                        0x01, 0x00, 0x00, 0x00, 0x08, 0x00, 0x0a, 0x00, 0x08, 0x00, 0x02, 0x00, 0x0c, 0x00, 0x02, 0x00,
                                        0x05, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x54, 0x00, 0x45, 0x00,
                                        0x53, 0x00, 0x54, 0x00, 0x04, 0x00, 0x00, 0x00, 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05,
                                        0x15, 0x00, 0x00, 0x00, 0x8f, 0xa8, 0xb9, 0xee, 0x08, 0xeb, 0x59, 0xeb, 0x87, 0xfd, 0xc8, 0x97,
                                        0x01, 0x00, 0x00, 0x00, 0x10, 0x00, 0x02, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00,
                                        0xe8, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            LsarLookupNamesResponse response = new LsarLookupNamesResponse(buffer);

            Assert.IsTrue(response.Count == 1);
            Assert.IsTrue(response.DomainList.Entries == 1);
            Assert.IsTrue(response.DomainList.Names[0].Name == "TEST");
            Assert.IsTrue(SIDHelper.ToString(response.DomainList.Names[0].Sid) == "S-1-5-21-4005144719-3948538632-2546531719");
            Assert.IsTrue(response.TranslatedNames.Items.Count == 1);
            Assert.IsTrue(response.TranslatedNames.Items[0].DomainIndex == 0);
            Assert.IsTrue(response.TranslatedNames.Items[0].RelativeId == 1000);
            Assert.IsTrue(response.TranslatedNames.Items[0].Use == LsaSIDNameUse.SidTypeUser);
        }

        [TestMethod]
        public void TestSid()
        {
            byte[] buffer = new byte[] { 0x01, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x15, 0x00, 0x00, 0x00, 0x8f, 0xa8, 0xb9, 0xee, 0x08, 0xeb, 0x59, 0xeb, 0x87, 0xfd, 0xc8, 0x97 };

            SID sid = new SID(buffer, 0);

            Assert.IsTrue(string.Equals(SIDHelper.ToString(sid),"S-1-5-21-4005144719-3948538632-2546531719", StringComparison.OrdinalIgnoreCase));
        }

        public void TestAll()
        {
            TestLsarLookupSidsResponse();
            TestLsarLookupNamesResponse();
            TestSid();
        }
    }
}
