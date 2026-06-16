namespace CCMS.Application.Helpers
{
    public static class DataMasker
    {
        public static string MaskAadhaar(string aadhaar)
        {
            if (string.IsNullOrWhiteSpace(aadhaar) || aadhaar.Length < 4)
                return aadhaar;

            // Mask all but last 4 digits: XXXXXXXX1234
            return new string('X', aadhaar.Length - 4) + aadhaar.Substring(aadhaar.Length - 4);
        }

        public static string MaskPan(string pan)
        {
            if (string.IsNullOrWhiteSpace(pan) || pan.Length < 4)
                return pan;

            // Typical PAN length is 10. We mask the first 6 characters: XXXXXX1234
            return new string('X', pan.Length - 4) + pan.Substring(pan.Length - 4);
        }

        public static string MaskAccountNumber(string accountNo)
        {
            if (string.IsNullOrWhiteSpace(accountNo) || accountNo.Length < 4)
                return accountNo;

            // Mask all but last 4 digits: XXXXXXX1234
            return new string('X', accountNo.Length - 4) + accountNo.Substring(accountNo.Length - 4);
        }
    }
}
