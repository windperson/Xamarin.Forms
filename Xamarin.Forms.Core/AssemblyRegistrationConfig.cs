using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Xamarin.Forms.Internals
{
	public class AssemblyRegistrationConfig
	{
		internal bool AllowsAssembly(Assembly assembly)
		{
			if (_includeRules == null && _excludeRules == null)
			{
				// If no rules have been specified, default to the old behavior
				// of including everything
				return true;
			}

			bool accept = _includeRules == null || _includeRules.Any(rule => rule.Matches(assembly));

			if (accept && _excludeRules != null)
			{
				accept = !_excludeRules.Any(rule => rule.Matches(assembly));
			}

			return accept;
		}

		internal class AssemblyRule
		{
			readonly Func<Assembly, bool> _matches;

			public AssemblyRule(Func<Assembly, bool> matches)
			{
				_matches = matches;
			}

			public bool Matches(Assembly assembly)
			{
				return _matches.Invoke(assembly);
			}
		}

		internal class ExecutingAssemblyRule : AssemblyRule
		{
			static string s_executingAssemblyName;

			static bool IsExecutingAssembly(Assembly assembly)
			{
				if (s_executingAssemblyName == null)
				{
					s_executingAssemblyName = Assembly.GetExecutingAssembly().FullName;
				}

				if (s_executingAssemblyName == null)
				{
					return false;
				}

				return assembly.FullName == s_executingAssemblyName;
			}

			public ExecutingAssemblyRule() : base(IsExecutingAssembly)
			{
			}
		}

		internal class PlatformAssemblyRule : AssemblyRule
		{
			static string s_platformServicesAssemblyName;

			static bool IsPlatformServicesAssembly(Assembly assembly)
			{
				if (s_platformServicesAssemblyName == null && Device.PlatformServices != null)
				{
					s_platformServicesAssemblyName = Device.PlatformServices.GetType().GetTypeInfo().Assembly.FullName;
				}

				if (s_platformServicesAssemblyName == null)
				{
					return false;
				}

				return assembly.FullName == s_platformServicesAssemblyName;
			}

			public PlatformAssemblyRule() : base(IsPlatformServicesAssembly)
			{
			}
		}

		List<AssemblyRule> _includeRules;
		List<AssemblyRule> _excludeRules;

		public void IncludeAssemblies(string pattern)
		{
			if (_includeRules == null)
			{
				_includeRules = new List<AssemblyRule>();
			}

			_includeRules.Add(new AssemblyRule(assembly => Regex.IsMatch(assembly.FullName, pattern)));
		}

		public void ExcludeAssemblies(string pattern)
		{
			if (_excludeRules == null)
			{
				_excludeRules = new List<AssemblyRule>();
			}

			_excludeRules.Add(new AssemblyRule(assembly => Regex.IsMatch(assembly.FullName, pattern)));
		}

		public void IncludeExecutingAssembly()
		{
			if (_includeRules == null)
			{
				_includeRules = new List<AssemblyRule>();
			}

			_includeRules.Add(new ExecutingAssemblyRule());  
		}

		public void IncludePlatformAssembly()
		{
			if (_includeRules == null)
			{
				_includeRules = new List<AssemblyRule>();
			}

			_includeRules.Add(new PlatformAssemblyRule());
		}
	}
}