using System;

namespace JukeWeb.Foundry.Utilities.Parser.CSSParser
{
	public enum AttributeOperator {
		/// <summary></summary>
		Equals,     // =
		/// <summary></summary>
		InList,     // ~=
		/// <summary></summary>
		Hyphenated, // |=
		/// <summary></summary>
		EndsWith,   // $=
		/// <summary></summary>
		BeginsWith, // ^=
		/// <summary></summary>
		Contains    // *=
	}
}