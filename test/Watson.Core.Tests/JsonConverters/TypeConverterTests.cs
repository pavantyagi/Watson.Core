using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Watson.Core.JsonConverters;

namespace Watson.Core.Tests.JsonConverters
{
    [TestClass]
    public class TypeConverterTests
    {
        [TestMethod]
        public void TypeConverter_CanConvertBool_IsTrue()
        {
            var converter = new TypeConverter<bool>();
            var canConvert = converter.CanConvert(typeof(bool));
            Assert.IsTrue(canConvert);
        }

        [TestMethod]
        public void TypeConverter_CanConvertBoolList_IsTrue()
        {
            var converter = new TypeConverter<IEnumerable<bool>>();
            var list = new List<bool> {false, true};
            var canConvert = converter.CanConvert(list.GetType());
            Assert.IsTrue(canConvert);
        }


        [TestMethod]
        public void TypeConverter_CanConvertString_IsFalse()
        {
            var converter = new TypeConverter<int>();

            Assert.IsFalse(converter.CanConvert(typeof (string)));
        }

        [TestMethod]
        public void TypeConverter_CanConvertInt_IsFalse()
        {
            var converter = new TypeConverter<bool>();
            var list = new List<int> {1, 2, 3};
            Assert.IsFalse(converter.CanConvert(list.GetType()));
        }
    }
}