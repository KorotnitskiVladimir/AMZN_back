namespace AMZN.Shared.Exceptions.Errors
{

    public static class ErrorCodes
    {

        // DTO validation (ModelState)
        public const string ValidationError = "validation.error";


        // Auth
        public const string AuthEmailTaken = "auth.email_taken";
        public const string AuthInvalidCredentials = "auth.invalid_credentials";
        public const string AuthInvalidRefreshToken = "auth.invalid_refresh_token";
        public const string AuthClaimsInvalid = "auth.claims_invalid";


        // System
        public const string DatabaseError = "database.error";
        public const string InternalError = "internal.error";


        // Rate limiting
        public const string TooManyRequests = "rate_limited.too_many_requests";

        // Product
        public const string ProductNotFound = "product.not_found";

    }

}
