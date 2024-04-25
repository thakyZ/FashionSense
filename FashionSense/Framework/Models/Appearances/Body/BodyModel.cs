using FashionSense.Framework.Interfaces.API;
using FashionSense.Framework.Models.Appearances.Generic;

namespace FashionSense.Framework.Models.Appearances.Body
{
    public class BodyModel : AppearanceModel
    {
        public int HeightOffset { get; set; }
        public int HairOffset { get; set; }
        public int ShirtOffset { get; set; }
        public int SleevesOffset { get; set; }
        public Size BodySize { get; set; }

        internal int GetFeatureOffset(IApi.Type type)
        {
            switch (type)
            {
                case IApi.Type.Hat:
                case IApi.Type.Hair:
                    return HairOffset;
                case IApi.Type.Shirt:
                    return ShirtOffset;
                case IApi.Type.Sleeves:
                    return SleevesOffset;
                default:
                    return 0;
            }
        }
    }
}
