﻿#nullable enable

namespace Monotype.Localization;

[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum | AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class LingoPrefixAttribute : Attribute
{
	public String? Prefix { get; set; }


	public LingoPrefixAttribute(String? prefix = null)
	{
		this.Prefix = prefix;
	}
}
