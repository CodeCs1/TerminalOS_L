namespace TerminalOS_L {
    public static class BitPort {
        public static byte GetBit(uint Value, int position) {
            return (byte)((Value >> position) & 1);
        }
        public static void SetBit(ref uint value, uint pos, byte bit) {
            //that a lot of cast.
            value = (uint)(value & ~(1 << (int)(pos)) | (uint)(bit << (int)pos));
        }
    }
}