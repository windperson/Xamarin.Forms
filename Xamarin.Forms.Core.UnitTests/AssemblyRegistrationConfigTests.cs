using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class AssemblyRegistrationConfigTests : BaseTestFixture
	{
		class MockAssembly : Assembly
		{
			public MockAssembly(string fullName)
			{
				FullName = fullName;
			}

			public override string FullName { get; }
		}

		readonly List<Assembly> _assemblies;

		public AssemblyRegistrationConfigTests()
		{
			_assemblies = new List<Assembly>
			{
				new MockAssembly("Xamarin.Forms.Platform.Android, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.ControlGallery.Android, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("FormsViewGroup, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Animated.Vector.Drawable, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Annotations, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Compat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Core.UI, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Core.Utils, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Design, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Fragment, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Media.Compat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Transition, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.v4, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.v7.AppCompat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.v7.CardView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.v7.Palette, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.v7.RecyclerView, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Android.Support.Vector.Drawable, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Firebase.AppIndexing, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Firebase.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.CustomAttributes, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.Maps.Android, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.Maps, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.Platform.Android.AppLinks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("mscorlib, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
				new MockAssembly("Xamarin.Forms.Platform, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.GooglePlayServices.Base, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.GooglePlayServices.Basement, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.GooglePlayServices.Tasks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.Forms.Xaml, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Xamarin.GooglePlayServices.Maps, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"),
				new MockAssembly("Java.Interop, Version=0.1.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"),
				new MockAssembly("System.Core, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
				new MockAssembly("System, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
				new MockAssembly("Mono.Android.Export, Version=1.0.6584.35159, Culture=neutral, PublicKeyToken=84e04ff9cfb79065"),
				new MockAssembly("System.Xml, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e"),
				new MockAssembly("__callback_factory__, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),
				new MockAssembly("MonoDroidConstructors, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"),

				new MockAssembly("Xamarin.Forms.Core.UnitTests") // Need this one to test Platform Assembly Rule
			};
		}

		[Test]
		public void NoRulesIncludesEverything()
		{
			var config = new AssemblyRegistrationConfig();
			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(_assemblies.Count));
		}

		[Test]
		public void ExcludeRemovesMatchingAssemblies()
		{
			// There are four assemblies with "GooglePlayServices" in the name
			// So if we exlcude them, we expect 38 assemblies to pass
			var config = new AssemblyRegistrationConfig();
			config.ExcludeAssemblies("GooglePlayServices");

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(38));
		}

		[Test]
		public void IncludeOnlyMatchingAssemblies()
		{
			// There are three assemblies with "Maps" in the name
			// So if we only include them, we expect 3 assemblies to pass
			var config = new AssemblyRegistrationConfig();
			config.IncludeAssemblies("Maps");

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(3));
		}

		[Test]
		public void IncludeProcessedBeforeExclude()
		{
			// 32 assemblies with Xamarin in the name
			// 2 of those with Firebase in the name

			var config = new AssemblyRegistrationConfig();
			config.IncludeAssemblies("Xamarin");
			config.ExcludeAssemblies("Firebase");

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(32 - 2));
		}

		[Test]
		public void IncludeExcludeOrderDoesNotMatter()
		{
			// 32 assemblies with Xamarin in the name
			// 2 of those with Firebase in the name

			var config = new AssemblyRegistrationConfig();
			config.ExcludeAssemblies("Firebase");
			config.IncludeAssemblies("Xamarin");

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(32 - 2));
		}

		[Test]
		public void IncludeRulesUnion()
		{
			var config = new AssemblyRegistrationConfig();
			config.IncludeAssemblies("Java"); // 1 assembly
			config.IncludeAssemblies(@"Mono\."); // 2 assemblies 

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(3));
		}

		[Test]
		public void IncludeRulesDoNotDuplicate()
		{
			var config = new AssemblyRegistrationConfig();
			config.IncludeAssemblies("Xamarin"); // 32 assemblies
			config.IncludeAssemblies("Xamarin"); // 32 assemblies 

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(32));
		}

		[Test]
		public void IncludeExecutingAssembly()
		{
			var config = new AssemblyRegistrationConfig();
			config.IncludeExecutingAssembly();

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(1));
			Assert.That(_assemblies.Where(config.AllowsAssembly).Single().FullName, Is.EqualTo("Xamarin.Forms.Core, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null"));
		}

		[Test]
		public void IncludePlatformAssembly()
		{
			var config = new AssemblyRegistrationConfig();
			config.IncludePlatformAssembly();

			Assert.That(_assemblies.Where(config.AllowsAssembly).Count(), Is.EqualTo(1));
			Assert.That(_assemblies.Where(config.AllowsAssembly).Single().FullName, Is.EqualTo("Xamarin.Forms.Core.UnitTests"));
		}
	}
}