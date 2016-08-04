using System;

namespace JukeWeb.Foundry.Utilities.Parser.CSSParser {
	/// <summary></summary>
	public enum Combinator {
		/// <summary></summary>
		ChildOf,				// >
		/// <summary></summary>
		PrecededImmediatelyBy,	// +
		/// <summary></summary>
		PrecededBy				// ~
	}
}