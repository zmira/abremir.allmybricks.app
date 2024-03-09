using System;
using System.Threading.Tasks;
using abremir.AllMyBricks.DataSynchronizer.Events.SetSynchronizationService;
using abremir.AllMyBricks.DataSynchronizer.Interfaces;
using abremir.AllMyBricks.DataSynchronizer.Services;
using Easy.MessageHub;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstituteAutoMocker.Standard;

namespace abremir.AllMyBricks.DataSynchronizer.Tests.Services
{
    [TestClass]
    public class SetSynchronizationServiceTests
    {
        private NSubstituteAutoMocker<SetSynchronizationService> _setSynchronizationService;

        [TestInitialize]
        public void TestInitialize()
        {
            _setSynchronizationService = new NSubstituteAutoMocker<SetSynchronizationService>();
        }

        [TestMethod]
        public async Task Synchronize_ThemeSynchronizerThrowsException_StopsAndExceptionIsPublished()
        {
            var themeSynchronizer = _setSynchronizationService.Get<IThemeSynchronizer>();
            themeSynchronizer.Synchronize().Throws(new Exception());
            var subthemeSynchronizer = _setSynchronizationService.Get<ISubthemeSynchronizer>();
            var fullSetSynchronizer = _setSynchronizationService.Get<IFullSetSynchronizer>();
            var partialSetSynchronzier = _setSynchronizationService.Get<IPartialSetSynchronizer>();
            var messageHub = _setSynchronizationService.Get<IMessageHub>();

            await _setSynchronizationService.ClassUnderTest.Synchronize();

            await themeSynchronizer.Received(1).Synchronize();
            await subthemeSynchronizer.DidNotReceive().Synchronize();
            await fullSetSynchronizer.DidNotReceive().Synchronize();
            await partialSetSynchronzier.DidNotReceive().Synchronize();
            messageHub.Received().Publish(Arg.Any<SetSynchronizationServiceException>());
        }

        [TestMethod]
        public async Task Synchronize_SubthemeSynchronizerThrowsException_StopsAndExceptionIsPublished()
        {
            var themeSynchronizer = _setSynchronizationService.Get<IThemeSynchronizer>();
            var subthemeSynchronizer = _setSynchronizationService.Get<ISubthemeSynchronizer>();
            subthemeSynchronizer.Synchronize().Throws(new Exception());
            var fullSetSynchronizer = _setSynchronizationService.Get<IFullSetSynchronizer>();
            var partialSetSynchronzier = _setSynchronizationService.Get<IPartialSetSynchronizer>();
            var messageHub = _setSynchronizationService.Get<IMessageHub>();

            await _setSynchronizationService.ClassUnderTest.Synchronize();

            await themeSynchronizer.Received(1).Synchronize();
            await subthemeSynchronizer.Received(1).Synchronize();
            await fullSetSynchronizer.DidNotReceive().Synchronize();
            await partialSetSynchronzier.DidNotReceive().Synchronize();
            messageHub.Received().Publish(Arg.Any<SetSynchronizationServiceException>());
        }

        [TestMethod]
        public async Task Synchronize_FullSetSynchronizerThrowsException_StopsAndExceptionIsPublished()
        {
            var themeSynchronizer = _setSynchronizationService.Get<IThemeSynchronizer>();
            var subthemeSynchronizer = _setSynchronizationService.Get<ISubthemeSynchronizer>();
            var fullSetSynchronizer = _setSynchronizationService.Get<IFullSetSynchronizer>();
            fullSetSynchronizer.Synchronize().Throws(new Exception());
            var partialSetSynchronzier = _setSynchronizationService.Get<IPartialSetSynchronizer>();
            var messageHub = _setSynchronizationService.Get<IMessageHub>();

            await _setSynchronizationService.ClassUnderTest.Synchronize();

            await themeSynchronizer.Received(1).Synchronize();
            await subthemeSynchronizer.Received(1).Synchronize();
            await fullSetSynchronizer.Received(1).Synchronize();
            await partialSetSynchronzier.DidNotReceive().Synchronize();
            messageHub.Received().Publish(Arg.Any<SetSynchronizationServiceException>());
        }

        [TestMethod]
        public async Task Synchronize_PartialSetSynchronizerThrowsException_StopsAndExceptionIsPublished()
        {
            var themeSynchronizer = _setSynchronizationService.Get<IThemeSynchronizer>();
            var subthemeSynchronizer = _setSynchronizationService.Get<ISubthemeSynchronizer>();
            var fullSetSynchronizer = _setSynchronizationService.Get<IFullSetSynchronizer>();
            var partialSetSynchronizer = _setSynchronizationService.Get<IPartialSetSynchronizer>();
            partialSetSynchronizer.Synchronize().Throws(new Exception());
            var messageHub = _setSynchronizationService.Get<IMessageHub>();

            await _setSynchronizationService.ClassUnderTest.Synchronize();

            await themeSynchronizer.Received(1).Synchronize();
            await subthemeSynchronizer.Received(1).Synchronize();
            await fullSetSynchronizer.Received(1).Synchronize();
            await partialSetSynchronizer.Received(1).Synchronize();
            messageHub.Received().Publish(Arg.Any<SetSynchronizationServiceException>());
        }

        [TestMethod]
        public async Task Synchronize_NoSynchronizerThrowsException_CompletesSuccessfully()
        {
            var themeSynchronizer = _setSynchronizationService.Get<IThemeSynchronizer>();
            var subthemeSynchronizer = _setSynchronizationService.Get<ISubthemeSynchronizer>();
            var fullSetSynchronizer = _setSynchronizationService.Get<IFullSetSynchronizer>();
            var partialSetSynchronizer = _setSynchronizationService.Get<IPartialSetSynchronizer>();
            var messageHub = _setSynchronizationService.Get<IMessageHub>();

            await _setSynchronizationService.ClassUnderTest.Synchronize();

            await themeSynchronizer.Received(1).Synchronize();
            await subthemeSynchronizer.Received(1).Synchronize();
            await fullSetSynchronizer.Received(1).Synchronize();
            await partialSetSynchronizer.Received(1).Synchronize();
            messageHub.DidNotReceive().Publish(Arg.Any<SetSynchronizationServiceException>());
        }
    }
}
