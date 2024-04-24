using FashionSense.Framework.Models.Appearances.Generic;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FashionSense.Framework.Models.Appearances.Body
{
    public class BodyModel : AppearanceModel
    {
        public Position BodyPosition { get; set; } = new Position() { X = 0, Y = 0 };
        public Size BodySize { get; set; }
    }
}
