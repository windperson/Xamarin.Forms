using System;
using System.Diagnostics;
using Windows.UI.Xaml.Input;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.UWP
{
	public static class KeyboardExtensions
	{
		public static InputScope ToInputScope(this Keyboard self)
		{
			if (self == null)
				throw new ArgumentNullException("self");

			var result = new InputScope();
			var name = new InputScopeName();
			if (self == Keyboard.Default)
			{
				name.NameValue = InputScopeNameValue.Default;
			}
			else if (self == Keyboard.Chat)
			{
				name.NameValue = InputScopeNameValue.Chat;
			}
			else if (self == Keyboard.Email)
			{
				name.NameValue = InputScopeNameValue.EmailSmtpAddress;
			}
			else if (self == Keyboard.Numeric)
			{
				name.NameValue = InputScopeNameValue.Number;
			}
			else if (self == Keyboard.Telephone)
			{
				name.NameValue = InputScopeNameValue.TelephoneNumber;
			}
			else if (self == Keyboard.Text)
			{
				name.NameValue = InputScopeNameValue.Default;
			}
			else if (self == Keyboard.Url)
			{
				name.NameValue = InputScopeNameValue.Url;
			}
			else
			{
				var custom = (CustomKeyboard)self;
				bool capitalizedSentenceEnabled = custom.Flags.HasFlag(KeyboardFlags.CapitalizeSentence);
				bool capitalizedWordsEnabled = custom.Flags.HasFlag(KeyboardFlags.CapitalizeWord);
				bool capitalizedCharacterEnabled = custom.Flags.HasFlag(KeyboardFlags.CapitalizeCharacter);
				bool spellcheckEnabled = custom.Flags.HasFlag(KeyboardFlags.Spellcheck);
				bool suggestionsEnabled = custom.Flags.HasFlag(KeyboardFlags.Suggestions);
				InputScopeNameValue nameValue = InputScopeNameValue.Default;
				
				if(capitalizedSentenceEnabled)
				{
					if (!spellcheckEnabled)
					{
						Log.Warning(null, "CapitalizeSentence only works when spell check is enabled");
					}
				}
				else if(capitalizedWordsEnabled)
				{
					if (!spellcheckEnabled)
					{
						Log.Warning(null, "CapitalizeWord only works when spell check is enabled");
					}

					nameValue = InputScopeNameValue.NameOrPhoneNumber;
				}

				if (capitalizedCharacterEnabled)
				{
					Log.Warning(null, "UWP doesn't support CapitalizeCharacter");
				}

				name.NameValue = nameValue;
			}

			result.Names.Add(name);
			return result;
		}
	}
}