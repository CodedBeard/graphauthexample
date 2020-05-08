namespace CodedBeard.GraphAuth
{
    public class AccessTokenResult
    {
        public string Token_Type { get; set; }
        public string Expires_In { get; set; }
        public string Ext_Expires_In { get; set; }
        public string Access_Token { get; set; }
    }
}