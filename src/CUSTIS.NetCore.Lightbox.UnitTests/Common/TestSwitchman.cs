using System;
using System.Threading;
using System.Threading.Tasks;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Processing;

namespace CUSTIS.NetCore.Lightbox.UnitTests.Common
{
    internal class TestSwitchman : ISwitchman
    {
        public long InvocationCount { get; private set; }

        public const string ErrMessage = "При обработке сообщения произошло исключение";

        [Switchman(nameof(ProcessMessage))]
        public void ProcessMessage()
        {
            InvocationCount++;
        }

        public void ThrowError()
        {
            throw new Exception(ErrMessage);
        }

        public async Task ProcessAsync()
        {
            await Task.Delay(100);

            throw new Exception("Произошла ошибочка");
        }

        public async Task<string> ProcessAsyncWithResult()
        {
            await Task.Delay(100);

            throw new Exception("Произошла ошибочка");
        }

        public CancellationToken? Token { get; private set; }

        public Dto? Dto { get; private set; }

        public ForwardContext? Context { get; private set; }

        [Switchman(nameof(ProcessWithToken))]
        public Task ProcessWithToken(CancellationToken token)
        {
            Token = token;

            return Task.CompletedTask;
        }

        [Switchman(nameof(ProcessDtoWithToken))]
        public Task ProcessDtoWithToken(Dto dto, CancellationToken token)
        {
            Token = token;
            Dto = dto;

            return Task.CompletedTask;
        }

        public Task ProcessDtoContextToken(Dto dto, ForwardContext context, CancellationToken token)
        {
            Token = token;
            Dto = dto;
            Context = context;

            return Task.CompletedTask;
        }

        public Task ProcessContextToken(ForwardContext context, CancellationToken token)
        {
            Token = token;
            Context = context;

            return Task.CompletedTask;
        }

        public Task ProcessObject(object dto)
        {
            Dto = (Dto)dto;

            return Task.CompletedTask;
        }

        public Task ProcessIllegalParam(string dto)
        {
            return Task.CompletedTask;
        }

        public void Reset()
        {
            Token = null;
            Dto = null;
            Context = null;
        }
    }
}