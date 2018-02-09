using System;

using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic; 

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using System.Diagnostics;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 1683, "Auto Capitalization Implementation")]
	public class Issue1683 : ContentPage
	{
		const string kContainerId = "Container";
		public Issue1683()
		{
			var layout = new StackLayout() { ClassId = kContainerId };

			KeyboardFlags[] flags = new[]
			{
				KeyboardFlags.None,
				KeyboardFlags.CapitalizeWord,
				KeyboardFlags.CapitalizeSentence,
				KeyboardFlags.CapitalizeCharacter,
				KeyboardFlags.CapitalizeNone,
				KeyboardFlags.All
			};

			List<Entry> entryViews = new List<Entry>();
			List<Editor> editorViews = new List<Editor>();
			List<InputView> inputViews = new List<InputView>();

			KeyboardFlags spellCheckForUwp = KeyboardFlags.None;

			if(Device.RuntimePlatform == Device.UWP)
			{
				spellCheckForUwp = KeyboardFlags.Spellcheck;
			}

			foreach (var flag in flags)
			{
				entryViews.Add(new Entry() { Keyboard = Keyboard.Create(flag | spellCheckForUwp), ClassId = $"Entry{flag}" });
				editorViews.Add(new Editor() { Keyboard = Keyboard.Create(flag | spellCheckForUwp), ClassId = $"Editor{flag}" });
			}


			entryViews.Add(new Entry() { ClassId = "EntryNoKeyboard" });
			editorViews.Add(new Editor() { ClassId = "EditorNoKeyboard" });

			inputViews.AddRange(entryViews);
			inputViews.AddRange(editorViews);

			if (Device.RuntimePlatform == Device.UWP)
			{
				layout.Children.Add(new Label() { Text = "Capitalization settings only work when using touch keyboard" });
				layout.Children.Add(new Label() { Text = "Character doesn't do anything on UWP" });
			}
			else if (Device.RuntimePlatform == Device.iOS)
			{
				layout.Children.Add(new Label() { Text = "All will use Sentence" });
				layout.Children.Add(new Label() { Text = "No Keyboard will use Sentence" });
			}
			else if (Device.RuntimePlatform == Device.Android)
			{
				layout.Children.Add(new Label() { Text = "All will use Sentence" });
				layout.Children.Add(new Label() { Text = "No Keyboard will use None" });
			}

			foreach (InputView child in inputViews)
			{
				var inputs = new StackLayout()
				{
					Orientation =  StackOrientation.Horizontal
				};

				if(child is Entry)
					(child as Entry).Text = "All the Same.";

				if(child is Editor)
					(child as Editor).Text = "All the Same.";


				child.HorizontalOptions = LayoutOptions.FillAndExpand;
				var theLabel = new Label();

				theLabel.SetBinding(Label.TextProperty, new Binding("ClassId", source: child));
				inputs.Children.Add(theLabel);
				inputs.Children.Add(child);
				layout.Children.Add(inputs);
			}

			Button rotate = new Button() { Text = "Change Capitalization Settings. Ensure they update correctly" };

			
			// This shifts everyones capitalization by one in order
			// to test that updating the field works as expected
			rotate.Clicked += (_, __) =>
			{
				var item1 = entryViews[0];
				entryViews.Remove(item1);
				entryViews.Add(item1);

				var item2 = editorViews[0];
				editorViews.Remove(item2);
				editorViews.Add(item2);

				for (int i = 0; i <= flags.Length; i++)
				{
					if(i == flags.Length)
					{
						entryViews[i].Keyboard = null;
						entryViews[i].ClassId = "EntryNoKeyboard";

						editorViews[i].Keyboard = null;
						editorViews[i].ClassId = "EntryNoKeyboard";
					}
					else
					{

						entryViews[i].Keyboard = Keyboard.Create(flags[i] | spellCheckForUwp);
						entryViews[i].ClassId = $"Entry{flags[i]}";

						editorViews[i].Keyboard = Keyboard.Create(flags[i] | spellCheckForUwp);
						editorViews[i].ClassId = $"Editor{flags[i]}";
					}
				}
			};



			StackLayout content = new StackLayout();
			content.Children.Add(new ScrollView()
			{
				Content = layout
			});

			content.Children.Add(rotate);

			Content = content;
		}
	}
}
