namespace TerminalOS_L {
    public static class BitPort {
        public static byte GetBit(uint Value, int position) {
            return (byte)((Value >> position) & 1);
        }
    }
}