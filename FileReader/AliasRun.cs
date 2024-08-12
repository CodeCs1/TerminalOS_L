namespace TerminalOS_L {
    public class AliasRun {
        public readonly string commandname;
        public AliasRun(string name) {this.commandname = name;}
        public virtual object ARun(string[] args) {
            return null;
        }
    }
}