using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Parser;

namespace ParserTests
{
    [TestFixture]
    public class ElmahXmlParserTests
    {
        private Guid _guid1;
        private Guid _guid2;

        [SetUp]
        protected void SetUp()
        {
            _guid1 = Guid.NewGuid();
            _guid2 = Guid.NewGuid();
        }

        [Test]
        public void ElmahXmlParser_ParseScriptName_GetList()
        {
            // arrange
            var expectedList = new Dictionary<Guid, string>() { { _guid1, "/folder1/test1.aspx" }, { _guid2, "/folder2/test2.aspx" }, };
            var helper = new Mock<IFileHelper>();
            var errorsXmlFileName = "errors.xml";
            helper.Setup(x => x.ReadAllLines(errorsXmlFileName)).Returns(new[] { 
                string.Format("{{{0}}},HTTP_X_ORIGINAL_URL:/folder1/test1.aspx&#xD;&#xA;H,", _guid1.ToString()),
                string.Format("{{{0}}},HTTP_X_ORIGINAL_URL:/folder2/test2.aspx&#xD;&#xA;H,", _guid2.ToString()),
            });
            var parser = new ElmahXmlParser(helper.Object);

            // act
            var actualList = parser.GetScriptNameList(errorsXmlFileName);

            // assert
            Assert.That(actualList, Is.Not.Empty, "Empty List");
            Assert.That(actualList, Is.EquivalentTo(expectedList), "Wrong List");
            Assert.That(_guid1, Is.Not.EqualTo(_guid2), "GUIDs are equal?!?");
        }

        [Test]
        public void ElmahXmlParser_ParseStackTrace_GetList()
        {
            // arrange
            var expectedList = new Dictionary<Guid, string>() { { _guid1, "System.Web.HttpException (0x80004005): Exception Message.&#xD;&#xA;   at Namespace1.ClassName1.Method(string something1)&#xD;&#xA;   at System.Web.Handlers.ProcessRequest(Params params)" }, { _guid2, "System.Web.HttpException (0x80004005): Exception Message.&#xD;&#xA;   at Namespace2.ClassName2.Method(string something2)&#xD;&#xA;   at System.Web.Handlers.ProcessRequest(Params params)" }, };
            var helper = new Mock<IFileHelper>();
            var errorsXmlFileName = "errors.xml";
            helper.Setup(x => x.ReadAllLines(errorsXmlFileName)).Returns(new[] { 
                string.Format("{{{0}}},detail=\"System.Web.HttpException (0x80004005): Exception Message.&#xD;&#xA;   at Namespace1.ClassName1.Method(string something1)&#xD;&#xA;   at System.Web.Handlers.ProcessRequest(Params params)\"    ", _guid1.ToString()),
                string.Format("{{{0}}},detail=\"System.Web.HttpException (0x80004005): Exception Message.&#xD;&#xA;   at Namespace2.ClassName2.Method(string something2)&#xD;&#xA;   at System.Web.Handlers.ProcessRequest(Params params)\"    ", _guid2.ToString()), 
            });
            var parser = new ElmahXmlParser(helper.Object);

            // act
            var actualList = parser.GetStackTraceList(errorsXmlFileName);

            // assert
            Assert.That(actualList, Is.Not.Empty, "Empty List");
            Assert.That(actualList, Is.EquivalentTo(expectedList), "Wrong List");
        }
    }
}
