﻿#nullable enable

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.DataAnnotations;

using Microsoft.Extensions.Localization;

namespace Bang.Lingo.Validation;

public class LocalizedValidationAttributeAdapterProvider : IValidationAttributeAdapterProvider
{
    private IValidationAttributeAdapterProvider innerProvider = new ValidationAttributeAdapterProvider();

    public IAttributeAdapter? GetAttributeAdapter(ValidationAttribute? attribute, IStringLocalizer? stringLocalizer)
    {
        if (attribute == null)
        {
            throw new ArgumentNullException(nameof(attribute));
        }

        switch (attribute)
        {
            case RequiredAttribute requiredAttribute:
                return new LocalizedRequiredAttributeAdapter(requiredAttribute, stringLocalizer);

            case RegularExpressionAttribute regexAttribute:
                return new LocalizedRegularExpressionAttributeAdapter(regexAttribute, stringLocalizer);

            case RangeAttribute rangeAttribute:
                return new LocalizedRangeAttributeAdapter(rangeAttribute, stringLocalizer);

            //case MinLengthAttribute minLengthAttribute:
            //	return new LocalizedMinLengthAttributeAdapter(minLengthAttribute, stringLocalizer);

            //case MaxLengthAttribute maxLengthAttribute:
            //	return new LocalizedMaxLengthAttributeAdapter(maxLengthAttribute, stringLocalizer);

            case EmailAddressAttribute emailAttribute:
                return new LocalizedEmailAddressAttributeAdapter(emailAttribute, stringLocalizer);

                //case RequiredIfNullAttribute requiredWithDependencyAttribute:
                //	return new LocalizedRequiredIfNullAttributeAdapter(requiredWithDependencyAttribute, stringLocalizer);
        }

        return innerProvider.GetAttributeAdapter(attribute, stringLocalizer);
    }
}