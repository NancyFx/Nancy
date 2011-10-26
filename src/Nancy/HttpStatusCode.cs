namespace Nancy
{
    /// <summary>
    /// HTTP Status Codes
    /// </summary>
    /// <remarks>The values are based on the list found at http://en.wikipedia.org/wiki/List_of_HTTP_status_codes </remarks>
    public enum HttpStatusCode
    {
        /// <summary>
        /// 100 Continue
        /// </summary>
        Continue = 100,
        /// <summary>
        /// 101 SwitchingProtocols
        /// </summary>
        SwitchingProtocols = 101,
        /// <summary>
        /// 102 Processing
        /// </summary>
        Processing = 102,
        /// <summary>
        /// 103 Checkpoint
        /// </summary>
        Checkpoint = 103,
        /// <summary>
        /// 200 OK
        /// </summary>
        OK = 200,
        /// <summary>
        /// 201 Created
        /// </summary>
        Created = 201,
        /// <summary>
        /// 202 Accepted
        /// </summary>
        Accepted = 202,
        /// <summary>
        /// 203 NonAuthoritativeInformation
        /// </summary>
        NonAuthoritativeInformation = 203,
        /// <summary>
        /// 204 NoContent
        /// </summary>
        NoContent = 204,
        /// <summary>
        /// 205 ResetContent
        /// </summary>
        ResetContent = 205,
        /// <summary>
        /// 206 PartialContent
        /// </summary>
        PartialContent = 206,
        /// <summary>
        /// 207 MultipleStatus
        /// </summary>
        MultipleStatus = 207,
        /// <summary>
        /// 226 IMUsed
        /// </summary>
        IMUsed = 226,
        /// <summary>
        /// 300 MultipleChoices
        /// </summary>
        MultipleChoices = 300,
        /// <summary>
        /// 301 MovedPermanently
        /// </summary>
        MovedPermanently = 301,
        /// <summary>
        /// 302 Found
        /// </summary>
        Found = 302,
        /// <summary>
        /// 303 SeeOther
        /// </summary>
        SeeOther = 303,
        /// <summary>
        /// 304 NotModified
        /// </summary>
        NotModified = 304,
        /// <summary>
        /// 305 UseProxy
        /// </summary>
        UseProxy = 305,
        /// <summary>
        /// 306 SwitchProxy
        /// </summary>
        SwitchProxy = 306,
        /// <summary>
        /// 307 TemporaryRedirect
        /// </summary>
        TemporaryRedirect = 307,
        /// <summary>
        /// 308 ResumeIncomplete
        /// </summary>
        ResumeIncomplete = 308,
        /// <summary>
        /// 400 BadRequest
        /// </summary>
        BadRequest = 400,
        /// <summary>
        /// 401 Unauthorized
        /// </summary>
        Unauthorized = 401,
        /// <summary>
        /// 402 PaymentRequired
        /// </summary>
        PaymentRequired = 402,
        /// <summary>
        /// 403 Forbidden
        /// </summary>
        Forbidden = 403,
        /// <summary>
        /// 404 NotFound
        /// </summary>
        NotFound = 404,
        /// <summary>
        /// 405 MethodNotAllowed
        /// </summary>
        MethodNotAllowed = 405,
        /// <summary>
        /// 406 NotAcceptable
        /// </summary>
        NotAcceptable = 406,
        /// <summary>
        /// 407 ProxyAuthenticationRequired
        /// </summary>
        ProxyAuthenticationRequired = 407,
        /// <summary>
        /// 408 RequestTimeout
        /// </summary>
        RequestTimeout = 408,
        /// <summary>
        /// 409 Conflict
        /// </summary>
        Conflict = 409,
        /// <summary>
        /// 410 Gone
        /// </summary>
        Gone = 410,
        /// <summary>
        /// 411 LengthRequired
        /// </summary>
        LengthRequired = 411,
        /// <summary>
        /// 412 PreconditionFailed
        /// </summary>
        PreconditionFailed = 412,
        /// <summary>
        /// 413 RequestEntityTooLarge
        /// </summary>
        RequestEntityTooLarge = 413,
        /// <summary>
        /// 414 RequestUriTooLong
        /// </summary>
        RequestUriTooLong = 414,
        /// <summary>
        /// 415 UnsupportedMediaType
        /// </summary>
        UnsupportedMediaType = 415,
        /// <summary>
        /// 416 RequestedRangeNotSatisfiable
        /// </summary>
        RequestedRangeNotSatisfiable = 416,
        /// <summary>
        /// 417 ExpectationFailed
        /// </summary>
        ExpectationFailed = 417,
        /// <summary>
        /// 418 ImATeapot
        /// </summary>
        ImATeapot = 418,
        /// <summary>
        /// 422 UnprocessableEntity
        /// </summary>
        UnprocessableEntity = 422,
        /// <summary>
        /// 423 Locked
        /// </summary>
        Locked = 423,
        /// <summary>
        /// 424 FailedDependency
        /// </summary>
        FailedDependency = 424,
        /// <summary>
        /// 425 UnorderedCollection
        /// </summary>
        UnorderedCollection = 425,
        /// <summary>
        /// 426 UpgradeRequired
        /// </summary>
        UpgradeRequired = 426,
        /// <summary>
        /// 444 NoResponse
        /// </summary>
        NoResponse = 444,
        /// <summary>
        /// 449 RetryWith
        /// </summary>
        RetryWith = 449,
        /// <summary>
        /// 450 BlockedByWindowsParentalControls
        /// </summary>
        BlockedByWindowsParentalControls = 450,
        /// <summary>
        /// 499 ClientClosedRequest
        /// </summary>
        ClientClosedRequest = 499,
        /// <summary>
        /// 500 InternalServerError
        /// </summary>
        InternalServerError = 500,
        /// <summary>
        /// 501 NotImplemented
        /// </summary>
        NotImplemented = 501,
        /// <summary>
        /// 502 BadGateway
        /// </summary>
        BadGateway = 502,
        /// <summary>
        /// 503 ServiceUnavailable
        /// </summary>
        ServiceUnavailable = 503,
        /// <summary>
        /// 504 GatewayTimeout
        /// </summary>
        GatewayTimeout = 504,
        /// <summary>
        /// 505 HttpVersionNotSupported
        /// </summary>
        HttpVersionNotSupported = 505,
        /// <summary>
        /// 506 VariantAlsoNegotiates
        /// </summary>
        VariantAlsoNegotiates = 506,
        /// <summary>
        /// 507 InsufficientStorage
        /// </summary>
        InsufficientStorage = 507,
        /// <summary>
        /// 509 BandwidthLimitExceeded
        /// </summary>
        BandwidthLimitExceeded = 509,
        /// <summary>
        /// 510 NotExtended
        /// </summary>
        NotExtended = 510
    }
}
