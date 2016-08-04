using System;
using System.Collections.Generic;

namespace JukeWeb.Foundry.Utilities.Parser.CSSParser {
	/// <summary></summary>
	public interface ISelectorContainer {
		/// <summary></summary>
		List<Selector> Selectors { get; set; }
	}
}