using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace MockSample
{
    [TestFixture]
    public class CommandProcessorUnitTest
    {
       [Test]
        public void SendCommand()
        {


            Mock<IFirmware> mockFirmware = new Mock<IFirmware>();
            Action<IFirmware> fu=new Action<IFirmware>((ob)=>ob.IsOkToSendCommand("Start"));
            //System.Linq.Expressions.Expression<Func<IFirmware,bool>> exp = (ob)=>ob.IsOkToSendCommand("Start");

            mockFirmware.Setup(ob => ob.IsOkToSendCommand("Start")).Returns(() => true);
           
            CommandProcessor commandProcessor = new CommandProcessor(mockFirmware.Object);
            Assert.AreEqual(1,commandProcessor.SendCommand("Start"));
            mockFirmware.Verify(ob=>ob.IsOkToSendCommand("Start"),Times.AtLeast(1),"DD");
        }
    }
}
