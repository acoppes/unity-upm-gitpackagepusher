using System.Collections;
using System.Collections.Generic;
using Gemserk.UPMGitPusher.Editor;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PublishPackageTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void TestPublishPackageShouldFindOnlyPackageJson()
        {
            try
            {
                var packageData = PublishVersionMenuItem.GetPackageData();
            }
            catch
            {
                Assert.Fail("Should not fail with exception");
            }
        }
    }
}
