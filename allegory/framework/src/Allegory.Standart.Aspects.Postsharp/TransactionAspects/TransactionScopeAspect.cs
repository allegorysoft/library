using System;
using System.Transactions;
using PostSharp.Aspects;

namespace Allegory.Standart.Aspects.Postsharp
{
    [Serializable]
    public class TransactionScopeAspect : OnMethodBoundaryAspect
    {
        public TransactionScopeOption TransactionScopeOption { get; set; }
        private Nullable<TransactionOptions> _transactionOptions = null;

        public TransactionScopeAspect() { }
        public TransactionScopeAspect(IsolationLevel isolationLevel)
        {
            _transactionOptions = new TransactionOptions()
            {
                IsolationLevel = isolationLevel,
            };
        }
        public TransactionScopeAspect(IsolationLevel isolationLevel, double timeOutFromSecond)
        {
            _transactionOptions = new TransactionOptions()
            {
                IsolationLevel = isolationLevel,
                Timeout = TimeSpan.FromSeconds(timeOutFromSecond)
            };
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            if (_transactionOptions.HasValue)
                args.MethodExecutionTag = new TransactionScope(TransactionScopeOption, _transactionOptions.Value);
            else
                args.MethodExecutionTag = new TransactionScope(TransactionScopeOption);
        }
        public override void OnSuccess(MethodExecutionArgs args)
        {
            ((TransactionScope)args.MethodExecutionTag).Complete();
        }
        public override void OnExit(MethodExecutionArgs args)
        {
            ((TransactionScope)args.MethodExecutionTag).Dispose();
        }
    }
}
