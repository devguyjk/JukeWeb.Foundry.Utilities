﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities.Libraries
{
    public class RegexLibrary
    {
        /// <summary>
        /// Format: 
        /// <p>Q: What NFL footballer saw his weight reach a league-leading 340 
		///	pounds in 1988?<br>
		///	A: William &quot;The Refrigerator&quot; Perry.</p>
		///	<p>Q: Who played defensive back for the New York Giants before he 
		///	coached the Cowboys?<br>
        ///
		///	A: Tom Landry. </p>
        ///	Question => Group 1 / Answer => Group 2
        /// </summary>
        public const string HTML_FLASHCARD1 = @"<p>(Q:.*(?:\r?\n?){1,3}.*\?)(?:.*(?:\r?\n?){1,3}.*)(A:.*)</p>";

        public const string HTML_TAGS = "<(?!\\/?p(?=>|\\s.*>))\\/?.*?>";
        public const string HTML = "<(\\/)?(\\w+)(?:(?:\\s+\\w+(?:\\s*=\\s*(?:\".*?\"|'.*?'|[^'\">\\s]+))?)+\\s*|\\s*)\\/?>";

        public const string URL_WWW = @"(?:www\.)";
        public const string URL_PROTOCOL = @"(?:[A-Za-z]{3,9}:(?:\/\/)?)";
        public const string URL_DOMAIN = @"[A-Za-z0-9\.\-]*[A-Za-z0-9\-]";
        public const string URL_TLD = @"\.(?:international|construction|contractors|enterprises|photography|productions|foundation|immobilien|industries|management|properties|technology|christmas|community|directory|education|equipment|institute|marketing|solutions|vacations|bargains|boutique|builders|catering|cleaning|clothing|computer|democrat|diamonds|graphics|holdings|lighting|partners|plumbing|supplies|training|ventures|academy|careers|company|cruises|domains|exposed|flights|florist|gallery|guitars|holiday|kitchen|neustar|okinawa|recipes|rentals|reviews|shiksha|singles|support|systems|agency|berlin|camera|center|coffee|condos|dating|estate|events|expert|futbol|kaufen|luxury|maison|monash|museum|nagoya|photos|repair|report|social|supply|tattoo|tienda|travel|viajes|villas|vision|voting|voyage|actor|build|cards|cheap|codes|dance|email|glass|house|mango|ninja|parts|photo|shoes|solar|today|tokyo|tools|watch|works|aero|arpa|asia|best|bike|blue|buzz|camp|club|cool|coop|farm|fish|gift|guru|info|jobs|kiwi|kred|land|limo|link|menu|mobi|moda|name|pics|pink|post|qpon|rich|ruhr|sexy|tips|vote|voto|wang|wien|wiki|zone|bar|bid|biz|cab|cat|ceo|com|edu|gov|int|kim|mil|net|onl|org|pro|pub|red|tel|uno|wed|xxx|xyz|ac|ad|ae|af|ag|ai|al|am|an|ao|aq|ar|as|at|au|aw|ax|az|ba|bb|bd|be|bf|bg|bh|bi|bj|bm|bn|bo|br|bs|bt|bv|bw|by|bz|ca|cc|cd|cf|cg|ch|ci|ck|cl|cm|cn|co|cr|cu|cv|cw|cx|cy|cz|de|dj|dk|dm|do|dz|ec|ee|eg|er|es|et|eu|fi|fj|fk|fm|fo|fr|ga|gb|gd|ge|gf|gg|gh|gi|gl|gm|gn|gp|gq|gr|gs|gt|gu|gw|gy|hk|hm|hn|hr|ht|hu|id|ie|il|im|in|io|iq|ir|is|it|je|jm|jo|jp|ke|kg|kh|ki|km|kn|kp|kr|kw|ky|kz|la|lb|lc|li|lk|lr|ls|lt|lu|lv|ly|ma|mc|md|me|mg|mh|mk|ml|mm|mn|mo|mp|mq|mr|ms|mt|mu|mv|mw|mx|my|mz|na|nc|ne|nf|ng|ni|nl|no|np|nr|nu|nz|om|pa|pe|pf|pg|ph|pk|pl|pm|pn|pr|ps|pt|pw|py|qa|re|ro|rs|ru|rw|sa|sb|sc|sd|se|sg|sh|si|sj|sk|sl|sm|sn|so|sr|st|su|sv|sx|sy|sz|tc|td|tf|tg|th|tj|tk|tl|tm|tn|to|tp|tr|tt|tv|tw|tz|ua|ug|uk|us|uy|uz|va|vc|ve|vg|vi|vn|vu|wf|ws|ye|yt|za|zm|zw)\b";
        public const string URL_SUFFIX = @"(?:[-A-Za-z0-9+&@#\/%?=~_()|!:,.;]*[-A-Za-z0-9+&@#\/%=~_()|])?";

        public static string URL
        {
            get
            {
                return string.Format(@"((?:(?:{0}{1})|(?:{2}{1})){3})", URL_PROTOCOL, URL_DOMAIN, URL_WWW, URL_SUFFIX);
            }
        }
    }
}
