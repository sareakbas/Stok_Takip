using System;

namespace Business.Exceptions
{
    public class BusinessException : Exception
    {
        public string ErrorCode { get; set; }

        public object[]? Parameters { get; }

        //base kısmı miras alınan sınıfı temsil ediyor
        public BusinessException(string errorCode) : base(errorCode)
        {
            ErrorCode = errorCode;
        }
    // Hata kodu ve yanındaki değerleri gönderdiğimiz kullanıım
    public BusinessException(string errorCode, params object[] parameters) : base(errorCode)
        {
            ErrorCode = errorCode;
            Parameters = parameters;
        }
    }
}