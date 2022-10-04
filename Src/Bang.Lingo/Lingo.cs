﻿#nullable enable

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System.Collections.Immutable;

namespace Bang.Lingo;

public class Lingo
{
	// Constructors
	public Lingo(LingoOptions? options = null)
	{
		options = options ?? new LingoOptions();

		this.MissingItemText = options.MissingItemText;
		this.BasePrefixes = options.BasePrefixes;
		this.Parameters = options.Parameters;

		this.LoadTranslations = options.GetLoadTranslations();
	}

	public Lingo(IOptions<LingoOptions> options) : this(options.Value)
	{
	}


	// Methods
	public void Load()
	{
		lock(this.Dictionaries)
		{
			this.Dictionaries = this.Dictionaries.Clear();

			this.LoadTranslations?.Invoke(this);
		}
	}

	public String? GetBasePrefix(Type? type)
	{
		String? prefix = null;

		while(type != null)
		{
			if(this.BasePrefixes.TryGetValue(type, out var typePrefix))
			{
				prefix = typePrefix + prefix;

				if(!prefix.IsPrefixed("."))
				{
					break;
				}
			}

			type = type.BaseType;
		}

		return prefix;
	}

	public TranslationDictionary GetDictionary(String? language = null)
	{
		lock(this.Dictionaries)
		{
			if(language.IsNullOrWhiteSpace())
			{
				language = Thread.CurrentThread.CurrentUICulture.Name;
			}

			if(!this.Dictionaries.TryGetValue(language!, out var dictionary))
			{
				this.Dictionaries = this.Dictionaries.Add(language!, dictionary = new TranslationDictionary(language!));
			}

			return dictionary;
		}
	}

	public Translator GetTranslator(String? language = null, String? prefix = null)
	{
		lock(this.Dictionaries)
		{
			return new Translator(this, this.GetDictionary(language), prefix);
		}
	}

	public Translator GetTranslator(HttpContext? httpContext, String? language = null)
	{
		var prefix = httpContext?.Items["Lingo.Prefix"] as String;

		return this.GetTranslator(language, prefix);
	}

	public String? ParseParams(String? text, String language)
	{
		if(this.Parameters != null && text?.Contains('{') == true)
		{
			foreach(var keyVal in this.Parameters)
			{
				if(text!.Contains($"{{{keyVal.Key}}}", StringComparison.OrdinalIgnoreCase))
				{
					text = text.Replace($"{{{keyVal.Key}}}", keyVal.Value(language, keyVal.Key), StringComparison.OrdinalIgnoreCase);
				}
			}
		}

		return text;
	}

	public String GetMissingText(String language, String? key)
	{
		return this.MissingItemText
			.Replace("{language}", language, StringComparison.OrdinalIgnoreCase)
			.Replace("{key}", key, StringComparison.OrdinalIgnoreCase);
	}



	#region Protected Area

	// Properties
	protected readonly String MissingItemText;

	protected readonly Dictionary<Type, String> BasePrefixes;

	protected readonly Dictionary<String, Func<String, String, String>> Parameters;

	protected event Action<Lingo>? LoadTranslations;


	protected ImmutableDictionary<String, TranslationDictionary> Dictionaries = ImmutableDictionary<String, TranslationDictionary>.Empty.WithComparers(StringComparer.OrdinalIgnoreCase);

	#endregion
}