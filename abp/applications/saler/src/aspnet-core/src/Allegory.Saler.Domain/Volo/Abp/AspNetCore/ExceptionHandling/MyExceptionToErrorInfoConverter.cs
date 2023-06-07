using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using System;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ExceptionHandling.Localization;
using Volo.Abp.Http;
using Volo.Abp.Localization.ExceptionHandling;

namespace Volo.Abp.AspNetCore.ExceptionHandling;

public class MyExceptionToErrorInfoConverter : DefaultExceptionToErrorInfoConverter
{
    public MyExceptionToErrorInfoConverter(
        IOptions<AbpExceptionLocalizationOptions> localizationOptions,
        IStringLocalizerFactory stringLocalizerFactory,
        IStringLocalizer<AbpExceptionHandlingResource> stringLocalizer,
        IServiceProvider serviceProvider)
        : base(localizationOptions, stringLocalizerFactory, stringLocalizer, serviceProvider)
    {

    }

    protected override RemoteServiceErrorInfo CreateErrorInfoWithoutCode(Exception exception, AbpExceptionHandlingOptions options)
    {
        if (exception is CodeAlreadyExistsException)
            return CreateCodeAlreadyExistsException(exception as CodeAlreadyExistsException);

        if (exception is CodeNotFoundException)
            return CreateCodeNotFoundException(exception as CodeNotFoundException);

        if (exception is NumberAlreadyExistsException)
            return CreateNumberAlreadyExistsException(exception as NumberAlreadyExistsException);

        if (exception is NumberNotFoundException)
            return CreateNumberNotFoundException(exception as NumberNotFoundException);

        if (exception is ThereIsTransactionRecordException)
            return CreateThereIsTransactionRecordCannotUpdateException(exception as ThereIsTransactionRecordException);

        return base.CreateErrorInfoWithoutCode(exception, options);
    }

    protected virtual RemoteServiceErrorInfo CreateCodeAlreadyExistsException(CodeAlreadyExistsException exception)
    {
        return new RemoteServiceErrorInfo(
              string.Format(
                  L["CodeAlreadyExistsErrorMessage"],
                  L[exception.EntityType.FullName],
                  exception.EntityCode
              )
          );
    }

    protected virtual RemoteServiceErrorInfo CreateCodeNotFoundException(CodeNotFoundException exception)
    {
        return new RemoteServiceErrorInfo(
                string.Format(
                    L["CodeNotFoundErrorMessage"],
                    L[exception.EntityType.FullName],
                    exception.EntityCode
                )
            );
    }

    protected virtual RemoteServiceErrorInfo CreateNumberAlreadyExistsException(NumberAlreadyExistsException exception)
    {
        return new RemoteServiceErrorInfo(
              string.Format(
                  L["NumberAlreadyExistsErrorMessage"],
                  L[exception.EntityType.FullName],
                  exception.EntityNumber
              )
          );
    }

    protected virtual RemoteServiceErrorInfo CreateNumberNotFoundException(NumberNotFoundException exception)
    {
        return new RemoteServiceErrorInfo(
                string.Format(
                    L["NumberNotFoundErrorMessage"],
                    L[exception.EntityType.FullName],
                    exception.EntityNumber
                )
            );
    }

    protected virtual RemoteServiceErrorInfo CreateThereIsTransactionRecordCannotUpdateException(ThereIsTransactionRecordException exception)
    {
        string transactionTypeName = L[exception.TransactionEntityType.FullName];

        return new RemoteServiceErrorInfo(
        string.Format(
            exception.IsDelete
            ? L["ThereIsTransactionRecordCannotDeleteErrorMessage"]
            : L["ThereIsTransactionRecordCannotUpdateErrorMessage"],
            string.Concat(transactionTypeName[0].ToString().ToUpper(), transactionTypeName.AsSpan(1)),
            L[exception.EntityType.FullName]
        ));
    }
}
