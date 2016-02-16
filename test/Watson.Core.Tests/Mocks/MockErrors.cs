namespace Watson.Core.Tests.Mocks
{
    public class MockErrors
    {
        public const string Error400 =
            " {\"help\":\"http://www.ibm.com/smarterplanet/us/en/ibmwatson/developercloud/doc/personality-insights/#overviewInput\",\"error\":\"The number of words 5 is less than the minimum number of words required for analysis: 100\",\"code\":400}";

        public const string Error415 =
            "{\"code\":415,\"error\":\"The Media Type [text/plain] of the input document is not supported. Auto correction was attempted, but the auto detected media type [application/x-tika-ooxml] is also not supported. Supported Media Types are: application/msword, application/vnd.openxmlformats-officedocument.wordprocessingml.document, application/pdf, text/html, application/xhtml+xml .\"} ";

        public const string Error401RawHtml =
            "<HTML><HEAD><meta content=\"text/html; charset=UTF-8\" http-equiv=\"Content-Type\"><TITLE>Watson Error</TITLE></HEAD><BODY><HR><p>Invalid access to resource - /personality-insights/api/v2/profile?headers=true</p><p>User access not Authorized.</p><p>Gateway Error Code : ERCD50-LDAP-NODN-ERR</p><p>Unable to communicate with Watson.</p><p>Request URL : /personality-insights/api/v2/profile?headers=true</p><p>Error Id :  csf_platform_prod_dp02-218615927</p><p>Date-Time : 2015-12-02T20:32:12-05:00</p></BODY></HTML>";
    }
}