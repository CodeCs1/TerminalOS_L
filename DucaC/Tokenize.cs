using System;
using System.Collections.Generic;

namespace DucaC {

    public enum TokenT {
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE, COMMA, DOT, MINUS,
        PLUS,SEMICOLON,SLASH,STAR, NOT,NOT_EQUAL,EQUAL,EQUAL_EQUAL,
        LESS,LESS_EQUAL,GREATER,GREATER_EQUAL,PLUS_EQUAL,MINUS_EQUAL,
        STAR_EQUAL,SLASH_EQUAL, PLUS_PLUS,MINUS_MINUS,
        EOF,
        STRING,NUMBER,IDENTIFIER,KeyWord
    }
    public class Token {
        public TokenT t;
        public String lexeme;
        public Object value;
        public int line;
        public Token(TokenT t, string lexeme, Object value, int line) {
            this.t = t;
            this.lexeme = lexeme;
            this.value = value;
            this.line = line;
        }
        public String toString() {
            return $"{t} {lexeme} {value}";
        }
    }
    public class Scanner {
        private String part;
        private List<Token> tokens = new List<Token>();
        private int start=0,current=0,line=1;
        public List<string> KeyWord;

        public Scanner(string part) {
            this.part = part;
            KeyWord = new List<string>() {
                "func",
                "if",
                "else",
                "ret",
                "nil",
                "var",
                "true",
                "false",
                "for"
            };
        }
        private bool isAtEnd() {
            return current >= part.Length-1;
        }

        private char next_token() {
            current++;
            return part[current];
        }
        private void AddToken(TokenT type) {
            AddToken(type,null);
        }

        private void AddToken(TokenT type, Object value) {
            String text = part.Substring(start,current);
            tokens.Add(new Token(type,text,value,line));
        }

        public List<Token> scan() {
            Console.WriteLine("Start!");
            while(!isAtEnd()) {
                Console.WriteLine("Step!");
                start = current;
                scanT();
            }
            tokens.Add(new Token(TokenT.EOF, "",null,line));
            return tokens;
        }

        private void Error(int line, string message) {
            Console.WriteLine($@"{message} at {line}");
        }


        private void scanT() {
            char c = next_token();
            Console.WriteLine("Step!");
            switch(c) {
                case '(': AddToken(TokenT.LEFT_PAREN); break;
                case ')': AddToken(TokenT.RIGHT_PAREN); break;
                case '{': AddToken(TokenT.LEFT_BRACE); break;
                case '}': AddToken(TokenT.RIGHT_BRACE); break;
                case ',': AddToken(TokenT.COMMA); break;
                case '.': AddToken(TokenT.DOT); break;
                case '-': 
                    if (match('=')) {
                        AddToken(TokenT.MINUS_EQUAL); 
                    }
                    else if (match('-')) {
                        AddToken(TokenT.MINUS_MINUS);
                    } else {
                        AddToken(TokenT.MINUS);
                    }
                    break;
                case '+': 
                    if (match('=')) {
                        AddToken(TokenT.PLUS_EQUAL); 
                    }
                    else if (match('+')) {
                        AddToken(TokenT.PLUS_PLUS);
                    } else {
                        AddToken(TokenT.PLUS);
                    }
                    break;
                case ';': AddToken(TokenT.SEMICOLON); break;
                case '*': AddToken(match('=') ? TokenT.STAR_EQUAL:TokenT.STAR); break; 
                case '!': AddToken(match('=') ? TokenT.NOT_EQUAL:TokenT.NOT); break;
                case '=':AddToken(match('=') ? TokenT.EQUAL_EQUAL : TokenT.EQUAL);break;
                case '<':AddToken(match('=') ? TokenT.LESS_EQUAL : TokenT.LESS);break;
                case '>':AddToken(match('=') ? TokenT.GREATER_EQUAL : TokenT.GREATER);break;
                case '/':
                    if (match('/')) {
                        while(peek() == '\n' && !isAtEnd()) next_token();
                    } else if (match('=')) {
                        AddToken(TokenT.SLASH_EQUAL);
                    } else {
                        AddToken(TokenT.SLASH);
                    }
                    break;
                case ' ': case '\r': case '\t': break;
                case '\n': line++; break;
                case '"':
                    while(peek() != '"' && !isAtEnd()) {
                        if (peek() == '\n') line++;
                        next_token();
                    }
                    if (isAtEnd()) {
                        Error(line,"Unterminated String.");
                        return;
                    }
                    next_token();
                    string value = part.Substring(start+1,current-1);
                    AddToken(TokenT.STRING, value);
                    break;
                default: 
                    if (isDigit(c)) {
                        while(isDigit(peek())) next_token();

                        if (peek() == '.' && isDigit(peekNext())) {
                            next_token();
                            while(isDigit(peek())) next_token();
                        }
                        AddToken(TokenT.NUMBER,Double.Parse(part.Substring(start,current)));
                    } else if (isAlpha(c)) {
                        Console.WriteLine("Step__!");
                        while(isAlphaNumeric(peek())) next_token();
                        String text = part.Substring(start,current);
                        TokenT type=0;
                        if (KeyWord.Contains(text)) {
                            type = TokenT.KeyWord;
                        } else {
                            type = TokenT.IDENTIFIER;
                        }
                        AddToken(type);
                    }
                    else {
                        Error(line,"Unexpected character"); break;
                    }
                    break;
            }
        }
        private bool isAlpha(char c) {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                    c == '_';
        }

        private bool isAlphaNumeric(char c) {
            return isAlpha(c) || isDigit(c);
        }

        private char peekNext() {
            if (current+1>=part.Length) return '\0';
            return part[current+1];
        }

        private bool isDigit(char c) {
            return c >= '0' && c <= '9';
        } 



        private char peek() {
            if (isAtEnd()) return '\0';
            return part[current];
        }

        private bool match(char exp) {
            if (isAtEnd()) return false;
            if (part[current] != exp) return false;
            current++;
            return true;

        }
    }
}