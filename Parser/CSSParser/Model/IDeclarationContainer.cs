using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace JukeWeb.Foundry.Utilities.Parser.CSSParser {
	/// <summary></summary>
	public interface IDeclarationContainer {
		/// <summary></summary>
		List<Declaration> Declarations { get; set; }
	}
}