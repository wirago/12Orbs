namespace Orbs.Helper
{
    public static class EffectConstants
    {
        public static readonly float[] BLUR_WEIGHTS = new float[15]
        {
            0.1061154f, 0.1028506f, 0.1028506f, 0.09364651f, 0.09364651f,
            0.0801001f, 0.0801001f, 0.06436224f, 0.06436224f, 0.04858317f,
            0.04858317f, 0.03445063f, 0.03445063f, 0.02294906f, 0.02294906f
        };

        public static readonly float[] BLUR_OFFSETS = new float[15]
        {
            0, 0.00125f, -0.00125f, 0.002916667f, -0.002916667f,
            0.004583334f, -0.004583334f, 0.00625f, -0.00625f, 0.007916667f,
            -0.007916667f, 0.009583334f, -0.009583334f, 0.01125f, -0.01125f
        };
    }
}
