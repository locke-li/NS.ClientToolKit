#if ENABLE_NUNIT_TEST
using System.Collections;
using System.Diagnostics;
using CenturyGame.ServiceLocation.Runtime;
using NUnit.Framework;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace Tests
{
    public class TestRegistration
    {
#region inner class

        class A { }

        class B { }
#endregion

        // A Test behaves as an ordinary method
        [Test]
        public void TestRegistrationEqualOrNotEqual()
        {
            ServiceRegistration r0 = new ServiceRegistration(typeof(A), "A");
            ServiceRegistration r1 = new ServiceRegistration(typeof(A), "A");
            ServiceRegistration r2 = new ServiceRegistration(typeof(B), "B");

            Assert.AreEqual(r0, r1, "The r0 is not equal r1 .");

            Assert.AreNotEqual(r0, r2, "The r0 is equal r2 .");

            Debug.Assert(r0.Equals(r1), "r0 not equals r1.");

            Debug.Assert(r0 != r2, "r0 == r2.");

            //ServiceRegistration r3 = new ServiceRegistration(null,null);
        }

        [Test]
        public void TestHashCode()
        {
            ServiceRegistration r0 = new ServiceRegistration(typeof(A), "A");
            ServiceRegistration r1 = new ServiceRegistration(typeof(A), "A");
            ServiceRegistration r2 = new ServiceRegistration(typeof(B), "B");

            //Debug.Log($"r0 : {r0.GetHashCode()} , r1 hashcode : {r1.GetHashCode()} .");
            //Debug.Log($"r0 type : {r0.Type.GetHashCode()} , r1 type hashcode : {r1.Type.GetHashCode()} .");
            //Debug.Log($"r0 key : {r0.Key.GetHashCode()} , r1 key hashcode : {r1.Key.GetHashCode()} .");

            //Debug.Log($"serviceregistration hashcode : {typeof(ServiceRegistration).GetHashCode()}");
            //Debug.Log($"r0 serviceregistration hashcode : {r0.GetType().GetHashCode()}");
            //Debug.Log($"r2 serviceregistration hashcode : {r2.GetType().GetHashCode()}");



            Assert.AreEqual(r0.GetHashCode(), r1.GetHashCode(), "r0 hashcode is not equal r1 .");
            Assert.AreNotEqual(r0.GetHashCode(), r2.GetHashCode(), "The hashcode of r0 is equal r2's hashcode .");

        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator TestRegistrationWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}
#endif