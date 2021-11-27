using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using CUSTIS.NetCore.Lightbox.Filters;
using CUSTIS.NetCore.Lightbox.Utils;

namespace CUSTIS.NetCore.Lightbox.Processing
{
    internal class SwitchmanValidator
    {
        private readonly List<MethodInfo> _manyArgsMethods = new();
        private readonly List<MethodInfo> _asyncVoidMethods = new();
        private IGrouping<string, SwitchmanInfo>[]? _ambiguousSubscribers;

        private static readonly Type[] AuxParamTypes = { typeof(CancellationToken), typeof(ForwardContext) };

        public bool Validate(MethodInfo methodInfo, SwitchmanAttribute switchmanAttribute)
        {
            var valid = true;

            if (methodInfo.GetParameters().Count(p => !AuxParamTypes.Contains(p.ParameterType)) > 1)
            {
                _manyArgsMethods.Add(methodInfo);
                valid = false;
            }

            if (methodInfo.IsAsyncMethod() && methodInfo.ReturnType == typeof(void))
            {
                _asyncVoidMethods.Add(methodInfo);
                valid = false;
            }

            return valid;
        }

        public void Validate(IReadOnlyCollection<SwitchmanInfo> switchmanInfos)
        {
            _ambiguousSubscribers = switchmanInfos.GroupBy(s => s.MessageType).Where(s => s.Count() > 1).ToArray();
        }

        public void ThrowIfNecessary()
        {
            var errors = new List<string>();

            if (_manyArgsMethods.Any())
            {
                errors.Add(
                    "В методах-стрелочниках допустимо использовать не более 1 dto-аргумента. " +
                    $"Ошибочные методы: [{_manyArgsMethods.ToJoinedString(ReflectionHelper.GetMemberName)}]");
            }

            if (_asyncVoidMethods.Any())
            {
                errors.Add("Асинхронные методы стрелочников должны возвращать Task. " +
                           $"Ошибочные методы: [{_asyncVoidMethods.ToJoinedString(ReflectionHelper.GetMemberName)}]");
            }

            if (_ambiguousSubscribers?.Any() ?? false)
            {
                var ambiguousStr = _ambiguousSubscribers
                    .Select(s => $"[{s.Key}] - [{s.Select(x => x.MethodInfo.GetMemberName()).ToJoinedString()}]")
                    .ToJoinedString(Environment.NewLine);

                errors.Add(
                    $"Допустима регистрация только одного стрелочника для каждого типа сообщения. " +
                    $"Для следующих сообщений зарегистрировано более одного стрелочника: {Environment.NewLine}" +
                    ambiguousStr);
            }

            if (errors.Any())
            {
                throw new InvalidOperationException(errors.ToJoinedString(Environment.NewLine));
            }
        }
    }
}