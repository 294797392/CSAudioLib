using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kagura.Player.Base
{
    public enum ID3V2Flags
    {
        /// <summary>
        /// Bit 7 in the 'ID3v2 flags' indicates whether or not
        /// unsynchronisation is applied on all frames (see section 6.1 for
        /// details); a set bit indicates usage.
        /// </summary>
        Unsynchronisation = 0x80,

        /// <summary>
        /// The second bit (bit 6) indicates whether or not the header is
        /// followed by an extended header. The extended header is described in
        /// section 3.2. A set bit indicates the presence of an extended
        /// header.
        /// </summary>
        ExtendedHeader = 0x40,

        /// <summary>
        /// The third bit (bit 5) is used as an 'experimental indicator'. This
        /// flag SHALL always be set when the tag is in an experimental stage.
        /// </summary>
        ExperimentalIndicator = 0x20,

        /// <summary>
        /// Bit 4 indicates that a footer (section 3.4) is present at the very
        /// end of the tag. A set bit indicates the presence of a footer.
        /// </summary>
        FooterPresent = 0x10
    }
}