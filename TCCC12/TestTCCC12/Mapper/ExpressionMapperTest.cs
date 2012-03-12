using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TCCC12.Mapper;

namespace TestTCCC12.Mapper
{
    [TestClass]
    public class ExpressionMapperTest
    {
        [TestMethod]
        public void TestFromAtoB()
        {
            var mapper = new ExpressionMapper<TestA, TestB>()
                .Map(x => x.AValue1, x => x.BValue1)
                .Map(x => x.AValue2, x => x.BValue2);

            var testA = new TestA()
                        {
                            AValue1 = "TestValue",
                            AValue2 = 42
                        };
            var testB = mapper.MapAtoB(testA);

            Assert.AreEqual("TestValue", testB.BValue1);
            Assert.AreEqual(42, testB.BValue2);
        }

        [TestMethod]
        public void TestFromBtoA()
        {
            var mapper = new ExpressionMapper<TestA, TestB>()
                .Map(x => x.AValue1, x => x.BValue1)
                .Map(x => x.AValue2, x => x.BValue2);

            var testB = new TestB()
            {
                BValue1 = "TestValue",
                BValue2 = 42
            };
            var testA = mapper.MapBtoA(testB);

            Assert.AreEqual("TestValue", testA.AValue1);
            Assert.AreEqual(42, testA.AValue2);
        }

        private class TestA
        {
            public string AValue1 { get; set; }
            public int AValue2 { get; set; }
        }

        private class TestB
        {
            public string BValue1 { get; set; }
            public int BValue2 { get; set; }
        }
    }
}

