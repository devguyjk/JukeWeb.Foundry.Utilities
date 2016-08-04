using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JukeWeb.Foundry.Utilities.Parser.CSSParser {
	/// <summary></summary>
	public interface IRuleSetContainer {
		/// <summary></summary>
		List<RuleSet> RuleSets { get; set; }
	}
}