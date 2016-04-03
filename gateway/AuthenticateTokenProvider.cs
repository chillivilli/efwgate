using System;

namespace Gateways
{
    public abstract class AuthenticateTokenProvider
    {
        static AuthenticateTokenProvider()
        {
            Current = new NullToken();
        }

        public static AuthenticateTokenProvider Current { get; set; }
        public abstract DateTime ExpiryDate { get; }
        public abstract string TokenKey { get; }

        public bool IsExpired
        {
            get { return Current.ExpiryDate < DateTime.Now; }
        }
    }

    public class SuccessToken : AuthenticateTokenProvider
    {
        private readonly DateTime _expiryDate;
        private readonly string _tokenKey;

        public SuccessToken(string expiryDate, string tokenKey)
            : this(DateTime.Parse(expiryDate), tokenKey)
        {
        }

        private SuccessToken(DateTime expiryDate, string tokenKey)
        {
            _expiryDate = expiryDate;
            _tokenKey = tokenKey;
        }

        public override DateTime ExpiryDate
        {
            get { return _expiryDate; }
        }

        public override string TokenKey
        {
            get { return _tokenKey; }
        }
    }

    public class ErrorToken : AuthenticateTokenProvider
    {
        private readonly string _error;

        public ErrorToken(string error)
        {
            _error = error;
        }

        public override DateTime ExpiryDate
        {
            get { return DateTime.MinValue; }
        }

        public override string TokenKey
        {
            get
            {
                throw new Exception(_error);
            }
        }
    }

    public class NullToken : AuthenticateTokenProvider
    {
        public override DateTime ExpiryDate
        {
            get { return DateTime.Now.AddDays(-1); }
        }

        public override string TokenKey
        {
            get { return string.Empty; }
        }
    }
}