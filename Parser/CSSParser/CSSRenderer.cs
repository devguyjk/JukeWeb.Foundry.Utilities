using System;
using System.Collections.Generic;
using System.Text;

namespace JukeWeb.Foundry.Utilities.Parser.CSSParser{

	public static class CSSRenderer {
		public static string Render(CSSDocument css) {
			StringBuilder txt = new StringBuilder();
			txt.Append(css.ToString());
			return txt.ToString();
		}
	}
}